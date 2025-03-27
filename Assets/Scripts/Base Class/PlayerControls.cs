using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using static Constants;
using Random = UnityEngine.Random;

public class PlayerControls : WorldObject {

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

    public bool HoldingBlockButton;


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
            CanvasElements.UICanvas.AbilitiesGamepad.transform.localScale = new Vector3(_gamepadAbilitiesActive ? 1.1f : 1, _gamepadAbilitiesActive ? 1.1f : 1, 1);
            CanvasElements.UICanvas.AbilitiesGamepad.GetComponent<CanvasGroup>().alpha = _gamepadAbilitiesActive ? 1 : 0.5f;
            CanvasElements.UICanvas.ItemsGamepad.transform.parent.Find("Binding Abilities").transform.localScale = new Vector3(_gamepadAbilitiesActive ? 1.4f : 1, _gamepadAbilitiesActive ? 1.4f : 1, 1);
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
            CanvasElements.UICanvas.ItemsGamepad.transform.localScale = new Vector3(_gamepadItemsActive ? 1.1f : 1, _gamepadItemsActive ? 1.1f : 1, 1);
            CanvasElements.UICanvas.ItemsGamepad.GetComponent<CanvasGroup>().alpha = _gamepadItemsActive ? 1 : 0.5f;
            CanvasElements.UICanvas.ItemsGamepad.transform.parent.Find("Binding Items").transform.localScale = new Vector3(_gamepadItemsActive ? 1.4f : 1, _gamepadItemsActive ? 1.4f : 1, 1);
        }
    }

    public void OnGamepadAbilitiesActivated()
    {
        GamepadAbilitiesActive = true;
        if(GamepadItemsActive) {
            OnPrepareForUltimateButtonPress();
        }
    }

    public void OnGamepadAbilitiesDeactivated()
    {
        GamepadAbilitiesActive = false;
        if(GamepadItemsActive) {
            OnPrepareForUltimateButtonRelease();
        }
    }

    public void OnGamepadItemsActivated()
    {
        GamepadItemsActive = true;
        if(GamepadAbilitiesActive) {
            OnPrepareForUltimateButtonPress();
        }
    }

    public void OnGamepadItemsDeactivated()
    {
        GamepadItemsActive = false;
        if(GamepadAbilitiesActive) {
            OnPrepareForUltimateButtonRelease();
        }
    }

    public bool SpammingButton = false;
    public bool IsOn = false;

    private void Update() {
        if(GameController.Instance?.Camera != null && Mouse.current?.position?.ReadValue() != null)
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

    public void OnToggleMenu() {
        if(GameController.Instance.transform.Find("Save or Load").gameObject.activeSelf) {
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
        if (GameController.Instance.GameplayMode == Constants.GameplayMode.Regular || GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect) {
            GameController.Instance.GameplayMode = Constants.GameplayMode.InMenu;
        }
        else if (GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu) {
            GameController.Instance.GameplayMode = (SaveFile.Instance.CurrentMission == null && SaveFile.Instance.GameType == GameType.Story) ? Constants.GameplayMode.MissionSelect : Constants.GameplayMode.Regular;
            
        }
    }

    public void OnInvokeButton() {
    }

    public void OnSelectDefaultObject()
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

    public void OnChangeSubMenuLeft()
    {
        MenuManager.Instance.SelectedSubMenu--;
    }

    public void OnChangeSubMenuRight()
    {
        MenuManager.Instance.SelectedSubMenu++;
    }

    public void OnStartRunUp() {
        _actions.AddDirection("Up");
    }

    public void OnStopRunUp() {
        _actions.RemoveDirection("Up");
    }

    public void OnStartRunRight() {
        _actions.AddDirection("Right");
    }

    public void OnStopRunRight() {
        _actions.RemoveDirection("Right");
    }

    public void OnStartRunDown() {
        _actions.AddDirection("Down");
    }

    public void OnStopRunDown() {
        _actions.RemoveDirection("Down");
    }

    public void OnStartRunLeft() {
        _actions.AddDirection("Left");
    }

    public void OnStopRunLeft() {
        _actions.RemoveDirection("Left");
    }

    public bool CheckIfSpecialGamepadBindingIsActive()
    {
        return ((Settings.Instance.ControlScheme == "Gamepad") && GamepadAbilitiesActive || GamepadItemsActive);
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
        HoldingBlockButton = true;
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
        HoldingBlockButton = false;
        if (_actions.CurrentAbilityBeingPerformed != null) {
            _actions.CurrentAbilityBeingPerformed.OnBlockButtonRelease();
        }
    }

    public void OnBasicAttackButtonPress() {
        if (CheckIfSpecialGamepadBindingIsActive())
        {
            return;
        }
        if (_actions.CurrentAbilityBeingPerformed != null) {
            _actions.CurrentAbilityBeingPerformed.OnMainButtonPress();
        }
        else {
            _actions.PerformBasicAttack();
        }
    }

    public void OnBasicAttackButtonRelease() {
        if (CheckIfSpecialGamepadBindingIsActive())
        {
            return;
        }
        if (_actions.CurrentAbilityBeingPerformed != null) {
            _actions.CurrentAbilityBeingPerformed.OnMainButtonRelease();
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

    public void OnHealButtonPress() {
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

    public void OnUseItem1()
    {
        if(SaveFile.Instance.EquippedItem1 != null && SaveFile.Instance.EquippedItem1 is not Tool_CrimsonFeather && SaveFile.Instance.EquippedItem1.Amount > 0 && (Settings.Instance.ControlScheme != "Gamepad" || GamepadItemsActive))
        {
            _actions.UseAbility(SaveFile.Instance.EquippedItem1.OnUseAbility, true, null, SaveFile.Instance.EquippedItem1);
        }
    }

    public void OnUseItem2()
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

    public void OnLeftStick() {
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

    public void OnShowDetailedDescriptions()
    {
        foreach (LabelInitializer labelInit in AllLabelInitializers)
        {
            if (labelInit.DisableDetailedDescription == false && labelInit.OriginalValue != null && labelInit.OriginalValue.Contains("Description") && Label.ContainsKey(Utils.ExtractLabelFromText(labelInit.OriginalValue).Replace("Description", "DescriptionDetailed")))
            {
                labelInit.RefreshLabel(true);
            }
        }
        if (MenuManager.Instance.SelectedSubMenu == 1 && MenuManager.Instance.CurrentDetailedItemDescription != null) {
            MenuManager.Instance.transform.Find("Inventory Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel(MenuManager.Instance.CurrentDetailedItemDescription.Category == Constants.ItemCategory.Quest ? " " : MenuManager.Instance.CurrentDetailedItemDescription.GetDescription(true) + MenuManager.Instance.GetModifierDescriptions(MenuManager.Instance.CurrentDetailedItemDescription, true));
        }
        if (MenuManager.Instance.SelectedSubMenu == 2 && MenuManager.Instance.CurrentSkillTreeTileDescription?.GetComponent<PassivePowerUpTile>() == null) {
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description").gameObject.SetActive(false);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").gameObject.SetActive(false);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").GetComponent<LabelInitializer>().string_params = MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().string_params;
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").GetComponent<LabelInitializer>().OriginalValue = MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().OriginalValue;
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").GetComponent<LabelInitializer>().RefreshLabel(true);
        }
    }

    public void OnHideDetailedDescriptions()
    {
        foreach (LabelInitializer labelInit in AllLabelInitializers)
        {
            if (labelInit.DisableDetailedDescription == false && labelInit.OriginalValue != null && labelInit.OriginalValue.Contains("DescriptionDetailed") && Label.ContainsKey(Utils.ExtractLabelFromText(labelInit.OriginalValue).Replace("DescriptionDetailed", "Description")))
            {
                labelInit.RefreshLabel(false);
            }
        }
        if (MenuManager.Instance.SelectedSubMenu == 1 && MenuManager.Instance.CurrentDetailedItemDescription != null) {
            MenuManager.Instance.transform.Find("Inventory Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel(MenuManager.Instance.CurrentDetailedItemDescription.Category == Constants.ItemCategory.Quest ? " " : MenuManager.Instance.CurrentDetailedItemDescription.GetDescription(false) + MenuManager.Instance.GetModifierDescriptions(MenuManager.Instance.CurrentDetailedItemDescription, false));
        }
        if (MenuManager.Instance.SelectedSubMenu == 2 && MenuManager.Instance.CurrentSkillTreeTileDescription?.GetComponent<PassivePowerUpTile>() == null) {
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description").gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").gameObject.SetActive(false);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().RefreshLabel(false);
            MenuManager.Instance.transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<LabelInitializer>().RefreshLabel(false);
        }
    }

    public void OnRotateStanceLeft() {
        if(Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl && Player.Instance.GetStanceOnTheLeft().StanceEffect is Stance_OmniMastery && ((Stance_OmniMastery)Player.Instance.GetStanceOnTheLeft().StanceEffect).UnlockedUpgrade1 && Player.Instance.EffectCooldowns.FirstOrDefault(e => e.Type == typeof(Stance_OmniMastery) && e.ExtraInfo == Player.Instance.GetStanceOnTheLeft().WeaponCategory.ToString()) == null) {
            Player.Instance.Actions.CleanseAllHardCrowdControl();
            Player.Instance.Actions.UseAbility(typeof(Ability_StanceSwitchLeft));
            Player.Instance.AddCooldown(new Cooldown(typeof(Stance_OmniMastery), Stance_OmniMastery.CleanseCooldownDuration, Player.Instance) {ExtraInfo = Player.Instance.GetStanceOnTheLeft().WeaponCategory.ToString()});
            Player.Instance.AddCooldown(new Cooldown(typeof(Ability_StanceSwitch), Constants.STANCE_SWITCH_COOLDOWN_OMNIMASTERY, Player.Instance));
        }
        else if (!Player.Instance.CheckIfAbilityOnCooldown(typeof(Ability_StanceSwitch)) && Ability.CheckIfCanPerformAbility(Player.Instance, typeof(Ability_StanceSwitchLeft)) && 
            (Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.Idle || Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.Moving || Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UsingAbility))
        {
            Player.Instance.Actions.UseAbility(typeof(Ability_StanceSwitchLeft));
            bool shorterCd = Player.Instance._currentStance.StanceEffect is Stance_OmniMastery || Player.Instance.GetStanceOnTheLeft().StanceEffect is Stance_OmniMastery;
            Player.Instance.AddCooldown(new Cooldown(typeof(Ability_StanceSwitch), shorterCd ? Constants.STANCE_SWITCH_COOLDOWN_OMNIMASTERY : Constants.STANCE_SWITCH_COOLDOWN, Player.Instance));
        }
    }

    public void OnRotateStanceRight() {
        if(Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl && Player.Instance.GetStanceOnTheRight().StanceEffect is Stance_OmniMastery && ((Stance_OmniMastery)Player.Instance.GetStanceOnTheRight().StanceEffect).UnlockedUpgrade1 && Player.Instance.EffectCooldowns.FirstOrDefault(e => e.Type == typeof(Stance_OmniMastery) && e.ExtraInfo == Player.Instance.GetStanceOnTheRight().WeaponCategory.ToString()) == null) {
            Player.Instance.Actions.CleanseAllHardCrowdControl();
            Player.Instance.Actions.UseAbility(typeof(Ability_StanceSwitchRight));
            Player.Instance.AddCooldown(new Cooldown(typeof(Stance_OmniMastery), Stance_OmniMastery.CleanseCooldownDuration, Player.Instance) {ExtraInfo = Player.Instance.GetStanceOnTheRight().WeaponCategory.ToString()});
            Player.Instance.AddCooldown(new Cooldown(typeof(Ability_StanceSwitch), Constants.STANCE_SWITCH_COOLDOWN_OMNIMASTERY, Player.Instance));
        }
        else if (!Player.Instance.CheckIfAbilityOnCooldown(typeof(Ability_StanceSwitch)) && Ability.CheckIfCanPerformAbility(Player.Instance, typeof(Ability_StanceSwitchRight)))
        {
            Player.Instance.Actions.UseAbility(typeof(Ability_StanceSwitchRight));
            bool shorterCd = Player.Instance._currentStance.StanceEffect is Stance_OmniMastery || Player.Instance.GetStanceOnTheRight().StanceEffect is Stance_OmniMastery;
            Player.Instance.AddCooldown(new Cooldown(typeof(Ability_StanceSwitch), shorterCd ? Constants.STANCE_SWITCH_COOLDOWN_OMNIMASTERY : Constants.STANCE_SWITCH_COOLDOWN, Player.Instance));
        }
    }


    public void OnOpenDebugConsole()
    {
        if(Settings.Instance.ConsoleEnabled) {
            DebugController.ConsoleOpen = true;
            DebugController.TimeScaleBeforeDebug = Time.timeScale;
            Time.timeScale = 0;
            GameController.Instance.PlayerInput.SwitchCurrentActionMap("Debug Console");
            CanvasElements.UICanvasObject.transform.Find("Debug Console").gameObject.SetActive(true);
            CanvasElements.UICanvasObject.transform.Find("Debug Console").GetComponent<TMP_InputField>().ActivateInputField();
        }
    }

    public void LoadNextLevel()
    {
        SurvivalController.LoadNextLevel();
    }

    public void OnSubmit()
    {
        EventManager.SubmitButtonPressed.Invoke();
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>() != null)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>().OnSubmit(null);
        }
    }

    public void OnCancel()
    {
        EventManager.CancelButtonPressed.Invoke();
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>() != null)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<ChangeSelectOnInput>().OnCancel(null);
        }

    }

    public void OnPrepareForUltimateButtonPress() {
        if(Player.Instance.Energy.Current >= Constants.ENERGY_REQUIRED_TO_USE_ULTIMATE && SaveFile.Instance.UltimatesUsedInCurrentCombat < SaveFile.Instance.MaxUltimateUsesPerCombat) {
            PrepareForUltimate();
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
                foreach(Transform child in CanvasElements.UICanvas.UltimateUses.transform) {
                    child.GetComponent<Image>().color = Color.red;
                } 
                UIManager.Instance.NotEnoughUltimateUsesWarningCounter = 75;
            }
        }
    }

    public void OnPrepareForUltimateButtonRelease() {
        StopPreparingUltimate();
    }

    public void PrepareForUltimate() {
        if(Utils.CheckIfUnitCanPerformActions(Player.Instance) == false) {
            return;
        }
        Player.Instance.PreparingForUltimate = true;
        Player.Instance.PlayAnimation("PreparingForUltimate_" + ((Player.Instance.CurrentStance.StanceEffect is Stance_MindOverMatter || Player.Instance.CurrentStance.StanceEffect is Stance_PowerWithoutLimit) ? "Magic" : Player.Instance.CurrentWeaponDamageCategory));
        foreach(Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            if(ability.Type != null) {
                ability.Icon.sprite = Resources.Load("Sprites/Ability/" + ability.Type.ToString().Replace("Ability_", "") + "_Ultimate", typeof(Sprite)) as Sprite;
                ability.CostLabel.text = "";
                ability.CooldownDisplay.fillAmount = 0;
            }
        }
        foreach(Transform child in CanvasElements.UICanvas.UltimateUses.transform) {
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
        foreach(Transform child in CanvasElements.UICanvas.UltimateUses.transform) {
            child.GetComponent<Image>().color = Color.white;
        } 
    }

    public void OnTurnOnCheatMode() {
        DebugController.Instance.ToggleCheatMode(1);
    }

    public void OnTurnOffCheatMode() {
        DebugController.Instance.ToggleCheatMode(0);
    }
    public void OnF1() {
        foreach(Unit u in Utils.GetAllUnits(true, true)) {
            u.Actions.UseAbility(typeof(AI_RoamAround));
        }
        
    }

    public void OnF2() {

    }

    public void OnF3() {

    }

    public void OnF4() {
    }

    public void OnF10() {
        ScreenCapture.CaptureScreenshot("3LC Screenshot " + DateTime.Now.ToString("yyyy-MM-dd\\THH-mm-ss\\Z") + ".png");
    }

    public void OnInputDebugConsoleCommand()
    {
        CanvasElements.UICanvasObject.transform.Find("Debug Console").GetComponent<DebugController>().InputDebugConsoleCommand();
    }

    public void OnCloseDebugConsole()
    {
        DebugController.CloseDebugConsole();
    }

    public void OnToggleLogs()
    {
        Debug.Log("(" + Utils.GetTimeStamp() + ") Turning Audit Logs " + (Settings.Instance.CreateAuditLogs ? "OFF" : "ON"));
        Settings.Instance.CreateAuditLogs = !Settings.Instance.CreateAuditLogs;
        Settings.Instance.Save();
    }

    public void OnToggleOnScreenLogs() {
        ConsoleToGUI.ShowOnScreenLogs = !ConsoleToGUI.ShowOnScreenLogs;
    }

    public void OnGoToNextDialogueLine() {
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

    public void OnToggleDialogueHistory() {
        if(UIManager.Instance.DialogueHistoryIsOpen == false) {
            UIManager.Instance.OpenDialogueHistory();
        }
        else {
            UIManager.Instance.CloseDialogueHistory();
        }
    }

    public void OnStartSkippingDialogue() {
        UIManager.Instance.SkippingDialogue = true;
        UIManager.Instance.SkipToNextDialogueLine();
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (UIManager.Instance.SkippingDialogue ? "ButtonStopSkip" : "ButtonSkip"), typeof(Sprite)) as Sprite;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Label").GetComponent<LabelInitializer>().SetLabel(UIManager.Instance.SkippingDialogue ? "{DialogueButton_StopSkip}" : "{DialogueButton_Skip}");
    }

    public void OnStopSkippingDialogue() {
        UIManager.Instance.SkippingDialogue = false;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (UIManager.Instance.SkippingDialogue ? "ButtonStopSkip" : "ButtonSkip"), typeof(Sprite)) as Sprite;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Label").GetComponent<LabelInitializer>().SetLabel(UIManager.Instance.SkippingDialogue ? "{DialogueButton_StopSkip}" : "{DialogueButton_Skip}");
    }

    public void OnInteractButtonPress() {
        if((Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.Idle || Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.Moving) && Player.Instance.ClosestInteractable != null) {
            Player.Instance.ClosestInteractable.ActivateInteractable();
        }
    }

    public void OnToggleTarget() {
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

    public void OnStopTargetting() {
        if(_player.CurrentTarget != null)
        {
            _player.CurrentTarget = null;
        }
    }

    public void OnChangeSub2MenuLeft()
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

    public void OnChangeSub2MenuRight()
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
}