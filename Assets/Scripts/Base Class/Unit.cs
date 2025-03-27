using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Effect;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Image = UnityEngine.UI.Image;
using Debug = UnityEngine.Debug;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine.U2D.Animation;
using UnityEngine.Rendering;

public class Unit : PermanentObject {

    public int Level = 1;
    public bool IsBoss = false;
    public bool IsMale = true;
    public bool ScaleStatsWithLevel = true;
    public bool ProducesHumanSounds = true;
    public float VoicePitch = 1;
    public int ExperienceGainOnDefeat = 0;

    private UnityEngine.UI.Slider _eliteEnemyIndicator;

    [HideInInspector]
    public GameObject NonRotatingElements;

    [HideInInspector]
    public UnitAI UnitAI;

    [HideInInspector]
    public Unit MostRecentEnemyHit;

    [HideInInspector]
    public AudioSource AudioSource;

    public string DisplayedName = "Default";
    public String Title;
    public String Portrait = "Default";

    public AnimationClip DefaultAnimation;

    [HideInInspector]
    public bool IsStaggered;

    [HideInInspector]
    public List<Area.FootstepsType> FootstepsOverride = new();

    [HideInInspector]
    public BoxCollider2D Collider;
    private int _colliderAdjustment = 0;

    private bool _knockedOut = false;
    public bool KnockedOut
    {
        get
        {
            return _knockedOut;
        }
        set
        {
            if(_knockedOut == value)
            {
                return;
            }
            Utils.CreateAuditLog("Unit (" + gameObject.name + ") Knocked Out: " + value);
            string path = Utils.GetGameObjectPath(gameObject);
            if(this is not Player && SaveFile.Instance != null && SaveFile.Instance.MidMissionInformation != null && SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits.ContainsKey(path) && SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits[path].KnockedOut) {
                Animator.speed = 0;
                Actions.SetFaceVariant("Eyes Closed");
                Animator.Play("KnockedOut_Extended", 0, 0.5f);
            }
            else {
                PlayAnimation("KnockedOut");
            }
            
            _knockedOut = value;
            foreach(InteractableObject inter in GetComponentsInChildren<InteractableObject>(true)) {
                inter.UpdateIndicatorVisiblity();
            }
            if (DebugController.WorldspaceDebugEnabled)
            {
                WorldSpaceDebugAction.GetComponent<TextMeshProUGUI>().text = "Action: Knocked Out";
            }
            if(EnemyGroup != null && String.IsNullOrWhiteSpace(EnemyGroup.ClassAndMethodToExecuteOnGroupDefeat) == false) {
                bool allDefeated = true;
                foreach(EnemyGroup enemy in Area.Instance.GetComponentsInChildren<EnemyGroup>().Where(eg => eg.GroupName == EnemyGroup.GroupName)) {
                    if(enemy.GetComponent<Unit>().KnockedOut == false) {
                        allDefeated = false;
                    }
                }
                if(allDefeated) {
                    MethodInfo method = Type.GetType(EnemyGroup.ClassAndMethodToExecuteOnGroupDefeat.Split(".")[0]).GetMethod(EnemyGroup.ClassAndMethodToExecuteOnGroupDefeat.Split(".")[1], BindingFlags.Public | BindingFlags.Static);
                    if(method == null) {
                        Debug.LogError("Could not find method (" +  EnemyGroup.ClassAndMethodToExecuteOnGroupDefeat.Split(".")[1] + ") for class (" + EnemyGroup.ClassAndMethodToExecuteOnGroupDefeat.Split(".")[0] + ") for interactable.");
                    }
                    else {
                        var shouldInteract = method.Invoke(null, null);
                    }
                }
            }
        }
    }

    public Effect EffectAnimationBeingPlayed { get; set; } = null;
    public Constants.DamageType DamageCategory = Constants.DamageType.None;
    public Constants.WeaponClass NPCWeaponClass = Constants.WeaponClass.None;

    public Constants.Faction Faction = Constants.Faction.Neutral;
    public float RiposteDamage { get; set; } = 100;
    public List<Unit> PotentialTargets { get; set; } = new List<Unit>();

    private int _ammo;

    public int Ammo {
        get {
            return _ammo;
        }
        set {
            int prevValue = _ammo;
            if (value < 0) {
                _ammo = 0;
            }
            else if (value > 99) {
                _ammo = 99;
            }
            else {
                _ammo = value;
            }
            if (this is Player) {
                CanvasElements.UICanvas.AmmoDisplay.transform.Find("Ammo Count").GetComponent<TextMeshProUGUI>().text = _ammo.ToString();
            }
            if(prevValue != _ammo) {
                EventManager.AmmoAmountChanged.Invoke();
            }
        }
    }
    protected List<Cooldown> _techniqueCooldowns = new();
    public ReadOnlyCollection<Cooldown> TechniqueCooldowns => _techniqueCooldowns.AsReadOnly();

    public Cooldown ToolCooldown {get; protected set;}
    protected List<Cooldown> _effectCooldowns = new();
    public ReadOnlyCollection<Cooldown> EffectCooldowns => _effectCooldowns.AsReadOnly();
    public int HitStopFramesRemaining = 0;

    [HideInInspector]
    public GameObject OnScreenBars;
    [HideInInspector]
    public bool CollisionTurnedOn = true;
    [HideInInspector]
    public EnemyGroup EnemyGroup;
    public bool CanEnterCombat = true;

