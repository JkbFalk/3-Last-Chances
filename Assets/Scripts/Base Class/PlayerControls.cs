using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using static Constants;
using Random = UnityEngine.Random;

public class PlayerControls : WorldObject {
    public static int BasicAttackButtonPressCounter = 0;
    public static float BasicAttackButtonHoldDuration = 0;
    public static float BlockButtonHoldDuration = 0;
    public static List<string> KeyboardViableInputs = new List<string> { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "escape", "space", "enter", "tab", "backquote", "quote", "semicolon", "comma", "period", "slash", "backslash", "leftBracket", "rightBracket", "minus", "equals", "backspace", "upArrow", "downArrow", "rightArrow", "leftArrow", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10", "f11", "f12", "leftShift", "rightShift", "leftAlt", "rightAlt", "leftCtrl", "rightCtrl", "insert", "home", "pageDown", "delete", "end", "pageUp", "escape", "leftMeta", "rightMeta", "capsLock" };
    public static List<string> MouseViableInputs = new List<string> {"middleButton", "leftButton", "rightButton", "forwardButton", "backButton"};
    public static List<string> GamepadViableInputs = new List<string> {"buttonNorth", "buttonSouth", "buttonEast", "buttonWest", "dpadup", "dpaddown", "dpadright", "dpadleft", "leftStickup", "leftStickdown", "leftStickright", "leftStickleft", "leftStickPress", "rightShoulder", "leftShoulder", "rightTrigger", "leftTrigger", "select", "start", "rightStickup", "rightStickdown", "rightStickright", "rightStickleft", "rightStickPress"};

    [HideInInspector]
    public Vector2 CurrentLeftStickPosition;

    [HideInInspector]
    public Vector2 CurrentWorldspacePointerPosition;

    private Actions _actions
    {
        get
        {
            return Player.Instance.Actions;
        }
    }

    private Player _player
    {
        get
        {
            return Player.Instance;
        }
    }



    private bool _gamepadAbilitiesActive = false;
    public bool GamepadAbilitiesActive
    {
        get => _gamepadAbilitiesActive;
        set
        {
            if (value)
            {
                GamepadItemsActive = false;
            }
            _gamepadAbilitiesActive = value;
            UIManager.Objects.AbilitiesGamepad.transform.localScale = new Vector3(_gamepadAbilitiesActive ? 1.1f : 1, _gamepadAbilitiesActive ? 1.1f : 1, 1);
            UIManager.Objects.AbilitiesGamepad.GetComponent<CanvasGroup>().alpha = _gamepadAbilitiesActive ? 1 : 0.5f;
            UIManager.Objects.GamepadBindingAbilitiesImage.transform.localScale = new Vector3(_gamepadAbilitiesActive ? 1.4f : 1, _gamepadAbilitiesActive ? 1.4f : 1, 1);
        }
    }
    private bool _gamepadItemsActive = false;
    public bool GamepadItemsActive
    {
        get => _gamepadItemsActive;
        set
        {
            if(value)
            {
                GamepadAbilitiesActive = false;
            }
            _gamepadItemsActive = value;
            UIManager.Objects.ItemsGamepad.transform.localScale = new Vector3(_gamepadItemsActive ? 1.1f : 1, _gamepadItemsActive ? 1.1f : 1, 1);
            UIManager.Objects.ItemsGamepad.GetComponent<CanvasGroup>().alpha = _gamepadItemsActive ? 1 : 0.5f;
            UIManager.Objects.GamepadBindingItemsImage.transform.localScale = new Vector3(_gamepadItemsActive ? 1.4f : 1, _gamepadItemsActive ? 1.4f : 1, 1);
        }
    }

    public void OnGamepadAbilitiesButtonPress()
    {
        GamepadAbilitiesActive = true;
        if(GamepadItemsActive) {
            OnPrepareUltimateButtonPress();
        }
    }

    public void OnGamepadAbilitiesButtonRelease()
    {
        GamepadAbilitiesActive = false;
        if(GamepadItemsActive) {
            StopPreparingUltimate();
        }
    }

    public void OnGamepadItemsButtonPress()
    {
        GamepadItemsActive = true;
        if(GamepadAbilitiesActive) {
            OnPrepareUltimateButtonPress();
        }
    }

    public void OnGamepadItemsButtonRelease()
    {
        GamepadItemsActive = false;
        if(GamepadAbilitiesActive) {
            StopPreparingUltimate();
        }
    }

    public bool SpammingButton = false;
    public bool IsOn = false;

    private void Update() {
        if (PlayerControls.BasicAttackButtonHoldDuration > 0)
        {
            PlayerControls.BasicAttackButtonHoldDuration += Time.deltaTime;
        }
        if (PlayerControls.BlockButtonHoldDuration > 0)
        {
            PlayerControls.BlockButtonHoldDuration += Time.deltaTime;
        }
        if (GameController.Instance?.Camera != null && Mouse.current?.position?.ReadValue() != null)
        {
            CurrentWorldspacePointerPosition = GameController.Instance.Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
        if(SpammingButton && IsOn) {
            OnBlockButtonRelease();
            IsOn = false;
        }
        else if(SpammingButton && !IsOn) {
            OnBlockButtonPress();
            IsOn = true;
        }
    }

    public void OnOpenCharacterMenuButtonPress() {
        MenuManager.Instance.SelectedSubMenu = 0;
        OnToggleMenuButtonPress();
    }

    public void OnOpenInventoryMenuButtonPress() {
        MenuManager.Instance.SelectedSubMenu = 1;
        OnToggleMenuButtonPress();
    }

    public void OnOpenSkillTreeMenuButtonPress() {
        MenuManager.Instance.SelectedSubMenu = 2;
        OnToggleMenuButtonPress();
    }

    public void OnOpenArchiveMenuButtonPress() {
        MenuManager.Instance.SelectedSubMenu = 3;
        OnToggleMenuButtonPress();
    }

    public void OnOpenTutorialsMenuButtonPress() {
        MenuManager.Instance.SelectedSubMenu = 4;
        OnToggleMenuButtonPress();
    }

    public void OnOpenOptionsMenuButtonPress() {
        MenuManager.Instance.SelectedSubMenu = 5;
        OnToggleMenuButtonPress();
    }

    public void OnToggleMenuButtonPress() {
        if(GameController.Objects.SaveAndLoadPanel.activeSelf) {
            GameController.Instance.ToggleLoadPanel(false);
            GameController.Instance.ToggleSavePanel(false);
            return;
        }
        if(GameController.Instance.GameplayMode == GameplayMode.InCutscene && UIManager.Instance.DialogueHistoryIsOpen) {
            UIManager.Instance.CloseDialogueHistory();
            return;
        }
        if(SceneManager.GetActiveScene().name == "MissionSelect" && Utils.GetSceneRootObject("Mission Select").transform.Find("Survival Type Selection").gameObject.activeSelf) {
            Utils.GetSceneRootObject("Mission Select").transform.Find("Survival Type Selection").gameObject.SetActive(false);
            return;
        }
        if (GameController.Instance.GameplayMode == Constants.GameplayMode.Regular || GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect || GameController.Instance.GameplayMode == GameplayMode.OnStartScreen || GameController.Instance.GameplayMode == Constants.GameplayMode.Paused) {
            GameController.Instance.GameplayMode = Constants.GameplayMode.InMenu;
        }
        else if (GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu) {
            GameController.Instance.GameplayMode = SceneManager.GetActiveScene().name == "StartScreen" ? GameplayMode.OnStartScreen : (SaveFile.Instance.CurrentMission == null && SaveFile.Instance.GameType == GameType.Story) ? Constants.GameplayMode.MissionSelect : Constants.GameplayMode.Regular;
            
        }
    }

    public void OnPauseScreenButtonPress() {
        if(GameController.Objects.SaveAndLoadPanel.activeSelf) {
            GameController.Instance.ToggleLoadPanel(false);
            GameController.Instance.ToggleSavePanel(false);
            return;
        }
        if(GameController.Instance.GameplayMode == GameplayMode.Paused && SceneManager.GetActiveScene().name == "MissionSelect") {
            GameController.Instance.GameplayMode = GameplayMode.MissionSelect;
        }
        else if(GameController.Instance.GameplayMode == GameplayMode.Paused) {
            GameController.Instance.GameplayMode = GameplayMode.Regular;
        }
        else {
            GameController.Instance.GameplayMode = GameplayMode.Paused;
        }
    }

    public void OnSelectDefaultObjectButtonPress()
    {
        if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.activeInHierarchy == false || EventSystem.current.currentSelectedGameObject.GetComponentInParent<CanvasGroup>() == null || EventSystem.current.currentSelectedGameObject.GetComponentInParent<CanvasGroup>().alpha == 0)
        {
            GamepadSelectObjectClosestToCenter();
            /*GameObject to_select = null;
            foreach(Selectable selectable in Selectable.allSelectablesArray)
            {
                if (selectable.GetComponentInParent<CanvasGroup>() != null && selectable.GetComponentInParent<CanvasGroup>().alpha == 1)
                {
                    to_select = selectable.gameObject;
                    break;
                }
            }
            Utils.CreateAuditLog("Selecdting default gamepad object: " + Utils.GetGameObjectPath(to_select));
            EventSystem.current.SetSelectedGameObject(to_select);*/
        }
    }

    public void OnChangeSubMenuLeftButtonPress()
    {
        MenuManager.Instance.SelectedSubMenu--;
    }

    public void OnChangeSubMenuRightButtonPress()
    {
        MenuManager.Instance.SelectedSubMenu++;
    }

    public void OnRunUpButtonPress() {
        _actions.AddDirection("Up");
    }

    public void OnRunUpButtonRelease() {
        _actions.RemoveDirection("Up");
    }

    public void OnRunRightButtonPress() {
        _actions.AddDirection("Right");
    }

    public void OnRunRightButtonRelease() {
        _actions.RemoveDirection("Right");
    }

    public void OnRunDownButtonPress() {
        _actions.AddDirection("Down");
    }

    public void OnRunDownButtonRelease() {
        _actions.RemoveDirection("Down");
    }

    public void OnRunLeftButtonPress() {
        _actions.AddDirection("Left");
    }

    public void OnRunLeftButtonRelease() {
        _actions.RemoveDirection("Left");
    }

    public bool CheckIfSpecialGamepadBindingIsActive()
    {
        return (Settings.Instance.ControlScheme == "Gamepad") && (GamepadAbilitiesActive || GamepadItemsActive);
    }

    public void OnDodgeButtonPress() {
        if (CheckIfSpecialGamepadBindingIsActive()) {
            return;
        }
        if (_actions.CurrentAbilityBeingPerformed != null) {
            _actions.CurrentAbilityBeingPerformed.OnDodgeButtonPress();
        }
        else if(Ability.CheckIfCanPerformAbility(Player.Instance, typeof(Ability_Dodge))){
            _actions.CurrentAbilityBeingPerformed = Player.Instance.Actions.GetDodgeTypeThatShouldBeUsed();
        }
    }

    public void OnDodgeButtonRelease() {
        if (CheckIfSpecialGamepadBindingIsActive())
        {
            return;
        }
        if (_actions.CurrentAbilityBeingPerformed != null) {
            _actions.CurrentAbilityBeingPerformed.OnDodgeButtonRelease();
        }
    }

    public void OnBlockButtonPress() {
        if (CheckIfSpecialGamepadBindingIsActive())
        {
            return;
        }
        PlayerControls.BlockButtonHoldDuration = 0.001f;
        if (_actions.CurrentAbilityBeingPerformed != null && !(_actions.CurrentAbilityBeingPerformed is Ability_Block)) {
            _actions.CurrentAbilityBeingPerformed.OnBlockButtonPress();
        }
        else {
            Player.Instance.Actions.UseAbility(typeof(Ability_Block));
        }
    }

    public void OnBlockButtonRelease() {
        if (CheckIfSpecialGamepadBindingIsActive())
        {
            return;
        }
        PlayerControls.BlockButtonHoldDuration = 0;
        if (_actions.CurrentAbilityBeingPerformed != null) {
            _actions.CurrentAbilityBeingPerformed.OnBlockButtonRelease();
        }
    }

    public void OnBasicAttackButtonPress() {
        if (CheckIfSpecialGamepadBindingIsActive())
        {
            return;
        }
        PlayerControls.BasicAttackButtonPressCounter++;
        PlayerControls.BasicAttackButtonHoldDuration = 0.001f;
        if (_actions.CurrentAbilityBeingPerformed != null)
        {
            _actions.CurrentAbilityBeingPerformed.OnBasicAttackButtonPress();
        }
        else
        {
            _actions.PerformBasicAttack();
        }
    }

    public void OnBasicAttackButtonRelease() {
        if (CheckIfSpecialGamepadBindingIsActive())
        {
            return;
        }
        PlayerControls.BasicAttackButtonHoldDuration = 0;
        if (_actions.CurrentAbilityBeingPerformed != null)
        {
            _actions.CurrentAbilityBeingPerformed.OnBasicAttackButtonRelease();
        }
    }

    public void OnScoutButtonPress() {
        if (Settings.Instance.ControlScheme == "Gamepad" && GamepadItemsActive == false)
        {
            return;
        }
        CameraController.Instance.CurrentlyScouting = true;
    }

    public void OnScoutButtonRelease() {
        CameraController.Instance.CurrentlyScouting = false;
    }

    public void OnAbility1ButtonPress() {
        PressAbilityButton(0);
    }

    public void OnAbility1ButtonRelease() {
        ReleaseAbilityButton(0);
    }

    public void OnAbility2ButtonPress() {
        PressAbilityButton(1);
    }

    public void OnAbility2ButtonRelease() {
        ReleaseAbilityButton(1);
    }

    public void OnAbility3ButtonPress() {
        PressAbilityButton(2);
    }

    public void OnAbility3ButtonRelease() {
        ReleaseAbilityButton(2);
    }

    public void OnAbility4ButtonPress() {
        PressAbilityButton(3);
    }

    public void OnAbility4ButtonRelease() {
        ReleaseAbilityButton(3);
    }

    public void OnUseItemHealingButtonPress() {
        if (SaveFile.Instance.HealChargesRemaining <= 0 || Player.Instance.Health.Current >= Player.Instance.Health.Maximum || (Settings.Instance.ControlScheme == "Gamepad" && GamepadItemsActive == false))
        {
            return;
        }
        if (_actions.CurrentAbilityBeingPerformed != null && _actions.CurrentAbilityBeingPerformed is Ability_Heal) {
            if(((Ability_Heal)_actions.CurrentAbilityBeingPerformed).ListeningForSecondInput) {
                ((Ability_Heal)_actions.CurrentAbilityBeingPerformed).PressedHealButton = true;
            }
        }
        else {
            _actions.UseAbility(typeof(Ability_Heal));
        }
    }

    public void OnUseItem1ButtonPress()
    {
        if(SaveFile.Instance.EquippedItem1 != null && SaveFile.Instance.EquippedItem1 is not Tool_CrimsonFeather && SaveFile.Instance.EquippedItem1.Amount > 0 && (Settings.Instance.ControlScheme != "Gamepad" || GamepadItemsActive))
        {
            _actions.UseAbility(SaveFile.Instance.EquippedItem1.OnUseAbility, true, null, SaveFile.Instance.EquippedItem1);
        }
    }

    public void OnUseItem2ButtonPress()
    {
        if (SaveFile.Instance.EquippedItem2 != null && SaveFile.Instance.EquippedItem2 is not Tool_CrimsonFeather && SaveFile.Instance.EquippedItem2.Amount > 0 && (Settings.Instance.ControlScheme != "Gamepad" || GamepadItemsActive))
        {
            _actions.UseAbility(SaveFile.Instance.EquippedItem2.OnUseAbility, true, null, SaveFile.Instance.EquippedItem2);
        }
    }

    public void PressAbilityButton(int ability_index) {
        if (Settings.Instance.ControlScheme == "Gamepad" && GamepadAbilitiesActive == false)
        {
            return;
        }
        if (_actions.CurrentAbilityBeingPerformed != null && _actions.CurrentAbilityBeingPerformed.GetType() == _player.CurrentStance.Abilities[ability_index].Type) {
            _actions.CurrentAbilityBeingPerformed.OnAbilityButtonPress();
        }
        else if(_player.CurrentStance.Abilities[ability_index].Type != null){
            _actions.UseAbility(_player.CurrentStance.Abilities[ability_index].Type);
        }
    }

    public void ReleaseAbilityButton(int ability_index) {
        if (Settings.Instance.ControlScheme == "Gamepad" && GamepadAbilitiesActive == false)
        {
            return;
        }
        if (_actions.CurrentAbilityBeingPerformed != null && _actions.CurrentAbilityBeingPerformed.GetType() == _player.CurrentStance.Abilities[ability_index].Type) {
            _actions.CurrentAbilityBeingPerformed.OnAbilityButtonRelease();
        }
    }

    public void OnLeftStickButtonPress() {
        CurrentLeftStickPosition = new Vector2(Gamepad.current.leftStick.x.ReadValue(), Gamepad.current.leftStick.y.ReadValue());
    }

    public static void GamepadSelectObjectClosestToCenter() {
        if(Settings.Instance.ControlScheme != "Gamepad") {
            return;
        }
        float distanceFromCenter = 9999;
        Selectable closestSelectable = null;
        Transform parentToSearch = SceneManager.GetActiveScene().name  == "StartScreen" ? Utils.GetSceneRootObject("Start Screen") : SceneManager.GetActiveScene().name == "MissionSelect" ? Utils.GetSceneRootObject("Mission Select") : MenuManager.Instance.transform;
        foreach(Selectable selectable in parentToSearch.GetComponentsInChildren<Selectable>()) {
            float distance = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), selectable.transform.position);
            if(selectable.interactable && distance < distanceFromCenter) {
                distanceFromCenter = distance;
                closestSelectable = selectable;
            }
        }
		            
