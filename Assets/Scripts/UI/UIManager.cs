using System.Linq.Expressions;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;
using UnityEngine.AI;

public class UIManager : MonoBehaviour {
    private static UIManager _instance = null;

    public static UIManager Instance {
        get {
            if (_instance == null) {
                _instance = GameController.Instance.GetComponentInChildren<UIManager>();
            }
            return _instance;
        }
    }

    public bool DialogueHistoryIsOpen = false;

    public bool CanGoToNextDialogueLine = true;
    public bool SkippingDialogue;
    public GameObject LinesContainer;
    public GameObject DialogueBoxLeft;
    public GameObject DialogueBoxRight;
    public Image DialogueIconLeft;
    public Image DialogueIconRight;
    public LabelInitializer DialogueSpeakerLeft;
    public LabelInitializer DialogueSpeakerRight;

    public Transform DialogueInteractIndicator;

    public Dialogue CurrentDialogue;
    public DialogueLineItem CurrentDialogueLineItem;
    private DialogueLine _currentDialogueLine;
    public DialogueLine CurrentDialogueLine {
        get => _currentDialogueLine;
        set {
            if(_currentDialogueLine != null && _currentDialogueLine != value) {
                _currentDialogueLine.OnEnd();
            }
            _currentDialogueLine = value;
            if(_currentDialogueLine == null || _currentDialogueLine.Id == "OpenShop") {
                return;
            }
            if(_currentDialogueLine != null) {
                _currentDialogueLine.OnStart();
            }
            DialogueBoxLeft.SetActive(_currentDialogueLine.ShowSpeakerBox && _currentDialogueLine.SpeakerUnit == Player.Instance);
            DialogueBoxRight.SetActive(_currentDialogueLine.ShowSpeakerBox && ((_currentDialogueLine.SpeakerName != null && _currentDialogueLine.SpeakerPortrait != null) || (_currentDialogueLine.SpeakerUnit != null && _currentDialogueLine.SpeakerUnit != Player.Instance)));
            string name = Utils.GetNameForUnit(_currentDialogueLine.SpeakerUnit, _currentDialogueLine.SpeakerName);
            if(_currentDialogueLine.Choices.Count == 0) {
                MenuManager.Instance.AddHistoryEntry(new NotificationController.InGameDialogue() { Id=_currentDialogueLine.Id, SpeakerName=name, SpeakerPortrait = _currentDialogueLine.SpeakerPortrait != null ? _currentDialogueLine.SpeakerPortrait : _currentDialogueLine?.SpeakerUnit?.Portrait}, false, _currentDialogueLine?.StringParams);
            }
            if(DialogueBoxLeft.activeSelf) {
                DialogueIconLeft.sprite = Resources.Load("Sprites/Face Portrait/Player", typeof(Sprite)) as Sprite;
                DialogueSpeakerLeft.SetLabel("{Name_Player}");
            }
            if(DialogueBoxRight.activeSelf) {
                DialogueIconRight.sprite = Resources.Load("Sprites/Face Portrait/" + ((!string.IsNullOrWhiteSpace(_currentDialogueLine.SpeakerPortrait) && _currentDialogueLine.SpeakerPortrait != "Default") ? _currentDialogueLine.SpeakerPortrait : (!string.IsNullOrWhiteSpace(_currentDialogueLine.SpeakerUnit.Portrait) && _currentDialogueLine.SpeakerUnit.Portrait != "Default") ? _currentDialogueLine.SpeakerUnit.Portrait : _currentDialogueLine.SpeakerUnit.IsMale ? "Default_Male" : "Default_Female"), typeof(Sprite)) as Sprite;
                DialogueSpeakerRight.SetLabel(name);
            }
            Utils.DestroyAllChildren(LinesContainer.transform);
            LinesContainer.transform.parent.Find("Clickable").gameObject.SetActive(_currentDialogueLine.Choices.Count == 0);
            if(_currentDialogueLine.Choices.Count == 0) {
                GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_DialogueLineItem")) as GameObject;
                item.GetComponent<DialogueLineItem>().Initialize(_currentDialogueLine);
                CurrentDialogueLineItem = item.GetComponent<DialogueLineItem>();
                item.transform.SetParent(LinesContainer.transform);
                item.transform.localScale = new Vector3(1, 1, 1);
            }
            else {
                CurrentDialogueLineItem = null;
                foreach(DialogueChoice choice in _currentDialogueLine.Choices) {
                    bool add_choice = true;
                    if(Type.GetType(CurrentDialogue.NameOfParentClass).GetMethod("CheckIfVisible_" + choice.Id) != null) {
                        add_choice = (bool)Type.GetType(CurrentDialogue.NameOfParentClass).GetMethod("CheckIfVisible_" + choice.Id).Invoke(null, null);
                    }
                    if(add_choice) {
                        GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_DialogueLineItem")) as GameObject;
                        item.GetComponent<DialogueLineItem>().Initialize(choice);
                        item.transform.SetParent(LinesContainer.transform);
                        item.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                for(int i = 0; i < LinesContainer.transform.childCount; i++) {
                    Navigation nav = LinesContainer.transform.GetChild(i).GetComponent<Button>().navigation;
                    nav.mode = Navigation.Mode.Explicit;
                    nav.selectOnUp = i == 0 ? null : LinesContainer.transform.GetChild(i - 1).GetComponent<Button>();
                    nav.selectOnDown = i == (LinesContainer.transform.childCount - 1) ? null : LinesContainer.transform.GetChild(i + 1).GetComponent<Button>();
                    LinesContainer.transform.GetChild(i).GetComponent<Button>().navigation = nav;
                }
            }
            if(_currentDialogueLine?.Choices?.Count != null && Settings.Instance.ControlScheme == "Gamepad") {
                GameController.Instance.WaitAndRunMethodRealtime(0.01f, SelectFirstChoice);
            }
            Utils.CreateAuditLog("Showing new dialogue line: " + _currentDialogueLine?.Id + " (-> " + _currentDialogueLine?.IdOfNextDialogueLine +") SpeakerUnit:" + _currentDialogueLine?.SpeakerUnit);
        }
    }

    public void SelectFirstChoice() {
        LinesContainer.transform.GetChild(0).GetComponent<Button>().Select();
    }

    public void Start() {
        DialogueBoxLeft = GameController.Instance.transform.Find("Dialogue Window/Window/Left Portrait").gameObject; 
        DialogueBoxRight = GameController.Instance.transform.Find("Dialogue Window/Window/Right Portrait").gameObject; 
        LinesContainer = GameController.Instance.transform.Find("Dialogue Window/Window/Scroll Rect/Viewport/Lines").gameObject;
        DialogueIconLeft = GameController.Instance.transform.Find("Dialogue Window/Window/Left Portrait/Image").GetComponent<Image>();
        DialogueIconRight = GameController.Instance.transform.Find("Dialogue Window/Window/Right Portrait/Image").GetComponent<Image>();
        DialogueSpeakerLeft = GameController.Instance.transform.Find("Dialogue Window/Window/Left Portrait/Title/Text").GetComponent<LabelInitializer>();
        DialogueSpeakerRight = GameController.Instance.transform.Find("Dialogue Window/Window/Right Portrait/Title/Text").GetComponent<LabelInitializer>();
    }

    public List<Tuple<Image, int>> NotEnoughEnergyWarnings = new List<Tuple<Image, int>>();
    public int NotEnoughUltimateUsesWarningCounter = 0;
 
    private void FixedUpdate() {
        DecrementNotEnoughEnergyWarningTimers();
    }
    
    public void ActivateCurrentlySelectedChoice() {
        foreach(DialogueChoice item in CurrentDialogueLine.Choices) {
            if(item.DialogueLineItem != null && item.DialogueLineItem.IsSelected) {
                item.DialogueLineItem.SelectChoice(item);
            }
        }
    }

    private void Update() {
        if(Player.Instance != null) {
            UpdateAbilityCooldownDisplays();
            UpdateToolCooldownDisplays();
        }
    }

    public void DisplayNotEnoughAmmoWarning() {
        CanvasElements.UICanvas.AmmoDisplay.GetComponent<Image>().color = Color.red;
        CanvasElements.UICanvas.AmmoDisplay.transform.Find("Ammo Count").GetComponent<TextMeshProUGUI>().color = Color.red;
        GameController.Instance.WaitAndRunMethod(1, HideNotEnoughAmmoWarning);
    }

    public void HideNotEnoughAmmoWarning() {
        CanvasElements.UICanvas.AmmoDisplay.GetComponent<Image>().color = Color.white;
        CanvasElements.UICanvas.AmmoDisplay.transform.Find("Ammo Count").GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    private void DecrementNotEnoughEnergyWarningTimers() {
        List<Tuple<Image, int>> filteredList = new List<Tuple<Image, int>>();
        foreach (Tuple<Image, int> display in NotEnoughEnergyWarnings) {
            if (display.Item2 == 0) {
                display.Item1.color = Color.white;
            }
            else {
                filteredList.Add(new Tuple<Image, int>(display.Item1, display.Item2 - 1));
            }
        }
        NotEnoughEnergyWarnings = filteredList;
        if(NotEnoughUltimateUsesWarningCounter == 1) {
            foreach(Transform child in CanvasElements.UICanvas.UltimateUses.transform) {
                child.GetComponent<Image>().color = Color.white;
            } 
        }
        if(NotEnoughUltimateUsesWarningCounter > 0) {
            NotEnoughUltimateUsesWarningCounter--;
        }
    }

    public void ShowGameOverScreen(string label = "GameOverLabelDeath", float black_screen_speed = 2) {
        Player.Instance.AddEffect(new Effect_Invincible(new(Player.Instance)), 5);
        Player.Instance.Actions.enabled = false;
        Player.Instance.KnockedOut = true;
        UIManager.Instance.ShowBlackScreen(black_screen_speed);
        CanvasElements.TransitionScreen.UpperText.GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(2);
        CanvasElements.TransitionScreen.LowerText.GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(2);
        if (SaveFile.Instance.GameType == GameType.Survival) {
            SaveFile.Instance.SurvivalChancesRemaining--;
            SaveFile.Instance.Save();
            if(SaveFile.Instance.SurvivalChancesRemaining > 0)
            {
                SurvivalController.LoadStageWithoutRewards = true;
                CanvasElements.TransitionScreen.UpperText.GetComponent<TextMeshProUGUI>().text = Label.Get("SurvivalLifeLost") + " " + SaveFile.Instance.SurvivalChancesRemaining.ToString();
                CanvasElements.TransitionScreen.LowerText.GetComponent<TextMeshProUGUI>().text = "";
                GameController.Instance.WaitAndRunMethod(5, RestartSurvivalStage);
                GameController.Instance.WaitAndRunMethod(4, HideGameOverText);
            }
            else
            {
                CanvasElements.TransitionScreen.UpperText.GetComponent<TextMeshProUGUI>().text = Label.Get(label);
                CanvasElements.TransitionScreen.LowerText.GetComponent<TextMeshProUGUI>().text = label == "GameOverLabelDeath" ? Label.Get("GameOverLabelLoadClueless") : "";
                GameController.Instance.WaitAndRunMethod(5, GoToStartScreen);
                GameController.Instance.WaitAndRunMethod(4, HideGameOverText);
            }
        }
        else if(SaveFile.Instance.CurrentMission != null)
        {
            CanvasElements.TransitionScreen.UpperText.GetComponent<TextMeshProUGUI>().text = Label.Get(label);
            CanvasElements.TransitionScreen.LowerText.GetComponent<TextMeshProUGUI>().text = label == "GameOverLabelDeath" ? Label.Get("GameOverLabelLoadClueless") : "";
            GameController.Instance.WaitAndRunMethod(5, ContinueGameOverScreen);
            GameController.Instance.WaitAndRunMethod(4, HideGameOverText);
        }
        else {
            CanvasElements.TransitionScreen.UpperText.GetComponent<TextMeshProUGUI>().text = Label.Get(label);
            CanvasElements.TransitionScreen.LowerText.GetComponent<TextMeshProUGUI>().text = label == "GameOverLabelDeath" ? Label.Get("GameOverLabelLoadClueless") : "";
            GameController.Instance.WaitAndRunMethod(5, GoToStartScreen);
            GameController.Instance.WaitAndRunMethod(4, HideGameOverText);
        }
    }

    public void RestartSurvivalStage() {
        GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;
        UIManager.Instance.DisplayAreaTransitionScreen(Label.Get("SurvivalLevelDisplay") + " " + SaveFile.Instance.SurvivalChancesRemaining.ToString());
        SurvivalController.LoadNextLevel();
    }

    public void HideGameOverText() {
        CanvasElements.TransitionScreen.UpperText.GetComponent<HideOrShowOverTime>().HideOverTime(1);
        CanvasElements.TransitionScreen.LowerText.GetComponent<HideOrShowOverTime>().HideOverTime(1);
    }

    public void ContinueGameOverScreen() {
        if(SaveFile.Instance.MidMissionInformation != null) {
            GameController.Instance.LoadMidMission();
        }
        else {
            SaveFile.Instance.CurrentMission.AbandonMission();
        }
    }

    public void GoToStartScreen()
    {
        GameController.Instance.GameplayMode = Constants.GameplayMode.OnStartScreen;
    }

    private void UpdateAbilityCooldownDisplays() {
        if (Player.Instance.CurrentStance?.Abilities == null) {
            return;
        }
        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            bool isStacksBased = ability.Type?.GetField("IsStacksBasedTechnique") != null;
            Cooldown abilityCooldown = Player.Instance.TechniqueCooldowns.FirstOrDefault(cooldown => cooldown.Type == ability.Type && cooldown.ExtraInfo == (Player.Instance.PreparingForUltimate ? "IsUltimate" : ""));
            if (abilityCooldown != null && abilityCooldown.RemainingDuration > 0) {
                float fillAmount = abilityCooldown.RemainingDuration / abilityCooldown.TotalDuration;
                ability.CooldownDisplay.fillAmount = fillAmount;
                double cooldown = abilityCooldown.RemainingDuration;
                ability.CooldownCounter.text = cooldown < 10 ? Math.Round(cooldown, 1).ToString().Replace(',', '.') : ((int)cooldown).ToString();
                if (isStacksBased && ((Player.Instance.PreparingForUltimate && Player.Instance.CurrentUltimateTechniqueStacks[ability.Type] > 0) || (Player.Instance.PreparingForUltimate == false && Player.Instance.CurrentTechniqueStacks[ability.Type] > 0))) {
                    ability.CooldownDisplay.color = new Color(ability.CooldownDisplay.color.r, ability.CooldownDisplay.color.g, ability.CooldownDisplay.color.b, 0.25f);
                }
                else {
                    ability.CooldownDisplay.color = new Color(ability.CooldownDisplay.color.r, ability.CooldownDisplay.color.g, ability.CooldownDisplay.color.b, 0.9f);
                }
            }
            else {
                ability.CooldownDisplay.fillAmount = 0;
                ability.CooldownCounter.text = "";
            }
            if(ability != null && ability.StacksCounter != null && ability.Type != null) {
                ability.StacksCounter.text = !isStacksBased ? "" : (Player.Instance.PreparingForUltimate ? Player.Instance.CurrentUltimateTechniqueStacks[ability.Type].ToString() : Player.Instance.CurrentTechniqueStacks[ability.Type].ToString());
            }
        }
    }

    private void UpdateToolCooldownDisplays()
    {
        for(int i =1; i <= 2; i++)
        {
            Item item = i == 1 ? SaveFile.Instance.EquippedItem1 : SaveFile.Instance.EquippedItem2;
            if(item != null)
            {
                if (Player.Instance.ToolCooldown != null && Player.Instance.ToolCooldown.RemainingDuration > 0)
                {
                    float fillAmount = Player.Instance.ToolCooldown.RemainingDuration / Player.Instance.ToolCooldown.TotalDuration;
                    CanvasElements.UICanvas.Items.transform.Find(i.ToString() + "/Cooldown").GetComponent<Image>().fillAmount = fillAmount;
                }
                else
                {
                    CanvasElements.UICanvas.Items.transform.Find(i.ToString() + "/Cooldown").GetComponent<Image>().fillAmount = 0;
                }
            }
        }
    }

    public Slider[] DisplayResourceBarsOnScreen(Unit targeting_unit) {
        if (CanvasElements.UICanvas.EliteEnemyDisplays.transform.childCount >= 3)
        {
            return null;
        }
        GameObject resources = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ResourceDisplay")) as GameObject;
        resources.GetComponentInChildren<TextMeshProUGUI>().text = Utils.GetTitleForUnit(targeting_unit);
        resources.transform.rotation = new Quaternion(0, 0, 0, 0);
        resources.transform.SetParent(CanvasElements.UICanvas.EliteEnemyDisplays.transform, false);
        return resources.GetComponentsInChildren<Slider>();
    }

    public void ShakeScreen(float duration = 0.1f, float magnitude = 0.05f, float damping = 1) {
        CameraController.Instance.ShakeScreen(duration, magnitude, damping);
    }

    public void DisplayNotEnoughEnergyWarningForGivenAbilityType(Type ability_type) {
        Image ability_display = Player.Instance.GetComponent<Player>().CurrentStance.GetDisplayOfGivenAbility(ability_type);
        Image display = ability_display.transform.Find("Cost").GetComponent<Image>();
        if (display.color == Color.red) {
            Tuple<Image, int> existingWarning = NotEnoughEnergyWarnings.FirstOrDefault(warning => warning.Item1 == display);
            if (existingWarning != null) {
                NotEnoughEnergyWarnings.Remove(existingWarning);
            }
        }
        NotEnoughEnergyWarnings.Add(new Tuple<Image, int>(display, 50));
        display.color = Color.red;
    }

    public void ShowStanceRotateLeft() {
        Constants.ItemCategory stance_to_replace = Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Ranged ? Constants.ItemCategory.Light : (Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Light ? Constants.ItemCategory.Heavy : Constants.ItemCategory.Ranged);
        GameObject new_display = InitializeNewStanceDisplay(stance_to_replace.ToString(), Player.Instance.GetCurrentStanceIndex() - 1 < 0 ? SaveFile.Instance.Stances[2] : SaveFile.Instance.Stances[Player.Instance.GetCurrentStanceIndex() - 1]);
        new_display.transform.SetSiblingIndex(0);
        float[] start_scales = new float[4] { 0.01f, 0.5f, 0.7f, 0.5f };
        float[] end_scales = new float[4] { 0.5f, 0.7f, 0.5f, 0.01f };
        for (int i = 0; i < CanvasElements.UICanvas.Stances.transform.childCount; i++) {
            Transform display = CanvasElements.UICanvas.Stances.transform.GetChild(i);
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            display.GetComponent<ChangeTransformOverTime>().SetScaleChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, start_scales[i], end_scales[i]);
            display.GetComponent<ChangeTransformOverTime>().SetPositionXChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, -450 + i * 125, -325 + i * 125);
            if (i == 0) {
                display.Find("Binding Left").gameObject.SetActive(true);
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").GetComponent<TextMeshProUGUI>().text = "<sprite name=\"StanceSwitchLeftBinding" + Settings.Instance.ControlScheme + "\">";
            }
            else if (i == 1) {
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").gameObject.SetActive(false);
            }
            else if (i == 2) {
                display.Find("Binding Right").gameObject.SetActive(true);
                display.Find("Binding Left").gameObject.SetActive(false);
                display.Find("Binding Right").GetComponent<TextMeshProUGUI>().text = "<sprite name=\"StanceSwitchRightBinding" + Settings.Instance.ControlScheme + "\">";
            }
        }
        Player.Instance.GetStanceForGivenWeapon(stance_to_replace).UIStanceDisplay = new_display.transform;
        Player.Instance.GetStanceForGivenWeapon(stance_to_replace).StanceCooldownDisplay = new_display.transform.Find("Cooldown").GetComponent<Image>();
        GameController.Instance.WaitAndRunMethod(Constants.STANCE_SWITCH_ROTATE_TIME, DeleteStanceDisplayOnTheRight);
    }

