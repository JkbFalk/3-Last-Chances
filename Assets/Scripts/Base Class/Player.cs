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

    [HideInInspector]
    public TextMeshProUGUI InteractIndicatorText;
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
        {typeof(Ability_Flamethrower), 1}
    };

    public Dictionary<Type, int> CurrentUltimateTechniqueStacks = new() {
        {typeof(Ability_TempestStrikes), 1},
        {typeof(Ability_Flamethrower), 1}
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
            CanvasElements.UICanvas.InCombatFill.GetComponent<Image>().fillAmount = _inCombatTimer / (float)Constants.DEFAULT_FIXED_FRAMES_UNTIL_EXITING_COMBAT;
        }
    }

    public void UpdateTechniqueStacksAmount(Type technique, int stacks, bool is_ultimate = false) {
        if(Player.Instance.InCombat == false) {
            UpdateAllStacksWhileNotInCombat();
            return;
        }
        Debug.Log($"Updating stacks for {technique}: {Player.Instance.CurrentTechniqueStacks[technique]} -> {stacks} (isUlt? {is_ultimate})");
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
        Cooldown cooldown = AbilityCooldowns.FirstOrDefault(cd => cd.Type == technique && cd.ExtraInfo == (is_ultimate ? "IsUltimate" : ""));
        if(cooldown != null && ((is_ultimate && CurrentUltimateTechniqueStacks[technique] == maxStacks) || (!is_ultimate && CurrentTechniqueStacks[technique] == maxStacks))) {
            cooldown.RemainingDuration = 0;
        }
        else if (cooldown == null) {
            AddCooldown(new Cooldown(technique, Ability.GetCooldown(technique), Player.Instance) {ExtraInfo = is_ultimate ? "IsUltimate" : ""});
        }
    }

    public void UpdateAllStacksWhileNotInCombat() {
        foreach(Type ability in Utils.GetAllCurrentlyEquippedAbilityTypes()) {
            Cooldown cooldown1 = AbilityCooldowns.FirstOrDefault(cd => cd.Type == ability && cd.ExtraInfo == "");
            if(cooldown1 != null) {
                cooldown1.RemainingDuration = 0;
            }
            Cooldown cooldown2 = AbilityCooldowns.FirstOrDefault(cd => cd.Type == ability && cd.ExtraInfo == "IsUltimate");
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
            InteractIndicatorText.gameObject.SetActive(true);
            InteractIndicatorText.text = string.Format(String.IsNullOrWhiteSpace(ClosestInteractable.SpecialInteractionLabel) ? Label.Get("Interact_Use") :  Label.Get(ClosestInteractable.SpecialInteractionLabel), new string[] {"<sprite name=\"InteractBinding" + Settings.Instance.ControlScheme + "\">"});
        }
        else if(validInteractables.Count == 0 && CanvasElements.UICanvasObject != null){
            ClosestInteractable = null;
            InteractIndicatorText.gameObject.SetActive(false);
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
                foreach (Effect mod in _currentStance.Weapon.FirstModifier)
                {
                    if(mod.RemainsActiveInOtherStances == false) {
                        Player.Instance.EndEffect(mod);
                    }
                }
                foreach (Effect mod in _currentStance.Weapon.SecondModifier)
                {
                    if(mod.RemainsActiveInOtherStances == false) {
                        Player.Instance.EndEffect(mod);
                    }
                }
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
                foreach (Effect mod in _currentStance.Weapon.FirstModifier)
                {
                    if(mod.RemainsActiveInOtherStances == false) {
                        mod.NonLinearEffectValue = _currentStance.Weapon.GetFirstModifierEffectValue(false);
                        mod.LinearEffectValue = _currentStance.Weapon.GetFirstModifierEffectValue();
                        mod.OnEffectValueChanged();
                        mod.IsRemovable = false;
                        mod.ShowsInMenu=false;
                        Player.Instance.AddEffect(mod);
                    }
                }
                foreach (Effect mod in _currentStance.Weapon.SecondModifier)
                {
                    if(mod.RemainsActiveInOtherStances == false) {
                        mod.NonLinearEffectValue = _currentStance.Weapon.GetFirstModifierEffectValue(false);
                        mod.LinearEffectValue = _currentStance.Weapon.GetFirstModifierEffectValue();
                        mod.OnEffectValueChanged();
                        mod.IsRemovable = false;
                        mod.ShowsInMenu=false;
                        Player.Instance.AddEffect(mod);
                    }
                }
            }
            CanvasElements.UICanvas.AmmoDisplay.gameObject.SetActive(CurrentStance.WeaponClass == Constants.WeaponClass.Gun || CurrentStance.WeaponClass == Constants.WeaponClass.Bow || CurrentStance.WeaponClass == Constants.WeaponClass.Cannon);
            EventManager.StanceSwitched.Invoke();
        }
    }

    public void ReplaceStanceDisplay() {
        if(CanvasElements.UICanvas.StanceGaugeContainer.transform.childCount > 0) {
            MonoBehaviour.Destroy(CanvasElements.UICanvas.StanceGaugeContainer.transform.GetChild(0).gameObject);
        }
        if(CurrentStance != null && CurrentStance.StanceEffect != null) {
            CurrentStance.StanceEffect.CreateStanceDisplay();
        }
    }

    public bool PreparingForUltimate = false;
    public GameObject CurrentStanceGauge;

    private static Player _instance = null;

    public static bool HasInstance() {
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
        Instance._abilityCooldowns.Clear();
        Instance.ItemsCooldown = null;
        Instance._effectCooldowns.Clear();
        Instance.PlayAnimation("Idle");
        Instance.Actions.IsFlipped = false;
        SaveFile.Instance.RefreshStances();
        foreach(Transform child in CanvasElements.UICanvas.Effects.transform) {
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
        foreach(string power_up in SaveFile.Instance.PermanentPowerUps) {
            foreach(Effect e in PassivePowerUpTile.GetPassivePowerUpEffects(power_up)) {
                Player.Instance.AddEffect(e);
            }
        }
        InventoryTile.RefreshUsableItemUI(1, SaveFile.Instance.EquippedItem1);
        InventoryTile.RefreshUsableItemUI(2, SaveFile.Instance.EquippedItem2);
        if(SaveFile.Instance.EquippedEnergyFamily != Ability.AbilityFamily.None) {
            Effect_EnergyUpgrade new_effect = (Effect_EnergyUpgrade)Activator.CreateInstance(Type.GetType("Effect_" + SaveFile.Instance.EquippedEnergyFamily + "Energy"), new object[] {null});
            new_effect.UpgradeLevel = SaveFile.Instance.EnergyUpgrades[SaveFile.Instance.EquippedEnergyFamily];
            Player.Instance.AddEffect(new_effect);
        }
        GameController.Instance.DefaultTimeSpeed = 1;
        Settings.Instance.FieldOfView = Settings.Instance.FieldOfView;
        return player_object.GetComponent<Player>();
    }

    public static void ChangeInCombatDependantUI(bool player_is_in_combat) {
        foreach(ChangeActiveStatusBasedOnInCombat act in MenuManager.Instance.GetComponentsInChildren<ChangeActiveStatusBasedOnInCombat>(true)) {
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

        if(player_is_in_combat) {
            CanvasElements.UICanvas.InCombatMask.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            CanvasElements.UICanvas.InCombatFill.SetActive(true);
        }
        else {
            CanvasElements.UICanvas.InCombatMask.GetComponent<UnityEngine.UI.Image>().color = Colors.OutOfCombat;
            CanvasElements.UICanvas.InCombatFill.SetActive(false);
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

    public Effect_EnergyUpgrade EnergyEffect;
    public Camera Camera { get; set; }
    public CameraController CameraController { get; set; }

    [HideInInspector]
    public GameObject InjuryIndicator;

    [HideInInspector]
    public GameObject StaggerIndicator;
    public List<Unit> EnemiesInCombatWithPlayer = new();

    public Dictionary<string, int> AbilityLevels = new Dictionary<string, int>();
    
    public ToolPower ToolPower { get; set; }
    public ItemPower ItemPower { get; set; }
    public EnergyGain EnergyGain { get; set; }

    public void EnterIdleState() {
        Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
        Actions.CurrentAbilityBeingPerformed = null;
    }

    public void Initialize() {
        CanvasElements.UICanvas.Items.transform.Find("Heal/Upgrade").GetComponent<TextMeshProUGUI>().text = "+" + SaveFile.Instance.HealUpgrades.ToString();
        SaveFile.Instance.MaxHealCharges = SaveFile.Instance.DifficultyLevel == 0 ? 10 : SaveFile.Instance.DifficultyLevel == 1 ? 5 : 2;
        SaveFile.Instance.HealChargesRemaining = SaveFile.Instance.MaxHealCharges;
        Actions = GetComponent<Actions>();
        Animator = GetComponent<Animator>();
        Camera = GetComponentInChildren<Camera>();
        CameraController = GetComponentInChildren<CameraController>();
        InteractIndicatorText = CanvasElements.UICanvasObject.transform.Find("Interact Indicator").GetComponent<TextMeshProUGUI>();
        InitializeStats();
        ToolPower = new ToolPower(this, 1);
        ItemPower = new ItemPower(this, 1);
        EnergyGain = new EnergyGain(this, 1);
        Stats.Add(ToolPower);
        Stats.Add(ItemPower);
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
            WeaponCategory = ItemCategory.Heavy,
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
            WeaponCategory = ItemCategory.Light,
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
            WeaponCategory = ItemCategory.Ranged,
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
        Cooldown stanceSwitchCd = AbilityCooldowns.Where(cd => cd.Type == typeof(Ability_StanceSwitch)).FirstOrDefault();
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
        return AbilityCooldowns.Where(cd => cd.Type == ability_type || cd.Type.IsSubclassOf(ability_type)).FirstOrDefault() != null;
    }

    public Stance GetStanceForGivenWeapon(Constants.ItemCategory weapon_category) {
        if (SaveFile.Instance.Stances[0].WeaponCategory == weapon_category) {
            return SaveFile.Instance.Stances[0];
        }
        else if (SaveFile.Instance.Stances[1].WeaponCategory == weapon_category) {
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
        switch(CurrentStance.WeaponCategory) {
            case ItemCategory.Heavy: return 0;
            case ItemCategory.Light: return 1;
            case ItemCategory.Ranged: return 2;
            default: return 0;
        }
    }
}