    private bool _inCombat = false;
    public bool InCombat
    {
        get
        {
            return _inCombat;
        }
        set
        {
            bool previous_value = _inCombat;
            if(GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene || (CheckIfUnderEffect(typeof(Effect_CannotEnterCombat)) && value == true)) {
                return;
            }
            _inCombat = value;
            if(this is not Player) {
                if(Rigidbody2D != null) {
                    Rigidbody2D.bodyType = _inCombat ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
                }
                if(EnemyDetection != null) {
                    EnemyDetection.gameObject.SetActive(value);
                }
            }
            if (previous_value == false && _inCombat == true)
            {
                if(SaveFile.Instance.GameType == Constants.GameType.Survival && this is not Player) {
                    Utils.AllEnemiesAttackPlayer();
                } 
                Utils.CreateAuditLog("Unit (" + gameObject.name + ") entering combat");
                EventManager.EnterCombat.Invoke(this);
                if (this is Player)
                {
                    foreach(Type ability in Utils.GetAllCurrentlyEquippedAbilityTypes()) {
                        bool isStacksBased = ability?.GetField("IsStacksBasedTechnique") != null;
                        if(isStacksBased) {
                            AddCooldown(new Cooldown(ability, Ability.GetCooldown(ability), Player.Instance));
                            AddCooldown(new Cooldown(ability, Ability.GetCooldown(ability), Player.Instance) {ExtraInfo = "IsUltimate"});
                        }
                    }
                    if((GameController.Instance.PlayBossMusicDuringNextCombat && !String.IsNullOrEmpty(Area.ComponentInstance.BossBattleMusic)) || !String.IsNullOrEmpty(Area.ComponentInstance.BattleMusic)) {
                        Utils.PlayIntermissionMusic(GameController.Instance.PlayBossMusicDuringNextCombat ? Area.ComponentInstance.BossBattleMusic : Area.ComponentInstance.BattleMusic);
                    }
                    foreach(InteractableObject inter in Area.Instance.GetComponentsInChildren<InteractableObject>(true)) {
                        inter.UpdateIndicatorVisiblity();
                    }
                    GetComponent<NavMeshObstacle>().enabled = CollisionTurnedOn;
                }
                else if(Utils.CheckIfUnitCanPerformActions(this) && UnitAI != null)
                {
                    Actions.FaceUnit(CurrentTarget);
                    if(UnitAI.NavMeshAgent == null) {
                        UnitAI.Start();
                    }
                    UnitAI.NavMeshAgent.enabled = true;
                    UnitAI.SkipObserve = true;
                    UnitAI.DecideOnNextAction();
                }
                if (OnScreenBars == null && IsBoss) {
                    Slider[] sliders = UIManager.Instance.DisplayResourceBarsOnScreen(this);
                    if(sliders != null)
                    {
                        OnScreenBars = sliders[0].transform.parent.gameObject;
                        _eliteEnemyIndicator = sliders[0];
                        Health.HUDSlider = sliders[0];
                        Health.HealthBars = sliders[0].transform.Find("Extra Health Bars").gameObject;
                        StaggerBar.HUDSlider = sliders[1];
                        StaggerBar.HUDFill = sliders[1].transform.Find("Fill Area/Fill").GetComponent<Image>();
                        StaggerBar.StaggerBars = sliders[1].transform.Find("Extra Stagger Bars").gameObject;
                        InitializeDisplays();
                        AdjustUIResourceBarsSize();
                    }
                }
                if(CurrentTarget == Player.Instance) {
                    Player.Instance.EnemiesInCombatWithPlayer.Add(this);
                }
            }
            if (previous_value == true && _inCombat == false)
            {
                Utils.CreateAuditLog("Unit (" + gameObject.name + ") leaving combat");
                EventManager.ExitCombat.Invoke(this);
                if (this is Player)
                {
                    Player.Instance.UpdateAllStacksWhileNotInCombat();
                    Player.Instance.UnitsInRangeForBackstab.Clear();
                    Utils.StopIntermissionMusic();
                    foreach(InteractableObject inter in Area.Instance.GetComponentsInChildren<InteractableObject>(true)) {
                        inter.UpdateIndicatorVisiblity();
                    }
                    GetComponent<NavMeshObstacle>().enabled = false;
                    foreach (Unit unit in Utils.GetAllUnits(true))
                    {
                        unit.InCombat = false;
                    }
                    foreach(Effect e in CurrentEffects.ToList()) {
                        if(e.IsRemovable) {
                            e.EndThisEffect();
                        }
                    }
                    Player.Instance.AddEffect(new Effect_Invincible(new(Player.Instance)) {ShowsInUI = false, IsRemovable = false}, 1f);
                    if(SaveFile.Instance.CurrentMission != null && SaveFile.Instance.CurrentMission.AutoSaveAfterCombat) {
                        GameController.Instance.ShouldSaveAfterCombat = true;
                        GameController.Instance.WaitAndRunMethod(1, GameController.Instance.SaveAfterCombat);
                    }
                    if(Area.ComponentInstance.AutoRestAfterCombat) {
                        Player.Instance.CompleteRest();
                    }
                }
                else
                {
                    PotentialTargets.Clear();
                    _currentTarget = null;
                    UnitAI.CurrentAIBehavior = Constants.AIBehavior.None;
                    UnitAI.PerformAction(typeof(AI_Wait));
                }
                
                if(IsBoss && OnScreenBars != null)
                {   
                    MonoBehaviour.Destroy(OnScreenBars);
                }
                ResetUnit();
                
                if(this is not Player) {
                    Player.Instance.EnemiesInCombatWithPlayer.Remove(this);
                    if(Player.Instance.EnemiesInCombatWithPlayer.Count == 0 && Player.Instance.InCombat) {
                        Player.Instance.InCombat = false;
                    }
                }
            }
            if(this is Player && previous_value != _inCombat) {
                Player.ChangeInCombatDependantUI(_inCombat);
            }
            if(this is Player && Player.Instance.CurrentStance != null)
            {
                foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities)
                {
                    MethodInfo check = ability?.Type?.GetMethod("CheckIfAbilityUsableDependingOnCombat", BindingFlags.Public | BindingFlags.Static);
                    if (check != null)
                    {
                        bool can_use = (bool)check.Invoke(null, new object[] { InCombat });
                        ability.AbilityGraphic.transform.Find("Disabled").gameObject.SetActive(!SaveFile.Instance.UnlockedAbilities.Contains(ability.Type) || can_use == false);
                    }
                    else
                    {
                        ability.AbilityGraphic.transform.Find("Disabled").gameObject.SetActive(ability.Type != null && !SaveFile.Instance.UnlockedAbilities.Contains(ability.Type));
                    }
                }
            }
            if(this is not Player && previous_value == false && value == true && EnemyGroup != null) {
                foreach(EnemyGroup enemy in Area.Instance.GetComponentsInChildren<EnemyGroup>().Where(eg => eg.GroupName == EnemyGroup.GroupName)) {
                    enemy.GetComponent<Unit>().AttackPlayer();
                }
            }
        }
    }

    [HideInInspector]
    public bool UnitCanFlipDirection = true;
    private Unit _currentTarget;
    public Dictionary<String, SpriteRendererInfo> SpriteRenderers = new Dictionary<String, SpriteRendererInfo>();
    public bool CanRun = true;

    private float _visibility = 1;
    [HideInInspector]
    public float Visibility
    {
        get
        {
            return _visibility;
        }
        set
        {
            _visibility = value;
        }
    }

    public bool RightArmInFrontOfWeapon { get; set; } = true;
    public bool LeftArmInFrontOfWeapon { get; set; } = true;
    public bool RightArmInFrontOfLeftArm { get; set; } = true;

    public void ToggleSetting(string setting_name) {
        switch (setting_name) {
            case "LeftArmInFrontOfWeapon": LeftArmInFrontOfWeapon = !LeftArmInFrontOfWeapon; break;
            case "RightArmInFrontOfWeapon": RightArmInFrontOfWeapon = !RightArmInFrontOfWeapon; break;
            case "RightArmInFrontOfLeftArm": RightArmInFrontOfLeftArm = !RightArmInFrontOfLeftArm; break;
        }
    }

    public void OnEnable() {
        if(DefaultAnimation != null && !InCombat) {
            PlayAnimation(DefaultAnimation.name.Replace("Dialogue_", ""), 0, 0);
            GameController.Instance.WaitAndRunMethod(Animator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration + 1.5f, RepeatDefaultAnimation);
        }
    }

    public void ChangeFaction(Constants.Faction faction) {
        if(Faction == Constants.Faction.Enemy && faction != Constants.Faction.Enemy && Player.Instance.PotentialTargets.Contains(this)) {
            Player.Instance.PotentialTargets.Remove(this);
        } 
        Constants.Faction prevFaction = Faction;
        Faction = faction;
        Rigidbody2D.bodyType = Faction == Constants.Faction.Neutral ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        if(NonRotatingElements != null) {
            NonRotatingElements.SetActive(Faction == Constants.Faction.Enemy);
        }
        if(prevFaction != Faction) {
            ResetUnit();
        }
        if(transform.Find("Enemy Detection") == null) {
            return;
        }
        foreach(EnemyDetection ed in transform.Find("Enemy Detection").GetComponentsInChildren<EnemyDetection>()) {
            ed.gameObject.SetActive(false);
            ed.gameObject.SetActive(true);
        }
    }

    public List<string> NonSymmetricSpriteRenderers;

    public Unit CurrentTarget {
        get => _currentTarget;
        set {
            Unit previousTarget = _currentTarget;
            if (value == null && !(this is Player) && PotentialTargets.Count > 0) {
                _currentTarget = PotentialTargets[PotentialTargets.Count - 1];
            }
            else {
                _currentTarget = value;
            }
            if(previousTarget != value) {
                Utils.CreateAuditLog("Unit (" + gameObject?.name + ") changed target: " + value?.gameObject?.name);
            }
            if(this is Player && previousTarget != _currentTarget) {
                EventManager.PlayerTargetChanged.Invoke();
            }
            if (this is Player) {
                if (_currentTarget != null && GameController.Instance.GameplayMode == Constants.GameplayMode.Regular) {
                    GameObject targetMark = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_TargetMark")) as GameObject;
                    targetMark.gameObject.name = "UI_TargetMark";
                    targetMark.transform.SetParent(_currentTarget.SpriteRenderers["Upper Body"].Bone, false);
                    targetMark.transform.localPosition = new Vector2(0.4f, 0);
                }
                if (previousTarget != null && previousTarget.transform.Find("Lower Body/Lower Body Bone/Upper Body/Upper Body Bone/UI_TargetMark") != null) {
                    Destroy(previousTarget.transform.Find("Lower Body/Lower Body Bone/Upper Body/Upper Body Bone/UI_TargetMark").gameObject);
                }
            }
            else if (WorldSpaceCanvas != null && _currentTarget != null && !IsBoss) {
                WorldSpaceCanvas.SetActive(_currentTarget != null);
            }
            if(!(this is Player) && _currentTarget != null && InCombat == false && previousTarget != _currentTarget)
            {
                InCombat = true;
                if(_currentTarget is Player && _currentTarget.InCombat == false)
                {
                    Player.Instance.InCombat = true;
                    Player.Instance.InCombatTimer = Constants.DEFAULT_FIXED_FRAMES_UNTIL_EXITING_COMBAT;
                }
            }
        }
    }

    private GameObject _worldSpaceCanvas;
    public GameObject WorldSpaceCanvas => Utils.GetGameObjectIfNull(ref _worldSpaceCanvas, "World Space Canvas", this);
    private GameObject _worldSpaceDebugAction;
    public GameObject WorldSpaceDebugAction => Utils.GetGameObjectIfNull(ref _worldSpaceDebugAction, "World Space Canvas/UI_DebugWorldspaceDisplay/Action", this);
    private GameObject _worldSpaceDebugAbility;
    public GameObject WorldSpaceDebugAbility => Utils.GetGameObjectIfNull(ref _worldSpaceDebugAbility, "World Space Canvas/UI_DebugWorldspaceDisplay/Ability", this);
    private GameObject _worldSpaceDebugAnimation;
    public GameObject WorldSpaceDebugAnimation => Utils.GetGameObjectIfNull(ref _worldSpaceDebugAnimation, "World Space Canvas/UI_DebugWorldspaceDisplay/Animation", this);
    private GameObject _effectDisplay;
    public GameObject EffectDisplay => Utils.GetGameObjectIfNull(ref _effectDisplay, "World Space Canvas/Effects", this);
    private GameObject _rankDisplay;
    public GameObject RankDisplay => Utils.GetGameObjectIfNull(ref _rankDisplay, "World Space Canvas/Rank Display", this);
    private GameObject _visualEffects;
    public GameObject VisualEffects => Utils.GetGameObjectIfNull(ref _visualEffects, "Visual Effects", this);
    private GameObject _enemyDetection;
    public GameObject EnemyDetection => Utils.GetGameObjectIfNull(ref _enemyDetection, "Enemy Detection", this);
    private GameObject _projectileSpawnLocation;
    public GameObject ProjectileSpawnLocation => Utils.GetGameObjectIfNull(ref _projectileSpawnLocation, "Projectile Spawn Location", this);

    public Actions Actions { get; protected set; }
    public Animator Animator { get; protected set; }
    public Rigidbody2D Rigidbody2D { get; set; }
    private SpriteRenderer _unitSpriteRenderer;
    public float DistanceAwayFromChaseTarget = 1f;

    [HideInInspector]
    public UnitColorChange UnitColorChange;

    public Health Health { get; set; }
    public StaggerBar StaggerBar { get; set; }

    public Energy Energy { get; set; }

    public Injury CurrentWeaponInjury {
        get {
            if (CurrentWeaponDamageCategory == Constants.DamageType.Heavy) {
                return HeavyInjury;
            }
            else if (CurrentWeaponDamageCategory == Constants.DamageType.Light) {
                return LightInjury;
            }
            else if (CurrentWeaponDamageCategory == Constants.DamageType.Ranged) {
                return RangedInjury;
            }
            else {
                return MagicInjury;
            }
        }
    }

    public Injury GetInjuryStatForGivenDamageType(Constants.DamageType damage_category) {
        if (damage_category == Constants.DamageType.Heavy) {
            return HeavyInjury;
        }
        else if (damage_category == Constants.DamageType.Light) {
            return LightInjury;
        }
        else if (damage_category == Constants.DamageType.Ranged) {
            return RangedInjury;
        }
        else if (damage_category == Constants.DamageType.CurrentWeapon) {
            return CurrentWeaponInjury;
        }
        else if (damage_category == Constants.DamageType.Magic) {
            return MagicInjury;
        }
        else {
            return CurrentWeaponInjury;
        }
    }

    public Stagger GetStaggerStatForGivenDamageType(Constants.DamageType damage_category) {
        if (damage_category == Constants.DamageType.Heavy) {
            return HeavyStagger;
        }
        else if (damage_category == Constants.DamageType.Light) {
            return LightStagger;
        }
        else if (damage_category == Constants.DamageType.Ranged) {
            return RangedStagger;
        }
        else if (damage_category == Constants.DamageType.CurrentWeapon) {
            return CurrentWeaponStagger;
        }
        else if (damage_category == Constants.DamageType.Magic) {
            return MagicStagger;
        }
        else {
            return CurrentWeaponStagger;
        }
    }

    public AttackSpeed GetAttackSpeedStatForGivenDamageType(Constants.DamageType damage_category) {
        if (damage_category == Constants.DamageType.Heavy) {
            return HeavyAttackSpeed;
        }
        else if (damage_category == Constants.DamageType.Light) {
            return LightAttackSpeed;
        }
        else if (damage_category == Constants.DamageType.Ranged) {
            return RangedAttackSpeed;
        }
        else if (damage_category == Constants.DamageType.CurrentWeapon) {
            return CurrentWeaponAttackSpeed;
        }
        else if (damage_category == Constants.DamageType.Magic) {
            return MagicAttackSpeed;
        }
        else {
            return CurrentWeaponAttackSpeed;
        }
    }
 
    public List<Stat> Stats = new List<Stat>();

    public Injury HeavyInjury { get; set; }
    public Injury LightInjury { get; set; }
    public Injury RangedInjury { get; set; }
    public Injury MagicInjury { get; set; }

    public Stagger CurrentWeaponStagger {
        get {
            if (CurrentWeaponDamageCategory == Constants.DamageType.Heavy) {
                return HeavyStagger;
            }
            else if (CurrentWeaponDamageCategory == Constants.DamageType.Light) {
                return LightStagger;
            }
            else if (CurrentWeaponDamageCategory == Constants.DamageType.Ranged) {
                return RangedStagger;
            }
            else {
                return MagicStagger;
            }
        }
    }

    public Stagger HeavyStagger { get; set; }
    public Stagger LightStagger { get; set; }
    public Stagger RangedStagger { get; set; }
    public Stagger MagicStagger { get; set; }

    public AttackSpeed HeavyAttackSpeed { get; set; }
    public AttackSpeed LightAttackSpeed { get; set; }
    public AttackSpeed RangedAttackSpeed { get; set; }
    public AttackSpeed MagicAttackSpeed { get; set; }

    public MovementSpeed MovementSpeed { get; set; }
    public Tenacity Tenacity { get; set; }
    public Control Control { get; set; }
    public DamageReduction DamageReduction { get; set; }
    public Penetration Penetration { get; set; }
    public CooldownReduction CooldownReduction { get; set; }

    public List<Effect> CurrentEffects { get; private set; } = new List<Effect>();

    public Constants.DamageType CurrentWeaponDamageCategory
    {
        get
        {
            if(!(this is Player))
            {
                return DamageCategory;
            }
            if(CurrentWeaponCategory == Constants.ItemCategory.Heavy)
            {
                return Constants.DamageType.Heavy;
            }
            else if (CurrentWeaponCategory == Constants.ItemCategory.Light)
            {
                return Constants.DamageType.Light;
            }
            else if (CurrentWeaponCategory == Constants.ItemCategory.Ranged)
            {
                return Constants.DamageType.Ranged;
            }
            else
            {
                return Constants.DamageType.None;
            }
        }
    }

    protected Constants.ItemCategory _currentWeaponCategory;

    public Constants.ItemCategory CurrentWeaponCategory {
        get {
            if (this is Player) {
                return Player.Instance.CurrentStance.WeaponCategory;
            }
            else {
                return _currentWeaponCategory;
            }
        }
    }

    private Constants.WeaponClass _currentWeaponClass;

    public Constants.WeaponClass CurrentWeaponClass
    {
        get
        {
            if (this is Player)
            {
                return Player.Instance.CurrentStance.WeaponClass;
            }
            else if(NPCWeaponClass != Constants.WeaponClass.None) {
                return NPCWeaponClass;
            }
            return _currentWeaponClass;
        }
    }

    private Constants.WeaponClass _currenRangedWeaponClass;

    public Constants.WeaponClass CurrentRangedWeaponClass
    {
        get
        {
            if (this is Player)
            {
                return Player.Instance.GetStanceForGivenWeapon(Constants.ItemCategory.Ranged).WeaponClass;
            }
            else if(NPCWeaponClass != Constants.WeaponClass.None) {
                return NPCWeaponClass;
            }
            return _currentWeaponClass;
        }
    }

    private AttackSpeed _currentWeaponAttackSpeed;

    public AttackSpeed CurrentWeaponAttackSpeed
    {
        get
        {
            if (this is Player)
            {
                if(Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_PowerWithoutLimit) || Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_MindOverMatter)) {
                    return MagicAttackSpeed;
                }
                return Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Heavy ? HeavyAttackSpeed : Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Light ? LightAttackSpeed :
                    Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Ranged ? RangedAttackSpeed : MagicAttackSpeed;
            }
            else
            {
                return CurrentWeaponCategory == Constants.ItemCategory.Heavy ? HeavyAttackSpeed : CurrentWeaponCategory == Constants.ItemCategory.Light ? LightAttackSpeed :
                    CurrentWeaponCategory == Constants.ItemCategory.Ranged ? RangedAttackSpeed : MagicAttackSpeed;
            }
        }
    }

    public Stat GetUnitStatCorrespondingToGivenStat(Stat given_stat) {
        return given_stat is Health ? Health :
        given_stat is StaggerBar ? StaggerBar :
        given_stat is Energy ? Energy :
        (given_stat is Injury && ((Injury)given_stat).Category == Constants.DamageType.Heavy) ? HeavyInjury :
        (given_stat is Injury && ((Injury)given_stat).Category == Constants.DamageType.Light) ? LightInjury :
        (given_stat is Injury && ((Injury)given_stat).Category == Constants.DamageType.Ranged) ? RangedInjury :
        (given_stat is Injury && ((Injury)given_stat).Category == Constants.DamageType.Magic) ? MagicInjury :
        (given_stat is Stagger && ((Stagger)given_stat).Category == Constants.DamageType.Heavy) ? HeavyStagger :
        (given_stat is Stagger && ((Stagger)given_stat).Category == Constants.DamageType.Light) ? LightStagger :
        (given_stat is Stagger && ((Stagger)given_stat).Category == Constants.DamageType.Ranged) ? RangedStagger :
        (given_stat is Stagger && ((Stagger)given_stat).Category == Constants.DamageType.Magic) ? MagicStagger :
        (given_stat is AttackSpeed && ((AttackSpeed)given_stat).Category == Constants.DamageType.Heavy) ? HeavyAttackSpeed :
        (given_stat is AttackSpeed && ((AttackSpeed)given_stat).Category == Constants.DamageType.Light) ? LightAttackSpeed :
        (given_stat is AttackSpeed && ((AttackSpeed)given_stat).Category == Constants.DamageType.Ranged) ? RangedAttackSpeed :
        (given_stat is AttackSpeed && ((AttackSpeed)given_stat).Category == Constants.DamageType.Magic) ? MagicAttackSpeed :
        given_stat is MovementSpeed ? MovementSpeed :
        given_stat is Tenacity ? Tenacity :
        given_stat is Control ? Control :
        given_stat is CooldownReduction ? CooldownReduction :
        null;
    }

    public List<float> HealthBars;

    private int _currentHealthBars = 1;

    public int CurrentHealthBars {
        get {
            return _currentHealthBars;
        }
        set {
            if(this is Player) {
                return;
            }
            _currentHealthBars = value;
            if (Health.HealthBars != null) {
                for (int i = Health.HealthBars.transform.childCount - 1; i >= 0; i--) {
                    Health.HealthBars.transform.GetChild(i).Find("Active").gameObject.SetActive(i + 1 < _currentHealthBars);
                    Health.HealthBars.transform.GetChild(i).Find("Broken").gameObject.SetActive(i + 1 >= _currentHealthBars);
                }
            }
            if(CurrentHealthBars > 0)
            {
                Health.Maximum = HealthBars[HealthBars.Count - CurrentHealthBars] * (IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1);
                Health.Current = HealthBars[HealthBars.Count - CurrentHealthBars];
            }
        }
    }

    [HideInInspector]
    public float StaggeredRegen = 0;

    public float DefaultStaggerBarRegenPercentage = 2;
    public List<float> StaggerBars;

    private int _currentStaggerBars = 1;

    public int CurrentStaggerBars {
        get {
            return _currentStaggerBars;
        }
        set {
            _currentStaggerBars = value > StaggerBars.Count ? StaggerBars.Count : value;
            if (StaggerBar.StaggerBars != null) {
                for (int i = StaggerBar.StaggerBars.transform.childCount - 1; i >= 0; i--) {
                    StaggerBar.StaggerBars.transform.GetChild(i).Find("Active").gameObject.SetActive(i + 1 < _currentStaggerBars);
                    StaggerBar.StaggerBars.transform.GetChild(i).Find("Broken").gameObject.SetActive(i + 1 >= _currentStaggerBars);
                }
            }
        }
    }

    public float BaseInjury = 100;
    public float BaseStagger = 100;
    public float BaseAttackSpeed = 1;
    public float BaseMovementSpeed = 1;
    public float BaseTenacity = 1;
    public float BaseControl = 1;
    public float BaseCooldownReduction = 1;

    public void Awake() {
        InitializeComponents();
        if(this is not Player) {
            if(transform.Find("Enemy Detection") == null) {
                if(Faction == Constants.Faction.Enemy) {
                    Debug.LogWarning("Could not find Enemy Detection on hostile unit: " + gameObject.name);
                }
            }
            else {
                transform.Find("Enemy Detection").gameObject.SetActive(false);
            }
            InitializeStats();
        }
        InitializeDisplays();
        InitializeSpriteRenderers();
        AdditionalUnitSpecificActionsOnAwake();
        if (UnitColorChange == null) {
            UnitColorChange = GetComponent<UnitColorChange>();
        }
        UnitColorChange.UpdateMaterialProperties();
        if(DefaultAnimation != null) {
            PlayAnimation(DefaultAnimation.name.Replace("Dialogue_", ""), 0, UnityEngine.Random.Range(0, 0.5f));
            GameController.Instance.WaitAndRunMethod(Animator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration + UnityEngine.Random.Range(0.5f, 2.5f), RepeatDefaultAnimation);
        }
        if(Rigidbody2D != null) {
            Rigidbody2D.bodyType = Faction == Constants.Faction.Neutral ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        }
        PutAllWeaponsBehind();
    }

    public void RepeatDefaultAnimation() {
        if(Animator.IsDestroyed() == false && !KnockedOut && !InCombat && DefaultAnimation != null && Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && Actions.CurrentActionBeingPerformed != Constants.ActionType.Moving) {
            GameController.Instance.WaitAndRunMethod(Animator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration + 1.5f, RepeatDefaultAnimation);
            if(GameController.Instance.GameplayMode != Constants.GameplayMode.InCutscene) {
                PlayAnimation(DefaultAnimation.name.Replace("Dialogue_", ""));
            }
        }
    }

    public string GetTextureName(String sr_name) {
        switch (sr_name) {
            case "Right Hand": return "Hand";
            case "Right Arm": return "Arm";
            case "Heavy": return "Heavy";
            case "Light Right": return "Light";
            case "Ranged": return "Ranged";
            case "Hair": return "Hair";
            case "Upper Body": return "Upper Body";
            case "Head": return "Head";
            case "Right Foot": return "Foot";
            case "Right Leg": return "Leg";
            case "Lower Body": return "Lower Body";
            case "Left Foot": return "Foot";
            case "Left Leg": return "Leg";
            case "Left Hand": return "Hand";
            case "Left Arm": return "Arm";
            case "Light Left": return "Light";
            default: return sr_name;
        }
    }

    public float CurrentHealthPercentage {get {return Health.Current / Health.Maximum * 100;}}

    public void AttackPlayer() {
        if(Actions.Unit == null || UnitAI == null) {
            Actions.Start();
            UnitAI.NavMeshAgent = GetComponent<NavMeshAgent>();
        }
        Effect regen = GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "DuelRegeneration"));
        if(regen != null) {
            regen.EndThisEffect();
        }
        ChangeFaction(Constants.Faction.Enemy);
        CurrentTarget = Player.Instance;
        Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
        InCombat = true;
        Actions.UseAbility(typeof(AI_Chase), true, Player.Instance);
    }

    public void SetToNeutralNPC() {
        if(Actions.Unit == null || UnitAI == null) {
            Actions.Start();
            UnitAI.NavMeshAgent = GetComponent<NavMeshAgent>();
        }
        ChangeFaction(Constants.Faction.Neutral);
        ResetUnit();
    }

    public bool IsHostile {
        get {
            return CheckIfHostileTowards(Constants.Faction.Ally);
        }
    }

    public bool CheckIfHostileTowards(Constants.Faction faction) {
        Dictionary<Constants.Faction, int> FactionAttitudes = Constants.FactionAttitudes[Faction];
        return FactionAttitudes[faction] == -1;
    }

    public void ResetUnit() {
        foreach(Effect e in CurrentEffects.ToList()) {
            if(e.IsRemovable) {
                 e.EndThisEffect();
            }
        }
        CurrentTarget = null;
        if(this is Player) {
            Health.Current = Health.Maximum;
            StaggerBar.Current = 0;
            foreach(Type technique in Player.Instance.CurrentTechniqueStacks.Keys.ToArray()) {
                Player.Instance.CurrentTechniqueStacks[technique] = 1;
            }
        }
        else {
            CurrentHealthBars = HealthBars.Count;
            Health.Maximum = HealthBars[0] * (IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1);
            Health.Current = HealthBars[0];
            CurrentStaggerBars = StaggerBars.Count;
            StaggerBar.Maximum = StaggerBars[0]* (IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1);
            StaggerBar.Current = 0;
            UnitAI.CurrentAIBehavior = Constants.AIBehavior.None;
        }
        _techniqueCooldowns.Clear();
        ToolCooldown = null;
        _effectCooldowns.Clear();
        InCombat = false;
        Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
    }

    public virtual void AdditionalUnitSpecificActionsOnFixedUpdate() {
    }

    public Unit GetClosestValidTarget(bool only_in_front = true) {
        float smallestDistance = 16;
        Unit currentCandidate = null;
        foreach(Unit u in Utils.GetSpecifiedUnits(new Func<Unit, bool> (u => CheckIfValidTarget(u)))) {
            float distance = Vector2.Distance(u.transform.position, transform.position);
            if((only_in_front == false || Utils.CheckIfGivenUnitIsInFrontOfUnit(this, u)) && distance < smallestDistance && distance < 15) {
                smallestDistance = distance;
                currentCandidate = u;
            }
        }
        return currentCandidate;
    }

    private void Update() {
        if(gameObject.activeSelf == false) {
            return;
        }
        if(Actions.CurrentAbilityBeingPerformed != null)
        {
            Actions.CurrentAbilityBeingPerformed.OnUpdate();
        }
        UpdateAllCooldowns();
        DecreaseEffectsDuration();
        CalculateRegeneration();
        foreach (SpriteRendererInfo sr_info in SpriteRenderers.Values) {
            if(sr_info.SortingGroup != null) {
                sr_info.SortingGroup.sortingOrder = (-1) * Mathf.RoundToInt(transform.position.y * 100) + sr_info.SortingOrder;
            }
            else {
                sr_info.SpriteRenderer.sortingOrder = (-1) * Mathf.RoundToInt(transform.position.y * 100) + sr_info.SortingOrder;
            }
        }
        foreach (Effect effect in CurrentEffects.ToList()) {
            effect.OnUpdate();
        }
    }

    protected virtual void AdditionalUnitSpecificActionsOnAwake() {
    }

    public Effect GetEffect(Func<Effect, bool> condition_check)
    {
        return CurrentEffects.FirstOrDefault(effect => condition_check.Invoke(effect));
    }

    public Effect GetEffect(Type effect_type)
    {
        return CurrentEffects.FirstOrDefault(effect => effect.GetType() == effect_type || effect.GetType().IsSubclassOf(effect_type));
    }

    public bool CheckIfUnderEffect(Type effect_type) {
        return CurrentEffects.FirstOrDefault(effect => effect.GetType() == effect_type || effect.GetType().IsSubclassOf(effect_type)) != null;
    }

    public bool CheckIfUnderEffectFromGivenAbility(Type under_effect, Type ability_type) {
        return CurrentEffects.FirstOrDefault(effect => effect.GetType() == under_effect && effect.SourceOfEffect.GetType() == ability_type) != null;
    }

    public void ToggleLeftArmInFrontOfWeapon() {
        LeftArmInFrontOfWeapon = !LeftArmInFrontOfWeapon;
    }

    public void ToggleRightArmInFrontOfWeapon() {
        RightArmInFrontOfWeapon = !RightArmInFrontOfWeapon;
    }

    private void InitializeComponents() {
        UnitAI = GetComponent<UnitAI>();
        AudioSource = GetComponent<AudioSource>();
        _unitSpriteRenderer = GetComponent<SpriteRenderer>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Actions = GetComponent<Actions>();
        Animator = GetComponent<Animator>();
        Animator.writeDefaultValuesOnDisable = true;
        UnitColorChange = GetComponent<UnitColorChange>();
        EnemyGroup = GetComponent<EnemyGroup>();
        Collider = transform.Find("Lower Body/Lower Body Bone").GetComponent<BoxCollider2D>();
        if(Collider != null) {
            _colliderAdjustment = (int)((Collider.offset.y + Collider.size.y / 2) * 100 * transform.lossyScale.y);
        }
        if (this is not Player)
        {
            Actions.IsFlipped = Actions.StartWithFlippedDirection;
            NonRotatingElements = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/" + (IsBoss ? "Boss" : "Regular") + " Enemy UI/World Space Canvas")) as GameObject;
            NonRotatingElements.transform.SetParent(transform, false);
            NonRotatingElements.name = "World Space Canvas";
            NonRotatingElements.transform.localEulerAngles = new Vector3(0, 0, 0);
            if(Actions.IsFlipped)
            {
                NonRotatingElements.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            if(Faction != Constants.Faction.Enemy) {
                NonRotatingElements.SetActive(false);
            }
        }
    }

    private void InitializeDisplays() {
        if (StaggerBars.Count > 1 && StaggerBar.StaggerBars != null) {
            for (int i = 0; i < StaggerBars.Count - 1; i++) {
                GameObject stagger_bar = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ExtraStaggerBar")) as GameObject;
                stagger_bar.transform.SetParent(StaggerBar.StaggerBars.transform, false);
            }
            CurrentStaggerBars = StaggerBars.Count;
        }
        if (HealthBars.Count > 1 && Health.HealthBars != null) {
            for (int i = 0; i < HealthBars.Count - 1; i++) {
                GameObject health_bar = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ExtraHealthBar")) as GameObject;
                health_bar.transform.SetParent(Health.HealthBars.transform, false);
            }
            CurrentHealthBars = HealthBars.Count;
        }
    }

    public void AdjustUIResourceBarsSize() {
        if(Health == null || StaggerBar == null || Health.HUDSlider == null || StaggerBar.HUDSlider == null) {
            return;
        }
        if(this is Player) {
            Health.HUDSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetValueBasedOnMinAndMax(Health.Maximum, 1000, 10000, Screen.width / 10, Screen.width / 2), 60);
            StaggerBar.HUDSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetValueBasedOnMinAndMax(StaggerBar.Maximum, 1000, 10000, Screen.width / 10, Screen.width / 2), 25);
        }
        else if (IsBoss)
        {
            Health.HUDSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetValueBasedOnMinAndMax(Health.Maximum, 500 * Utils.GetExpectedPowerForLevel(Level), 3000 * Utils.GetExpectedPowerForLevel(Level), Screen.width / 16, Screen.width / 6), 10);
            StaggerBar.HUDSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetValueBasedOnMinAndMax(StaggerBar.Maximum, 500 * Utils.GetExpectedPowerForLevel(Level), 3000 * Utils.GetExpectedPowerForLevel(Level), Screen.width / 16, Screen.width / 6), 10);
        }
        else {
            Health.HUDSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetValueBasedOnMinAndMax(Health.Maximum, 200 * Utils.GetExpectedPowerForLevel(Level), 2000 * Utils.GetExpectedPowerForLevel(Level), 100, 500), 10);
            StaggerBar.HUDSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(Utils.GetValueBasedOnMinAndMax(StaggerBar.Maximum, 200 * Utils.GetExpectedPowerForLevel(Level), 2000 * Utils.GetExpectedPowerForLevel(Level), 100, 500), 10);
            
        }
    }

    public void InitializeSpriteRenderers() {
        SpriteRenderers = new();
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>(true)) {
            SpriteRendererInfo sr_info = new SpriteRendererInfo(sr.name, sr);
            switch (sr_info.Name.Replace(" (Unused)", "")) {
                case "Right Hand": { sr_info.SortingOrder = 16; sr_info.IsInFront = true; break; }
                case "Right Arm": { sr_info.SortingOrder = 15; sr_info.IsInFront = true; break; }
                case "Consumable": { sr_info.SortingOrder = 14; sr_info.IsInFront = true; break; }
                case "Projectile": { sr_info.SortingOrder = 14; sr_info.IsInFront = true; break; }
                case "Heavy": { sr_info.SortingOrder = 13; sr_info.IsInFront = true; break; }
                case "Light Right": { sr_info.SortingOrder = 12; sr_info.IsInFront = true; break; }
                case "Ranged": { sr_info.SortingOrder = 11; sr_info.IsInFront = true; break; }
                case "Hair": { sr_info.SortingOrder = 10; sr_info.IsInFront = true; break; }
                case "Upper Body": { sr_info.SortingOrder = 9; sr_info.IsInFront = true; break; }
                case "Head": { sr_info.SortingOrder = 8; sr_info.IsInFront = true; break; }
                case "Right Foot": { sr_info.SortingOrder = 7; sr_info.IsInFront = true; break; }
                case "Right Leg": { sr_info.SortingOrder = 6; sr_info.IsInFront = true; break; }
                case "Lower Body": { sr_info.SortingOrder = 5; sr_info.IsInFront = true; break; }
                case "Left Foot": { sr_info.SortingOrder = 4; sr_info.IsInFront = false; break; }
                case "Left Leg": { sr_info.SortingOrder = 3; sr_info.IsInFront = false; break; }
                case "Left Hand": { sr_info.SortingOrder = 2; sr_info.IsInFront = false; break; }
                case "Left Arm": { sr_info.SortingOrder = 1; sr_info.IsInFront = false; break; }
                case "Light Left": { sr_info.SortingOrder = 0; sr_info.IsInFront = false; break; }
                default: { sr_info.SortingOrder = 100; break; }
            }
            sr_info.ColorChange = sr_info.SpriteRenderer.GetComponent<ColorChange>();
            if (!SpriteRenderers.ContainsKey(sr_info.Name.Replace(" (Unused)", "")) && sr_info.SortingOrder != 100) {
                SpriteRenderers.Add(sr_info.Name.Replace(" (Unused)", ""), sr_info);
            }
        }
    }

    public void LateUpdate() {
        if (Visibility < 1) {
            //SpriteRenderer[] sprite_renderers = GetComponents<SpriteRenderer>().Concat(GetComponentsInChildren<SpriteRenderer>(true)).ToArray();
            foreach (SpriteRendererInfo sr in SpriteRenderers.Values) {
                sr.SpriteRenderer.color = new Color(sr.SpriteRenderer.color.r, sr.SpriteRenderer.color.g, sr.SpriteRenderer.color.b, Visibility);
            }
        }
    }

    public void PutElementInFront(string element_name) {
        if (SpriteRenderers.ContainsKey(element_name)) {
            SpriteRenderers[element_name].IsInFront = true;
            foreach(SortingOrder sort_order in SpriteRenderers[element_name].SpriteRenderer.GetComponentsInChildren<SortingOrder>())
            {
                sort_order.AdjustSortingOrder = 100;
            }
            RecalculateSortingOrder();
        }
    }

    public void PutElementBehind(string element_name) {
        if (SpriteRenderers.ContainsKey(element_name)) {
            SpriteRenderers[element_name].IsInFront = false;
            foreach (SortingOrder sort_order in SpriteRenderers[element_name].SpriteRenderer.GetComponentsInChildren<SortingOrder>())
            {
                sort_order.AdjustSortingOrder = -100;
            }
            RecalculateSortingOrder();
        }
    }

    public void PutAllWeaponsInFront() {
        if (SpriteRenderers.ContainsKey("Light Right"))
        {
            PutElementInFront("Light Right");
        }
        if (SpriteRenderers.ContainsKey("Light Left"))
        {
            PutElementInFront("Light Left");
        }
        if (SpriteRenderers.ContainsKey("Heavy"))
        {
            PutElementInFront("Heavy");
        }
        if (SpriteRenderers.ContainsKey("Ranged"))
        {
            PutElementInFront("Ranged");
        }
    }

    public void PutAllWeaponsBehind() {
        if (SpriteRenderers.ContainsKey("Light Right")) {
            PutElementBehind("Light Right");
        }
        if (SpriteRenderers.ContainsKey("Light Left")) {
            PutElementBehind("Light Left");
        }
        if (SpriteRenderers.ContainsKey("Heavy")) {
            PutElementBehind("Heavy");
        }
        if (SpriteRenderers.ContainsKey("Ranged")) {
            PutElementBehind("Ranged");
        }
    }

    public void SetDefaultSortingOrder() {
        SpriteRenderers["Right Arm"].IsInFront = true;
        SpriteRenderers["Left Arm"].IsInFront = false;
        SpriteRenderers["Right Leg"].IsInFront = true;
        SpriteRenderers["Left Leg"].IsInFront = false;
        SpriteRenderers["Hair"].IsInFront = true;
        SpriteRenderers["Head"].IsInFront = true;
        SpriteRenderers["Upper Body"].IsInFront = true;
        SpriteRenderers["Lower Body"].IsInFront = true;
        if(Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && (Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" || Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "IdleInCombat") && SpriteRenderers.ContainsKey("Consumable")) {
            SpriteRenderers["Consumable"].SpriteRenderer.enabled = false;
        }
        PutAllWeaponsBehind();
        Actions.TurnOffWeaponTrail("Heavy");
        Actions.TurnOffWeaponTrail("Light");
        Actions.TurnOffWeaponTrail("Ranged");
        Actions.SetFaceVariant("Regular");
        RecalculateSortingOrder();
    }

    public void RecalculateSortingOrder() {
        List<string> new_order = new List<string>();
        if (SpriteRenderers.ContainsKey("Heavy") && SpriteRenderers["Heavy"].IsInFront == false)
            { new_order.Add("Heavy"); }
        if (SpriteRenderers.ContainsKey("Light Right") && SpriteRenderers["Light Right"].IsInFront == false)
            { new_order.Add("Light Right"); }
        if (SpriteRenderers.ContainsKey("Ranged") && SpriteRenderers["Ranged"].IsInFront == false)
            { new_order.Add("Ranged"); }
        if (SpriteRenderers.ContainsKey("Light Left") && SpriteRenderers["Light Left"].IsInFront == false)
            { new_order.Add("Light Left"); }
        if (SpriteRenderers.ContainsKey("Projectile") && SpriteRenderers["Projectile"].IsInFront == false)
            { new_order.Add("Projectile"); }
        if (SpriteRenderers.ContainsKey("Consumable") && SpriteRenderers["Consumable"].IsInFront == false)
            { new_order.Add("Consumable"); }  
        if (SpriteRenderers["Left Arm"].IsInFront == false) { new_order.Add("Left Arm"); new_order.Add("Left Hand"); }
        if (SpriteRenderers["Right Arm"].IsInFront == false) { new_order.Add("Right Arm"); new_order.Add("Right Hand"); }
        new_order.Add("Lower Body");
        if (SpriteRenderers["Left Leg"].IsInFront == false) { new_order.Add("Left Leg"); new_order.Add("Left Foot"); }
        if (SpriteRenderers["Right Leg"].IsInFront == false) { new_order.Add("Right Leg"); new_order.Add("Right Foot"); }
        if (SpriteRenderers["Left Leg"].IsInFront == true) { new_order.Add("Left Leg"); new_order.Add("Left Foot"); }
        if (SpriteRenderers["Right Leg"].IsInFront == true) { new_order.Add("Right Leg"); new_order.Add("Right Foot"); }
        new_order.Add("Head");
        new_order.Add("Upper Body");
        new_order.Add("Hair");
        if (SpriteRenderers["Left Arm"].IsInFront == true && LeftArmInFrontOfWeapon == false) { new_order.Add("Left Arm"); new_order.Add("Left Hand"); }
        if (SpriteRenderers["Right Arm"].IsInFront == true && RightArmInFrontOfWeapon == false) { new_order.Add("Right Arm"); new_order.Add("Right Hand"); }
        if (SpriteRenderers.ContainsKey("Heavy") && SpriteRenderers["Heavy"].IsInFront == true)
            { new_order.Add("Heavy"); }
        if (SpriteRenderers.ContainsKey("Light Right") && SpriteRenderers["Light Right"].IsInFront == true)
            { new_order.Add("Light Right"); }
        if (SpriteRenderers.ContainsKey("Ranged") && SpriteRenderers["Ranged"].IsInFront == true)
            { new_order.Add("Ranged"); }
        if (SpriteRenderers.ContainsKey("Light Left") && SpriteRenderers["Light Left"].IsInFront == true)
            { new_order.Add("Light Left"); }
        if (SpriteRenderers.ContainsKey("Projectile") && SpriteRenderers["Projectile"].IsInFront == true)
            { new_order.Add("Projectile"); }
        if (SpriteRenderers.ContainsKey("Consumable") && SpriteRenderers["Consumable"].IsInFront == true)
            { new_order.Add("Consumable"); }  
        if (SpriteRenderers["Left Arm"].IsInFront == true && LeftArmInFrontOfWeapon == true) { new_order.Add("Left Arm"); new_order.Add("Left Hand"); }
        if (SpriteRenderers["Right Arm"].IsInFront == true && RightArmInFrontOfWeapon == true) { new_order.Add("Right Arm"); new_order.Add("Right Hand"); }
        for (int i = 0; i < new_order.Count; i++) {
            SpriteRenderers[new_order[i]].SortingOrder = i;
        }
    }

    public void InitializeStats() {
        Health = new Health(this, this is Player ? 1000 + SaveFile.Instance.HealthGainedFromTraining : HealthBars[0] * (IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1));
        StaggerBar = new StaggerBar(this, this is Player ? 1000 + SaveFile.Instance.StaggerBarGainedFromTraining : StaggerBars[0] * (IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1));
        Energy = new Energy(this, 100);
        AdjustUIResourceBarsSize();

        bool use_default = !(this is Player) || SaveFile.Instance.EquippedHeavyWeapon == null;
        HeavyInjury = new Injury(Constants.DamageType.Heavy, this, !use_default ? SaveFile.Instance.EquippedHeavyWeapon.BaseInjury : BaseInjury);
        LightInjury = new Injury(Constants.DamageType.Light, this, !use_default ? SaveFile.Instance.
        EquippedLightWeapon.BaseInjury : BaseInjury);
        RangedInjury = new Injury(Constants.DamageType.Ranged, this, !use_default ? SaveFile.Instance.EquippedRangedWeapon.BaseInjury : BaseInjury);
        MagicInjury = new Injury(Constants.DamageType.Magic, this, !use_default ? 100 : BaseInjury);

        HeavyStagger = new Stagger(Constants.DamageType.Heavy, this, !use_default ? SaveFile.Instance.EquippedHeavyWeapon.BaseStagger : BaseStagger);
        LightStagger = new Stagger(Constants.DamageType.Light, this, !use_default ? SaveFile.Instance.EquippedLightWeapon.BaseStagger : BaseStagger);
        RangedStagger = new Stagger(Constants.DamageType.Ranged, this, !use_default ? SaveFile.Instance.EquippedRangedWeapon.BaseStagger : BaseStagger);
        MagicStagger = new Stagger(Constants.DamageType.Magic, this, !use_default ? 100 : BaseStagger);

        HeavyAttackSpeed = new AttackSpeed(Constants.DamageType.Heavy, this, !use_default ? SaveFile.Instance.EquippedHeavyWeapon.BaseAttackSpeed : BaseAttackSpeed);
        LightAttackSpeed = new AttackSpeed(Constants.DamageType.Light, this, !use_default ? SaveFile.Instance.EquippedLightWeapon. BaseAttackSpeed : BaseAttackSpeed);
        RangedAttackSpeed = new AttackSpeed(Constants.DamageType.Ranged, this, !use_default ? SaveFile.Instance.EquippedRangedWeapon.BaseAttackSpeed : BaseAttackSpeed);
        MagicAttackSpeed = new AttackSpeed(Constants.DamageType.Magic, this, !use_default ? 1.0f : BaseAttackSpeed);
        if(DebugController.MaxAttackSpeed) {
            Effect_ChangeCompositeStat buff = new(this, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new ("Cheat")) {PercentageAmount = 1000};
            DebugController.SpeedBuffs.Add(buff);
            AddEffect(buff);
        }

        if(this is not Player)
        {
            Effect_ChangeStat hp_regen_outside_combat = new Effect_ChangeStat( Health, new ("Cheat"))
            {
                IsRemovable = false,
                RegenerationPercentageAmount = 5,
                DependenceOnCombatStatus = Effect_ChangeStat.DependenceOnCombatStatusEnum.OnlyWorksOutOfCombat
            };
            AddEffect(hp_regen_outside_combat);
        }

        MovementSpeed = new MovementSpeed(this, BaseMovementSpeed);
        Tenacity = new Tenacity(this, BaseTenacity * (IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1));
        Control = new Control(this, BaseControl);
        DamageReduction = new DamageReduction(this, 1);
        Penetration = new Penetration(this, 1);
        CooldownReduction = new CooldownReduction(this, BaseCooldownReduction);

        CurrentHealthBars = HealthBars.Count;
        CurrentStaggerBars = StaggerBars.Count;

        Stats.AddRange(new List<Stat> { Health, StaggerBar, Energy, HeavyInjury, LightInjury, RangedInjury, MagicInjury, HeavyStagger, LightStagger, RangedStagger, MagicStagger, HeavyAttackSpeed, LightAttackSpeed, RangedAttackSpeed, MagicAttackSpeed, MovementSpeed, Tenacity, Control, CooldownReduction, DamageReduction, Penetration});
    }

    private void FixedUpdate() {
        if(HitStopFramesRemaining > 0) {
            HitStopFramesRemaining--;
            if(HitStopFramesRemaining == 0) {
                Animator.SetFloat("Heavy Attack Speed", HeavyAttackSpeed.ScaledWithCombatSpeed);
                Animator.SetFloat("Light Attack Speed", LightAttackSpeed.ScaledWithCombatSpeed);
                Animator.SetFloat("Ranged Attack Speed", RangedAttackSpeed.ScaledWithCombatSpeed);
                Animator.SetFloat("Magic Attack Speed", MagicAttackSpeed.ScaledWithCombatSpeed);
                if(this is Player) {
                    Animator.SetFloat("Technique Speed", Player.Instance.TechniqueSpeed);
                }
            }
        }
        EffectFixedUpdates();
        AdditionalUnitSpecificActionsOnFixedUpdate();
    }

    private void EffectFixedUpdates() {
        foreach (Effect effect in CurrentEffects.ToList()) {
            effect.OnFixedUpdate();
        }
    }

    private void CalculateRegeneration() {
        if (Health != null && Health.Current < Health.Maximum && Health.Regeneration != 0) {
            Health.Current += Health.Regeneration * Time.deltaTime;
        }
        if (StaggerBar != null && StaggerBar.Current > 0 && StaggerBar.Regeneration != 0 && !CheckIfUnderEffect(typeof(Effect_Staggered)) && !CheckIfUnderEffect(typeof(Effect_Staggered))) {
            StaggerBar.Current -= StaggerBar.Regeneration * Time.deltaTime;
            if(InCombat == false && StaggerBar.Current > 0)
            {
                StaggerBar.Current -= (StaggerBar.Maximum / 5) * Time.deltaTime;
            }
        }
        else if(CheckIfUnderEffect(typeof(Effect_Staggered)) || CheckIfUnderEffect(typeof(Effect_Staggered)))
        {
            StaggerBar.Current -= StaggeredRegen * Time.deltaTime;
        }
        if (Energy != null && Energy.Regeneration != 0) {
            Energy.Current += Energy.Regeneration * Time.deltaTime;
        }
    }

    public bool CheckIfEffectIsOnCooldown(Effect effect)
    {
        return EffectCooldowns.FirstOrDefault(eff => effect.GetType() == eff.Type) != null;
    }

    public bool CheckIfEffectIsOnCooldown(Type type)
    {
        return EffectCooldowns.FirstOrDefault(effect => type == effect.Type) != null;
    }

    private void DecreaseEffectsDuration() {
        foreach (Effect effect in CurrentEffects.ToList()) {
            effect.OnUpdate();
            if (effect.BaseDuration > 0) {
                effect.RemainingDuration -= Time.deltaTime;
                if (effect.RemainingDuration <= 0) {
                    EndEffect(effect);
                    if (effect.AdditionalEffectsAffectingTargetDuringEffect.Count > 0) {
                        foreach (Effect e in effect.AdditionalEffectsAffectingTargetDuringEffect) {
                            Utils.CreateAuditLog("Removing effect " + e.GetType() + " due to " + effect.GetType() + " for unit " + gameObject.name + ":" + Utils.GetStackTrace());
                            EndEffect(e);
                        }
                    }
                }
                else if (effect.EffectIndicatorCooldownDisplay != null) {
                    effect.EffectIndicatorCooldownDisplay.fillAmount = 1f - (effect.RemainingDuration / effect.BaseDuration);
                }
            }
        }
        foreach(Cooldown cd in EffectCooldowns.Where(cd => cd.ShowCooldownInEffectUI)) {
            if(cd.CooldownDisplay == null) {
                Transform effectsDisplay = CanvasElements.UICanvas.Effects.transform;
                GameObject effectIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_EffectIcon")) as GameObject;
                effectIndicator.GetComponent<Image>().sprite = Resources.Load("Sprites/UI/CooldownEffectIcon", typeof(Sprite)) as Sprite;
                effectIndicator.transform.SetParent(effectsDisplay, false);
                effectIndicator.transform.Find("EffectImage").GetComponent<Image>().sprite = cd.CooldownGraphic != null ? cd.CooldownGraphic : Resources.Load("Sprites/" + cd.PathToCooldownGraphic, typeof(Sprite)) as Sprite;
                cd.CooldownDisplay = effectIndicator.transform.Find("CooldownDisplay").GetComponent<Image>();
                cd.CooldownDisplay.fillAmount = 0;
            }
            else {
                cd.CooldownDisplay.fillAmount = 1f - (cd.RemainingDuration / cd.TotalDuration);
            }
        }
    }

    private void UpdateAllCooldowns() {
        if(Time.deltaTime == 0) {
            return;
        }
        foreach (Cooldown cooldown in TechniqueCooldowns.ToArray()) {
            cooldown.RemainingDuration -= Time.deltaTime;
            if(cooldown.RemainingDuration <= 0) {
                bool isStacksBased = cooldown.Type?.GetField("IsStacksBasedTechnique") != null;
                int maxStacks = 0;
                bool isUltimate = cooldown.ExtraInfo == "IsUltimate";
                if(cooldown.Type?.GetProperty((isUltimate ? "Ultimate" : "") + "MaxStacks") != null) {
                    maxStacks = (int)cooldown.Type?.GetProperty((isUltimate ? "Ultimate" : "")  + "MaxStacks").GetValue(null);
                }
                if(isStacksBased && ((isUltimate && Player.Instance.CurrentUltimateTechniqueStacks[cooldown.Type] < maxStacks) || (cooldown.ExtraInfo == "" && Player.Instance.CurrentTechniqueStacks[cooldown.Type] < maxStacks))) {
                    Player.Instance.UpdateTechniqueStacksAmount(cooldown.Type, isUltimate ? Player.Instance.CurrentUltimateTechniqueStacks[cooldown.Type] + 1 : Player.Instance.CurrentTechniqueStacks[cooldown.Type] + 1, isUltimate);
                    if((isUltimate && Player.Instance.CurrentUltimateTechniqueStacks[cooldown.Type] < maxStacks) || (cooldown.ExtraInfo == "" && Player.Instance.CurrentTechniqueStacks[cooldown.Type] < maxStacks)) {
                        cooldown.RemainingDuration = cooldown.TotalDuration;
                    }
                }
                else {
                    _techniqueCooldowns.Remove(cooldown);
                }
                if(cooldown.ShowCooldownInEffectUI && cooldown.CooldownDisplay != null) {
                    MonoBehaviour.Destroy(cooldown.CooldownDisplay.transform.parent.gameObject);
                }
            }
        }
        foreach (Cooldown cooldown in EffectCooldowns.ToArray())
        {
            cooldown.RemainingDuration -= Time.deltaTime;
            if(cooldown.RemainingDuration <= 0) {
                _effectCooldowns.Remove(cooldown);
                if(cooldown.ShowCooldownInEffectUI && cooldown.CooldownDisplay != null) {
                    MonoBehaviour.Destroy(cooldown.CooldownDisplay.transform.parent.gameObject);
                }
            }
        }
        if(ToolCooldown != null) {
            ToolCooldown.RemainingDuration -= Time.deltaTime;
            if(ToolCooldown.RemainingDuration <= 0) {
                ToolCooldown = null;
            }
        }
    }

    public void PlayAnimation(string animation_name, float transition_duration = Constants.DEFAULT_CROSSFADE_DURATION, float time = 0) {
        if(KnockedOut || gameObject.activeSelf == false)
        {
            return;
        }
        string extractedName = animation_name.Replace("BA_", "").Replace("NPCAbility_", "").Replace("Effect_", "").Replace("Ability_", "");
        if (Animator != null && Animator.HasState(0, Animator.StringToHash(extractedName))) {
            if (DebugController.WorldspaceDebugEnabled) {
                WorldSpaceDebugAnimation.GetComponent<TextMeshProUGUI>().text = "Animation: " + extractedName;
            }
            Animator.CrossFade(extractedName, transition_duration, 0, time);
        }
        else if (Animator == null || Animator.gameObject == null || Animator.gameObject.activeSelf == false) {
            throw new MissingReferenceException($"Tried to play animation on inactive gameObject ({Animator?.gameObject?.name}): or null Animator ({Animator}) {extractedName}");
        }
        else {
            throw new MissingReferenceException("Animator does not contain animation called: '" + extractedName + "'");
        }
    }

    public void AddCooldown(Effect effect, float cooldown_duration)
    {
        Cooldown cd = new Cooldown(effect.GetType(), cooldown_duration, this);
        AddCooldown(cd);
    }

    public void AddCooldown(Type type, float cooldown_duration)
    {
        Cooldown cd = new Cooldown(type, cooldown_duration, this);
        AddCooldown(cd);
    }

    public void AddCooldown(Ability ability, float cooldown_duration)
    {
        Cooldown cd = new Cooldown(ability.GetType(), cooldown_duration, this);
        AddCooldown(cd);
    }

    public void AddCooldown(Item item) {
        Cooldown cd = new Cooldown(item.GetType(), Item.GetCooldown(item.GetType()), this);
        AddCooldown(cd);
    }

    public void AddCooldown(Cooldown cooldown) {
        if(cooldown.Type.IsSubclassOf(typeof(Effect))) {
            Cooldown existing_cd = _effectCooldowns.FirstOrDefault(cd => cd.Type == cooldown.Type);
            if(existing_cd != null) {
                _effectCooldowns.Remove(existing_cd);
            }
            _effectCooldowns.Add(cooldown);
        }
        else if(cooldown.Type.IsSubclassOf(typeof(Ability))) {
            Cooldown existing_cd = _techniqueCooldowns.FirstOrDefault(cd => cd.Type == cooldown.Type && cd.ExtraInfo == cooldown.ExtraInfo);
            if(existing_cd != null) {
                _techniqueCooldowns.Remove(existing_cd);
            }
            _techniqueCooldowns.Add(cooldown);
        }
        else if(cooldown.Type.IsSubclassOf(typeof(Item))) {
            ToolCooldown = cooldown;
        }
        else {
            Debug.LogError("Tried to add cooldown for object that is not Effect, Ability or Item: " + cooldown.Type);
        }
        List<Effect> extras = Player.Instance.CurrentEffects.Where(e => e.GetType() == typeof(Effect_AddExtraCooldown) && ((Effect_AddExtraCooldown)e).AbilityItemOrEffectCooldown == cooldown.Type).ToList();
        if(extras.Count > 0) {
            foreach(Effect_AddExtraCooldown e in extras) {
                cooldown.TotalDuration += e.CooldownIncrease;
            }
        }
        cooldown.RemainingDuration = cooldown.TotalDuration;
        EventManager.CooldownAdded.Invoke(cooldown);
    }

    public void RemoveCooldown(Cooldown cd) {
        if(cd == ToolCooldown) {
            ToolCooldown = null;
        }
        else if(_effectCooldowns.Contains(cd)) {
            _effectCooldowns.Remove(cd);
        }
        else if(_techniqueCooldowns.Contains(cd)) {
            _techniqueCooldowns.Remove(cd);
        }
        else {
            Debug.LogError("Tried to remove cooldown that was not found on any cooldown list for unit " + gameObject.name + ": " +  cd.Type);
            return;           
        }
        if(cd.ShowCooldownInEffectUI && cd.CooldownDisplay != null) {
            MonoBehaviour.Destroy(cd.CooldownDisplay.transform.parent.gameObject);
        }
    }

    public void RemoveCooldown(Type cooldown_target_effect_ability_or_item) {
        if(cooldown_target_effect_ability_or_item.IsSubclassOf(typeof(Effect))) {
            Cooldown cd = _effectCooldowns.FirstOrDefault(cd => cd.Type == cooldown_target_effect_ability_or_item.GetType());
            if(cd != null) {
                _effectCooldowns.Remove(cd);
            }
            else {
                Debug.LogError("Tried to remove Effect cooldown that does not exist: " + cooldown_target_effect_ability_or_item);
            }
        }
        else if(cooldown_target_effect_ability_or_item.IsSubclassOf(typeof(Ability))) {
            Cooldown cd = _techniqueCooldowns.FirstOrDefault(cd => cd.Type == cooldown_target_effect_ability_or_item);
            if(cd != null) {
                _techniqueCooldowns.Remove(cd);
            }
            else {
                Debug.LogError("Tried to remove Ability cooldown that does not exist: " + cooldown_target_effect_ability_or_item);
            }
        }
        else if(cooldown_target_effect_ability_or_item.IsSubclassOf(typeof(Item))) {
            ToolCooldown = null;
        }
        else {
            Debug.LogError("Tried to remove cooldown for object that is not Effect, Ability or Item: " + cooldown_target_effect_ability_or_item);
        }
    }

    public void RemoveAllCooldowns() {
        _effectCooldowns.Clear();
        _techniqueCooldowns.Clear();
        ToolCooldown = null;
    }

    public void AddEffect(Effect effect_to_add, float seconds = 0) {
        if ((effect_to_add.Type == EffectType.Debuff && CheckIfUnderEffect(typeof(Effect_Invincible))) || (effect_to_add.CanBeNegatedByImmunityToCrowdControl && (effect_to_add.IsHardCrowdControl() || effect_to_add.IsSoftCrowdControl()) && CheckIfUnderEffect(typeof(Effect_Unstunnable)))) {
            return;
        }
        if (effect_to_add.SourceOfEffect == null) {
            effect_to_add = effect_to_add.SetAbilityCreatingThisEffect(Actions.CurrentAbilityBeingPerformed);
        }
        if (effect_to_add.UnitCreatingTheEffect == null) {
            effect_to_add = effect_to_add.SetSourceOfEffect(effect_to_add.SourceOfEffect != null ? effect_to_add.SourceOfEffect.User : this);
        }
        if (effect_to_add.TargetOfEffect == null) {
            effect_to_add = effect_to_add.SetTargetOfEffect(this);
            effect_to_add.AdditionalActionsOnSettingTargetOfEffect(this);
        }
        effect_to_add.BaseDuration = seconds;
        effect_to_add.Id = effect_to_add.TargetOfEffect + "-" + effect_to_add.GetType() + "-" + effect_to_add.BaseDuration + "-" + UnityEngine.Random.Range(0, 100000);
        if(effect_to_add.BehaviourWhenDuplicateEffect != BehaviourWhenDuplicateEffectEnum.AllowDuplicate) {
            if(effect_to_add.CheckIfEffectAlreadyAppliedAndHandleBehaviour()) {
                return;
            }
        }
        CurrentEffects.Add(effect_to_add);
        effect_to_add.OnStart();
        if (effect_to_add.AdditionalEffectsAffectingTargetDuringEffect.Count > 0)
        {
            foreach (Effect e in effect_to_add.AdditionalEffectsAffectingTargetDuringEffect)
            {
                e.ShowsInMenu = false;
                e.CountsAsSeparateEffect = false;
                AddEffect(e, effect_to_add.BaseDuration > 0 ? effect_to_add.BaseDuration : 0);
            }
        }
    }

    public bool CheckIfValidTarget(Unit target) {
            return !target.KnockedOut && CheckIfHostileTowards(target.Faction) && !target.CheckIfUnderEffect(typeof(Effect_CannotContinueCombat));
    }

    public void EndEffect(Effect effect_to_remove) {
        if(Actions == null || KnockedOut || CurrentEffects == null)
        {
            return;
        }
        if (CurrentEffects.Contains(effect_to_remove)) {
            CurrentEffects.Remove(effect_to_remove);
            effect_to_remove.OnEnd();
        }
    }

    public void EndEffect(Type effect_type_to_remove) {
        if(Actions == null || KnockedOut || CurrentEffects == null)
        {
            return;
        }
        Effect effect = CurrentEffects.FirstOrDefault(e => e.GetType() == effect_type_to_remove || e.GetType().IsSubclassOf(effect_type_to_remove));
        if(effect != null) {
            effect.EndThisEffect();
        }
    }

    public void SetUnitCollision(bool should_collide) {
        gameObject.layer = should_collide ? LayerMask.NameToLayer("Units") : LayerMask.NameToLayer("Ignore Collision");
        SpriteRenderers["Lower Body"].Bone.gameObject.layer = should_collide ? LayerMask.NameToLayer("Units") : LayerMask.NameToLayer("Ignore Collision");
        SpriteRenderers["Lower Body"].Bone.GetComponent<BoxCollider2D>().isTrigger = should_collide;
        GetComponent<NavMeshObstacle>().enabled = should_collide;
        CollisionTurnedOn = should_collide;
        Utils.CreateAuditLog("Collision turned " + (should_collide ? "ON" : "OFF"));
    }

    public void ApplyForce(Vector2 vector, Ability source) {
        if(!CheckIfUnderEffect(typeof(Effect_Immovable)) || (source != null && source.User == this))
        {
            Rigidbody2D.AddForce(vector, ForceMode2D.Force);
        }
    }

    public void OnDestroy() {
        if (_eliteEnemyIndicator != null) {
            Destroy(_eliteEnemyIndicator.transform.parent.gameObject);
        }
    }

    public virtual void UnitSpecificActionsAfterFlipDirection() {
    }

    public virtual bool CheckIfUnitShouldDecideCustomAction()
    {
        return false;
    }

    public void SetWeaponActive(string weapon_name, bool should_be_active) {
        SpriteRenderers["Upper Body"].Bone.transform.Find(weapon_name + (should_be_active ? " (Unused)" : "")).gameObject.name = weapon_name + (should_be_active ? "" :  " (Unused)");
        SpriteRenderers["Upper Body"].Bone.transform.Find(weapon_name + (should_be_active ? "" : " (Unused)")).GetComponent<SpriteRenderer>().sortingLayerName = should_be_active ? "Player" : "Behind";
        Animator.Rebind();
    }

    public class SpriteRendererInfo {
        public SpriteRenderer SpriteRenderer;
        public Transform Bone;
        public UnitWeapon Weapon;
        public UnityEngine.U2D.Animation.SpriteResolver SpriteResolver;
        public ColorChange ColorChange;
        public SortingGroup SortingGroup;
        private bool _isInFront;
        public bool IsInFront {
            get => _isInFront;
            set {
                _isInFront = value;
            }
        }
        public int SortingOrder = 100;
        public int DefaultSortingOrder;
        public String Name;
        public MaterialPropertyBlock MaterialPropertyBlock;

        public SpriteRendererInfo(string name, SpriteRenderer sprite_renderer) {
            Name = name.Replace(" (Unused)", "");
            SpriteRenderer = sprite_renderer;
            SpriteResolver = SpriteRenderer.GetComponent<SpriteResolver>();
            SortingGroup = SpriteRenderer.GetComponent<SortingGroup>();
            if(SortingGroup != null && SortingGroup.sortingLayerName == "Default") {
                SortingGroup.sortingLayerName = "Player";
            }
            Bone = sprite_renderer.transform.Find(Name + " Bone");
            if(Bone == null)
            {
                Bone = sprite_renderer.transform.Find(Name + " Bone 1");
            }
            if (Bone != null && (Name == "Heavy" || Name == "Light Right" || Name == "Light Left" || Name == "Ranged" || Name == "Projectile")) {
                Weapon = Bone.GetComponent<UnitWeapon>();
            }
            MaterialPropertyBlock = new MaterialPropertyBlock();
        }
    }
}