    public GameObject InitializeNewStanceDisplay(String stance_to_replace, Stance stance) {
        FieldInfo family = stance.StanceEffect.GetType().GetField("Family", BindingFlags.Public | BindingFlags.Static);
        GameObject new_display = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_StanceDisplay")) as GameObject;
        new_display.transform.Find("Border Upper").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
        new_display.transform.Find("Border Lower").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
        new_display.GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
        new_display.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + stance_to_replace, typeof(Sprite)) as Sprite;
        new_display.transform.Find("Stance Border").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + stance.StanceEffect.GetType().ToString(), typeof(Sprite)) as Sprite;
        new_display.name = "UI_" + stance_to_replace.ToString() + "StanceDisplay";
        new_display.transform.SetParent(CanvasElements.UICanvas.Stances.transform, true);
        new_display.transform.localPosition = new Vector2(0, 50);
        return new_display;
    }

    public void DeleteStanceDisplayOnTheRight() {
        MonoBehaviour.Destroy(Player.Instance.CurrentStance.UIStanceDisplay.parent.GetChild(Player.Instance.CurrentStance.UIStanceDisplay.parent.childCount - 1).gameObject);
    }

    public void ResetStanceDisplay()
    {
        if(CanvasElements.UICanvas.Stances.transform.childCount > 0) {
            for(int i = CanvasElements.UICanvas.Stances.transform.childCount - 1; i >= 0; i--) {
                MonoBehaviour.Destroy(CanvasElements.UICanvas.Stances.transform.GetChild(i).gameObject);
            }
        }
        float[] scales = new float[3] { 0.5f, 0.7f, 0.5f };
        int count = 0;
        foreach(int index in new int[] {2, 0, 1}) {
            if(SaveFile.Instance.Stances[index].StanceEffect == null) {
                SaveFile.Instance.Stances[index].StanceEffect = (Effect_Stance)Activator.CreateInstance(typeof(Stance_None), new object[] {null});
            }
            SaveFile.Instance.Stances[index].UIStanceDisplay = InitializeNewStanceDisplay(SaveFile.Instance.Stances[index].WeaponCategory.ToString(), SaveFile.Instance.Stances[index]).transform;
            SaveFile.Instance.Stances[index].StanceCooldownDisplay = SaveFile.Instance.Stances[index].UIStanceDisplay.transform.Find("Cooldown").GetComponent<Image>();
            Transform display = SaveFile.Instance.Stances[index].UIStanceDisplay;
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            display.transform.localScale = new Vector3(scales[count], scales[count], scales[count]);
            display.transform.localPosition = new Vector2((Settings.Instance.ControlScheme == "Keyboard" ? -325 : -345) + count * 125, 50);
            if (count == 0) {
                display.Find("Binding Left").gameObject.SetActive(true);
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").GetComponent<TextMeshProUGUI>().text = "<sprite name=\"StanceSwitchLeftBinding" + Settings.Instance.ControlScheme + "\">";
            }
            else if (count == 1) {
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").gameObject.SetActive(false);
            }
            else if (count == 2) {
                display.Find("Binding Right").gameObject.SetActive(true);
                display.Find("Binding Left").gameObject.SetActive(false);
                display.Find("Binding Right").GetComponent<TextMeshProUGUI>().text = "<sprite name=\"StanceSwitchRightBinding" + Settings.Instance.ControlScheme + "\">";
            }
            count++;
        }
    }

    public void ToggleLoadingScreen(bool show = true) {
        CanvasElements.TransitionScreen.LoadingScreen.SetActive(show);
    }

    public void ShowStanceRotateRight() {
        Constants.ItemCategory stance_to_replace = Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Light ? Constants.ItemCategory.Ranged : (Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Ranged ? Constants.ItemCategory.Heavy : Constants.ItemCategory.Light);
        GameObject new_display = InitializeNewStanceDisplay(stance_to_replace.ToString(), Player.Instance.GetCurrentStanceIndex() + 1 > 2 ? SaveFile.Instance.Stances[0] : SaveFile.Instance.Stances[Player.Instance.GetCurrentStanceIndex() + 1]);
        new_display.transform.SetSiblingIndex(3);
        float[] start_scales = new float[4] { 0.5f, 0.7f, 0.5f, 0.01f };
        float[] end_scales = new float[4] { 0.01f, 0.5f, 0.7f, 0.5f };
        for (int i = 0; i < CanvasElements.UICanvas.Stances.transform.childCount; i++) {
            Transform display = CanvasElements.UICanvas.Stances.transform.GetChild(i);
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            display.GetComponent<ChangeTransformOverTime>().SetScaleChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, start_scales[i], end_scales[i]);
            display.GetComponent<ChangeTransformOverTime>().SetPositionXChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, -325 + i * 125, -450 + i * 125);
            if (i == 1) {
                display.Find("Binding Left").gameObject.SetActive(true);
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").GetComponent<TextMeshProUGUI>().text = "<sprite name=\"StanceSwitchLeftBinding" + Settings.Instance.ControlScheme + "\">";
            }
            else if (i == 2) {
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").gameObject.SetActive(false);
            }
            else if (i == 3) {
                display.Find("Binding Right").gameObject.SetActive(true);
                display.Find("Binding Left").gameObject.SetActive(false);
                display.Find("Binding Right").GetComponent<TextMeshProUGUI>().text = "<sprite name=\"StanceSwitchRightBinding" + Settings.Instance.ControlScheme + "\">";
            }
        }
        Player.Instance.GetStanceForGivenWeapon(stance_to_replace).UIStanceDisplay = new_display.transform;
        Player.Instance.GetStanceForGivenWeapon(stance_to_replace).StanceCooldownDisplay = new_display.transform.Find("Cooldown").GetComponent<Image>();
        GameController.Instance.WaitAndRunMethod(Constants.STANCE_SWITCH_ROTATE_TIME, DeleteStanceDisplayOnTheLeft);
    }

    public void DeleteStanceDisplayOnTheLeft() {
        MonoBehaviour.Destroy(Player.Instance.CurrentStance.UIStanceDisplay.parent.GetChild(0).gameObject);
    }

    public void DisplayAreaTransitionScreen(string area_name) {
        UIManager.Instance.HideBlackScreen(1);
        if(!String.IsNullOrWhiteSpace(area_name)) {
            CanvasElements.TransitionScreen.AreaName.GetComponent<CanvasGroup>().alpha = 0;
            if(Label.ContainsKey("Area_" + area_name)) {
                CanvasElements.TransitionScreen.AreaName.GetComponent<LabelInitializer>().SetLabel("{Area_" + area_name + "}");
            }
            else {
                CanvasElements.TransitionScreen.AreaName.GetComponent<TextMeshProUGUI>().text = "";
            }
            CanvasElements.TransitionScreen.AreaName.GetComponent<HideOrShowOverTime>().ShowOverTime(1);
            GameController.Instance.WaitAndRunMethodRealtime(3, HideTransitionText);
        }
    }

    public void HideTransitionText() {
        CanvasElements.TransitionScreen.AreaName.GetComponent<HideOrShowOverTime>().HideOverTime(1f);
    }

    public void StartDialogue(Dialogue dialogue) {
        GameController.Instance.ShouldSaveAfterCombat = false;
        DebugController.Instance.ToggleCheatMode(0);
        GameController.Instance.InterruptMusicOnDeath = true;
        Player.Instance.InCombat = false;
        SkippingDialogue = false;
        GameController.Instance.GameplayMode = Constants.GameplayMode.InCutscene;
        Player.Instance.Rigidbody2D.velocity = Vector2.zero;
        GameController.Instance.transform.Find("Dialogue Window").gameObject.SetActive(true);
        if(Type.GetType(dialogue.NameOfParentClass).GetMethod("OnStart_" + dialogue.NameOfDialogue) != null) {
            Type.GetType(dialogue.NameOfParentClass).GetMethod("OnStart_" + dialogue.NameOfDialogue).Invoke(null, null);
        }
        CanvasElements.TransitionScreen.AreaName.GetComponent<HideOrShowOverTime>().HideOverTime(0.05f);
        ShowBlackScreen(0);
        foreach(Unit u in Utils.GetAllUnits(false, true)) {
            u.GetComponent<NavMeshAgent>().enabled = false;
        }
        HideBlackScreen(0.5f);
        CurrentDialogue = dialogue;
        CurrentDialogueLine = dialogue.Lines[0];
        if(dialogue.ReturnUnitsToOriginalPositions) {
            dialogue.PlayerStartedPosition = Player.Instance.transform.position;
            dialogue.PlayerStartedFlipped = Player.Instance.Actions.IsFlipped;
            if(dialogue.DialogueSpeaker != null) {
                if(dialogue?.DialogueSpeaker?.Animator?.GetCurrentAnimatorClipInfo(0) != null && dialogue?.DialogueSpeaker?.Animator?.GetCurrentAnimatorClipInfo(0).Length > 0) {
                    dialogue.SpeakerStartedAnimation = dialogue.DialogueSpeaker.Animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                }
                dialogue.SpeakerStartedPosition = dialogue.DialogueSpeaker.transform.position;
                dialogue.SpeakerStartedFlipped = dialogue.DialogueSpeaker.Actions.IsFlipped;
            }
        }
        if(dialogue.DialogueSpeaker != null && dialogue.SpeakerStartingPosition != Vector2.zero) {
            dialogue.DialogueSpeaker.transform.position = dialogue.SpeakerStartingPosition;
        }
        if(dialogue.DialogueSpeaker != null && dialogue.SpeakerStartingFlipped.HasValue) {
            dialogue.DialogueSpeaker.Actions.IsFlipped = dialogue.SpeakerStartingFlipped.Value;
        }
        Debug.Log("MISC1 Player.Instance.transform.position: " + dialogue.PlayerStartingPosition);
        if(dialogue.PlayerStartingPosition != Vector2.zero) {
            Debug.Log("MISC2 Player.Instance.transform.position: " + Player.Instance.transform.position);
            Player.Instance.transform.position = dialogue.PlayerStartingPosition;
        }
        if(dialogue.PlayerStartingFlipped.HasValue) {
            Player.Instance.Actions.IsFlipped = dialogue.PlayerStartingFlipped.Value;
        }
        //GameController.Instance.WaitAndRunMethodRealtime(0.01f, UpdateDialoguePositions);
    }

    /*public void UpdateDialoguePositions() {
        if(CurrentDialogue == null) {
            return;
        }
        Dialogue dialogue = CurrentDialogue;
        if(dialogue.DialogueSpeaker != null && dialogue.SpeakerStartingPosition != Vector2.zero) {
            dialogue.DialogueSpeaker.transform.position = dialogue.SpeakerStartingPosition;
        }
        if(dialogue.DialogueSpeaker != null && dialogue.SpeakerStartingFlipped.HasValue) {
            dialogue.DialogueSpeaker.Actions.IsFlipped = dialogue.SpeakerStartingFlipped.Value;
        }
        Debug.Log("MISC1 Player.Instance.transform.position: " + dialogue.PlayerStartingPosition);
        if(dialogue.PlayerStartingPosition != Vector2.zero) {
            Debug.Log("MISC2 Player.Instance.transform.position: " + Player.Instance.transform.position);
            Player.Instance.transform.position = dialogue.PlayerStartingPosition;
        }
        if(dialogue.PlayerStartingFlipped.HasValue) {
            Player.Instance.Actions.IsFlipped = dialogue.PlayerStartingFlipped.Value;
        }
    }*/

    public void EndDialogue() {
        GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;
        GameController.Instance.transform.Find("Dialogue Window").gameObject.SetActive(false);
        CurrentDialogueLine = null;
        if(CurrentDialogue.ReturnUnitsToOriginalPositions) {
            Debug.Log("MISC3 Player.Instance.transform.position: " + Player.Instance.transform.position);
            Player.Instance.transform.position = CurrentDialogue.PlayerStartedPosition;
            Player.Instance.Actions.IsFlipped = CurrentDialogue.PlayerStartedFlipped;
            if(CurrentDialogue.DialogueSpeaker != null && CurrentDialogue.DialogueSpeaker.gameObject.activeSelf) {
                try {
                    CurrentDialogue.DialogueSpeaker.PlayAnimation(CurrentDialogue.SpeakerStartedAnimation.name.Replace("Dialogue_", ""));   
                }
                catch(Exception ex) {
                    Debug.LogWarning($"Could not play animation {CurrentDialogue?.SpeakerStartedAnimation?.name} for {CurrentDialogue?.DialogueSpeaker} ({ex?.StackTrace})");
                }
                CurrentDialogue.DialogueSpeaker.transform.position = CurrentDialogue.SpeakerStartedPosition;
                CurrentDialogue.DialogueSpeaker.Actions.IsFlipped = CurrentDialogue.SpeakerStartedFlipped;
            }
        }
        if(CurrentDialogue.DialogueSpeaker != null && CurrentDialogue.SpeakerEndingPosition != Vector2.zero) {
            CurrentDialogue.DialogueSpeaker.transform.position = CurrentDialogue.SpeakerEndingPosition;
        }
        if(CurrentDialogue.DialogueSpeaker != null && CurrentDialogue.SpeakerEndingFlipped.HasValue) {
            CurrentDialogue.DialogueSpeaker.Actions.IsFlipped = CurrentDialogue.SpeakerEndingFlipped.Value;
        }
        if(CurrentDialogue.PlayerEndingPosition != Vector2.zero) {
            Debug.Log("MISC4 Player.Instance.transform.position: " + Player.Instance.transform.position);
            Player.Instance.transform.position = CurrentDialogue.PlayerEndingPosition;
        }
        if(CurrentDialogue.PlayerEndingFlipped.HasValue) {
            Player.Instance.Actions.IsFlipped = CurrentDialogue.PlayerEndingFlipped.Value;
        }
        if(Type.GetType(CurrentDialogue.NameOfParentClass).GetMethod("OnEnd_" + CurrentDialogue.NameOfDialogue) != null) {
            Type.GetType(CurrentDialogue.NameOfParentClass).GetMethod("OnEnd_" + CurrentDialogue.NameOfDialogue).Invoke(null, null);
        }
        ShowBlackScreen(0);
        foreach(Unit u in Utils.GetAllUnits(false, true)) {
            u.GetComponent<NavMeshAgent>().enabled = true;
        }
        Player.Instance.GetComponent<NavMeshAgent>().enabled = false;
        bool should_autosave = CurrentDialogue.AutoSaveOnDialogueEnd;
        CurrentDialogue = null;
        if(DialogueInteractIndicator != null) {
            DialogueInteractIndicator.gameObject.SetActive(true);
            DialogueInteractIndicator = null;
        }
        Utils.DestroyAllChildren(CanvasElements.DialogueNotifications.transform);
        GameController.Instance.WaitAndRunMethod(0.01f, SetPlayerIdle, should_autosave);
    }

    public void SetPlayerIdle(bool save) {
        Player.Instance.Actions.CurrentActionBeingPerformed = ActionType.Idle;
        HideBlackScreen(0.5f);
        if(SaveFile.Instance.CurrentMission != null && save) {
            GameController.Instance.MakeAutoSave();
        }
    }

    public void ProgressToNextDialogueLine() {
        ProgressToNextDialogueLine(null);
    }

    public void ProgressToNextDialogueLine(DialogueLine dialogue_line) { 
        if(CurrentDialogue != null && CurrentDialogueLine != null && CurrentDialogueLineItem != null && CurrentDialogueLine.Choices.Count == 0 && CurrentDialogueLineItem.TextMeshPro != null && CurrentDialogueLineItem.TextMeshPro.maxVisibleCharacters < CurrentDialogueLineItem.TextMeshPro.text.Length) {
            CurrentDialogueLineItem.TextMeshPro.maxVisibleCharacters = CurrentDialogueLineItem.TextMeshPro.text.Length;
            return;
        }
        if(CanGoToNextDialogueLine == false) {
            return;
        }
        else if(dialogue_line != null) {
            CurrentDialogueLine = dialogue_line;
        }
        else if(CurrentDialogueLine.IdOfNextDialogueLine == "END"){
            UIManager.Instance.EndDialogue();
        }
        else if(CurrentDialogueLine.IdOfNextDialogueLine != null){
            CurrentDialogueLine = CurrentDialogue.Lines.FirstOrDefault(line => line.Id == CurrentDialogueLine.IdOfNextDialogueLine);
        }
        if(CurrentDialogueLine != null && CurrentDialogueLine.Choices.Count == 0 && Settings.Instance.AutoSkipReadDialogue && SaveFile.Instance.ReadDialogueLines.Contains(CurrentDialogueLine.Id)) {
            GameController.Instance.WaitAndRunMethod(0.02f, CheckIfShouldAutoSkipDialogueLine, new string[] {CurrentDialogueLine.Id});
        }
        if(CurrentDialogueLine != null && !SaveFile.Instance.ReadDialogueLines.Contains(CurrentDialogueLine.Id)) {
            SaveFile.Instance.ReadDialogueLines.Add(CurrentDialogueLine.Id);
        }
    }

    public void CheckIfShouldAutoSkipDialogueLine(string[] dialogue_line_id) {
        if(CurrentDialogueLine.Id == dialogue_line_id[0]) {
            ProgressToNextDialogueLine();
        }
    }

    public void ShowBlackScreen(float time = 0.5f) {
        CanvasElements.TransitionScreenObject.GetComponentInChildren<HideOrShowOverTime>(true).ShowOverTimeFromZero(time);
    }

    public void HideBlackScreen(float time = 0.5f) {
        CanvasElements.TransitionScreenObject.GetComponentInChildren<HideOrShowOverTime>(true).HideOverTimeFromFull(time);
    }

    public void OpenDialogueHistory() {
        DialogueHistoryIsOpen = true;
        GameController.Instance.transform.Find("Dialogue Window/Dialogue History").gameObject.SetActive(true);
        for(int i = 0; i < 5; i++) {
            GameController.Instance.WaitAndRunMethodRealtime(0.05f * i, ScrollToNewestHistoryEntry);
        }
        if(Settings.Instance.ControlScheme == "Gamepad" && GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").childCount > 0) {
            foreach(HighlightOnGamepadSelect item in GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").GetComponentsInChildren<HighlightOnGamepadSelect>()) {
                item.OnDeselect(null);
            }
            GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").GetChild(0).GetComponent<Button>().Select();
        }
    }

    public void ScrollToNewestHistoryEntry() {
        GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").transform.parent.Find("Scrollbar").GetComponent<Scrollbar>().value = 0;
        GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").GetChild(GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").transform.childCount - 1).GetComponent<Button>().Select();
        GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").GetChild(GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").transform.childCount - 1).GetComponent<CenterScrollRectOnItemWhenSelected>().CenterOnItem();
    }

    public void CloseDialogueHistory() {
        DialogueHistoryIsOpen = false;
        GameController.Instance.transform.Find("Dialogue Window/Dialogue History").gameObject.SetActive(false);
    }

    public void SkipToNextDialogueLine() {
        if(CurrentDialogueLine != null && CurrentDialogue != null && CurrentDialogueLine?.Choices?.Count == 0 && (Settings.Instance.AllowSkipUnreadDialogue || SaveFile.Instance.ReadDialogueLines.Contains(CurrentDialogueLine.IdOfNextDialogueLine)) && SkippingDialogue && GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene) {
            ProgressToNextDialogueLine();
            GameController.Instance.WaitAndRunMethod(0.1f, SkipToNextDialogueLine);
        }
    }

    public void ToggleSkippingDialogue() {
        if(SkippingDialogue) {
            SkippingDialogue = false;
        }
        else {
            SkippingDialogue = true;
            SkipToNextDialogueLine();
        }
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SkippingDialogue ? "ButtonStopSkip" : "ButtonSkip"), typeof(Sprite)) as Sprite;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Label").GetComponent<LabelInitializer>().SetLabel(SkippingDialogue ? "{DialogueButton_StopSkip}" : "{DialogueButton_Skip}");
    }
}