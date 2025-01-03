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

    public static void ApplyCamouflageToPlayer() {
        Player.Instance.UnitColorChange.Hair = Colors.GetColorFromCode("#DBA600");
        Player.Instance.UnitColorChange.Eye = Colors.GetColorFromCode("#359C34");
        Player.Instance.UnitColorChange.UpdateMaterialProperties();
    }
}
