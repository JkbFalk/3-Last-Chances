using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Constants;
using UnityEngine.AI;

public class Player : Unit {

    public float StancePower = 1;
    public float ItemPower = 1;
    public float ToolPower = 1;
    public float PassivePowerUpPower = 1;

    [HideInInspector]
    public Collider2D Hitbox;
    private float _perfectBlockSpeed = 1;
    public float PerfectBlockSpeed {
        get => _perfectBlockSpeed;
        set {
            int min_value = IsStaggered ? 2 : 1;
            _perfectBlockSpeed = value > 4 ? 4 : value < min_value ? min_value : value;
            Animator.SetFloat("Perfect Block Speed", _perfectBlockSpeed);
        }
    }

    private float _techniqueSpeed = 1;
    public float TechniqueSpeed {
        get => _techniqueSpeed;
        set {
            _techniqueSpeed = value > 5 ? 5 : value < 0.2f ? 0.2f : value;
            Animator.SetFloat("Technique Speed", _techniqueSpeed);
        }
    }

    private bool _isPerfectlyBlocking = false;
    public bool IsPerfectlyBlocking {
        get => _isPerfectlyBlocking;
        set {
            _isPerfectlyBlocking = value;
            if(DebugController.WorldspaceDebugEnabled) {
                transform.Find("World Space Canvas/PerfectBlockIndicator").gameObject.SetActive(value);
            }
        }
    }

    public string CurrentArea;
    public List<Unit> UnitsInRangeForBackstab = new List<Unit>();
    public Dictionary<Type, int> CurrentTechniqueStacks = new() {
        {typeof(Ability_TempestStrikes), 1},
        {typeof(Ability_Flamethrower), 1},
        {typeof(Ability_SpearsOfIce), 1},
        {typeof(Ability_Fortify), 1},
        {typeof(Ability_ShadowInfusion), 1},
        {typeof(Ability_LightningSpeed), 1},
        {typeof(Ability_FinalBlast), 1}
    };

    public Dictionary<Type, int> CurrentUltimateTechniqueStacks = new() {
        {typeof(Ability_TempestStrikes), 1},
        {typeof(Ability_Flamethrower), 1},
        {typeof(Ability_SpearsOfIce), 1},
        {typeof(Ability_Fortify), 1},
        {typeof(Ability_ShadowInfusion), 1},
        {typeof(Ability_LightningSpeed), 1},
        {typeof(Ability_FinalBlast), 1}
    };

    private float _backstabCooldown = 20;
    public float BackstabCooldown
    {
        get
        {
            return _backstabCooldown < 5 ? 5 : _backstabCooldown;
        }
        set
        {
            _backstabCooldown = value;
        }
    }

    private int _inCombatTimer = 0;
    public int InCombatTimer
    {
        get
        {
            return _inCombatTimer;
        }
        set
        {
            _inCombatTimer = value;
            UIManager.Objects.InCombatFillImage.fillAmount = _inCombatTimer / (float)Constants.DEFAULT_FIXED_FRAMES_UNTIL_EXITING_COMBAT;
        }
    }

    public void UpdateTechniqueStacksAmount(Type technique, int stacks, bool is_ultimate = false) {
        if(Player.Instance.InCombat == false) {
            UpdateAllStacksWhileNotInCombat();
            return;
        }
        foreach(Stance.EquippedAbility ability in CurrentStance.Abilities) {
            ability.StacksCounter.text = ability.Type?.GetField("IsStacksBasedTechnique") == null ? "" : Player.Instance.CurrentTechniqueStacks[ability.Type].ToString();
        }
        int maxStacks = 0;
        if(technique.GetProperty((is_ultimate ? "Ultimate" : "") + "MaxStacks") != null) {
            maxStacks = (int)technique.GetProperty((is_ultimate ? "Ultimate" : "") + "MaxStacks").GetValue(null);
        }
        if(is_ultimate) {
            CurrentUltimateTechniqueStacks[technique] = stacks > maxStacks ? maxStacks : stacks;
        }
        else {
            CurrentTechniqueStacks[technique] = stacks > maxStacks ? maxStacks : stacks;
        }
        Cooldown cooldown = TechniqueCooldowns.FirstOrDefault(cd => cd.Type == technique && cd.Identifier == (is_ultimate ? "IsUltimate" : ""));
        if(cooldown != null && ((is_ultimate && CurrentUltimateTechniqueStacks[technique] == maxStacks) || (!is_ultimate && CurrentTechniqueStacks[technique] == maxStacks))) {
            cooldown.RemainingDuration = 0;
        }
        else if (cooldown == null) {
            AddCooldown(new Cooldown(technique, Ability.GetCooldown(technique), Player.Instance, is_ultimate ? "IsUltimate" : ""));
        }
    }