        if(closestSelectable != null) {
            closestSelectable.Select();
        }
    }

    public static List<LabelInitializer> AllLabelInitializers = new();

    public void OnDetailedDescriptionsButtonPress()
    {
        foreach (LabelInitializer labelInit in AllLabelInitializers)
        {
            if (labelInit.DisableDetailedDescription == false && labelInit.OriginalValue != null && labelInit.OriginalValue.Contains("Description") && Label.ContainsKey(Utils.ExtractLabelFromText(labelInit.OriginalValue).Replace("Description", "DescriptionDetailed")))
            {
                labelInit.RefreshLabel(true);
            }
        }
        if (MenuManager.Instance.SelectedSubMenu == 1 && MenuManager.Instance.CurrentDetailedItemDescription != null) {
            MenuManager.Objects.InventoryItemDescriptionLabel.SetLabel(MenuManager.Instance.CurrentDetailedItemDescription.Type == Constants.ItemType.Quest ? " " : MenuManager.Instance.CurrentDetailedItemDescription.GetDescription(true) + MenuManager.Instance.GetModifierDescriptions(MenuManager.Instance.CurrentDetailedItemDescription, true));
        }
        if (MenuManager.Instance.SelectedSubMenu == 2 && MenuManager.Instance.CurrentSkillTreeTileDescription?.GetComponent<PassivePowerUpTile>() == null) {
            MenuManager.Objects.SkillTreeDescription.SetActive(false);
            MenuManager.Objects.SkillTreeFlavorText.SetActive(false);
            MenuManager.Objects.SkillTreeLongDescription.SetActive(true);
            MenuManager.Objects.SkillTreeLongDescriptionLabel.string_params = MenuManager.Objects.SkillTreeDescriptionLabel.string_params;
            MenuManager.Objects.SkillTreeLongDescriptionLabel.OriginalValue = MenuManager.Objects.SkillTreeDescriptionLabel.OriginalValue;
            MenuManager.Objects.SkillTreeLongDescriptionLabel.RefreshLabel(true);
        }
    }

    public void OnDetailedDescriptionsButtonRelease()
    {
        foreach (LabelInitializer labelInit in AllLabelInitializers)
        {
            if (labelInit.DisableDetailedDescription == false && labelInit.OriginalValue != null && labelInit.OriginalValue.Contains("DescriptionDetailed") && Label.ContainsKey(Utils.ExtractLabelFromText(labelInit.OriginalValue).Replace("DescriptionDetailed", "Description")))
            {
                labelInit.RefreshLabel(false);
            }
        }
        if (MenuManager.Instance.SelectedSubMenu == 1 && MenuManager.Instance.CurrentDetailedItemDescription != null) {
            MenuManager.Instance.transform.Find("Inventory Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel(MenuManager.Instance.CurrentDetailedItemDescription.Type == Constants.ItemType.Quest ? " " : MenuManager.Instance.CurrentDetailedItemDescription.GetDescription(false) + MenuManager.Instance.GetModifierDescriptions(MenuManager.Instance.CurrentDetailedItemDescription, false));
        }
        if (MenuManager.Instance.SelectedSubMenu == 2 && MenuManager.Instance.CurrentSkillTreeTileDescription?.GetComponent<PassivePowerUpTile>() == null) {
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description").gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").gameObject.SetActive(false);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().RefreshLabel(false);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<LabelInitializer>().RefreshLabel(false);
        }
    }

    public void OnRotateStanceLeftButtonPress() {
        ChangeToSpecifiedDamageTypeStance(Player.Instance.CurrentStance.DamageType == DamageType.Heavy ? DamageType.Ranged : Player.Instance.CurrentStance.DamageType == DamageType.Light ? DamageType.Heavy : DamageType.Light);
    }

    public void OnRotateStanceRightButtonPress() {
        ChangeToSpecifiedDamageTypeStance(Player.Instance.CurrentStance.DamageType == DamageType.Heavy ? DamageType.Light : Player.Instance.CurrentStance.DamageType == DamageType.Light ? DamageType.Ranged : DamageType.Heavy);
    }

    public void OnEquipHeavyStanceButtonPress() {
        ChangeToSpecifiedDamageTypeStance(DamageType.Heavy);
    }

    public void OnEquipLightStanceButtonPress() {
        ChangeToSpecifiedDamageTypeStance(DamageType.Light);
    }

    public void OnEquipRangedStanceButtonPress() {
        ChangeToSpecifiedDamageTypeStance(DamageType.Ranged);
    }

    public void ChangeToSpecifiedDamageTypeStance(DamageType damage_type) {
        if(Player.Instance.CurrentStance.DamageType == damage_type) {
            return;
        }
        if(Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl && SaveFile.Instance.Stances[0].StanceEffect is Stance_OmniMastery && ((Stance_OmniMastery)SaveFile.Instance.Stances[0].StanceEffect).UnlockedUpgrade1 && Player.Instance.EffectCooldowns.FirstOrDefault(e => e.Type == typeof(Stance_OmniMastery) && e.Identifier == damage_type.ToString()) == null) {
            Player.Instance.Actions.CleanseAllHardCrowdControl();
            Player.Instance.Actions.UseAbility(GetStanceSwitch(damage_type));
            Player.Instance.AddCooldown(new Cooldown(typeof(Stance_OmniMastery), Stance_OmniMastery.CleanseCooldownDuration, Player.Instance, damage_type.ToString()));
            Player.Instance.AddCooldown(new Cooldown(typeof(Ability_StanceSwitch), Constants.STANCE_SWITCH_COOLDOWN_OMNIMASTERY, Player.Instance));
        }
        else if (!Player.Instance.CheckIfAbilityOnCooldown(typeof(Ability_StanceSwitch)) && Ability.CheckIfCanPerformAbility(Player.Instance, GetStanceSwitch(damage_type)))
        {
            Player.Instance.Actions.UseAbility(GetStanceSwitch(damage_type));
            bool shorterCd = Player.Instance._currentStance.StanceEffect is Stance_OmniMastery || Player.Instance.GetStanceOnTheRight().StanceEffect is Stance_OmniMastery;
            Player.Instance.AddCooldown(new Cooldown(typeof(Ability_StanceSwitch), shorterCd ? Constants.STANCE_SWITCH_COOLDOWN_OMNIMASTERY : Constants.STANCE_SWITCH_COOLDOWN, Player.Instance));
        }
    }

    public Type GetStanceSwitch(DamageType switch_to) {
        if((Player.Instance.CurrentStance.DamageType == DamageType.Heavy && switch_to == DamageType.Light) || (Player.Instance.CurrentStance.DamageType == DamageType.Light && switch_to == DamageType.Ranged) || (Player.Instance.CurrentStance.DamageType == DamageType.Ranged && switch_to == DamageType.Heavy)) {
            return typeof(Ability_StanceSwitchRight);
        }
        if((Player.Instance.CurrentStance.DamageType == DamageType.Heavy && switch_to == DamageType.Ranged) || (Player.Instance.CurrentStance.DamageType == DamageType.Light && switch_to == DamageType.Heavy) || (Player.Instance.CurrentStance.DamageType == DamageType.Ranged && switch_to == DamageType.Light)) {
            return typeof(Ability_StanceSwitchLeft);
        }
        return null;
    }


    public void OnOpenDebugConsoleButtonPress()
    {
        if(Settings.Instance.ConsoleEnabled) {
            DebugController.ConsoleOpen = true;
            DebugController.TimeScaleBeforeDebug = Time.timeScale;
            Time.timeScale = 0;
            GameController.Instance.PlayerInput.SwitchCurrentActionMap("Debug Console");
            UIManager.Objects.DebugConsole.gameObject.SetActive(true);
            UIManager.Objects.DebugConsole.GetComponent<TMP_InputField>().ActivateInputField();
        }
    }

    public void LoadNextLevel()
    {
        SurvivalController.LoadNextLevel();
    }

    public void OnSubmitButtonPress()
    {
        EventManager.SubmitButtonPressed.Invoke();
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>() != null)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>().OnSubmit(null);
        }
    }

    public void OnCancelButtonPress()
    {
        EventManager.CancelButtonPressed.Invoke();
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>() != null)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>().OnCancel(null);
        }

    }

    public void OnPrepareUltimateButtonPress() {
        if(Player.Instance.Energy.Current >= Constants.ENERGY_REQUIRED_TO_USE_ULTIMATE && SaveFile.Instance.UltimatesUsedInCurrentCombat < SaveFile.Instance.MaxUltimateUsesPerCombat) {
            PrepareUltimate();
        }
        else 
        {
            if(Player.Instance.Energy.Current < Constants.ENERGY_REQUIRED_TO_USE_ULTIMATE) {
                foreach(Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
                    Image display = ability.AbilityGraphic.transform.Find("Cost").GetComponent<Image>();
                    if (display.color == Color.red) {
                        Tuple<Image, int> existingWarning = UIManager.Instance.NotEnoughEnergyWarnings.FirstOrDefault(warning => warning.Item1 == display);
                        if (existingWarning != null) {
                            UIManager.Instance.NotEnoughEnergyWarnings.Remove(existingWarning);
                        }
                    }
                    UIManager.Instance.NotEnoughEnergyWarnings.Add(new Tuple<Image, int>(display, 75));
                    display.color = Color.red;
                }
            }
            if(SaveFile.Instance.UltimatesUsedInCurrentCombat >= SaveFile.Instance.MaxUltimateUsesPerCombat) {
                foreach(Transform child in UIManager.Objects.UltimateUses.transform) {
                    child.GetComponent<Image>().color = Color.red;
                } 
                UIManager.Instance.NotEnoughUltimateUsesWarningCounter = 75;
            }
        }
    }

    public void OnPrepareUltimateButtonRelease() {
        StopPreparingUltimate();
    }

    public void PrepareUltimate() {
        if(Utils.CheckIfUnitCanPerformActions(Player.Instance) == false) {
            return;
        }
        Player.Instance.PreparingForUltimate = true;
        Player.Instance.PlayAnimation("PreparingForUltimate_" + ((Player.Instance.CurrentStance.StanceEffect is Stance_MindOverMatter || Player.Instance.CurrentStance.StanceEffect is Stance_PowerWithoutLimit) ? "Magic" : Player.Instance.CurrentWeaponDamageType));
        foreach(Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            if(ability.Type != null) {
                ability.Icon.sprite = Resources.Load("Sprites/Ability/" + ability.Type.ToString().Replace("Ability_", "") + "_Ultimate", typeof(Sprite)) as Sprite;
                ability.CostLabel.text = "<sprite name=\"" + Ability.GetFamily(ability.Type).ToString() + "\"/>";
                ability.CooldownDisplay.fillAmount = 0;
            }
        }
        foreach(Transform child in UIManager.Objects.UltimateUses.transform) {
            Image img = child.GetComponent<Image>();
            if(img.sprite.name.Contains("CanBeUsed")) {
                child.GetComponent<Image>().color = Colors.Gold;
            }
        } 
        UIManager.Instance.NotEnoughUltimateUsesWarningCounter = 75;
    }

    public void StopPreparingUltimate() {
        Player.Instance.PreparingForUltimate = false;
        if(Player.Instance.Actions.CurrentActionBeingPerformed == ActionType.Idle) {
            Player.Instance.PlayAnimation(Player.Instance.InCombat ? "IdleInCombat" : "Idle");
        }
        foreach(Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            ability.RefreshDisplayForEquippedAbility();
        }
        foreach(Transform child in UIManager.Objects.UltimateUses.transform) {
            child.GetComponent<Image>().color = Color.white;
        } 
    }

    public void OnCheatModeButtonPress() {
        DebugController.Instance.ToggleCheatMode(1);
    }

    public void OnCheatModeButtonRelease() {
        DebugController.Instance.ToggleCheatMode(0);
    }
    public void OnSpecialAction1ButtonPress() {
    }

    public void OnSpecialAction2ButtonPress() {
        Settings.Instance.Keybinds.Add(new Settings.Keybind() {ActionName = UnityEngine.Random.Range(1, 1000).ToString(), KeyboardBinding1 = UnityEngine.Random.Range(1, 1000).ToString()});
    }

    public void OnSpecialAction3ButtonPress() {

    }

    public void OnQuickSaveButtonPress() {
        SaveFile.Instance.Save("QuickSave.es3");
        ScreenCapture.CaptureScreenshot("3LC Screenshot " + DateTime.Now.ToString("yyyy-MM-dd\\THH-mm-ss\\Z") + ".png");
    }

    public void OnQuickLoadButtonPress() {
        GameController.Instance.SaveOrLoadGame(-3);
        ScreenCapture.CaptureScreenshot("3LC Screenshot " + DateTime.Now.ToString("yyyy-MM-dd\\THH-mm-ss\\Z") + ".png");
    }

    public void OnTakeScreenshotButtonPress() {
        ScreenCapture.CaptureScreenshot("3LC Screenshot " + DateTime.Now.ToString("yyyy-MM-dd\\THH-mm-ss\\Z") + ".png");
    }

    public void OnInputDebugConsoleCommandButtonPress()
    {
        UIManager.Objects.DebugConsole.GetComponent<DebugController>().InputDebugConsoleCommand();
    }

    public void OnCloseDebugConsoleButtonPress()
    {
        DebugController.CloseDebugConsole();
    }

    public void OnToggleLogsButtonPress()
    {
        Debug.Log("(" + Utils.GetTimeStamp() + ") Turning Audit Logs " + (Settings.Instance.CreateAuditLogs ? "OFF" : "ON"));
        Settings.Instance.CreateAuditLogs = !Settings.Instance.CreateAuditLogs;
        Settings.Instance.Save();
    }

    public void OnToggleOnScreenLogsButtonPress() {
        ConsoleToGUI.ShowOnScreenLogs = !ConsoleToGUI.ShowOnScreenLogs;
    }

    public void OnGoToNextDialogueLineButtonPress() {
        if(UIManager.Instance.CurrentDialogueLine != null && UIManager.Instance.DialogueHistoryIsOpen == false) {
            if(UIManager.Instance.CurrentDialogueLine.Choices.Count > 0) {
                UIManager.Instance.ActivateCurrentlySelectedChoice();
            } else {
                UIManager.Instance.ProgressToNextDialogueLine();
            }
        }
        else {
            UIManager.Instance.CloseDialogueHistory();
        }
    }

    public void OnToggleDialogueHistoryButtonPress() {
        if(UIManager.Instance.DialogueHistoryIsOpen == false) {
            UIManager.Instance.OpenDialogueHistory();
        }
        else {
            UIManager.Instance.CloseDialogueHistory();
        }
    }

    public void OnSkipDialogueButtonPress() {
        UIManager.Instance.SkippingDialogue = true;
        UIManager.Instance.SkipToNextDialogueLine();
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (UIManager.Instance.SkippingDialogue ? "ButtonStopSkip" : "ButtonSkip"), typeof(Sprite)) as Sprite;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Label").GetComponent<LabelInitializer>().SetLabel(UIManager.Instance.SkippingDialogue ? "{DialogueButton_StopSkip}" : "{DialogueButton_Skip}");
    }

    public void OnSkipDialogueButtonRelease() {
        UIManager.Instance.SkippingDialogue = false;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (UIManager.Instance.SkippingDialogue ? "ButtonStopSkip" : "ButtonSkip"), typeof(Sprite)) as Sprite;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Label").GetComponent<LabelInitializer>().SetLabel(UIManager.Instance.SkippingDialogue ? "{DialogueButton_StopSkip}" : "{DialogueButton_Skip}");
    }

    public void OnInteractButtonPress() {
        if((Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.Idle || Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.Moving) && Player.Instance.ClosestInteractable != null) {
            Player.Instance.ClosestInteractable.ActivateInteractable();
        }
    }

    public void OnToggleTargetButtonPress() {
        Player.Instance.PotentialTargets = Player.Instance.PotentialTargets.Where(unit => unit != null).ToList();
        if (_player.PotentialTargets.Count == 1 && _player.CurrentTarget == _player.PotentialTargets[0]) {
            _player.CurrentTarget = null;
        }
        else if (_player.PotentialTargets.Count == 1 || (_player.CurrentTarget == null && _player.PotentialTargets.Count >= 1)) {
            _player.CurrentTarget = _player.PotentialTargets[0];
        }
        else if (_player.PotentialTargets.Count > 1) {
            int indexOfCurrentTarget = _player.PotentialTargets.IndexOf(_player.CurrentTarget);
            if (indexOfCurrentTarget == _player.PotentialTargets.Count - 1) {
                _player.CurrentTarget = _player.PotentialTargets[0];
            }
            else {
                _player.CurrentTarget = _player.PotentialTargets[indexOfCurrentTarget + 1];
            }
        }
    }

    public void OnStopTargettingButtonPress() {
        if(_player.CurrentTarget != null)
        {
            _player.CurrentTarget = null;
        }
    }

    public void OnChangeSub2MenuLeftButtonPress()
    {
        if(MenuManager.Instance.SelectedSubMenu == 1) {
            if (MenuManager.Instance.SelectedSortingIndex == 0)
            {
                MenuManager.Instance.ChangeSelectedInventoryCategory(10);
            }
            else
            {
                MenuManager.Instance.ChangeSelectedInventoryCategory(MenuManager.Instance.SelectedSortingIndex - 1);
            }
        }
        else if(MenuManager.Instance.SelectedSubMenu == 2) {
            if (MenuManager.Instance.SelectedSkillTree == 0)
            {
                MenuManager.Instance.ChangeDisplayedSkillTree(6);
            }
            else
            {
                MenuManager.Instance.ChangeDisplayedSkillTree(MenuManager.Instance.SelectedSkillTree - 1);
            }
        }
    }

    public void OnChangeSub2MenuRightButtonPress()
    {
        if(MenuManager.Instance.SelectedSubMenu == 1) {
            if (MenuManager.Instance.SelectedSortingIndex == 10)
            {
                MenuManager.Instance.ChangeSelectedInventoryCategory(0);
            }
            else
            {
                MenuManager.Instance.ChangeSelectedInventoryCategory(MenuManager.Instance.SelectedSortingIndex + 1);
            }
        }
        else if(MenuManager.Instance.SelectedSubMenu == 2) {
            if (MenuManager.Instance.SelectedSkillTree == 6)
            {
                MenuManager.Instance.ChangeDisplayedSkillTree(0);
            }
            else
            {
                MenuManager.Instance.ChangeDisplayedSkillTree(MenuManager.Instance.SelectedSkillTree + 1);
            }
        }
    }

    public void SaveDefaultKeybindsToSettings() {
        Transform keyBinds = MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/Scroll Rect/Items");
        foreach(InputAction input in GameController.Instance.PlayerInput.actions.FindActionMap("Regular").actions) {
            Transform keyBind = keyBinds.transform.Find(input.name);
            if(keyBind != null) {
                Settings.Keybind newKeybind = new Settings.Keybind() {ActionName = input.name};
                bool hadKeyboard1 = false, hadGamepad1 = false;
                foreach(InputBinding binding in input.bindings) {
                    if(binding.effectivePath.Contains("<Keyboard>/") || binding.effectivePath.Contains("<Mouse>/")) {
                        if(hadKeyboard1) {
                            newKeybind.KeyboardBinding2 = GetFormattedInputPath(binding.effectivePath);
                        }
                        else {
                            newKeybind.KeyboardBinding1 = GetFormattedInputPath(binding.effectivePath);
                        }
                        hadKeyboard1 = true;
                    }
                    else if(binding.effectivePath.Contains("<Gamepad>/") ) {
                        if(hadGamepad1) {
                            newKeybind.GamepadBinding2 = GetFormattedInputPath(binding.effectivePath);
                        }
                        else {
                            newKeybind.GamepadBinding1 = GetFormattedInputPath(binding.effectivePath);
                        }
                        hadGamepad1 = true;
                    }
                }
                Settings.Instance.Keybinds.Add(newKeybind);
            }
        }
        Settings.Instance.Save();
    }

    public void WaitForChangedKeybindInput(GameObject game_object) {
        InputAction action = GameController.Instance.PlayerInput.actions.FindActionMap("Regular").FindAction(game_object.transform.parent.gameObject.name);
        InputBinding inputBinding = action.bindings[game_object.name == "KeyboardBinding1" ? 0 : game_object.name == "KeyboardBinding2" ? 1 : game_object.name == "GamepadBinding1" ? 2 : 3];
        if(action.GetBindingIndex(inputBinding) == -1) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("UnexpectedKeybindError"), new List<string> {action.name, action.bindings.Count.ToString(), inputBinding.path}));
            return;
        }
        InputActionRebindingExtensions.RebindingOperation rebind = action.PerformInteractiveRebinding(action.GetBindingIndex(inputBinding));
        MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/InputWaitingRoom").gameObject.SetActive(true);
        rebind.OnPotentialMatch(
            operation =>
            {
                if (game_object.name.Contains("Keyboard") && operation.selectedControl.path.Contains("Gamepad")) {
                    NotificationController.ShowTextNotification(Label.Get("WrongGamepadKeybindInput"));
                    MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/InputWaitingRoom").gameObject.SetActive(false);
                    operation.Cancel();
                    return;
                }
                else if(game_object.name.Contains("Gamepad") && (operation.selectedControl.path.Contains("Keyboard") || operation.selectedControl.path.Contains("Mouse"))) {
                    NotificationController.ShowTextNotification(Label.Get("WrongKeyboardKeybindInput"));
                    MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/InputWaitingRoom").gameObject.SetActive(false);
                    operation.Cancel();
                    return;
                }
                string curatedPath = GetFormattedInputPath(operation.selectedControl.path);
                if (game_object.name.Contains("Keyboard") && !PlayerControls.MouseViableInputs.Contains(curatedPath) && !PlayerControls.KeyboardViableInputs.Contains(curatedPath))
                {
                    NotificationController.ShowTextNotification(Label.Get("IncorrectKeybindInput"));
                    MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/InputWaitingRoom").gameObject.SetActive(false);
                    operation.Cancel();
                    return;
                }
                else if (game_object.name.Contains("Gamepad") && !PlayerControls.GamepadViableInputs.Contains(curatedPath))
                {
                    NotificationController.ShowTextNotification(Label.Get("IncorrectKeybindInput"));
                    MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/InputWaitingRoom").gameObject.SetActive(false);
                    operation.Cancel();
                    return;
                }
            }
        );
        rebind.OnComplete(
            operation =>
            {
                Settings.Keybind keybind = Settings.Instance.Keybinds.FirstOrDefault(keybind => keybind.ActionName == action.name);
                string binding = GetFormattedInputPath(operation.selectedControl.path);
                if(game_object.name == "KeyboardBinding1") {
                    keybind.KeyboardBinding1 = binding;
                }
                else if(game_object.name == "KeyboardBinding2") {
                    keybind.KeyboardBinding2 = binding;
                }
                else if(game_object.name == "GamepadBinding1") {
                    keybind.GamepadBinding1 = binding;
                }
                else if(game_object.name == "GamepadBinding2") {
                    keybind.GamepadBinding2 = binding;
                }
                ReloadKeybindsOnAction(Settings.Instance.Keybinds.FirstOrDefault(keybind => keybind.ActionName == action.name), action);
                bool isKeyboard = game_object.name.Contains("Keyboard");
                foreach(Settings.Keybind checkedKeybind in Settings.Instance.Keybinds) {
                    bool changed = false;
                    if(checkedKeybind != keybind && isKeyboard && checkedKeybind.KeyboardBinding1 == binding) {
                        checkedKeybind.KeyboardBinding1 = "";
                        changed = true;
                    }
                    if(checkedKeybind != keybind && isKeyboard && checkedKeybind.KeyboardBinding2 == binding) {
                        checkedKeybind.KeyboardBinding2 = "";
                        changed = true;
                    }
                    if(changed) {
                        ReloadKeybindsOnAction(checkedKeybind, GameController.Instance.PlayerInput.actions.FindActionMap("Regular").FindAction(checkedKeybind.ActionName));
                    }
                }
                Settings.Instance.Save();
                MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/InputWaitingRoom").gameObject.SetActive(false);
                operation.Dispose();
            }
        );
        rebind.Start();
        GameController.Instance.WaitAndRunMethodRealtime(5, CheckIfShouldEndRebindingOperation, new object[] {rebind});
    }

    public void CheckIfShouldEndRebindingOperation(object[] objs) {
        InputActionRebindingExtensions.RebindingOperation rebind = (InputActionRebindingExtensions.RebindingOperation)objs[0];
        if(rebind.completed == false) {
            MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/InputWaitingRoom").gameObject.SetActive(false);
            NotificationController.ShowTextNotification(Label.Get("WaitedTooLongForKeybind"));
            rebind.Cancel();
        }
    }

    public void LoadSettingsKeybinds() {
        LoadKeybinds(Settings.Instance.Keybinds, false);
    }

    public void LoadDefaultKeybinds() {
        LoadKeybinds(GetKeybindsFromFile(Application.persistentDataPath + "/DefaultKeybinds.json"));
    }

    public void LoadAlternateKeyboardKeybinds() {
        LoadKeybinds(GetKeybindsFromFile(Application.persistentDataPath + "/AlternateKeyboardKeybinds.json"));
    }

    public void LoadAlternateGamepadKeybinds() {
        LoadKeybinds(GetKeybindsFromFile(Application.persistentDataPath + "/AlternateGamepadKeybinds.json"));
    }

    public string GetFormattedInputPath(string unformatted_input_path) {
        string[] splitUpPath = unformatted_input_path.Split("/");
        bool getTwoLastParts = unformatted_input_path.EndsWith("/up") || unformatted_input_path.EndsWith("/right") || unformatted_input_path.EndsWith("/down") || unformatted_input_path.EndsWith("/left");
        return getTwoLastParts ? splitUpPath[splitUpPath.Length - 2] + splitUpPath[splitUpPath.Length - 1] : splitUpPath[splitUpPath.Length - 1];
    }

    public void LoadKeybinds(List<Settings.Keybind> keybinds, bool change_settings_keybinds = true) {
        List<Settings.Keybind> settings_keybinds = Settings.Instance.Keybinds;
        InputAction stopTargetting = GameController.Instance.PlayerInput.actions.FindActionMap("Regular").FindAction("StopTargettingButtonPress");
        foreach(Settings.Keybind keybind in keybinds) {
            if(GameController.Instance.PlayerInput.actions.FindActionMap("Regular").FindAction(keybind.ActionName) != null) {
                if(change_settings_keybinds) {
                    Settings.Keybind k = settings_keybinds.FirstOrDefault(k => k.ActionName == keybind.ActionName);
                    k.KeyboardBinding1 = keybind.KeyboardBinding1;
                    k.KeyboardBinding2 = keybind.KeyboardBinding2;
                    k.GamepadBinding1 = keybind.GamepadBinding1;
                    k.GamepadBinding2 = keybind.GamepadBinding2;
                }
                InputAction actionButtonPress = GameController.Instance.PlayerInput.actions.FindActionMap("Regular").FindAction(keybind.ActionName);
                if(actionButtonPress != null) {
                    ReloadKeybindsOnAction(keybind, actionButtonPress);
                }
                InputAction actionButtonRelease = GameController.Instance.PlayerInput.actions.FindActionMap("Regular").FindAction(keybind.ActionName.Replace("ButtonPress", "ButtonRelease"));
                if(actionButtonRelease != null) {
                    ReloadKeybindsOnAction(keybind, actionButtonRelease);
                }
                if(keybind.ActionName == "ToggleTargetButtonPress") {
                    ReloadKeybindsOnAction(keybind, stopTargetting);
                }
                if(keybind.ActionName == "ToggleMenuButtonPress") {
                    SetMenuKeybind(keybind);
                }
                if(keybind.ActionName == "PauseScreenButtonPress") {
                    SetPauseKeybind(keybind);
                }
            }
        }
    }

    public void SetMenuKeybind(Settings.Keybind keybind) {
        InputAction menuButtonPressInMenu = GameController.Instance.PlayerInput.actions.FindActionMap("Menu").FindAction(keybind.ActionName);
        if(menuButtonPressInMenu != null) {
            ReloadKeybindsOnAction(keybind, menuButtonPressInMenu);
        }
        InputAction menuButtonPressInMissionSelect = GameController.Instance.PlayerInput.actions.FindActionMap("Mission Select").FindAction(keybind.ActionName);
        if(menuButtonPressInMissionSelect != null) {
            ReloadKeybindsOnAction(keybind, menuButtonPressInMissionSelect);
        }
    }

    public void SetPauseKeybind(Settings.Keybind keybind) {
        InputAction pauseButtonPressInMenu = GameController.Instance.PlayerInput.actions.FindActionMap("Menu").FindAction(keybind.ActionName);
        if(pauseButtonPressInMenu != null) {
            ReloadKeybindsOnAction(keybind, pauseButtonPressInMenu);
        }
        InputAction pauseButtonPressInMissionSelect = GameController.Instance.PlayerInput.actions.FindActionMap("Mission Select").FindAction(keybind.ActionName);
        if(pauseButtonPressInMissionSelect != null) {
            ReloadKeybindsOnAction(keybind, pauseButtonPressInMissionSelect);
        }
    }

    public void ReloadKeybindsOnAction(Settings.Keybind keybind, InputAction action) {
        Transform keyBindInUI = MenuManager.Instance.transform.Find("Options Window/Options/Keybinds/Scroll Rect/Items/" + keybind.ActionName);
        int bindCount = action.bindings.Count;
        for(int i = 0; i < bindCount; i++) {
            action.ChangeBinding(0).Erase();
        }
        InputBinding keyboard1 = new InputBinding() {path = (PlayerControls.MouseViableInputs.Contains(keybind.KeyboardBinding1) ? "<Mouse>/" : "<Keyboard>/") + keybind.KeyboardBinding1, groups = "Mouse and Keyboard"};
        action.AddBinding(keyboard1);
        InputBinding keyboard2 = new InputBinding() {path = (PlayerControls.MouseViableInputs.Contains(keybind.KeyboardBinding2) ? "<Mouse>/" : "<Keyboard>/") + keybind.KeyboardBinding2, groups = "Mouse and Keyboard"};
        action.AddBinding(keyboard2);
        InputBinding gamepad1 = new InputBinding() {path = "<Gamepad>/" + keybind.GamepadBinding1, groups = "Gamepad"};
        action.AddBinding(gamepad1);
        InputBinding gamepad2 = new InputBinding() {path = "<Gamepad>/" + keybind.GamepadBinding2, groups = "Gamepad"};
        action.AddBinding(gamepad2);
        UpdateKeybindImageInUI(keybind.KeyboardBinding1, "KeyboardBinding1", keyBindInUI, keybind.ActionName);
        UpdateKeybindImageInUI(keybind.KeyboardBinding2, "KeyboardBinding2", keyBindInUI, keybind.ActionName);
        UpdateKeybindImageInUI(keybind.GamepadBinding1, "GamepadBinding1", keyBindInUI, keybind.ActionName);
        UpdateKeybindImageInUI(keybind.GamepadBinding2, "GamepadBinding2", keyBindInUI, keybind.ActionName);
    }

    public void UpdateKeybindImageInUI(string keybind_path, string keybind_type, Transform key_bind_in_ui, string action_name) {
        if(key_bind_in_ui == null || (keybind_type.Contains("Keyboard") && key_bind_in_ui.gameObject.name.StartsWith("Gamepad"))) {
            return;
        }
        key_bind_in_ui.transform.Find(keybind_type + "/Binding").GetComponent<Image>().enabled = keybind_path != "";
        key_bind_in_ui.transform.Find(keybind_type + "/Binding").GetComponent<Image>().sprite = Utils.LoadSpriteFromMultiple(keybind_type.Contains("Keyboard") ? "Keyboard Bindings" : $"Gamepad Bindings ({Settings.Instance.GamepadType})", GetFormattedInputPath(keybind_path));
        if(action_name == "Ability1ButtonPress") {
            UIManager.Objects.KeyboardAbility1UIText.SetLabel("[Ability1ButtonPress]");
            UIManager.Objects.GamepadAbility1UIText.SetLabel("[Ability1ButtonPress]");
        }
        else if(action_name == "Ability2ButtonPress") {
            UIManager.Objects.KeyboardAbility2UIText.SetLabel("[Ability2ButtonPress]");
            UIManager.Objects.GamepadAbility2UIText.SetLabel("[Ability2ButtonPress]");
        }
        else if(action_name == "Ability3ButtonPress") {
            UIManager.Objects.KeyboardAbility3UIText.SetLabel("[Ability3ButtonPress]");
            UIManager.Objects.GamepadAbility3UIText.SetLabel("[Ability3ButtonPress]");
        }
        else if(action_name == "Ability4ButtonPress") {
            UIManager.Objects.KeyboardAbility4UIText.SetLabel("[Ability4ButtonPress]");
            UIManager.Objects.GamepadAbility4UIText.SetLabel("[Ability4ButtonPress]");
        }
        else if(action_name == "UseItem1ButtonPress") {
            UIManager.Objects.KeyboardItem1UIText.SetLabel("[UseItem1ButtonPress]");
            UIManager.Objects.GamepadItem1UIText.SetLabel("[UseItem1ButtonPress]");
        }
        else if(action_name == "UseItem2ButtonPress") {
            UIManager.Objects.KeyboardItem2UIText.SetLabel("[UseItem2ButtonPress]");
            UIManager.Objects.GamepadItem2UIText.SetLabel("[UseItem2ButtonPress]");
        }
        else if(action_name == "UseItemHealingButtonPress") {
            UIManager.Objects.KeyboardItemHealingUIText.SetLabel("[UseItemHealingButtonPress]");
            UIManager.Objects.GamepadItemHealingUIText.SetLabel("[UseItemHealingButtonPress]");
        }
    }

    public List<Settings.Keybind> GetKeybindsFromFile(string file_name) {
        if (File.Exists(file_name))
        {
            string fileContents = File.ReadAllText(file_name);
            Settings settings = JsonUtility.FromJson<Settings>(fileContents);
            return settings.Keybinds;
        }
        else
        {
            Utils.CreateAuditLog("Could not find Keybinds file to load: " + file_name);
            return null;
        }
    }
}