
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Constants;
using static Effect;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using System.Collections;
using System.IO;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Unity.Properties;
using System.IO.Compression;
using Unity.VisualScripting.FullSerializer;

public class Utils {
    public static readonly Dictionary<string, AudioClip> LoadedAudioClips = new Dictionary<string, AudioClip>();
    public static readonly Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();

    public static AnimationClip GetAnimationClip(Animator animator, string clip_name) {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clip_name)
            {
                return clip;
            }
        }
        return null;
    }

    public static bool CheckIfCurrenTargetIsInFrontOfUnit(Unit unit, float max_distance = 2) {
        if(unit.CurrentTarget == null) {
            return false;
        }
        if(unit.Actions.IsFlipped && unit.CurrentTarget.transform.position.x > unit.transform.position.x) {
            return false;
        }
        if(!unit.Actions.IsFlipped && unit.CurrentTarget.transform.position.x < unit.transform.position.x) {
            return false;
        }
        return Vector2.Distance(unit.transform.position, unit.CurrentTarget.transform.position) < max_distance;
    }

    public static string ConvertCharacterFromForeignLanguages(string character) {
        switch(character) {
            case "Ą": return "A";
            case "Ę": return "E";
            case "Ó": return "U";
            case "1": return "O";
            case "2": return "U";
            case "3": return "I";
            case "4": return "O";
            case "5": return "E";
            case "6": return "I";
            case "7": return "E";
            case "8": return "I";
            case "9": return "A";
            case "0": return "O";
            default: return character;
        }
    }

    public static List<Unit> GetAllUnits(bool get_only_hostile = false, bool get_only_alive = true) {
        List<Unit> units = new List<Unit>();
        foreach(Unit unit in Area.Instance.GetComponentsInChildren<Unit>(true)) {
            if((get_only_hostile == false || unit.IsHostile) && (get_only_alive == false || unit.GetComponent<Unit>().KnockedOut == false)) {
                units.Add(unit.GetComponent<Unit>());
                RecentlyFoundNPCs.Add(unit.gameObject);
            }
        }
        return units;
    }

    public static List<Unit> GetSpecifiedUnits(Func<Unit, bool> condition_that_units_have_to_fulfill) {
        List<Unit> units = new List<Unit>();
        foreach(Unit unit in Area.Instance.GetComponentsInChildren<Unit>(true)) {
            if(condition_that_units_have_to_fulfill != null && condition_that_units_have_to_fulfill.Invoke(unit)) {
                units.Add(unit.GetComponent<Unit>());
                RecentlyFoundNPCs.Add(unit.gameObject);
            }
        }
        return units;
    }

    public static string ExtractLabelFromText(string text) {
        string extracted_label = "";
        bool extracting = false;
        for(int i = 0; i < text.Length; i++) {
            if(text[i] == '{') {
                extracting = true;
            }
            else if(text[i] == '}') {
                extracting = false;
            }
            else if(extracting) {
                extracted_label += text[i];
            }
        }
        return extracted_label;
    }

    public static List<GameObject> RecentlyFoundNPCs = new List<GameObject>();

    public static Unit GetUnit(string npc_name, bool only_get_active = false) {
        GameObject obj = RecentlyFoundNPCs.FirstOrDefault(go => go != null && (only_get_active == false || go.gameObject.activeInHierarchy) && (go.name == "Unit_" + npc_name || go.name == npc_name));
        if(obj != null) {
            return obj.GetComponent<Unit>();
        }
        foreach(string potentialPrefix in new[] {"NPCs/Unit_", "Unit_", "NPCs/", ""}) {
            Transform foundUnit = Area.Instance.transform.Find(potentialPrefix + npc_name);
            if(foundUnit != null && (only_get_active == false || foundUnit.gameObject.activeInHierarchy)) {
                RecentlyFoundNPCs.Add(foundUnit.gameObject);
                return foundUnit.GetComponent<Unit>();
            }
        }
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        foreach(GameObject go in units) {
            if((go.name == npc_name || go.name == "Unit_" + npc_name) && go.GetComponent<Unit>() != null && (only_get_active == false || go.activeInHierarchy)) {
                if(!RecentlyFoundNPCs.Contains(go)) {
                    RecentlyFoundNPCs.Add(go);
                }
                return go.GetComponent<Unit>();
            }
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject go in units) {
            if((go.name == npc_name || go.name == "Unit_" + npc_name) && go.GetComponent<Unit>() != null && (only_get_active == false || go.activeInHierarchy)) {
                if(!RecentlyFoundNPCs.Contains(go)) {
                    RecentlyFoundNPCs.Add(go);
                }
                return go.GetComponent<Unit>();
            }
        }
        return null;
    }

    public static List<Type> GetAllCurrentlyEquippedAbilityTypes() {
        List<Type> abilities = new();
        foreach(Stance stance in SaveFile.Instance.Stances) {
            foreach(Stance.EquippedAbility ability in stance.Abilities) {
                if(abilities.Contains(ability.Type) == false) {
                    abilities.Add(ability.Type);
                }
            }
        }
        return abilities;
    }

    public static string GetStackTrace()
    {
        string stack_trace = "";
        StackTrace st = new StackTrace(true);
        for (int i = 0; i < st.FrameCount; i++)
        {
            StackFrame sf = st.GetFrame(i);
            stack_trace += sf.GetFileName() + "(" + sf.GetFileLineNumber() + "," + sf.GetFileColumnNumber() +"): " + sf.GetMethod() + "; ";
        }
        return stack_trace;
    }

    public static string GetTimeStamp()
    {
        //return DateTime.Now.ToString("yyyy-MM-dd\\THH:mm:ss\\Z");
        return DateTime.Now.ToString("HH:mm:ss");
    }

    public static void DestroyAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            parent.GetChild(i).gameObject.SetActive(false);
            MonoBehaviour.Destroy(parent.GetChild(i).gameObject);
        }
    }

    public static WeaponClass GetPlayerWeaponClassForDamageType(Constants.DamageType damage_type) {
        if(damage_type == DamageType.Heavy) {
            return SaveFile.Instance.EquippedHeavyWeapon.WeaponClass;
        }
        else if(damage_type == DamageType.Light) {
            return SaveFile.Instance.EquippedLightWeapon.WeaponClass;
        }
        else if(damage_type == DamageType.Ranged) {
            return SaveFile.Instance.EquippedRangedWeapon.WeaponClass;
        }
        else {
            return WeaponClass.Magic;
        }
    }

    public static void KnockbackEnemyBasedOnMeleeWeaponDistance(Damage damage, float optimal_distance) {
        float distance = Vector2.Distance(damage.TargetOfDamage.transform.position, damage.SourceOfDamage.User.transform.position);
        if(distance >= optimal_distance) {
            return;
        }
        bool source_to_the_left_of_target = damage.SourceOfDamage.User.transform.position.x > damage.TargetOfDamage.transform.position.x ? false : true;
        Vector2 direction_vector_towards_target = (damage.TargetOfDamage.transform.position - (damage.SourceOfDamage.User.transform.position + (source_to_the_left_of_target ? Vector3.left : Vector3.right))).normalized;
        if(distance < optimal_distance) {
            damage.TargetOfDamage.ApplyForce(direction_vector_towards_target * (optimal_distance - distance) * 5, damage.SourceOfDamage);
        }
    }

    public static int GetRandomSoundNumber(string type) {
        return type switch {
            "Footsteps_Wood" => UnityEngine.Random.Range(1, 21),
            "Footsteps_Water" => UnityEngine.Random.Range(1, 21),
            "Footsteps_Ice" => UnityEngine.Random.Range(1, 30),
            "Footsteps_Snow" => UnityEngine.Random.Range(1, 29),
            "Footsteps_Dirt" => UnityEngine.Random.Range(1, 6),
            "Footsteps_Grass" => UnityEngine.Random.Range(1, 20),
            "Footsteps_Concrete" => UnityEngine.Random.Range(1, 21),
            "Footsteps_Earth" => UnityEngine.Random.Range(1, 6),
            "Footsteps_Lava" => UnityEngine.Random.Range(1, 6),
            "Footsteps_Gravel" => UnityEngine.Random.Range(1, 21),
            "Footsteps_Sand" => UnityEngine.Random.Range(1, 21),
            "Footsteps_Metal" => UnityEngine.Random.Range(1, 11),
            "Fire_Hit" => UnityEngine.Random.Range(1, 3),
            "Metal_Hit" => UnityEngine.Random.Range(1, 4),
            "Wood_Hit" => UnityEngine.Random.Range(1, 4),
            "Rock_Hit" => UnityEngine.Random.Range(1, 4),
            "Fire_Destroy" => UnityEngine.Random.Range(1, 4),
            "Metal_Destroy" => UnityEngine.Random.Range(1, 4),
            "Wood_Destroy" => UnityEngine.Random.Range(1, 4),
            "Rock_Destroy" => UnityEngine.Random.Range(1, 2),
            "Gun_BasicAttack" => UnityEngine.Random.Range(1, 8),
            "Cannon_BasicAttack" => UnityEngine.Random.Range(1, 8),
            "Bow_Draw" => UnityEngine.Random.Range(1, 16),
            "Bow_Release" => UnityEngine.Random.Range(1, 13),
            "TurnBookPage" => UnityEngine.Random.Range(1, 5),
            _ => 1
        };
    }

    public static AudioClip DefaultMusic;
    public static AudioClip IntermissionMusic;
    private static AudioClip NextMusic;

    public static void SetDefaultMusic(string music_filename) {
        if (GameController.Instance.InterruptMusicOnDeath == false) {
            return;
        }
        if (LoadedAudioClips.ContainsKey(music_filename)) {
            Utils.CreateAuditLog("Setting default music: " + music_filename);
            DefaultMusic = LoadedAudioClips[music_filename];
            PlayCrossFadeMusic(LoadedAudioClips[music_filename]);
        }
        else {
            GameController.Instance.StartCoroutine(PlayMusicAsync(music_filename, true));
        }
        GameController.Instance.WaitAndRunMethod(1, ChangeCurrentMusicVolume, 100);
    }

    public static void PlayIntermissionMusic(string music_filename, float intermission_time_in_seconds = 0.5f) {
        if (GameController.Instance.InterruptMusicOnDeath == false) {
            return;
        }
        if (LoadedAudioClips.ContainsKey(music_filename)) {
            Utils.CreateAuditLog("Playing intermission music: " + music_filename);
            IntermissionMusic = LoadedAudioClips[music_filename];
            PlayCrossFadeMusic(LoadedAudioClips[music_filename]);
        }
        else {
            GameController.Instance.StartCoroutine(PlayMusicAsync(music_filename));
        }
        GameController.Instance.WaitAndRunMethod(1, ChangeCurrentMusicVolume, 100);
    }

    public static void StopIntermissionMusic(float intermission_time_in_seconds = 0.5f) {
        if(GameController.Objects.Music.clip == DefaultMusic || GameController.Instance.InterruptMusicOnDeath == false) {
            return;
        }
        Utils.CreateAuditLog($"Stopping intermission music ({IntermissionMusic}) and returning to default: {DefaultMusic}" );
        PlayCrossFadeMusic(DefaultMusic);
    }

    private static void PlayCrossFadeMusic(AudioClip music_to_play, float intermission_time_in_seconds = 0.5f) {
        if(GameController.Objects.Music.clip == null || GameController.Objects.Music.isPlaying == false) {
            GameController.Objects.Music.clip = music_to_play;
            GameController.Objects.Music.Play();
            return;
        }
        if(GameController.Objects.Music.clip == music_to_play && GameController.Objects.Music.isPlaying) {
            return;
        }
        for(int i = 1; i <= 10; i++) {
            GameController.Instance.WaitAndRunMethodRealtime(intermission_time_in_seconds / 10 * i, ChangeCurrentMusicVolume, (int)(100 - 10 * i));
            GameController.Instance.WaitAndRunMethodRealtime(intermission_time_in_seconds + intermission_time_in_seconds / 10 * i, ChangeCurrentMusicVolume, (int)(10 * i));
        }
        GameController.Instance.WaitAndRunMethodRealtime(intermission_time_in_seconds, ChangeCurrentMusic);
        NextMusic = music_to_play;
    }

    private static void ChangeCurrentMusicVolume(int volume) {
        GameController.Objects.Music.volume = 0.15f * volume / 100f * Settings.Instance.MusicVolume;
    }

    public static string GetNameForUnit(Unit unit, string name = "") {
        return 
            unit == null && name == "" ? Label.Get("Name_Unknown") :
            Label.ContainsKey("Name_" + name) ? Label.Get("Name_" + name) : 
            Label.ContainsKey(name) ? Label.Get(name) : 
            Label.ContainsKey("Title_" + name) ? Label.Get("Title_" + name) : 
            unit == null ? Label.Get("Name_Unknown") :
            Label.ContainsKey("Name_" + unit.Title) ? Label.Get("Name_" + unit.Title) : 
            Label.ContainsKey(unit.Title) ? Label.Get(unit.Title) : 
            Label.ContainsKey("Title_" + unit.Title) ? Label.Get("Title_" + unit.Title) : 
            Label.ContainsKey("Name_" + CleanUpUnitGameObjectName(unit.gameObject.name)) ? Label.Get("Name_" + CleanUpUnitGameObjectName(unit.gameObject.name)) : 
            Label.ContainsKey(CleanUpUnitGameObjectName(unit.gameObject.name)) ? Label.Get(CleanUpUnitGameObjectName(unit.gameObject.name)) : 
            Label.ContainsKey("Title_" + CleanUpUnitGameObjectName(unit.gameObject.name)) ? Label.Get("Title_" + CleanUpUnitGameObjectName(unit.gameObject.name)) : 
            Label.ContainsKey("Name_" + unit.DisplayedName) ? Label.Get("Name_" + unit.DisplayedName) : 
            Label.ContainsKey(unit.DisplayedName) ? Label.Get(unit.DisplayedName) : 
            Label.ContainsKey("Title_" + unit.DisplayedName) ? Label.Get("Title_" + unit.DisplayedName) : 
            unit.IsMale ? Label.Get("Name_Default_Male") : Label.Get("Name_Default_Female");
    }

    public static string GetTitleForUnit(Unit unit) {
        return 
            unit == null ? Label.Get("Title_UnknownEnemy") :
            Label.ContainsKey("Title_" + unit.Title) ? Label.Get("Title_" + unit.Title) : 
            Label.ContainsKey(unit.Title) ? Label.Get(unit.Title) : 
            Label.ContainsKey("Name_" + unit.Title) ? Label.Get("Name_" + unit.Title) : 
            Label.ContainsKey("Title_" + CleanUpUnitGameObjectName(unit.gameObject.name)) ? Label.Get("Title_" + CleanUpUnitGameObjectName(unit.gameObject.name)) : 
            Label.ContainsKey(CleanUpUnitGameObjectName(unit.gameObject.name)) ? Label.Get(CleanUpUnitGameObjectName(unit.gameObject.name)) : 
            Label.ContainsKey("Name_" + CleanUpUnitGameObjectName(unit.gameObject.name)) ? Label.Get("Name_" + CleanUpUnitGameObjectName(unit.gameObject.name)) : 
            Label.ContainsKey("Title_" + unit.DisplayedName) ? Label.Get("Title_" + unit.DisplayedName) : 
            Label.ContainsKey(unit.DisplayedName) ? Label.Get(unit.DisplayedName) : 
            Label.ContainsKey("Name_" + unit.DisplayedName) ? Label.Get("Name_" + unit.DisplayedName) : 
            Label.Get("Title_UnknownEnemy");
    }

    public static string CleanUpUnitGameObjectName(string text) {
        return text.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").Replace("Unit_", "").Replace("(Clone)", "");  
    }

    public static string GetDisplayedNameForName(string name) {
        return 
            name == null ? Label.Get("Name_Unknown") :
            Label.ContainsKey("Name_" + name) ? Label.Get("Name_" + name) : 
            Label.ContainsKey(name) ? Label.Get(name) : 
            Label.ContainsKey("Title_" + name) ? Label.Get("Title_" + name) : 
            Label.ContainsKey("Name_" + name.Replace("Unit_", "").Replace("(Clone)", "")) ? Label.Get("Name_" + name.Replace("Unit_", "").Replace("(Clone)", "")) : 
            Label.ContainsKey(name.Replace("Unit_", "").Replace("(Clone)", "")) ? Label.Get(name.Replace("Unit_", "").Replace("(Clone)", "")) : 
            Label.ContainsKey("Title_" + name.Replace("Unit_", "").Replace("(Clone)", "")) ? Label.Get("Title_" + name.Replace("Unit_", "").Replace("(Clone)", "")) : 
            Label.Get("Name_Unknown");
    }

    private static void ChangeCurrentMusic() {
        GameController.Objects.Music.clip = NextMusic;
        GameController.Objects.Music.Play();
    }

    private static IEnumerator PlayMusicAsync(string music_filename, bool set_music_as_default = false) {
        ResourceRequest request = Resources.LoadAsync("Sounds/Music/" + music_filename, typeof(AudioClip));
        yield return request;
        if(request.asset == null) {
            Debug.LogError("Could not find music:" + music_filename);
        }
        if(!LoadedAudioClips.ContainsKey(music_filename)) {
            LoadedAudioClips.Add(music_filename, request.asset as AudioClip);
        }
        Utils.CreateAuditLog("Loading and playing music: " + music_filename);
        if(set_music_as_default) {
            DefaultMusic = request.asset as AudioClip;
        }
        PlayCrossFadeMusic(request.asset as AudioClip);
    }

    public static void PlaySoundEffect(AudioSource source_of_sound, string sound_filename, float volume = 1) {
        AudioSource audio_source = source_of_sound == null ? GameController.Instance.GetComponent<AudioSource>() : source_of_sound.GetComponent<AudioSource>();
        if (audio_source == null) {
            Debug.LogError("Could not play sound effect (" + sound_filename + ") because unit (" + source_of_sound.gameObject.name + ") does not have an AudioSource attached to it");
        }
        if (LoadedAudioClips.ContainsKey(sound_filename)) {
            audio_source.PlayOneShot(LoadedAudioClips[sound_filename], volume * Settings.Instance.SoundVolume);
        }
        else {
            GameController.Instance.StartCoroutine(PlaySoundEffectAsync(source_of_sound, sound_filename, volume));
        }
    }

    public static void PlaySoundEffect(AudioSource source_of_sound, AudioClip sound_clip, float volume = 1) {
        AudioSource audio_source = source_of_sound == null ? GameController.Instance.GetComponent<AudioSource>() : source_of_sound.GetComponent<AudioSource>();
        if (audio_source == null) {
            Debug.LogError("Could not play sound effect (" + sound_clip + ") because unit (" + source_of_sound.gameObject.name + ") does not have an AudioSource attached to it");
        }
        audio_source.PlayOneShot(sound_clip, volume * Settings.Instance.SoundVolume);
    }

    private static IEnumerator PlaySoundEffectAsync(AudioSource source_of_sound, string sound_filename, float volume = 1) {
        ResourceRequest request = Resources.LoadAsync("Sounds/Sound Effects/" + sound_filename, typeof(AudioClip));
        yield return request;
        if(request.asset == null) {
            Debug.LogError("Could not find sound effect: " + sound_filename);
        }
        if(!LoadedAudioClips.ContainsKey(sound_filename)) {
            LoadedAudioClips.Add(sound_filename, request.asset as AudioClip);
        }
        if(source_of_sound != null) {
            source_of_sound.PlayOneShot(request.asset as AudioClip, volume * Settings.Instance.SoundVolume);
        }
    }

    public static GameObject GetGameObjectIfNull(ref GameObject game_object, string path, Unit unit = null) {
        if (game_object == null) {
            game_object = unit.transform.Find(path)?.gameObject;
            if(game_object == null) {
                Debug.LogWarning("Could not find unit (" + unit.gameObject.name + ") gameobject under path: " + path);
            }
        }
        return game_object;
    }

    public static Transform GetOnDestroyObject(Transform parent) {
        foreach(Transform child in parent) {
            if(child.gameObject.name.Contains("OnDestroy")) {
                return child;
            }
        }
        return null;
    }

    public static bool CheckIfUnitCanPerformActions(Unit unit) {
        if(unit == null || unit.Actions == null) {
            return false;
        }
        Constants.ActionType currentAction = unit.Actions.CurrentActionBeingPerformed;
        return
        currentAction == Constants.ActionType.Idle ||
        currentAction == Constants.ActionType.Moving ||
        (currentAction == Constants.ActionType.UsingAbility && unit.Actions.CurrentAbilityBeingPerformed != null && unit.Actions.CurrentAbilityBeingPerformed.CanInterruptCurrentAbility);
    }

    public static bool CheckIfUnitCanMove(Unit unit) {
        AnimatorClipInfo[] currentAnimations = unit.Animator.GetCurrentAnimatorClipInfo(0);
        if (currentAnimations.Length == 0) {
            return true;
        }
        string currentAnimationName = currentAnimations[0].clip.name;
        Constants.ActionType currentAction = unit.Actions.CurrentActionBeingPerformed;
        return currentAnimationName == null ||
            currentAction == Constants.ActionType.Idle ||
            currentAction == Constants.ActionType.Moving ||
            (currentAction == Constants.ActionType.UsingAbility && unit.Actions.CurrentAbilityBeingPerformed != null && (unit.Actions.CurrentAbilityBeingPerformed.CanMoveWhileUsing || unit.Actions.CurrentAbilityBeingPerformed.GetType().IsSubclassOf(typeof(AI))));
    }

    public static Sprite GetGraphicForAbility(string ability_type) {
        if(string.IsNullOrWhiteSpace(ability_type) || ability_type == "Null") {
            return null;
        }
        return Resources.Load("Sprites/Ability/" + ability_type.ToString().Replace("Ability_", ""), typeof(Sprite)) as Sprite;
    }

    public static Vector2 GetDirectionVector(Vector2 source, Vector2 target, bool source_faces_left, float max_angle = 90) {
        Vector2 direction_vector = (target - source).normalized;
        Vector2 absolute_vector = new Vector2(Math.Abs(direction_vector.x), Math.Abs(direction_vector.y));
        float angle_of_direction_vector = ConvertRadiansToDegrees((float)Math.Atan(absolute_vector.y / absolute_vector.x));
        if ((source_faces_left && direction_vector.x > 0) || (!source_faces_left && direction_vector.x < 0)) {
            Vector2 vector_reduced_to_max_angle = new Vector2((float)(Math.Sin(ConvertDegreesToRadians(90 - max_angle)) / Math.Sin(ConvertDegreesToRadians(max_angle))), 1).normalized;
            return new Vector2(direction_vector.x > 0 ? -vector_reduced_to_max_angle.x : vector_reduced_to_max_angle.x, direction_vector.y > 0 ? vector_reduced_to_max_angle.y : -vector_reduced_to_max_angle.y).normalized;
        }
        if (angle_of_direction_vector > max_angle) {
            Vector2 vector_reduced_to_max_angle = new Vector2((float)(Math.Sin(ConvertDegreesToRadians(90 - max_angle)) / Math.Sin(ConvertDegreesToRadians(max_angle))), 1).normalized;
            return new Vector2(direction_vector.x > 0 ? vector_reduced_to_max_angle.x : -vector_reduced_to_max_angle.x, direction_vector.y > 0 ? vector_reduced_to_max_angle.y : -vector_reduced_to_max_angle.y).normalized;
        }
        return direction_vector;
    }

    public static bool GetAreOppositeDirections(string direction1, string direction2) {
        if ((direction1 == "Up" && direction2 == "Down") || (direction1 == "Down" && direction2 == "Up")) {
            return true;
        }
        if ((direction1 == "Left" && direction2 == "Right") || (direction1 == "Right" && direction2 == "Left")) {
            return true;
        }
        return false;
    }

    public static Vector2 GetPositionGivenDistanceAwayBasedOnTwoPoints(Vector2 center_point, Vector2 direction_point, float distance) {
        Vector2 reposition_vector = new Vector2(direction_point.x - center_point.x, direction_point.y - center_point.y).normalized * distance;
        return center_point + reposition_vector;
    }

    public static float ConvertDegreesToRadians(float angle_in_degrees) {
        return (float)(Math.PI / 180 * angle_in_degrees);
    }

    public static float ConvertRadiansToDegrees(float angle_in_radians) {
        return (float)(angle_in_radians * (180 / Math.PI));
    }

    public static string GetCurrentAndNextAnimationName(Animator animator) {
        if (animator.GetCurrentAnimatorClipInfo(0).Length == 0) {
            return "";
        }
        string animation_name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        string next_animation_name = animator.GetNextAnimatorClipInfo(0).Length > 0 ? animator.GetNextAnimatorClipInfo(0)[0].clip.name : "";
        return animation_name + "," + next_animation_name;
    }

    public static int GetCodeForGivenAction(Constants.ActionType action) {
        return action switch {
            Constants.ActionType.Idle => 0,
            Constants.ActionType.Moving => 1,
            Constants.ActionType.UsingAbility => 2,
            Constants.ActionType.UnderHardCrowdControl => 3,
            Constants.ActionType.InCutscene => 4,
            _ => -1,
        };
    }

    public static bool CheckIfColliderIsValidUnitHitbox(Collider2D other)
    {
        return GameController.Instance.GameplayMode == Constants.GameplayMode.Regular && (other.gameObject.layer == LayerMask.NameToLayer("Environment") || (other.CompareTag("Hitbox") == true && other is CapsuleCollider2D));
    }

    public static Projectile SendProjectileBackTowardsSource(Damage damage, Unit unit_riposting, Ability source_of_redirection, bool send_towards_current_target_if_not_null = false) {
        damage.Injury = 0;
        damage.Stagger = 0;
        GameObject cloned_projectile = MonoBehaviour.Instantiate(damage.DamagingObject.gameObject);
        cloned_projectile.transform.SetParent(Area.Instance.transform, true);
        Projectile projectile = cloned_projectile.GetComponent<Projectile>();

        projectile.FlightSpeed *= 1.25f;
        projectile.MaxFlightDistance *= 1.25f;
        projectile.StartLocation = projectile.transform.position;
        projectile.SourceAbility = source_of_redirection;
        source_of_redirection.OriginalRipostedAbility = damage.SourceOfDamage.OriginalRipostedAbility != null ? damage.SourceOfDamage.OriginalRipostedAbility : damage.SourceOfDamage;
        source_of_redirection.OriginalRipostedAbility.RipostedCount++;
        if(projectile.HomingOntoUnit != null) {
            projectile.HomingOntoUnit = damage.SourceOfDamage.User;
        }
        projectile.gameObject.name = "Projectile Redirect";
        Utils.PlaySoundEffect(unit_riposting.AudioSource, "Steel/SteelCollision" + UnityEngine.Random.Range(1, 4), 0.4f);
        Vector2 inFrontOfUnit = unit_riposting.Actions.IsFlipped ? unit_riposting.transform.position + Vector3.left : unit_riposting.transform.position + Vector3.right;
        Utils.CreateVisualEffect(new(source_of_redirection), "Counter", inFrontOfUnit.x, inFrontOfUnit.y);
        Unit targetEnemy = (send_towards_current_target_if_not_null && source_of_redirection?.User?.CurrentTarget != null) ? source_of_redirection.User.CurrentTarget : damage.SourceOfDamage.User;
        projectile.transform.up = (targetEnemy.transform.position - source_of_redirection.User.transform.position).normalized;

        damage.DamagingObject.DealingDamage = false;
        if(damage.DamagingObject.IsDestroyed == false) {
            damage.DamagingObject.IsDestroyed = true;
            MonoBehaviour.Destroy(damage.DamagingObject.gameObject);
        }
        return projectile;
    }

    /// <summary>
    /// For ranges set to 10 - 20, and values set to 100 - 200, if given range is 15 then returns 150
    /// </summary>
    /// <param name="range"></param>
    /// <param name="min_range"></param>
    /// <param name="max_range"></param>
    /// <param name="value_at_min_range"></param>
    /// <param name="value_at_max_range"></param>
    /// <returns></returns>
    public static float GetValueBasedOnMinAndMax(float range, float min_range, float max_range, float value_at_min_range, float value_at_max_range) {
        if ((max_range > min_range && range >= max_range) || (max_range < min_range && range <= max_range)) {
            return value_at_max_range;
        }
        if ((max_range > min_range && range <= min_range) || (max_range < min_range && range >= min_range)) {
            return value_at_min_range;
        }
        else {
            float range1 = max_range - min_range;
            float range2 = value_at_max_range - value_at_min_range;
            float step = range2 / range1;
            float range_above_min = range - min_range;
            return value_at_min_range + step * range_above_min;
        }
    }

    public static string DetermineHitTypeBasedOnAbilityWeaponClass(Constants.WeaponClass weapon_class, bool is_projectile = false) {
        if (is_projectile && weapon_class == Constants.WeaponClass.Gun) {
            return "Bullet";
        }
        else if (is_projectile && weapon_class == Constants.WeaponClass.Bow) {
            return "Arrow";
        }
        else if ((is_projectile && weapon_class == Constants.WeaponClass.Cannon) && weapon_class == Constants.WeaponClass.Gun || weapon_class == Constants.WeaponClass.Bow || weapon_class == Constants.WeaponClass.Shield || weapon_class == WeaponClass.Gauntlets) {
            return "SmallBlunt";
        }
        else if (weapon_class == Constants.WeaponClass.Axe || weapon_class == Constants.WeaponClass.Hammer || weapon_class == Constants.WeaponClass.Cannon) {
            return "LargeBlunt";
        }
        else if (weapon_class == Constants.WeaponClass.Daggers || weapon_class == Constants.WeaponClass.TwinBlades) {
            return "SmallSharp";
        }
        else if (weapon_class == Constants.WeaponClass.Longblade || weapon_class == Constants.WeaponClass.Polearm || weapon_class == Constants.WeaponClass.Greatsword) {
            return "LongSharp";
        }
        else if (weapon_class == Constants.WeaponClass.Magic) {
            return "Magic";
        }
        else if (weapon_class == Constants.WeaponClass.Ice) {
            return "Ice";
        }
        else if (weapon_class == Constants.WeaponClass.Fire) {
            return "Fire";
        }
        else {
            return "Neutral";
        }
    }

    public static string DetermineSwingSoundBasedOnAbilityWeaponClass(Constants.WeaponClass weapon_class) {
        if (weapon_class == Constants.WeaponClass.Gun || weapon_class == Constants.WeaponClass.Bow || weapon_class == Constants.WeaponClass.Shield || weapon_class == WeaponClass.Gauntlets) {
            return "SmallBlunt";
        }
        else if (weapon_class == Constants.WeaponClass.Hammer || weapon_class == Constants.WeaponClass.Cannon) {
            return "LargeBlunt";
        }
        else if (weapon_class == Constants.WeaponClass.Axe || weapon_class == Constants.WeaponClass.Greatsword) {
            return "LargeBlade";
        }
        else if (weapon_class == Constants.WeaponClass.Daggers) {
            return "SmallBlade";
        }
        else if (weapon_class == Constants.WeaponClass.TwinBlades ) {
            return "MediumBlade";
        }
        else if (weapon_class == Constants.WeaponClass.Longblade) {
            return "LongBlade";
        }
        else if (weapon_class == Constants.WeaponClass.Polearm) {
            return "Polearm";
        }
        else if (weapon_class == Constants.WeaponClass.Magic) {
            return "Magic";
        }
        else {
            return "Neutral";
        }
    }

    public static Transform GetSceneRootObject(string gameObject_name) {
        foreach(GameObject go in SceneManager.GetActiveScene().GetRootGameObjects()) {
            if(go.name == gameObject_name) {
                return go.transform;
            }
        }
        Debug.LogError($"Could not find root gameObject with name {gameObject_name} for scene {SceneManager.GetActiveScene().name}");
        return null;
    }

    public static void CopyWeaponCollider(BoxCollider2D collider, DamageType type) {
        if(type == DamageType.Heavy) {
            CopySpecificWeaponCollider(collider, "Heavy");
        }
        if(type == DamageType.Light) {
            CopySpecificWeaponCollider(collider, "Light Right");
            CopySpecificWeaponCollider(collider, "Light Left");
        }
    }

    private static void CopySpecificWeaponCollider(BoxCollider2D collider, string weapon_name) {
            MonoBehaviour.Destroy(Player.Instance.SpriteRenderers[weapon_name].Bone.GetComponent<BoxCollider2D>());
            BoxCollider2D newColl = Player.Instance.SpriteRenderers[weapon_name].Bone.gameObject.AddComponent<BoxCollider2D>();
            newColl.size = new Vector2(collider.size.x, collider.size.y);
            newColl.offset = new Vector2(collider.offset.x, collider.offset.y);
            newColl.isTrigger = true;
    }

    public static string DestinationName;
    public static bool ShouldStartFlipped = false;
 
    public static void MoveIntoArea(bool move_instant, string area_name, string area_display_name = null, bool loaded_save = false) {
        GameController.Instance.CurrentAreaOnUpdateMethod = null;
        SaveFile.Instance.CurrentAreaLoadedFromSave = loaded_save;
        if(move_instant) {
            ContinueMoveIntoArea(new string[] {area_name, area_display_name, loaded_save ? "true" : "false"});
        }
        else {
            Utils.CreateAuditLog("Starting to move into area: " + area_name);
            UIManager.Instance.ShowBlackScreen(0.3f);
            for(int i = 1; i <= 5; i++) {
                GameController.Instance.WaitAndRunMethodRealtime(0.05f * i, ChangeCurrentMusicVolume, (int)(GameController.Objects.Music.volume * 100 - GameController.Objects.Music.volume * 20 * i));
            }
            GameController.Instance.WaitAndRunMethod(0.25f, ShowAreaTransitionScreen);
            GameController.Instance.WaitAndRunMethod(0.3f, ContinueMoveIntoArea, new string[] {area_name, area_display_name, loaded_save ? "true" : "false"});
        }
    }

    public static void ShowAreaTransitionScreen() {
        UIManager.Instance.ToggleLoadingScreen(true);
    }

    public static void ContinueMoveIntoArea(string[] string_params) {
        Utils.CreateAuditLog("Moving into area: " + string_params[0]);
        UIManager.Instance.ToggleLoadingScreen(true);
        if(GameController.Instance.InterruptMusicOnDeath) {
            GameController.Objects.Music.Stop();
        }
        GameController.Instance.DynamicSortingOrders.Clear();
        GameController.Instance.StopAllCoroutines();
        GameController.Instance.LoadingNewArea = true;
        GameController.Instance.StartCoroutine(LoadSceneAsync(string_params));
    }

    private static IEnumerator LoadSceneAsync(string[] string_params) {
        GameController.Instance.ResetAllCoroutinesAndRemoveAllListeners(false);
        if(SceneManager.GetActiveScene().name == "MissionSelect") {
            Utils.GetSceneRootObject("Mission Select").gameObject.SetActive(false);
            Player.Instance.gameObject.SetActive(false);
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(string_params[0], LoadSceneMode.Single);
        UnityEngine.UI.Slider loadingBar = GameController.Objects.TransitionLoadProgress.GetComponent<UnityEngine.UI.Slider>();
        while (!asyncLoad.isDone)
        {
            loadingBar.value = asyncLoad.progress;
            yield return null;
        }
        Player.ResetPlayer();
        Player.ChangeInCombatDependantUI(false);
        UIManager.Instance.DisplayAreaTransitionScreen(string_params[1] != null ? string_params[1] : string_params[0]);
        Player.Instance.CurrentArea = string_params[0];
        if(!string.IsNullOrWhiteSpace(DestinationName)) {
            Player.Instance.transform.position = Area.ComponentInstance.transform.Find("Interactables/" + DestinationName).transform.position;
            if(ShouldStartFlipped) {
                Player.Instance.Actions.IsFlipped = true;
            }
        }
        else if(Area.ComponentInstance != null && Area.ComponentInstance?.transform?.Find("Player Start Position") != null) {
            Player.Instance.transform.position = Area.ComponentInstance.transform.Find("Player Start Position").transform.position;
        }
        else if(Area.ComponentInstance != null && Area.ComponentInstance?.transform?.Find("Player Start Position (Flipped)") != null) {
            Player.Instance.transform.position = Area.ComponentInstance.transform.Find("Player Start Position (Flipped)").transform.position;
            Player.Instance.Actions.IsFlipped = true;
        }
        else if(Area.ComponentInstance != null && Area.ComponentInstance?.transform?.Find("Player Start Positions") != null) {
            Transform positions = Area.ComponentInstance.transform.Find("Player Start Positions");
            Transform chosen_pos = positions.GetChild(UnityEngine.Random.Range(0, positions.childCount));
            Player.Instance.transform.position = chosen_pos.transform.position;
            if(chosen_pos.gameObject.name.Contains("(Flipped)")) {
                Player.Instance.Actions.IsFlipped = true;
            }
        }
        else {
            Player.Instance.transform.position = Vector2.zero;
        }
        RecentlyFoundNPCs.Clear();
        if(GameController.Instance.GameplayMode != GameplayMode.InfoPrompt && string_params[0] != "StartScreen" && string_params[0] != "MissionSelect") {
            GameController.Instance.GameplayMode = GameplayMode.Regular;
        }
        DestinationName = null;
        ShouldStartFlipped= false;
        MenuManager.Instance.ResetAndRefreshAllMenus();
        UIManager.Instance.ToggleLoadingScreen(false);
        GameController.Objects.TransitionLoadProgress.GetComponent<UnityEngine.UI.Slider>().value = 0;
        GameController.Objects.Music.volume = Settings.Instance.MusicVolume * 0.15f;
        GameController.Instance.WaitAndRunMethod(0.01f, UpdateFieldOfView);
        EventManager.FinishedLoadingArea.Invoke();
        EventManager.FinishedLoadingArea.RemoveAllListeners();
        if(string_params[2] == "false") {
            GameController.Instance.LoadingNewArea = false;
        }
        CameraController.Instance.transform.parent.transform.position = new Vector3(Player.Instance.transform.position.x, Player.Instance.transform.position.y, -100);
        Player.Instance.SetInteractPromptToClosestInteractable();
    }

    public static void UpdateFieldOfView() {
        Settings.Instance.FieldOfView = Settings.Instance.FieldOfView;
    }

    public static List<Unit> SpawnUnits(List<string> units, int enemy_level = 1, float aggressiveness = 1, bool is_elite = false)
    {
        Transform enemy_spawns = Area.Instance.transform.Find(is_elite ? "Boss Spawns" : "Enemy Spawns");
        List<Unit> result = new List<Unit>();
        for (int i = 0; i < units.Count; i++)
        {
            result.Add(SpawnUnit(units[i], enemy_spawns, enemy_level, aggressiveness));
        }
        return result;
    }

        
    public static Dictionary<string, int> GetGroupedListOfUnits(List<string> units) {
        Dictionary<string, int> result = new Dictionary<string, int>();
        for(int i = 0; i < units.Count; i++) {
            if(result.ContainsKey(units[i])) {
                result[units[i]]++;
            }
            else {
                result[units[i]] = 1;
            }
        }
        return result;
    }

    public static void ShowMissionObjective(string title_label, string objective_label, string path_to_graphic, List<string> string_params = null) {
        DestroyAllChildren(UIManager.Objects.ObjectivesDisplay.transform);
        GameObject questDisplayItem = UIManager.Objects.ObjectivesDisplay.transform.Find(title_label)?.gameObject;
        if (questDisplayItem == null) {
            GameObject questDisplay = UIManager.Objects.ObjectivesDisplay;
            questDisplayItem = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_MissionObjective")) as GameObject;
            questDisplayItem.transform.SetParent(questDisplay.transform);
        }
        questDisplayItem.transform.Find("Title").GetComponent<LabelInitializer>().SetLabel(Label.ContainsKey(title_label) ? "{" + title_label  + "}" : title_label);
        if(string_params != null) {
            questDisplayItem.transform.Find("Description").GetComponent<LabelInitializer>().string_params = string_params;
        }
        questDisplayItem.transform.Find("Description").GetComponent<LabelInitializer>().SetLabel(Label.ContainsKey(objective_label) ? "{" + objective_label  + "}" : objective_label);
        if(!string.IsNullOrWhiteSpace(path_to_graphic)) {
            questDisplayItem.transform.Find("Title/Icon").gameObject.SetActive(true);
            questDisplayItem.transform.Find("Title/Icon").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load("Sprites/" + path_to_graphic, typeof(Sprite)) as Sprite;
        }
        questDisplayItem.transform.localScale = new Vector3(1, 1, 1);
    }

    public static void ShowMissionObjective(QuestObjective objective) {
        DestroyAllChildren(UIManager.Objects.ObjectivesDisplay.transform);
        GameObject questDisplayItem = UIManager.Objects.ObjectivesDisplay.transform.Find(objective.ParentQuest.ToString())?.gameObject;
        if (questDisplayItem != null) {
            MonoBehaviour.Destroy(questDisplayItem);
        }
        SaveFile.Instance.CurrentObjectiveDisplayed = objective;
        GameObject questDisplay = UIManager.Objects.ObjectivesDisplay;
        questDisplayItem = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_MissionObjective")) as GameObject;
        questDisplayItem.transform.SetParent(questDisplay.transform);
        questDisplayItem.name = objective.ParentQuest.ToString().Replace("(Clone)", "");
        questDisplayItem.transform.Find("Title").GetComponent<LabelInitializer>().SetLabel("{" + objective.ParentQuest  + "}");
        questDisplayItem.transform.Find("Description").GetComponent<LabelInitializer>().string_params = objective.DescriptionParameters;
        questDisplayItem.transform.Find("Description").GetComponent<LabelInitializer>().SetLabel("{" + objective.ParentQuest  + "_" + objective.Number + "}");
        if(!string.IsNullOrWhiteSpace(SaveFile.Instance.GetQuest(objective.ParentQuest).Icon)) {
            questDisplayItem.transform.Find("Title/Icon").gameObject.SetActive(true);
            questDisplayItem.transform.Find("Title/Icon").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load("Sprites/" + SaveFile.Instance.GetQuest(objective.ParentQuest).Icon, typeof(Sprite)) as Sprite;
        }
        questDisplayItem.transform.localScale = new Vector3(1, 1, 1);
    }

    public static void AllEnemiesAttackPlayer() {
        foreach(Unit unit in Utils.GetAllUnits(true, true)) {
            if(unit == null || unit.UnitAI == null) {
                unit.Actions.Start();
                unit.UnitAI.NavMeshAgent = unit.GetComponent<NavMeshAgent>();
            }
            unit.CurrentTarget = Player.Instance;
            unit.InCombat = true;
        }
    }

    public static Unit SpawnUnit(string path_to_prefab, Transform enemy_spawns, int level, float aggressiveness = 1, bool infinite_spawn = false)
    {
        GameObject unit_object = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Unit/" + path_to_prefab)) as GameObject;
        Unit unit = unit_object.GetComponent<Unit>();
        Transform spawn = null;
        if(!infinite_spawn) {
            spawn = enemy_spawns.GetChild(UnityEngine.Random.Range(0, enemy_spawns.childCount));
        } 
        else {
            float biggest_distance = 0;
            foreach(Transform t in enemy_spawns) {
                if(Vector2.Distance(Player.Instance.transform.position, t.position) > biggest_distance) {
                    biggest_distance = Vector2.Distance(Player.Instance.transform.position, t.position);
                    spawn = t;
                }
            }
        }
        if (spawn.gameObject.name.Contains("(Flipped)"))
        {
            unit_object.GetComponent<Actions>().IsFlipped = true;
        }
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(spawn.transform.position, out closestHit, 500, 1))
        {
            unit_object.transform.position = new Vector3(closestHit.position.x, closestHit.position.y, 0);
        }
        unit_object.transform.SetParent(GameObject.FindGameObjectWithTag("Area").transform.Find("NPCs"));
        unit.Level = level;
        unit.ScaleStatsWithLevel = true;
        unit.InitializeStats();
        unit.UnitAI.BaseAggressiveness = aggressiveness;
        unit_object.gameObject.name = unit_object.gameObject.name + "_" + Guid.NewGuid().ToString();
        if(!infinite_spawn) {
            spawn.transform.SetParent(spawn.transform.parent.parent);
            MonoBehaviour.Destroy(spawn.gameObject);
        }
        return unit;
    }

    public static void CreateAuditLog(string log)
    {
        if(Settings.Instance.CreateAuditLogs)
        {
            Debug.Log(GetTimeStamp() + ": " + log);
        }
        else
        {
            UIManager.Instance.GetComponent<ConsoleToGUI>().Log("(" + GetTimeStamp() + ") " + log, null, LogType.Log);
        }
    }

    public static void AdjustRemainingCounteredAnimation(Unit unit) {
        Effect countered = unit.CurrentEffects.FirstOrDefault(effect => effect.GetType() == typeof(Effect_BackstepCountered) || effect.GetType() == typeof(Effect_RollCountered) || effect.GetType() == typeof(Effect_RiposteCountered));
        if(countered == null || countered.EffectEnded) {
            return;
        }
        unit.Animator.SetFloat("Special Animation Speed", (4f - (4f * unit.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime)) / countered.RemainingDuration);
    }

    public static void PushUnitIntoPosition(Unit unit, Vector3 position, Ability source, float intensity = 0.35f)
    {
        unit.ApplyForce((position - unit.transform.position) * Vector2.Distance(unit.transform.position, position) * intensity, source);
    }

    public static string InsertLabelsIntoText(string text, GameObject game_object = null, string effect_name = "")
    {
        if(string.IsNullOrEmpty(text))
        {
            return text;
        }
        string result = "";
        string label_key = "";
        string icon_key = "";
        bool coloring_text = false;
        bool reading_key = false;
        bool reading_icon = false;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '{')
            {
                if(text.Length > i+5 && (text[i+3] == '[' || text[i+4] == '[')) {
                    if(text.Length > i+5) {
                        string check = text.Substring(i+3, 3);
                        if(check == "[D]") {
                            result += $"<color={Colors.LabelDamage}>";
                            coloring_text = true;
                        }
                        else if(check == "[I]" || check == "[H]") {
                            result += $"<color={Colors.LabelHealth}>";
                            coloring_text = true;
                        }
                        else if(check == "[S]") {
                            result += $"<color={Colors.LabelStagger}>";
                            coloring_text = true;
                        }
                        else if(check == "[E]") {
                            result += $"<color={Colors.LabelEnergy}>";
                            coloring_text = true;
                        }
                    }
                    if(text.Length > i+6) {
                        string check = text.Substring(i+3, 4);
                        if(check == "%[D]" || check == "[HD]" || check == "[LD]" || check == "[RD]" || check == "[MD]" || check == "[TD]" || check == "[WD]") {
                            result += $"<color={Colors.LabelDamage}>";
                            coloring_text = true;
                        }
                        else if(check == "[HI]" || check == "[LI]" || check == "[RI]" || check == "[MI]" || check == "%[H]" || check == "%[I]" || check == "[TI]" || check == "[WI]") {
                            result += $"<color={Colors.LabelHealth}>";
                            coloring_text = true;
                        }
                        else if(check == "[HS]" || check == "[LS]" || check == "[RS]" || check == "[MS]" || check == "[SD]" || check == "%[S]" || check == "[SB]" || check == "[TS]" || check == "[WS]") {
                            result += $"<color={Colors.LabelStagger}>";
                            coloring_text = true;
                        }
                        else if(check == "%[E]" || check == "[EG]") {
                            result += $"<color={Colors.LabelEnergy}>";
                            coloring_text = true;
                        }
                        else if(check == "[CD]") {
                            result += $"<color={Colors.LabelCooldown}>";
                            coloring_text = true;
                        }
                    }
                    if(text.Length > i+7) {
                        string check = text.Substring(i+3, 5);
                        if(check == "%[HD]" || check == "%[LD]" || check == "%[RD]" || check == "%[MD]" || check == "%[TD]" || check == "%[WD]") {
                            result += $"<color={Colors.LabelDamage}>";
                            coloring_text = true;
                        }
                        else if(check == "%[HI]" || check == "%[LI]" || check == "%[RI]" || check == "%[MI]" || check == "%[TI]" || check == "%[WI]") {
                            result += $"<color={Colors.LabelHealth}>";
                            coloring_text = true;
                        }
                        else if(check == "%[HS]" || check == "%[LS]" || check == "%[RS]" || check == "%[MS]" || check == "%[SB]" || check == "%[TS]" || check == "%[WS]") {
                            result += $"<color={Colors.LabelStagger}>";
                            coloring_text = true;
                        }
                        else if(check == "%[EG]") {
                            result += $"<color={Colors.LabelEnergy}>";
                            coloring_text = true;
                        }
                        else if(check == "%[CD]") {
                            result += $"<color={Colors.LabelCooldown}>";
                            coloring_text = true;
                        }
                        else if(check == "[CDR]") {
                            result += $"<color={Colors.LabelCooldown}>";
                            coloring_text = true;
                        }
                    }
                    if(text.Length > i+8) {
                        string check = text.Substring(i+3, 6);
                        if(check == "%[CDR]") {
                            result += $"<color={Colors.LabelCooldown}>";
                            coloring_text = true;
                        }
                    } 
                }
                if (Char.IsDigit(text[i + 1]) && text[i + 2] == '}')
                {
                    result += "{" + text[i + 1] + "}";
                    i += 2;
                    if(coloring_text) {
                        coloring_text = false;
                        if(text[i+1] == '%') {
                            i++;
                            result += "%</color>";
                        }
                        else {
                            result += "</color>";
                        }
                    }
                }
                else
                {
                    reading_key = true;
                }
            }
            else if (text[i] == '}')
            {
                reading_key = false;
                if(coloring_text) {
                    coloring_text = false;
                    result += "</color>";
                }
                if(!Label.ContainsKey(label_key))
                {
                    if(game_object != null) {
                        Debug.LogWarning("Could not find label '" + label_key + "' for gameObject: " + Utils.GetGameObjectPath(game_object));
                    }
                    result += label_key;
                }
                else {
                    result += Label.Get(label_key);
                }
                if (label_key.EndsWith("Description") && Label.ContainsKey(label_key.Replace("Description", "DescriptionDetailed")))
                {
                    result += " <link=\"" + label_key + "Detailed\"><sprite name=\"Detailed\"></link>";
                }
                label_key = "";
            }
            else if (text[i] == '[')
            {
                reading_icon = true;
            }
            else if (text[i] == ']')
            {
                reading_icon = false;
                result += GetIconForPhrase(icon_key);
                icon_key = "";
            }
            else if (reading_key)
            {
                label_key += text[i];
            }
            else if (reading_icon)
            {
                icon_key += text[i];
            }
            else
            {
                result += text[i];
            }
        }
        return result;
    }

    public static string GetFormattedFlag(string non_formatted_flag) {
        return non_formatted_flag.Replace("[Cycle]", SaveFile.Instance.Cycle.ToString());
    }

    public static bool CheckIfItemGradeSufficientLevel(Item.ItemGrade grade, string warning_label) {
        if(grade == Item.ItemGrade.Excellent && SaveFile.Instance.Level < 15) {
            NotificationController.ShowTextNotification(warning_label, new List<string> {"15", Label.Get("ItemGrade_Excellent_Colored")});
            return false;
        }
        else if(grade == Item.ItemGrade.Masterful && SaveFile.Instance.Level < 30) {
            NotificationController.ShowTextNotification(warning_label, new List<string> {"30", Label.Get("ItemGrade_Masterful_Colored")});
            return false;
        }
        else if(grade == Item.ItemGrade.Flawless && SaveFile.Instance.Level < 40) {
            NotificationController.ShowTextNotification(warning_label, new List<string> {"40", Label.Get("ItemGrade_Flawless_Colored")});
            return false;
        }
        else if(grade == Item.ItemGrade.Ultimate && SaveFile.Instance.Level < 50) {
            NotificationController.ShowTextNotification(warning_label, new List<string> {"50", Label.Get("ItemGrade_Ultimate_Colored")});
            return false;
        }
        return true;
    }

    public static string GetIconForPhrase(string phrase, string effect_name = "") {
        if(phrase.EndsWith("ButtonPress")) {
            Settings.Keybind keybind = Settings.Instance.Keybinds.FirstOrDefault(k => k.ActionName == phrase);
            if(keybind == null) {
                return "???";
            }
            if(phrase == "ManualAim") {
                return "<sprite name=\"" + (Settings.Instance.GamepadType == "Xbox" ? "Xbox_" : "PS_") + "rightStickup\">";
            }
            if(phrase == "DetailedDescriptionsButtonPress") {
                return "<sprite name=\"" + (Settings.Instance.GamepadType == "Xbox" ? "Xbox_" : "PS_") + "start\">";
            }
            else if(Settings.Instance.ControlScheme == "Keyboard") {
                return "<sprite name=\"Keyboard_" + keybind.KeyboardBinding1 + "\">";
            }
            if(Settings.Instance.ControlScheme == "Gamepad" && phrase.Contains("Ability")) {
                Settings.Keybind abilitiesKeybind = Settings.Instance.Keybinds.FirstOrDefault(k => k.ActionName == "GamepadAbilitiesButtonPress");
                return $"<sprite name=\"{(Settings.Instance.GamepadType == "Xbox" ? "Xbox_" : "PS_") + abilitiesKeybind.GamepadBinding1}\">+<sprite name=\"" + (Settings.Instance.GamepadType == "Xbox" ? "Xbox_" : "PS_") + keybind.GamepadBinding1 + "\">";
            }
            if(Settings.Instance.ControlScheme == "Gamepad" && (phrase.Contains("Item") || phrase.Contains("Scout"))) {
                Settings.Keybind itemsKeybind = Settings.Instance.Keybinds.FirstOrDefault(k => k.ActionName == "GamepadItemsButtonPress");
                return $"<sprite name=\"{(Settings.Instance.GamepadType == "Xbox" ? "Xbox_" : "PS_") + itemsKeybind.GamepadBinding1}\">+<sprite name=\"" + (Settings.Instance.GamepadType == "Xbox" ? "Xbox_" : "PS_") + keybind.GamepadBinding1 + "\">";
            }
            else if(Settings.Instance.ControlScheme == "Gamepad") {
                return "<sprite name=\"" + (Settings.Instance.GamepadType == "Xbox" ? "Xbox_" : "PS_") + keybind.GamepadBinding1 + "\">";
            }
            return "???";
        }
        else if(phrase=="RED") {
            return $"<color={Colors.LabelHealth}>";
        }
        else if(phrase=="/RED") {
            return "</color>";
        }
        else if(phrase=="PURPLE") {
            return $"<color={Colors.LabelStagger}>";
        }
        else if(phrase=="/PURPLE") {
            return "</color>";
        }
        else if(phrase=="BLUE") {
            return $"<color={Colors.LabelEnergy}>";
        }
        else if(phrase=="/BLUE") {
            return "</color>";
        }
        else if(phrase=="GREY") {
            return $"<color={Colors.LabelCooldown}>";
        }
        else if(phrase=="/GREY") {
            return "</color>";
        }
        else if(phrase=="GREEN") {
            return $"<color={Colors.LabelDamage}>";
        }
        else if(phrase=="/GREEN") {
            return "</color>";
        }
        else if(phrase=="/C") {
            return "</color>";
        }
        else {
            string linkType = 
                Label.ContainsKey("Stat_" + phrase + "_Description") ? "Stat" :
                (IconShortcuts.ContainsKey(phrase) && Label.ContainsKey("Stat_" + IconShortcuts[phrase] + "_Description")) ? "StatShort" :
                Label.ContainsKey("Effect_" + phrase + "_Description") ? "Basic" :
                (IconShortcuts.ContainsKey(phrase) && Label.ContainsKey("Effect_" + IconShortcuts[phrase] + "_Description")) ? "Short" : "";
            return 
                (linkType == "Stat" ? $"<link=\"Stat_{phrase}_Description>" :
                linkType == "StatShort" ? $"<link=\"Stat_{IconShortcuts[phrase]}_Description>" :
                linkType == "Basic" ? $"<link=\"Effect_{phrase}_Description>" : 
                linkType == "Short" ? $"<link=\"Effect_{IconShortcuts[phrase]}_Description>" : "") +
                "<sprite name=\"" + (IconShortcuts.ContainsKey(phrase) ? IconShortcuts[phrase] : phrase) + "\">" + 
                (linkType != "" ? "</link>" : "");
        }
    }

    public static Dictionary<string, string> IconShortcuts = new Dictionary<string, string> {
        {"D", "Damage"},
        {"HD", "HeavyDamage"},
        {"LD", "LightDamage"},
        {"RD", "RangedDamage"},
        {"MD", "MagicDamage"},
        {"I", "Injury"},
        {"HI", "HeavyInjury"},
        {"LI", "LightInjury"},
        {"RI", "RangedInjury"},
        {"MI", "MagicInjury"},
        {"S", "Stagger"},
        {"HS", "HeavyStagger"},
        {"LS", "LightStagger"},
        {"RS", "RangedStagger"},
        {"MS", "MagicStagger"},
        {"H", "Health"},
        {"SB", "StaggerBar"},
        {"E", "Energy"},
        {"EG", "EnergyGain"},
        {"A", "Armor"},
        {"T", "Tenacity"},
        {"CD", "Cooldown"},
        {"CDR", "CooldownReduction"},
        {"C", "Control"},
        {"MOV", "MovementSpeed"},
        {"MOVE", "MovementSpeed"},
        {"SPEED", "MovementSpeed"},
        {"SP", "MovementSpeed"},
        {"AS", "AttackSpeed"},
        {"HAS", "HeavyAttackSpeed"},
        {"LAS", "LightAttackSpeed"},
        {"RAS", "RangedAttackSpeed"},
        {"MAS", "MagicAttackSpeed"},
        {"WD", "WeaponDamage"},
        {"WI", "WeaponInjury"},
        {"WS", "WeaponStagger"},
        {"TD", "TechniqueDamage"},
        {"TI", "TechniqueInjury"},
        {"TS", "TechniqueStagger"},
        {"BA", "BasicAttack"},
    };

    public static bool CheckIfPlayerIsFacingUnit(Unit unit)
    {
        return (Player.Instance.transform.position.x > unit.transform.position.x && Player.Instance.Actions.IsFlipped) || (Player.Instance.transform.position.x <= unit.transform.position.x && Player.Instance.Actions.IsFlipped == false);
    }

    public static string GetImageNameForStat(string stat_name) 
    {
        switch(stat_name)
        {
            case "HealthRegenerationInCombat": return "HealthRegeneration";
            case "HealthGainFromBasicAttackDamage": return "HealthRegeneration";
            case "HealthGainFromAbilityDamage": return "HealthRegeneration";
            case "BetterPotions": return "MorePotionCharges";
            case "BetterGrenades": return "MoreGrenadeCharges";
            default: return stat_name;
        }
    }

    public static void AddPowerUpToPlayer(string power_up) {
        if(power_up.Contains("Stance_")) {
            Type type = Type.GetType(power_up.Replace("_Unlock", "").Replace("_Upgrade1", "").Replace("_Upgrade2", "").Replace("_Upgrade3", ""));
            string upgrade = power_up.Contains("_Upgrade1") ? "1" : power_up.Contains("_Upgrade2") ? "2" : power_up.Contains("_Upgrade3") ? "3" : "0";
            SaveFile.Instance.UnlockStance(type, upgrade);
        }
        else if(power_up.Contains("_Unlock")) {
            SaveFile.Instance.UnlockAbility(Type.GetType(power_up.Replace("_Unlock", "")));
        }
        else if(power_up.Contains("_UpgradeA")) {
            SaveFile.Instance.UnlockAbilityMasteryA(Type.GetType(power_up.Replace("_UpgradeA", "")));
        }
        else if(power_up.Contains("_UpgradeB")) {
            SaveFile.Instance.UnlockAbilityMasteryB(Type.GetType(power_up.Replace("_UpgradeB", "")));
        }
        else {
            SaveFile.Instance.SurvivalPowerUps.Add(power_up);
        }
    }

    public static void ShowLevelUpSelection(List<string> selection)
    {
        /*if(SaveFile.Instance.SkillTreeSurvivalType) {
            return;
        }
        GameController.Instance.GameplayMode = Constants.GameplayMode.InfoPrompt;
        PassiveSelect passive_select = GameController.GameObjects.LevelUpPassives.GetComponent<PassiveSelect>();
        passive_select.PassiveList1 = selection;
        passive_select.PassiveList2 = new List<string> {};
        passive_select.InitializeOptions();*/
    }

    public static string GetFormattedFloat(float number, int force_show_decimals = -1)
    {
        if(force_show_decimals == -1) {
            return number < 100 ? Math.Round(number, 1).ToString().Replace(",", ".") : Math.Round(number, 0).ToString().Replace(",", ".");
        }
        else if(force_show_decimals == 0) {
            return Math.Round(number, 0).ToString().Replace(",", ".");
        }
        float rounded = (float)Math.Round(number, 2);
        int digits = GetFloatDigits(rounded);
        if(force_show_decimals == 2 && digits == 0) {
            return rounded.ToString().Replace(",", ".") + ".00";
        } 
        else if(force_show_decimals == 2 && digits == 1) {
            return rounded.ToString().Replace(",", ".") + "0";
        } 
        else if(force_show_decimals == 1 && digits == 0) {
            return rounded.ToString().Replace(",", ".") + ".0";
        } 
        return rounded.ToString().Replace(",", ".");
    }

    public static int GetFloatDigits(float number) {
        int i;
        for(i = 0; number % 1 != 0; i++) {
            number *= 10;
        }
        return i;
    }


    public static List<string> RoundAllNumbers(List<string> string_params)
    {
        List<string> rounded_params = new List<string>();
        foreach (string param in string_params)
        {
            float parsed_result;
            if (float.TryParse(param, out parsed_result))
            {
                rounded_params.Add(((int)Math.Round(parsed_result, 0)).ToString());
            }
            else
            {
                rounded_params.Add(param);
            }
        }
        return rounded_params;
    }

    public static float GetExpectedPowerForLevel(int level)
    {
        if (level < 1)
        {
            return GetValueBasedOnMinAndMax(level, -25, 1, 0.3f, Constants.EXPECTED_POWER_AT_LEVEL_1);
        }
        else if(level <= 10)
        {
            return GetValueBasedOnMinAndMax(level, 1, 10, Constants.EXPECTED_POWER_AT_LEVEL_1, Constants.EXPECTED_POWER_AT_LEVEL_10);
        }
        else if (level <= 20)
        {
            return GetValueBasedOnMinAndMax(level, 10, 20, Constants.EXPECTED_POWER_AT_LEVEL_10, Constants.EXPECTED_POWER_AT_LEVEL_20);
        }
        else if (level <= 30)
        {
            return GetValueBasedOnMinAndMax(level, 20, 30, Constants.EXPECTED_POWER_AT_LEVEL_20, Constants.EXPECTED_POWER_AT_LEVEL_30);
        }
        else if (level <= 40)
        {
            return GetValueBasedOnMinAndMax(level, 30, 40, Constants.EXPECTED_POWER_AT_LEVEL_30, Constants.EXPECTED_POWER_AT_LEVEL_40);
        }
        else if (level <= 50)
        {
            return GetValueBasedOnMinAndMax(level, 40, 50, Constants.EXPECTED_POWER_AT_LEVEL_40, Constants.EXPECTED_POWER_AT_LEVEL_50);
        }
        else
        {
            return GetValueBasedOnMinAndMax(level, 50, 100, Constants.EXPECTED_POWER_AT_LEVEL_50, Constants.EXPECTED_POWER_AT_LEVEL_100);
        }
    }

    public static int GetScaledExperienceGain(int level, int base_exp)
    {
        if (level < 1)
        {
            return (int)(base_exp * GetValueBasedOnMinAndMax(level, -25, 1, 0.5f, 1)) / 100 * 100;
        }
        else if (level <= 10)
        {
            return (int)(base_exp * GetValueBasedOnMinAndMax(level, 1, 10, 1, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_10)) / 100 * 100;
        }
        else if (level <= 20)
        {
            return (int)(base_exp * GetValueBasedOnMinAndMax(level, 10, 20, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_10, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_20)) / 100 * 100;
        }
        else if (level <= 30)
        {
            return (int)(base_exp * GetValueBasedOnMinAndMax(level, 20, 30, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_20, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_30)) / 100 * 100;
        }
        else if (level <= 40)
        {
            return (int)(base_exp * GetValueBasedOnMinAndMax(level, 30, 40, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_30, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_40)) / 100 * 100;
        }
        else if (level <= 50)
        {
            return (int)(base_exp * GetValueBasedOnMinAndMax(level, 40, 50, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_40, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_50)) / 100 * 100;
        }
        else
        {
            return (int)(base_exp * GetValueBasedOnMinAndMax(level, 50, 100, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_50, Constants.EXPERIENCE_MULTIPLIER_AT_LEVEL_100)) / 100 * 100;
        }
    }

    public static float GetExpectedControlLevel(int level)
    {
        if (level < 1)
        {
            return GetValueBasedOnMinAndMax(level, -25, 1, 0.5f, 1);
        }
        else if (level <= 10)
        {
            return GetValueBasedOnMinAndMax(level, 1, 10, 1, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_10);
        }
        else if (level <= 20)
        {
            return GetValueBasedOnMinAndMax(level, 10, 20, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_10, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_20);
        }
        else if (level <= 30)
        {
            return GetValueBasedOnMinAndMax(level, 20, 30, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_20, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_30);
        }
        else if (level <= 40)
        {
            return GetValueBasedOnMinAndMax(level, 30, 40, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_30, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_40);
        }
        else if (level <= 50)
        {
            return GetValueBasedOnMinAndMax(level, 40, 50, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_40, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_50);
        }
        else
        {
            return GetValueBasedOnMinAndMax(level, 50, 100, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_50, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_100);
        }
    }

    public static int GetCalculatedGain(int amount) {
        int scaled_amount = (int)(SaveFile.Instance.DifficultyLevel == 0 ? amount * 1.5f : amount);
        return scaled_amount - (scaled_amount % 10);
    }

    public static int GetLevelAdjustmentBasedOnUnitCount(int unit_count) {
        switch(unit_count) {
            case 0: return 0;
            case 1: return 10;
            case 2: return 6;
            case 3: return 2;
            case 4: return -1;
            case 5: return -3;
            case 6: return -5;
            default: return unit_count * -1;
        }
    }

    public static float GetAggresivenessBasedOnUnitCount(int unit_count) {
        return 2.5f / unit_count + 0.5f;
    }

    public static Sprite LoadSpriteFromMultiple(string fileName, string spriteName) {
        Sprite[] all = Resources.LoadAll<Sprite>("Sprites/" + fileName);
        foreach( var s in all)
        {
            if (s.name == spriteName)
            {
                return s;
            }
        }
        return null;
    }

    public static void ScrollToTopOrBottom(Transform transform, bool top = true) {
        Canvas.ForceUpdateCanvases();
		UpdateLayout_Internal(transform);
    }

	private static void UpdateLayout_Internal(Transform xform)
	{
		if (xform == null || xform.Equals(null))
		{
			return;
		}

		for (int x = 0; x < xform.childCount; ++x)
		{
			UpdateLayout_Internal(xform.GetChild(x));
		}

		foreach (var layout in xform.GetComponents<LayoutGroup>())
		{
			layout.CalculateLayoutInputVertical();
			layout.CalculateLayoutInputHorizontal();
		}
		foreach (var fitter in xform.GetComponents<ContentSizeFitter>())
		{
			fitter.SetLayoutVertical();
			fitter.SetLayoutHorizontal();
		}
	}
    public static string GetGameObjectPath(GameObject obj)
    {
        if(obj == null) {
            return "null";
        }
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public static bool CheckIfGivenUnitIsInFrontOfUnit(Unit unit, Unit unit_to_check_if_in_front) {
        return (unit.Actions.IsFlipped && unit_to_check_if_in_front.transform.position.x < unit.transform.position.x) || (!unit.Actions.IsFlipped && unit_to_check_if_in_front.transform.position.x > unit.transform.position.x);
    }

    public static void UpdateIndicatorScaleBasedOnTime(GameObject indicator, float remaining_time) {
        if(indicator == null) {
            return;
        }
        else if(remaining_time <= 0) {
            indicator.transform.localScale = new Vector3(1, 1, 1);
            return;
        }
        else {
            indicator.transform.localScale = new Vector3(1 + remaining_time * (Constants.NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_SIZE - 1) * 10, 1 + remaining_time * (Constants.NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_SIZE - 1) * 10, 1);
        }
    }

    public static void CopyItemAppearanceForPlayer(Constants.ItemType type, string prefab_name)
    {
        GameObject item_to_copy_appearance_from = MonoBehaviour.Instantiate(Resources.Load(type == ItemType.None ? $"Prefabs/{prefab_name}" : $"Prefabs/{type}/{prefab_name}")) as GameObject;
        if (type == ItemType.Heavy)
        {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers[type.ToString()].SpriteRenderer.gameObject, item_to_copy_appearance_from.gameObject, false);
        }
        else if(type == ItemType.Ranged) {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers[type.ToString()].SpriteRenderer.gameObject, item_to_copy_appearance_from.gameObject, false);
            if(prefab_name.Contains("Bow")) {
                Player.Instance.SpriteRenderers["Ranged"].SpriteRenderer.GetComponent<SpriteSkin>().boneTransforms[0] = null;
            }
            else if(prefab_name.Contains("Gun")) {

            }
        }
        else  if (type == ItemType.Light)
        {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Light Right"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Light Right").gameObject, false);
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Light Left"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Light Left").gameObject, false);
            if (Player.Instance.Actions.IsFlipped)
            {
                FlipNonSymmetricSpritePlacement("Light", Player.Instance);
            }
        }
        else if (type == ItemType.Gloves)
        {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Right Hand"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Right Hand").gameObject, false);
            Player.Instance.SpriteRenderers["Right Hand"].Bone.transform.localScale = item_to_copy_appearance_from.transform.Find("Right Hand/Right Hand Bone").localScale;
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Left Hand"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Left Hand").gameObject, false);
            Player.Instance.SpriteRenderers["Left Hand"].Bone.transform.localScale = item_to_copy_appearance_from.transform.Find("Left Hand/Left Hand Bone").localScale;
            if(Player.Instance.Actions.IsFlipped)
            {
                FlipNonSymmetricSpritePlacement("Hand", Player.Instance);
            }
        }
        else if (type == ItemType.Helmet)
        {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Hair"].SpriteRenderer.gameObject, item_to_copy_appearance_from.gameObject, false);
        }
        else if (type == ItemType.Outfit)
        {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Lower Body"].SpriteRenderer.gameObject, item_to_copy_appearance_from.gameObject, false);
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Upper Body"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Lower Body Bone/Upper Body").gameObject, false);
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Right Arm"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Lower Body Bone/Upper Body/Upper Body Bone/Right Arm").gameObject, false);
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Left Arm"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Lower Body Bone/Upper Body/Upper Body Bone/Left Arm").gameObject, false);
            if (Player.Instance.Actions.IsFlipped)
            {
                FlipNonSymmetricSpritePlacement("Arm", Player.Instance);
            }
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Right Leg"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Lower Body Bone/Right Leg").gameObject, false);
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Left Leg"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Lower Body Bone/Left Leg").gameObject, false);
            if (Player.Instance.Actions.IsFlipped)
            {
                FlipNonSymmetricSpritePlacement("Leg", Player.Instance);
            }
        }
        else if (type == ItemType.Boots)
        {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Right Foot"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Right Foot").gameObject, false);
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Left Foot"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.Find("Left Foot").gameObject, false);
            if (Player.Instance.Actions.IsFlipped)
            {
                FlipNonSymmetricSpritePlacement("Foot", Player.Instance);
            }
        }
        else if (type == ItemType.Tool || type == ItemType.Quest || type == ItemType.None)
        {
            CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Consumable"].SpriteRenderer.gameObject, item_to_copy_appearance_from.transform.gameObject, false);
            Player.Instance.SpriteRenderers["Consumable"].SpriteRenderer.transform.localScale = Vector3.one;
        }
        Player.Instance.UnitColorChange.UpdateMaterialProperties();
        MonoBehaviour.Destroy(item_to_copy_appearance_from);
    }

    public static bool CheckIfGameObjectIsBehindUnit(GameObject game_object, Unit unit) {
        if(unit.Actions.IsFlipped && game_object.transform.position.x > unit.transform.position.x) {
            return true;
        }
        if(unit.Actions.IsFlipped == false && game_object.transform.position.x < unit.transform.position.x) {
            return true;
        }
        return false;
    }

    public static void FlipNonSymmetricSpritePlacement(string element_name, Unit unit)
    {
        string primary_element = unit.Actions.IsFlipped ? "Left " + element_name : "Right " + element_name;
        string secondary_element = unit.Actions.IsFlipped ? "Right " + element_name : "Left " + element_name;
        if (element_name == "Light")
        {
            primary_element = unit.Actions.IsFlipped ? element_name + " Left": element_name + " Right";
            secondary_element = unit.Actions.IsFlipped ? element_name + " Right" : element_name + " Left";
        }
        ColorChange primary_color_change = unit.SpriteRenderers[primary_element].ColorChange;
        Dictionary<string, object> color_change_params = new Dictionary<string, object>();
        foreach (FieldInfo field in typeof(ColorChange).GetFields())
        {
            color_change_params.Add(field.Name, field.GetValue(primary_color_change));
        }
        ColorChange secondary_color_change = unit.SpriteRenderers[secondary_element].ColorChange;
        string first_element_category = unit.SpriteRenderers[primary_element].SpriteResolver.GetCategory();
        string first_element_label = unit.SpriteRenderers[primary_element].SpriteResolver.GetLabel();
        unit.SpriteRenderers[primary_element].SpriteResolver.SetCategoryAndLabel(unit.SpriteRenderers[secondary_element].SpriteResolver.GetCategory(), unit.SpriteRenderers[secondary_element].SpriteResolver.GetLabel());
        foreach (FieldInfo field in typeof(ColorChange).GetFields())
        {
            field.SetValue(primary_color_change, field.GetValue(secondary_color_change));
        }
        primary_color_change.UpdateMaterialProperties();
        unit.SpriteRenderers[primary_element].SpriteResolver.ResolveSpriteToSpriteRenderer();
        unit.SpriteRenderers[secondary_element].SpriteResolver.SetCategoryAndLabel(first_element_category, first_element_label);
        foreach (FieldInfo field in typeof(ColorChange).GetFields())
        {
            field.SetValue(secondary_color_change, color_change_params[field.Name]);
        }
        secondary_color_change.UpdateMaterialProperties();
        unit.SpriteRenderers[secondary_element].SpriteResolver.ResolveSpriteToSpriteRenderer();
    }

    public static void CopyGameObjectAppearance(GameObject copy_into, GameObject copy_from, bool should_destroy_object_being_copied = true)
    {
        copy_into.GetComponent<SpriteResolver>().SetCategoryAndLabel(copy_from.GetComponent<SpriteResolver>().GetCategory(), copy_from.GetComponent<SpriteResolver>().GetLabel());
        copy_into.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();

        ColorChange element_sprite_color_change = copy_into.GetComponent<ColorChange>();
        ColorChange replacemenet_sprite_color_change = copy_from.GetComponent<ColorChange>();
        foreach (FieldInfo field in typeof(ColorChange).GetFields())
        {
            field.SetValue(element_sprite_color_change, field.GetValue(replacemenet_sprite_color_change));
        }
        if (copy_from.transform.childCount > 0 && copy_from.transform.GetChild(0).gameObject.name.Contains("Bone"))
        {
            copy_into.transform.GetChild(0).localScale = copy_from.transform.GetChild(0).localScale;
        }

        element_sprite_color_change.UpdateMaterialProperties();
        BoxCollider2D col1 = copy_into.transform.GetChild(0).GetComponent<BoxCollider2D>();
        BoxCollider2D col2 = copy_from.transform.GetChild(0).GetComponent<BoxCollider2D>();
        if(col1 != null && col2 != null) {
            col1.offset = col2.offset;
            col1.size = col2.size;
        }

        if(copy_into.transform.childCount > 0 && copy_into.transform.GetChild(0).transform.Find("Weapon Trail") != null) {
            copy_into.transform.GetChild(0).transform.Find("Weapon Trail").transform.localScale = copy_from.transform.GetChild(0).transform.Find("Weapon Trail").transform.localScale;
            copy_into.transform.GetChild(0).transform.Find("Weapon Trail").transform.localPosition = copy_from.transform.GetChild(0).transform.Find("Weapon Trail").transform.localPosition;
            copy_into.transform.GetChild(0).transform.Find("Weapon Trail").transform.localRotation = copy_from.transform.GetChild(0).transform.Find("Weapon Trail").transform.localRotation;
        }
        if(should_destroy_object_being_copied) {
            MonoBehaviour.Destroy(copy_from.gameObject);
        }
    }

    public static GameObject CreateVisualEffect(SourceOfEffect source, string prefab_name, float pos_x = 0, float pos_y = 0)
    {
        if(Area.Instance == null) {
            return null;
        }
        GameObject new_vfx = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/VisualEffect_" + prefab_name)) as GameObject;
        new_vfx.name = prefab_name;
        new_vfx.transform.SetParent(Area.Instance.transform);
        SetUpTransform(source, new_vfx, pos_x, pos_y);
        AddDynamicSortOrders(new_vfx);
        if (source?.SourceAbility != null)
        {
            ScaleParticleSystemsWithAttackSpeed(source.SourceAbility.AttackSpeed, new_vfx);
        }
        return new_vfx;
    }

    public static AreaOfEffect CreateAreaOfEffect(SourceOfEffect source, string prefab_name, float pos_x = 0, float pos_y = 0)
    {
        if(Area.Instance == null) {
            return null;
        }
        UnityEngine.Object flipped_asset = Resources.Load("Prefabs/AreaOfEffect/AreaOfEffect_" + prefab_name + "_Flipped");
        GameObject new_aoe;
        if (source.User.Actions.IsFlipped && flipped_asset != null)
        {
            new_aoe = MonoBehaviour.Instantiate(flipped_asset) as GameObject;
            new_aoe.name = prefab_name + "_Flipped";
        }
        else
        {
            new_aoe = MonoBehaviour.Instantiate(Resources.Load("Prefabs/AreaOfEffect/AreaOfEffect_" + prefab_name)) as GameObject;
            new_aoe.name = prefab_name;
        }
        new_aoe.transform.SetParent(Area.Instance.transform);
        SetUpTransform(source, new_aoe, pos_x, pos_y);
        AddDynamicSortOrders(new_aoe);
        ScaleParticleSystemsWithAttackSpeed(source.SourceAbility.AttackSpeed, new_aoe);
        AreaOfEffect[] aoes = new_aoe.GetComponentsInChildren<AreaOfEffect>(true);
        for(int i =0; i <aoes.Length; i++)
        {
            aoes[i].SourceAbility = source.SourceAbility;
        }
        return aoes[0];
    }

    public static Sprite LoadSaveFileScreenshot(string file_path) {
        if (string.IsNullOrEmpty(file_path)) {
            return null;
        }
        if (System.IO.File.Exists(file_path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(file_path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }

    public static float GetEffectiveCrowdControlDuration(Unit source_of_cc, Unit target_of_cc) {
        float controlVsTenacity = source_of_cc.Control.Current - target_of_cc.Tenacity.Current;
        if(controlVsTenacity >= 0) {
            return 1 + controlVsTenacity / 100;
        }
        else {
            return 1 / (1 + Math.Abs(controlVsTenacity / 100));
        }
    }

    public static Projectile CreateProjectile(SourceOfEffect source, string prefab_name, float pos_x = 0, float pos_y = 0)
    {
        if(Area.Instance == null) {
            return null;
        }
        GameObject new_projectile;
        if(prefab_name == "GunBasicAttack" && source.User.CheckIfUnderEffect(typeof(Effect_ShadowInfusion_Ultimate)) && ((Effect_ShadowInfusion_Ultimate)source.User.GetEffect(typeof(Effect_ShadowInfusion_Ultimate))).DamageCategory == DamageType.Ranged) {
            new_projectile = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Projectile/Projectile_SpiritWeapon")) as GameObject;
        }
        else if(prefab_name == "CannonBasicAttack" && Player.Instance.CurrentStance.StanceEffect is not Stance_None) {
            new_projectile = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Projectile/Projectile_CannonBasicAttack")) as GameObject;
        }
        else
        {
            new_projectile = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Projectile/Projectile_" + prefab_name)) as GameObject;
        }
        new_projectile.name = prefab_name;
        new_projectile.transform.SetParent(Area.Instance.transform);
        SetUpTransform(source, new_projectile, pos_x, pos_y);
        AddDynamicSortOrders(new_projectile);
        ScaleParticleSystemsWithAttackSpeed(source.SourceAbility.AttackSpeed, new_projectile);
        new_projectile.transform.Rotate(0, 0, source.User.Actions.IsFlipped ? 90 : -90, Space.Self);
        Projectile projectile = new_projectile.GetComponent<Projectile>();
        projectile.SourceAbility = source.SourceAbility;
        if (source.User.CurrentTarget != null)
        {
            new_projectile.transform.up = Utils.GetDirectionVector(source.User.ProjectileSpawnLocation.transform.position, source.User.CurrentTarget.transform.position, source.User.Actions.IsFlipped, 60);
        }
        else
        {
            new_projectile.transform.up = Utils.GetDirectionVector(Vector2.zero, source.User.Actions.SavedAimDirection != Vector2.zero ? source.User.Actions.SavedAimDirection : source.User.Actions.GetCurrentAimVector(), source.User.Actions.IsFlipped, 60);
        }
        if(source.SourceAbility != null) {
            source.SourceAbility.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        }
        Transform on_create_vfx = projectile.transform.Find("OnCreate");
        if(on_create_vfx != null)
        {
            on_create_vfx.gameObject.SetActive(true);
            on_create_vfx.SetParent(on_create_vfx.parent.parent);
        }
        EventManager.ProjectileCreated.Invoke(projectile);
        return projectile;
    }
    
    private static void SetUpTransform(SourceOfEffect source, GameObject game_object, float pos_x, float pos_y)
    {
        if (pos_x == 0 && pos_y == 0 && source != null)
        {
            game_object.transform.position = source.User.ProjectileSpawnLocation.transform.position;
            game_object.transform.localEulerAngles = new Vector3(source.User.ProjectileSpawnLocation.transform.eulerAngles.x, source.User.ProjectileSpawnLocation.transform.eulerAngles.y, source.User.ProjectileSpawnLocation.transform.eulerAngles.z);
        }
        else
        {
            game_object.transform.position = new Vector2(pos_x, pos_y);
            if(source != null) {
                game_object.transform.localEulerAngles = new Vector3(0, source.User.Actions.IsFlipped ? 180 : 0, 0);
            }
        }
        foreach(AttachObjectToBodyPart attach in game_object.GetComponentsInChildren<AttachObjectToBodyPart>())
        {
            attach.Unit = source != null ? source.User : null;
        }
        foreach(DamagingObject item in game_object.GetComponentsInChildren<DamagingObject>(true))
        {
            item.SourceAbility = source.SourceAbility;
        }
        if(game_object.transform.Find("DetachOnStart") != null)
        {
            GameObject detach = game_object.transform.Find("DetachOnStart").gameObject;
            detach.transform.SetParent(game_object.transform.parent);
            detach.transform.position = game_object.transform.position;
            detach.transform.up = game_object.transform.up;
        }
        foreach(AudioSource audioSource in game_object.GetComponentsInChildren<AudioSource>(true)) {
            Area.ComponentInstance.AudioSourceOriginalVolumes.Add(audioSource, audioSource.volume);
            audioSource.volume *= Settings.Instance.SoundVolume;
        }
    }

    private static void AddDynamicSortOrders(GameObject game_object)
    {
        foreach (Renderer renderer in game_object.GetComponentsInChildren<Renderer>())
        {
            if (renderer.sortingLayerName == "Player" && renderer.GetComponent<SortingOrder>() == null)
            {
                renderer.gameObject.AddComponent<SortingOrder>();
                renderer.GetComponent<SortingOrder>().AdjustSortingOrder = 20;
            }

        }
    }

    private static void ScaleParticleSystemsWithAttackSpeed(AttackSpeed speed, GameObject game_object)
    {
        foreach (ParticleSystem system in game_object.GetComponentsInChildren<ParticleSystem>())
        {
            if(system.GetComponent<TemporaryObject>() == null || system.GetComponent<TemporaryObject>().AlwaysSameSimulationSpeed == false) {
                ParticleSystem.MainModule main = system.main;
                main.simulationSpeed = speed.ScaledWithCombatSpeed;
            }
        }
        foreach (SetChildActiveAfterNSeconds set_active in game_object.GetComponentsInChildren<SetChildActiveAfterNSeconds>())
        {
            set_active.Seconds = set_active.Seconds / speed.ScaledWithCombatSpeed;
            set_active.ChangeBackAfterNSeconds = set_active.ChangeBackAfterNSeconds / speed.ScaledWithCombatSpeed;
        }
    }

    
    public static string GetFormattedInteger(int number) {
        string stringNumber = number.ToString();
        string result = "";
        int count = 0;
        for(int i = stringNumber.Length - 1; i >= 0; i--) {
            result = stringNumber[i] + result;
            count++;
            if(count == 3 && i != 0) {
                result = "," + result;
                count = 0;
            }
        }
        return result;
    }

    public static Dictionary<string, GameObject> PathsToGameObjects = new Dictionary<string, GameObject>();
    public static GameObject GetGameObject(string path) {
        if (GameController.Instance == null || String.IsNullOrWhiteSpace(path))
        {  
            return null;
        }
        if (PathsToGameObjects.ContainsKey(path)) {
            return PathsToGameObjects[path];
        }
        GameObject foundGameObject = GameController.Instance.transform.Find(path)?.gameObject;
        if(foundGameObject == null) {
            return null;
        }
        PathsToGameObjects.Add(path, foundGameObject);
        return foundGameObject;
    }

    public static Dictionary<string, Behaviour> PathsToComponents = new Dictionary<string, Behaviour>();
    public static Behaviour GetComponent(string path, Type component_type) {
        if (GameController.Instance == null || String.IsNullOrWhiteSpace(path))
        {  
            return null;
        }
        if (PathsToComponents.ContainsKey(path)) {
            return PathsToComponents[path];
        }
        Behaviour foundComponent = GameController.Instance.transform.Find(path)?.GetComponent(component_type) as Behaviour;
        if(foundComponent == null) {
            return null;
        }
        PathsToComponents.Add(path, foundComponent);
        return foundComponent;
    }

    public enum CanvasType {UI, Menu, Shop, StartScreen}

    public static void SetActiveOnCanvasGroup(CanvasType canvas_type, bool is_active)
    {
        GameObject gameObject = 
            canvas_type == CanvasType.UI ? UIManager.Instance?.gameObject : 
            canvas_type == CanvasType.Menu ? MenuManager.Instance?.gameObject : 
            canvas_type == CanvasType.Shop ? GameController.Objects.Shop?.gameObject :
            canvas_type == CanvasType.StartScreen ? Utils.GetSceneRootObject("Start Screen")?.gameObject : null;
        if(gameObject != null) {
            CanvasGroup item = gameObject.GetComponent<CanvasGroup>();
            item.alpha = is_active ? 1 : 0;
            item.interactable = is_active;
            item.blocksRaycasts = is_active;
        }
    }
}