    public void UpdateAllStacksWhileNotInCombat() {
        foreach(Type ability in Utils.GetAllCurrentlyEquippedAbilityTypes()) {
            Cooldown cooldown1 = TechniqueCooldowns.FirstOrDefault(cd => cd.Type == ability && cd.Identifier == "");
            if(cooldown1 != null) {
                cooldown1.RemainingDuration = 0;
            }
            Cooldown cooldown2 = TechniqueCooldowns.FirstOrDefault(cd => cd.Type == ability && cd.Identifier == "IsUltimate");
            if(cooldown2 != null) {
                cooldown2.RemainingDuration = 0;
            }
            if(Player.Instance.InCombat == false && ability != null) {
                CurrentTechniqueStacks[ability] = 1;
                CurrentUltimateTechniqueStacks[ability] = 1;
            }
        }
        foreach(Stance.EquippedAbility ability in CurrentStance.Abilities) {
            ability.StacksCounter.text = ability.Type?.GetField("IsStacksBasedTechnique") == null ? "" : Player.Instance.CurrentTechniqueStacks[ability.Type].ToString();
        }
    }

    public List<InteractableObject> NearbyInteractables = new List<InteractableObject>();
    public InteractableObject ClosestInteractable;

    public void AddInteractable(InteractableObject interactable) {
        NearbyInteractables.Add(interactable);
        SetInteractPromptToClosestInteractable();
    }

    public void RemoveInteractable(InteractableObject interactable) {
        NearbyInteractables.Remove(interactable);
        SetInteractPromptToClosestInteractable();
    }

    public void SetInteractPromptToClosestInteractable() {
        if(GameController.Instance.LoadingNewArea) {
            return;
        }
        List<InteractableObject> validInteractables = NearbyInteractables.Where(inter => inter.CanBeInteractedWith).OrderByDescending(inter => inter.HasInteractionPriority).ThenBy(inter => Vector2.Distance(Player.Instance.transform.position, inter.transform.position)).ToList();
        if(validInteractables.Count > 0) {
            ClosestInteractable = validInteractables[0];
            UIManager.Objects.InteractIndicatorText.gameObject.SetActive(true);
            UIManager.Objects.InteractIndicatorText.text = string.Format(String.IsNullOrWhiteSpace(ClosestInteractable.SpecialInteractionLabel) ? Label.Get("Interact_Use") :  Label.Get(ClosestInteractable.SpecialInteractionLabel), new string[] {Settings.Instance.ControlScheme == "Keyboard" ? $"<sprite name=\"Keyboard_{Settings.Instance.Keybinds.FirstOrDefault(k => k.ActionName == "InteractButtonPress").KeyboardBinding1}\">" : $"<sprite name=\"{Settings.Instance.GamepadType}_{Settings.Instance.Keybinds.FirstOrDefault(k => k.ActionName == "InteractButtonPress").GamepadBinding1}\">"});
        }
        else if(validInteractables.Count == 0 && UIManager.Instance != null){
            ClosestInteractable = null;
            UIManager.Objects.InteractIndicatorText.gameObject.SetActive(false);
        }
    }

    public Stance _currentStance;
    public Stance CurrentStance {
        get {
            return _currentStance;
        }
        set {
            if(_currentStance != null && _currentStance.Weapon != null)
            {
                _currentStance.Weapon.DeactivateItemEffects();
            }
            Stance previousValue = _currentStance;
            _currentStance = value;
            ReplaceStanceDisplay();
            if(CurrentStance.WeaponClass != Constants.WeaponClass.None) {
                PlayAnimation(CurrentStance.WeaponClass.ToString() + "_StanceSwitch");
            }
            if(previousValue != null) {
                previousValue.StanceEffect.OnStanceDeactivated();
            }
            if(_currentStance?.StanceEffect != null) {
                _currentStance.StanceEffect.OnStanceActivated();
            }
            foreach (Stance.EquippedAbility ability in CurrentStance.Abilities) {
                ability.RefreshDisplayForEquippedAbility();
            }
            if (_currentStance != null && _currentStance.Weapon != null && previousValue != null)
            {
                _currentStance.Weapon.ActivateItemEffects();
            }
            UIManager.Objects.AmmoDisplayImage.gameObject.SetActive(Player.Instance.CurrentStance == SaveFile.Instance.Stances[2]);
            EventManager.StanceSwitched.Invoke();
        }
    }

