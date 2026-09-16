using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Area : MonoBehaviour
{
    public bool AutoRestAfterCombat = false;
    public string ExplorationMusic;
    public string BattleMusic;
    public string BossBattleMusic;
    public int Level = 1;
    public Dictionary<AudioSource, float> AudioSourceOriginalVolumes = new();

    [Header("Combat Preload")]
    [Tooltip("VisualEffect names without the VisualEffect_ prefix, e.g. WallHit")]
    public List<string> PreloadVisualEffects = new List<string>();
    [Tooltip("Projectile names without the Projectile_ prefix, e.g. GunBasicAttack")]
    public List<string> PreloadProjectiles = new List<string>();
    [Tooltip("AreaOfEffect names without the AreaOfEffect_ prefix")]
    public List<string> PreloadAreaOfEffects = new List<string>();
    [Tooltip("Sprite paths relative to Resources/Sprites/, e.g. UI/Money")]
    public List<string> PreloadSprites = new List<string>();
    public int VisualEffectPoolSize = 2;
    public int ProjectilePoolSize = 4;
    public int DamageNumberPoolSize = 16;

    public enum FootstepsType { Dirt, Grass, Wood, Concrete, Gravel, Earth, None, Lava, Water, Ice, Sand, Metal};
    public FootstepsType Footsteps; 
    public string ClassNameForArea;

    private static GameObject _instance;
    public static GameObject Instance {
        get {
            if(_instance == null) {
                _instance = GameObject.FindGameObjectWithTag("Area");
            }
            return _instance;
        }
        set {
            _instance = value;
        }
    }

    private static Area _componentInstance;
    public static Area ComponentInstance {
        get {
            if(_componentInstance == null && Instance != null) {
                _componentInstance = Instance.GetComponent<Area>();
            }
            return _componentInstance;
        }
    }

    public bool ApplyPlayerCamouflage = false;

    public Dictionary<Tilemap, FootstepsOverride> TilemapsWithFootstepOverrides = new();
    public void Start() {
        EventManager.FinishedLoadingArea.AddListener(InitializeArea);
    }

    public static void ClearInstance() {
        _instance = null;
        _componentInstance = null;
    }

    public void InitializeArea() {
        GameController.Instance.CurrentAreaOnUpdateMethod = null;
        if(SaveFile.Instance.CurrentMission != null && SaveFile.Instance.CurrentMission.MusicOnStart != null) {
            Utils.SetDefaultMusic(SaveFile.Instance.CurrentMission.MusicOnStart); 
        }
        else if(!string.IsNullOrWhiteSpace(ExplorationMusic)) {
            Utils.SetDefaultMusic(ExplorationMusic);
        }
        if(SaveFile.Instance.CurrentMission != null && SaveFile.Instance.CurrentMission.EnemyLevel != 1) {
            Level = SaveFile.Instance.CurrentMission.EnemyLevel;
        }
        if(Level != 1) {
            foreach(Unit u in Utils.GetAllUnits()) {
                u.Level = Level;
                u.AdjustUIResourceBarsSize();
            }
        }
        SaveFile.Instance.RefreshFlagBehaviours();
        if(ApplyPlayerCamouflage) {
            ApplyCamouflageToPlayer();
            GameController.Instance.WaitAndRunMethod(0.01f, ApplyCamouflageToPlayer);
        }
        if(String.IsNullOrWhiteSpace(ClassNameForArea) == false && Type.GetType(ClassNameForArea).GetMethod("OnStart") != null) {
            Type.GetType(ClassNameForArea).GetMethod("OnStart").Invoke(null, null);
        }
        else if(Type.GetType("Area_" + gameObject.name.Replace("(Clone)", ""))?.GetMethod("OnStart") != null) {
            Type.GetType("Area_" + gameObject.name.Replace("(Clone)", "")).GetMethod("OnStart").Invoke(null, null);
        }
        if(String.IsNullOrWhiteSpace(ClassNameForArea) == false && Type.GetType(ClassNameForArea).GetMethod("OnUpdate") != null) {
            GameController.Instance.CurrentAreaOnUpdateMethod = Type.GetType(ClassNameForArea).GetMethod("OnUpdate");
        }
        else if(Type.GetType("Area_" + gameObject.name.Replace("(Clone)", ""))?.GetMethod("OnUpdate") != null) {
            GameController.Instance.CurrentAreaOnUpdateMethod = Type.GetType("Area_" + gameObject.name.Replace("(Clone)", "")).GetMethod("OnUpdate");
        }
        foreach(AudioSource audioSource in GetComponentsInChildren<AudioSource>(true)) {
            AudioSourceOriginalVolumes.Add(audioSource, audioSource.volume);
            audioSource.volume *= Settings.Instance.SoundVolume;
        }
        foreach(FootstepsOverride footstepOverride in GetComponentsInChildren<FootstepsOverride>(true)) {
            Tilemap tilemap = footstepOverride.GetComponent<Tilemap>();
            if(tilemap != null) {
                TilemapsWithFootstepOverrides.Add(footstepOverride.GetComponent<Tilemap>(), footstepOverride);
            }
        }
    }

    public IEnumerator PreloadCombatAssets(System.Action<float> onProgress = null) {
        List<string> prefabPaths = new List<string> {
            "Prefabs/UI/UI_InjuryIndicator",
            "Prefabs/UI/UI_StaggerIndicator"
        };
        foreach (string name in PreloadVisualEffects) {
            if (!string.IsNullOrWhiteSpace(name)) {
                prefabPaths.Add("Prefabs/VisualEffect/VisualEffect_" + name);
            }
        }
        foreach (string name in PreloadProjectiles) {
            if (!string.IsNullOrWhiteSpace(name)) {
                prefabPaths.Add("Prefabs/Projectile/Projectile_" + name);
            }
        }
        foreach (string name in PreloadAreaOfEffects) {
            if (!string.IsNullOrWhiteSpace(name)) {
                prefabPaths.Add("Prefabs/AreaOfEffect/AreaOfEffect_" + name);
                prefabPaths.Add("Prefabs/AreaOfEffect/AreaOfEffect_" + name + "_Flipped");
            }
        }
        int total = prefabPaths.Count + PreloadSprites.Count;
        if (total == 0) {
            onProgress?.Invoke(1f);
            yield break;
        }
        int completed = 0;
        foreach (string path in prefabPaths) {
            int poolSize = path.Contains("UI_InjuryIndicator") || path.Contains("UI_StaggerIndicator")
                ? DamageNumberPoolSize
                : path.Contains("/Projectile/")
                    ? ProjectilePoolSize
                    : VisualEffectPoolSize;
            yield return ObjectPool.Warm(path, poolSize);
            completed++;
            onProgress?.Invoke((float)completed / total);
        }
        foreach (string spritePath in PreloadSprites) {
            if (!string.IsNullOrWhiteSpace(spritePath)) {
                yield return ResourceCache.LoadAsync<Sprite>("Sprites/" + spritePath);
            }
            completed++;
            onProgress?.Invoke((float)completed / total);
        }
    }

    public static void ApplyCamouflageToPlayer() {
        Player.Instance.UnitColorChange.Hair = Colors.GetColorFromCode("#DBA600");
        Player.Instance.UnitColorChange.Eye = Colors.GetColorFromCode("#359C34");
        Player.Instance.UnitColorChange.UpdateMaterialProperties();
    }
}