    public void ReplaceStanceDisplay() {
        if(UIManager.Objects.StanceGaugeContainer.transform.childCount > 0) {
            MonoBehaviour.Destroy(UIManager.Objects.StanceGaugeContainer.transform.GetChild(0).gameObject);
        }
        if(CurrentStance != null && CurrentStance.StanceEffect != null) {
            CurrentStance.StanceEffect.CreateStanceDisplay();
        }
    }

    public Vector2 PlayerSavedPosition;

    public bool PreparingForUltimate = false;
    public GameObject CurrentStanceGauge;

    private static Player _instance = null;

    public static bool HasInstance()
    {
        return _instance != null;
    }

    public static Player Instance {
        get {
            if(_instance == null) {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _instance = player.GetComponent<Player>();
                }
                else {
                    _instance = ResetPlayer();
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    
    public static Player ResetPlayer()
    {
        if (_instance != null)
        {
            _instance.gameObject.tag = "Unit";
            MonoBehaviour.Destroy(_instance.gameObject);
        }
        GameObject player_object = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Unit/NPCs/Player")) as GameObject;
        if(Area.Instance != null) {
            player_object.transform.SetParent(Area.Instance.transform);
        }
        Instance = player_object.GetComponent<Player>();
        Instance.Initialize();
        Instance.Energy.Current = Constants.FULLY_RESTED_INITIAL_ENERGY;
        Instance.Ammo = Constants.FULLY_RESTED_INITIAL_AMMO;
        Instance._techniqueCooldowns.Clear();
        Instance.ToolCooldown = null;
        Instance._effectCooldowns.Clear();
        Instance.PlayAnimation("Idle");
        Instance.Actions.IsFlipped = false;
        SaveFile.Instance.RefreshStances();
        foreach(Transform child in UIManager.Objects.Effects.transform) {
            MonoBehaviour.Destroy(child.gameObject);
        }
        foreach(Item item in MenuManager.Instance.EquippedItems) {
            if(item != null) {
                item.Equip();
            }
        }
        foreach(PassivePowerUpTile tile in MenuManager.Instance.PowerUpTiles) {
            if(SaveFile.Instance.UnlockedPowerUps.Contains(tile.Id)) {
                tile.ApplyPowerUpOfThisTile();
            }
        }
        foreach(Tuple<string, int> power_up in SaveFile.Instance.PermanentPowerUps) {
            foreach(Effect e in EffectList.GetEffect(power_up.Item1, power_up.Item2)) {
                Player.Instance.AddEffect(e);
            }
        }
        InventoryTile.RefreshUsableItemUI(1, SaveFile.Instance.EquippedItem1);
        InventoryTile.RefreshUsableItemUI(2, SaveFile.Instance.EquippedItem2);
        GameController.Instance.DefaultTimeSpeed = 1;
        Settings.Instance.FieldOfView = Settings.Instance.FieldOfView;
        return player_object.GetComponent<Player>();
    }

    public static void ChangeInCombatDependantUI(bool player_is_in_combat) {
        /*foreach(ChangeActiveStatusBasedOnInCombat act in MenuManager.Instance.GetComponentsInChildren<ChangeActiveStatusBasedOnInCombat>(true)) {
            if(act.ChangeInteractableInsteadOfActive && player_is_in_combat && act.WhenInCombatChangeToActive) {
                act.GetComponent<Selectable>().interactable = true;
            }
            else if(act.ChangeInteractableInsteadOfActive && player_is_in_combat && act.WhenInCombatChangeToInactive) {
                act.GetComponent<Selectable>().interactable = false;
            }
            else if(act.ChangeInteractableInsteadOfActive && !player_is_in_combat && act.WhenNotInCombatChangeToActive) {
                act.GetComponent<Selectable>().interactable = true;
            }
            else if(act.ChangeInteractableInsteadOfActive && !player_is_in_combat && act.WhenNotInCombatChangeToInactive) {
                act.GetComponent<Selectable>().interactable = false;
            }
            else if(player_is_in_combat && act.WhenInCombatChangeToActive) {
                act.gameObject.SetActive(true);
            }
            else if(player_is_in_combat && act.WhenInCombatChangeToInactive) {
                act.gameObject.SetActive(false);
            }
            else if(!player_is_in_combat && act.WhenNotInCombatChangeToActive) {
                act.gameObject.SetActive(true);
            }
            else if(!player_is_in_combat && act.WhenNotInCombatChangeToInactive) {
                act.gameObject.SetActive(false);
            }
        }
        */
        if(player_is_in_combat) {
            UIManager.Objects.InCombatMaskImage.color = Color.white;
            UIManager.Objects.InCombatFillImage.gameObject.SetActive(true);
        }
        else {
            UIManager.Objects.InCombatMaskImage.color = Colors.OutOfCombat;
            UIManager.Objects.InCombatFillImage.gameObject.SetActive(false);
        }
    }

    public static void RestAtBonfire(InteractableObject obj) {
        UIManager.Instance.ShowBlackScreen(4.5f);
        Player.Instance.Actions.CurrentActionBeingPerformed = ActionType.Idle;
        Player.Instance.Actions.TryingToMoveInDirection.Clear();
        Player.Instance.PlayAnimation("RestAtBonfire");
        GameController.Instance.GetComponent<PlayerInput>().enabled = false;
        GameController.Instance.WaitAndRunMethod(5, RestAtBonfireContinued, obj.gameObject);
    }

    public static void RestAtBonfireContinued(GameObject bonfire) {
        bonfire.transform.Find("OnDestroy").gameObject.SetActive(true);
        bonfire.transform.Find("OnDestroy").SetParent(bonfire.transform.parent);
        MonoBehaviour.Destroy(bonfire);
        UIManager.Instance.HideBlackScreen(2);
        GameController.Instance.GetComponent<PlayerInput>().enabled = true;
        Player.Instance.CompleteRest();
    }

    public void CompleteRest() {
        Player.Instance.InCombat = false;
        Player.Instance.Health.Current = Player.Instance.Health.Maximum;
        Player.Instance.StaggerBar.Current = 0;
        Player.Instance.Energy.Current = Constants.FULLY_RESTED_INITIAL_ENERGY;
        Player.Instance.Ammo = Constants.FULLY_RESTED_INITIAL_AMMO;
        SaveFile.Instance.UltimatesUsedInCurrentCombat = 0;
        RemoveAllCooldowns();
        foreach(Type tool in SaveFile.Instance.ToolRemainingAmounts.Keys.ToList()) {
            SaveFile.Instance.ToolRemainingAmounts[tool] = SaveFile.Instance.ToolMaxAmounts[tool];
        }
        foreach(Type technique in Player.Instance.CurrentTechniqueStacks.Keys.ToList()) {
            Player.Instance.CurrentTechniqueStacks[technique] = 1;
        }
        GameController.Instance.WaitAndRunMethod(0.1f, ExitCombat);
    }

    public void ExitCombat() {
        Player.Instance.InCombat = false;
    }

    [HideInInspector]
    public GameObject InjuryIndicator;

    [HideInInspector]
    public GameObject StaggerIndicator;
    public int MostRecentLongbladeBANumber = 0;
    public int GetLongbladeBANumber() {
        if (MostRecentLongbladeBANumber == 1)
        {
            return new int[] { 2, 3, 4, 5 }[UnityEngine.Random.Range(0, 4)];
        }
        else if (MostRecentLongbladeBANumber == 2)
        {
            return new int[] { 1, 3, 4, 5 }[UnityEngine.Random.Range(0, 4)];
        }
        else if (MostRecentLongbladeBANumber == 3)
        {
            return new int[] { 1, 2, 4, 5 }[UnityEngine.Random.Range(0, 4)];
        }
        else if (MostRecentLongbladeBANumber == 4)
        {
            return new int[] { 1, 2, 3, 5 }[UnityEngine.Random.Range(0, 4)];
        }
        else if (MostRecentLongbladeBANumber == 5)
        {
            return new int[] { 1, 2, 3, 4 }[UnityEngine.Random.Range(0, 4)];
        }
        return UnityEngine.Random.Range(1, 6);
    }
    public int MostRecentGauntletBANumber = 0;
    public int GetGauntletBANumber() {
        if(MostRecentGauntletBANumber == 1){
            return new int[] {2, 3, 5}[UnityEngine.Random.Range(0, 3)];
        }
        else if(MostRecentGauntletBANumber == 2){
            return new int[] {1, 3, 4}[UnityEngine.Random.Range(0, 3)];
        }
        else if(MostRecentGauntletBANumber == 3){
            return new int[] {1, 2, 4, 5}[UnityEngine.Random.Range(0, 4)];
        }
        else if(MostRecentGauntletBANumber == 4){
            return new int[] {2, 3, 5}[UnityEngine.Random.Range(0, 3)];
        }
        else if(MostRecentGauntletBANumber == 5){
            return new int[] {1, 3, 4}[UnityEngine.Random.Range(0, 3)];
        }
        return UnityEngine.Random.Range(1, 6);
    }

    public void PerformGauntletStrongBasicAttack() {
        int number = GetGauntletBANumber();
        if(number == 1) {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_R1(this);
        }
        else if(number == 2) {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_R2(this);
        }
        else if(number == 3) {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_R3(this);
        }
        else if(number == 4) {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_R4(this);
        }
        else if(number == 5) {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_R5(this);
        }
        MostRecentGauntletBANumber = number;
    }

    public void PerformGauntletBasicAttack()
    {
        int number = GetGauntletBANumber();
        if (number == 1)
        {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_L1(this);
        }
        else if (number == 2)
        {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_L2(this);
        }
        else if (number == 3)
        {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_L3(this);
        }
        else if (number == 4)
        {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_L4(this);
        }
        else if (number == 5)
        {
            Actions.CurrentAbilityBeingPerformed = new BA_Gauntlets_L5(this);
        }
        MostRecentGauntletBANumber = number;
    }
    public List<Unit> EnemiesInCombatWithPlayer = new();

    public Dictionary<string, int> AbilityLevels = new Dictionary<string, int>();
    public EnergyGain EnergyGain { get; set; }

    public void EnterIdleState() {
        Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
        Actions.CurrentAbilityBeingPerformed = null;
    }

    public void Initialize() {
        UIManager.Objects.HealingItemText.text = "+" + SaveFile.Instance.HealUpgrades.ToString();
        SaveFile.Instance.MaxHealCharges = SaveFile.Instance.DifficultyLevel == 0 ? 10 : SaveFile.Instance.DifficultyLevel == 1 ? 5 : 2;
        SaveFile.Instance.HealChargesRemaining = SaveFile.Instance.MaxHealCharges;
        Actions = GetComponent<Actions>();
        Animator = GetComponent<Animator>();
        InitializeStats();
        EnergyGain = new EnergyGain(this, 0);
        Stats.Add(EnergyGain);
        
        Hitbox = SpriteRenderers["Lower Body"].Bone.GetComponent<CapsuleCollider2D>();
        if(SaveFile.Instance.Stances == null) {
            InitializeStances();
        }
        else {
            CurrentStance = SaveFile.Instance.Stances[0];
            UIManager.Instance.ResetStanceDisplay();
        }
        StaggerBar.HUDFill.color = Colors.StaggerColor;
        EventManager.PlayerObjectReinitialized.Invoke();
    }

    public void InitializeStances() {
        SaveFile.Instance.Stances = new List<Stance>();
        SaveFile.Instance.Stances.Add(new Stance {
            WeaponType = ItemType.Heavy,
            Abilities = new List<Stance.EquippedAbility> {
                new Stance.EquippedAbility(null, 1, DamageType.Heavy),
                new Stance.EquippedAbility(null, 2, DamageType.Heavy),
                new Stance.EquippedAbility(null, 3, DamageType.Heavy),
                new Stance.EquippedAbility(null, 4, DamageType.Heavy),
                },
                StanceEffect = new Stance_None(new(Player.Instance))
        }
        );
        SaveFile.Instance.Stances.Add(new Stance {
            WeaponType = ItemType.Light,
            Abilities = new List<Stance.EquippedAbility> {
                new Stance.EquippedAbility(null, 1, DamageType.Light),
                new Stance.EquippedAbility(null, 2, DamageType.Light),
                new Stance.EquippedAbility(null, 3, DamageType.Light),
                new Stance.EquippedAbility(null, 4, DamageType.Light),
                },
                StanceEffect = new Stance_None(new(Player.Instance))
        }
        );
        SaveFile.Instance.Stances.Add(new Stance {
            WeaponType = ItemType.Ranged,
            Abilities = new List<Stance.EquippedAbility> {
                new Stance.EquippedAbility(null, 1, DamageType.Ranged),
                new Stance.EquippedAbility(null, 2, DamageType.Ranged),
                new Stance.EquippedAbility(null, 3, DamageType.Ranged),
                new Stance.EquippedAbility(null, 4, DamageType.Ranged),
                },
                StanceEffect = new Stance_None(new(Player.Instance))
        }
        );
        UIManager.Instance.ResetStanceDisplay();
        Ammo = 6;
        CurrentStance = SaveFile.Instance.Stances[0];
        Energy.MarkAbilitiesWithNotEnoughEnergy();
        foreach(Item item in MenuManager.Instance.EquippedItems) {
            item.Equip();
        }
    }

    public override void AdditionalUnitSpecificActionsOnFixedUpdate() {
        Cooldown stanceSwitchCd = TechniqueCooldowns.Where(cd => cd.Type == typeof(Ability_StanceSwitch)).FirstOrDefault();
        if (stanceSwitchCd != null) {
            foreach (Stance stance in SaveFile.Instance.Stances)
            {
                if(stance.StanceCooldownDisplay != null) {
                    stance.StanceCooldownDisplay.fillAmount = stanceSwitchCd.RemainingDuration / stanceSwitchCd.TotalDuration;
                }
            }
        }
        else if(CurrentStance != null && CurrentStance.StanceCooldownDisplay != null && CurrentStance.StanceCooldownDisplay.fillAmount != 0)
        {
            foreach (Stance stance in SaveFile.Instance.Stances)
            {
                if(stance.StanceCooldownDisplay != null) {
                    stance.StanceCooldownDisplay.fillAmount = 0;
                }
            }
        }
        if (InCombat && InCombatTimer > 0 && PotentialTargets.Count == 0)
        {
            InCombatTimer--;
        }
        if (InCombat && InCombatTimer <= 0)
        {
            InCombat = false;
        }
        if(PerfectBlockSpeed > 1 || (IsStaggered && PerfectBlockSpeed > 2)) {
            PerfectBlockSpeed -= 0.002f + PerfectBlockSpeed / 250;
        }
        if(NearbyInteractables.Count > 0) {
            SetInteractPromptToClosestInteractable();
        }
    }

    public bool CheckIfAbilityOnCooldown(Type ability_type)
    {
        return TechniqueCooldowns.Where(cd => cd.Type == ability_type || cd.Type.IsSubclassOf(ability_type)).FirstOrDefault() != null;
    }

    public Stance GetStanceForGivenWeapon(Constants.ItemType weapon_type) {
        if (SaveFile.Instance.Stances[0].WeaponType == weapon_type) {
            return SaveFile.Instance.Stances[0];
        }
        else if (SaveFile.Instance.Stances[1].WeaponType == weapon_type) {
            return SaveFile.Instance.Stances[1];
        }
        else {
            return SaveFile.Instance.Stances[2];
        }
    }

    public Stance GetStanceOnTheLeft() {
        if (Player.Instance.CurrentStance == SaveFile.Instance.Stances[0]) {
            return SaveFile.Instance.Stances[2];
        }
        else if (Player.Instance.CurrentStance == SaveFile.Instance.Stances[1]) {
            return SaveFile.Instance.Stances[0];
        }
        else {
            return SaveFile.Instance.Stances[1];
        }
    }

    public Stance GetStanceOnTheRight() {
        if (Player.Instance.CurrentStance == SaveFile.Instance.Stances[0]) {
            return SaveFile.Instance.Stances[1];
        }
        else if (Player.Instance.CurrentStance == SaveFile.Instance.Stances[1]) {
            return SaveFile.Instance.Stances[2];
        }
        else {
            return SaveFile.Instance.Stances[0];
        }
    }

    public int GetCurrentStanceIndex() {
        switch(CurrentStance.WeaponType) {
            case ItemType.Heavy: return 0;
            case ItemType.Light: return 1;
            case ItemType.Ranged: return 2;
            default: return 0;
        }
    }
}