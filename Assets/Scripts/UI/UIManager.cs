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
using Unity.VisualScripting;

public class UIManager : MonoBehaviour {
    private static UIManager _instance = null;

    public static UIManager Instance {
        get {
            if (_instance == null && GameController.Instance.IsDestroyed() == false) {
                _instance = GameController.Instance?.GetComponentInChildren<UIManager>();
            }
            return _instance;
        }
    }

    public bool DialogueHistoryIsOpen = false;

    public bool CanGoToNextDialogueLine = true;
    public bool SkippingDialogue;
    public Image DialogueIconLeft;
    public Image DialogueIconRight;
    public LabelInitializer DialogueSpeakerLeft;
    public LabelInitializer DialogueSpeakerRight;
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
            GameController.Objects.DialogueBoxLeft.SetActive(_currentDialogueLine.ShowSpeakerBox && _currentDialogueLine.SpeakerUnit == Player.Instance);
            GameController.Objects.DialogueBoxRight.SetActive(_currentDialogueLine.ShowSpeakerBox && ((_currentDialogueLine.SpeakerName != null && _currentDialogueLine.SpeakerPortrait != null) || (_currentDialogueLine.SpeakerUnit != null && _currentDialogueLine.SpeakerUnit != Player.Instance)));
            string name = Utils.GetNameForUnit(_currentDialogueLine.SpeakerUnit, _currentDialogueLine.SpeakerName);
            if(_currentDialogueLine.Choices.Count == 0) {
                MenuManager.Instance.AddHistoryEntry(new NotificationController.InGameDialogue() { Id=_currentDialogueLine.Id, SpeakerName=name, SpeakerPortrait = _currentDialogueLine.SpeakerPortrait != null ? _currentDialogueLine.SpeakerPortrait : _currentDialogueLine?.SpeakerUnit?.Portrait}, false, _currentDialogueLine?.StringParams);
            }
            if(GameController.Objects.DialogueBoxLeft.activeSelf) {
                DialogueIconLeft.sprite = Resources.Load("Sprites/Face Portrait/Player", typeof(Sprite)) as Sprite;
                DialogueSpeakerLeft.SetLabel("{Name_Player}");
            }
            if(GameController.Objects.DialogueBoxRight.activeSelf) {
                DialogueIconRight.sprite = Resources.Load("Sprites/Face Portrait/" + ((!string.IsNullOrWhiteSpace(_currentDialogueLine.SpeakerPortrait) && _currentDialogueLine.SpeakerPortrait != "Default") ? _currentDialogueLine.SpeakerPortrait : (!string.IsNullOrWhiteSpace(_currentDialogueLine.SpeakerUnit.Portrait) && _currentDialogueLine.SpeakerUnit.Portrait != "Default") ? _currentDialogueLine.SpeakerUnit.Portrait : _currentDialogueLine.SpeakerUnit.IsMale ? "Default_Male" : "Default_Female"), typeof(Sprite)) as Sprite;
                DialogueSpeakerRight.SetLabel(name);
            }
            Utils.DestroyAllChildren(GameController.Objects.DialogueLinesContainer.transform);
            GameController.Objects.DialogueLinesContainer.transform.parent.Find("Clickable").gameObject.SetActive(_currentDialogueLine.Choices.Count == 0);
            if(_currentDialogueLine.Choices.Count == 0) {
                GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_DialogueLineItem")) as GameObject;
                item.GetComponent<DialogueLineItem>().Initialize(_currentDialogueLine);
                CurrentDialogueLineItem = item.GetComponent<DialogueLineItem>();
                item.transform.SetParent(GameController.Objects.DialogueLinesContainer.transform);
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
                        item.transform.SetParent(GameController.Objects.DialogueLinesContainer.transform);
                        item.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                for(int i = 0; i < GameController.Objects.DialogueLinesContainer.transform.childCount; i++) {
                    Navigation nav = GameController.Objects.DialogueLinesContainer.transform.GetChild(i).GetComponent<Button>().navigation;
                    nav.mode = Navigation.Mode.Explicit;
                    nav.selectOnUp = i == 0 ? null : GameController.Objects.DialogueLinesContainer.transform.GetChild(i - 1).GetComponent<Button>();
                    nav.selectOnDown = i == (GameController.Objects.DialogueLinesContainer.transform.childCount - 1) ? null : GameController.Objects.DialogueLinesContainer.transform.GetChild(i + 1).GetComponent<Button>();
                    GameController.Objects.DialogueLinesContainer.transform.GetChild(i).GetComponent<Button>().navigation = nav;
                }
            }
            if(_currentDialogueLine?.Choices?.Count != null && Settings.Instance.ControlScheme == "Gamepad") {
                GameController.Instance.WaitAndRunMethodRealtime(0.01f, SelectFirstChoice);
            }
            Utils.CreateAuditLog("Showing new dialogue line: " + _currentDialogueLine?.Id + " (-> " + _currentDialogueLine?.IdOfNextDialogueLine +") SpeakerUnit:" + _currentDialogueLine?.SpeakerUnit);
        }
    }

    public void SelectFirstChoice() {
        GameController.Objects.DialogueLinesContainer.transform.GetChild(0).GetComponent<Button>().Select();
    }

    public void Start() {
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
        Objects.AmmoDisplayImage.color = new Color(1, 0, 0, 1);
        GameController.Instance.WaitAndRunMethod(1, HideNotEnoughAmmoWarning);
    }

    public void HideNotEnoughAmmoWarning() {
        Objects.AmmoDisplayImage.color = new Color(1, 0, 0, 0);
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
            foreach(Transform child in Objects.UltimateUses.transform) {
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
        GameController.Objects.TransitionUpperText.GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(2);
        GameController.Objects.TransitionLowerText.GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(2);
        if (SaveFile.Instance.GameType == GameType.Survival) {
            SaveFile.Instance.SurvivalChancesRemaining--;
            SaveFile.Instance.Save();
            if(SaveFile.Instance.SurvivalChancesRemaining > 0)
            {
                SurvivalController.LoadStageWithoutRewards = true;
                GameController.Objects.TransitionUpperText.GetComponent<TextMeshProUGUI>().text = Label.Get("SurvivalLifeLost") + " " + SaveFile.Instance.SurvivalChancesRemaining.ToString();
                GameController.Objects.TransitionLowerText.GetComponent<TextMeshProUGUI>().text = "";
                GameController.Instance.WaitAndRunMethod(5, RestartSurvivalStage);
                GameController.Instance.WaitAndRunMethod(4, HideGameOverText);
            }
            else
            {
                GameController.Objects.TransitionUpperText.GetComponent<TextMeshProUGUI>().text = Label.Get(label);
                GameController.Objects.TransitionLowerText.GetComponent<TextMeshProUGUI>().text = label == "GameOverLabelDeath" ? Label.Get("GameOverLabelLoadClueless") : "";
                GameController.Instance.WaitAndRunMethod(5, GoToStartScreen);
                GameController.Instance.WaitAndRunMethod(4, HideGameOverText);
            }
        }
        else if(SaveFile.Instance.CurrentMission != null)
        {
            GameController.Objects.TransitionUpperText.GetComponent<TextMeshProUGUI>().text = Label.Get(label);
            GameController.Objects.TransitionLowerText.GetComponent<TextMeshProUGUI>().text = label == "GameOverLabelDeath" ? Label.Get("GameOverLabelLoadClueless") : "";
            GameController.Instance.WaitAndRunMethod(5, ContinueGameOverScreen);
            GameController.Instance.WaitAndRunMethod(4, HideGameOverText);
        }
        else {
            GameController.Objects.TransitionUpperText.GetComponent<TextMeshProUGUI>().text = Label.Get(label);
            GameController.Objects.TransitionLowerText.GetComponent<TextMeshProUGUI>().text = label == "GameOverLabelDeath" ? Label.Get("GameOverLabelLoadClueless") : "";
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
        GameController.Objects.TransitionUpperText.GetComponent<HideOrShowOverTime>().HideOverTime(1);
        GameController.Objects.TransitionLowerText.GetComponent<HideOrShowOverTime>().HideOverTime(1);
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
            Cooldown abilityCooldown = Player.Instance.TechniqueCooldowns.FirstOrDefault(cooldown => cooldown.Type == ability.Type && cooldown.Identifier == (Player.Instance.PreparingForUltimate ? "IsUltimate" : ""));
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
                    Objects.Items.transform.Find(i.ToString() + "/Cooldown").GetComponent<Image>().fillAmount = fillAmount;
                }
                else
                {
                    Objects.Items.transform.Find(i.ToString() + "/Cooldown").GetComponent<Image>().fillAmount = 0;
                }
            }
        }
    }

    public Slider[] DisplayResourceBarsOnScreen(Unit targeting_unit) {
        if (Objects.EliteEnemyDisplays.transform.childCount >= 3)
        {
            return null;
        }
        GameObject resources = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ResourceDisplay")) as GameObject;
        resources.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = Utils.GetTitleForUnit(targeting_unit);
        resources.transform.rotation = new Quaternion(0, 0, 0, 0);
        resources.transform.SetParent(Objects.EliteEnemyDisplays.transform, false);
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
        Constants.ItemType stance_to_replace = Player.Instance.CurrentStance.WeaponType == Constants.ItemType.Ranged ? Constants.ItemType.Light : (Player.Instance.CurrentStance.WeaponType == Constants.ItemType.Light ? Constants.ItemType.Heavy : Constants.ItemType.Ranged);
        GameObject new_display = InitializeNewStanceDisplay(stance_to_replace.ToString(), Player.Instance.GetCurrentStanceIndex() - 1 < 0 ? SaveFile.Instance.Stances[2] : SaveFile.Instance.Stances[Player.Instance.GetCurrentStanceIndex() - 1]);
        new_display.transform.SetSiblingIndex(0);
        float[] start_scales = new float[4] { 0.01f, 0.5f, 0.7f, 0.5f };
        float[] end_scales = new float[4] { 0.5f, 0.7f, 0.5f, 0.01f };
        for (int i = 0; i < Objects.Stances.transform.childCount; i++) {
            Transform display = Objects.Stances.transform.GetChild(i);
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            display.GetComponent<ChangeTransformOverTime>().SetScaleChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, start_scales[i], end_scales[i]);
            display.GetComponent<ChangeTransformOverTime>().SetPositionXChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, -450 + i * 125, -325 + i * 125);
            if (i == 0) {
                display.Find("Binding Left").gameObject.SetActive(true);
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").GetComponent<LabelInitializer>().SetLabel("[RotateStanceLeftButtonPress]");
            }
            else if (i == 1) {
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").gameObject.SetActive(false);
            }
            else if (i == 2) {
                display.Find("Binding Right").gameObject.SetActive(true);
                display.Find("Binding Left").gameObject.SetActive(false);
                display.Find("Binding Right").GetComponent<LabelInitializer>().SetLabel("[RotateStanceRightButtonPress]");
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
        new_display.transform.SetParent(Objects.Stances.transform, true);
        new_display.transform.localPosition = new Vector2(0, 50);
        return new_display;
    }

    public void DeleteStanceDisplayOnTheRight() {
        MonoBehaviour.Destroy(Player.Instance.CurrentStance.UIStanceDisplay.parent.GetChild(Player.Instance.CurrentStance.UIStanceDisplay.parent.childCount - 1).gameObject);
    }

    public void ResetStanceDisplay()
    {
        if(Objects.Stances.transform.childCount > 0) {
            for(int i = Objects.Stances.transform.childCount - 1; i >= 0; i--) {
                MonoBehaviour.Destroy(Objects.Stances.transform.GetChild(i).gameObject);
            }
        }
        float[] scales = new float[3] { 0.5f, 0.7f, 0.5f };
        int count = 0;
        foreach(int index in new int[] {2, 0, 1}) {
            if(SaveFile.Instance.Stances[index].StanceEffect == null) {
                SaveFile.Instance.Stances[index].StanceEffect = (Effect_Stance)Activator.CreateInstance(typeof(Stance_None), new object[] {null});
            }
            SaveFile.Instance.Stances[index].UIStanceDisplay = InitializeNewStanceDisplay(SaveFile.Instance.Stances[index].WeaponType.ToString(), SaveFile.Instance.Stances[index]).transform;
            SaveFile.Instance.Stances[index].StanceCooldownDisplay = SaveFile.Instance.Stances[index].UIStanceDisplay.transform.Find("Cooldown").GetComponent<Image>();
            Transform display = SaveFile.Instance.Stances[index].UIStanceDisplay;
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            display.transform.localScale = new Vector3(scales[count], scales[count], scales[count]);
            display.transform.localPosition = new Vector2((Settings.Instance.ControlScheme == "Keyboard" ? -325 : -345) + count * 125, 50);
            if (count == 0) {
                display.Find("Binding Left").gameObject.SetActive(true);
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").GetComponent<LabelInitializer>().SetLabel("[RotateStanceLeftButtonPress]");
            }
            else if (count == 1) {
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").gameObject.SetActive(false);
            }
            else if (count == 2) {
                display.Find("Binding Right").gameObject.SetActive(true);
                display.Find("Binding Left").gameObject.SetActive(false);
                display.Find("Binding Right").GetComponent<LabelInitializer>().SetLabel("[RotateStanceRightButtonPress]");
            }
            count++;
        }
    }

    public void ToggleLoadingScreen(bool show = true) {
        GameController.Objects.TransitionLoadingScreen.SetActive(show);
    }

    public void ShowStanceRotateRight() {
        Constants.ItemType stance_to_replace = Player.Instance.CurrentStance.WeaponType == Constants.ItemType.Light ? Constants.ItemType.Ranged : (Player.Instance.CurrentStance.WeaponType == Constants.ItemType.Ranged ? Constants.ItemType.Heavy : Constants.ItemType.Light);
        GameObject new_display = InitializeNewStanceDisplay(stance_to_replace.ToString(), Player.Instance.GetCurrentStanceIndex() + 1 > 2 ? SaveFile.Instance.Stances[0] : SaveFile.Instance.Stances[Player.Instance.GetCurrentStanceIndex() + 1]);
        new_display.transform.SetSiblingIndex(3);
        float[] start_scales = new float[4] { 0.5f, 0.7f, 0.5f, 0.01f };
        float[] end_scales = new float[4] { 0.01f, 0.5f, 0.7f, 0.5f };
        for (int i = 0; i < Objects.Stances.transform.childCount; i++) {
            Transform display = Objects.Stances.transform.GetChild(i);
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            display.GetComponent<ChangeTransformOverTime>().SetScaleChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, start_scales[i], end_scales[i]);
            display.GetComponent<ChangeTransformOverTime>().SetPositionXChangeOverTime(Constants.STANCE_SWITCH_ROTATE_TIME, -325 + i * 125, -450 + i * 125);
            if (i == 1) {
                display.Find("Binding Left").gameObject.SetActive(true);
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").GetComponent<LabelInitializer>().SetLabel("[RotateStanceLeftButtonPress]");
            }
            else if (i == 2) {
                display.Find("Binding Right").gameObject.SetActive(false);
                display.Find("Binding Left").gameObject.SetActive(false);
            }
            else if (i == 3) {
                display.Find("Binding Right").gameObject.SetActive(true);
                display.Find("Binding Left").gameObject.SetActive(false);
                display.Find("Binding Right").GetComponent<LabelInitializer>().SetLabel("[RotateStanceRightButtonPress]");
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
            GameController.Objects.TransitionAreaName.GetComponent<CanvasGroup>().alpha = 0;
            if(Label.ContainsKey("Area_" + area_name)) {
                GameController.Objects.TransitionAreaName.GetComponent<LabelInitializer>().SetLabel("{Area_" + area_name + "}");
            }
            else {
                GameController.Objects.TransitionAreaName.GetComponent<TextMeshProUGUI>().text = "";
            }
            GameController.Objects.TransitionAreaName.GetComponent<HideOrShowOverTime>().ShowOverTime(1);
            GameController.Instance.WaitAndRunMethodRealtime(3, HideTransitionText);
        }
    }

    public void HideTransitionText() {
        GameController.Objects.TransitionAreaName.GetComponent<HideOrShowOverTime>().HideOverTime(1f);
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
        GameController.Objects.TransitionAreaName.GetComponent<HideOrShowOverTime>().HideOverTime(0.05f);
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
        if(dialogue.PlayerStartingPosition != Vector2.zero) {
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
        if(dialogue.PlayerStartingPosition != Vector2.zero) {
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
        Utils.DestroyAllChildren(GameController.Objects.DialogueNotifications.transform);
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
        GameController.Objects.TransitionScreen.GetComponentInChildren<HideOrShowOverTime>(true).ShowOverTimeFromZero(time);
    }

    public void HideBlackScreen(float time = 0.5f) {
        GameController.Objects.TransitionScreen.GetComponentInChildren<HideOrShowOverTime>(true).HideOverTimeFromFull(time);
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

    public void ToggleSkippingDialogue()
    {
        if (SkippingDialogue)
        {
            SkippingDialogue = false;
        }
        else
        {
            SkippingDialogue = true;
            SkipToNextDialogueLine();
        }
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SkippingDialogue ? "ButtonStopSkip" : "ButtonSkip"), typeof(Sprite)) as Sprite;
        GameController.Instance.transform.Find("Dialogue Window/Window/Buttons/Button Skip/Label").GetComponent<LabelInitializer>().SetLabel(SkippingDialogue ? "{DialogueButton_StopSkip}" : "{DialogueButton_Skip}");
    }

    public static class Objects
    {

        private static string _extraInfoInPauseScreen = "Pause Screen/Extra Info";
        private static string _extraInfoInUI = "Extra Info";
        public static GameObject Notifications => Utils.GetGameObject("UI/Notifications");
        public static GameObject NotificationList => Utils.GetGameObject("UI/Notifications/List");
        public static GameObject MoneyDisplay => Utils.GetGameObject($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Mission Info/Money");
        public static TextMeshProUGUI MoneyDisplayText => (TextMeshProUGUI)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Mission Info/Money", typeof(TextMeshProUGUI));
        public static LabelInitializer WeekDisplayLabel => (LabelInitializer)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Mission Info/Week", typeof(LabelInitializer));
        public static TextMeshProUGUI ExperienceBarLeftText => (TextMeshProUGUI)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Level/Background/Left Level", typeof(TextMeshProUGUI));
        public static TextMeshProUGUI ExperienceBarRightText => (TextMeshProUGUI)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Level/Background/Right Level", typeof(TextMeshProUGUI));
        public static Slider ExperienceBarSlider => (Slider)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Level", typeof(Slider));
        public static HideOrShowOverTime InGameDialogueHideOrShow => (HideOrShowOverTime)Utils.GetComponent("UI/Notifications/In-Game Dialogue", typeof(HideOrShowOverTime));
        public static TextMeshProUGUI InGameDialogueText => (TextMeshProUGUI)Utils.GetComponent("/UI/Notifications/In-Game Dialogue/Background/Text", typeof(TextMeshProUGUI));
        public static Image InGameDialoguePortraitImage => (Image)Utils.GetComponent("UI/Notifications/In-Game Dialogue/Portrait and Title/Portrait/Image", typeof(Image));
        public static TextMeshProUGUI InGameDialoguePortraitTitle => (TextMeshProUGUI)Utils.GetComponent("UI/Notifications/In-Game Dialogue/Portrait and Title/Portrait/Title/Text", typeof(TextMeshProUGUI));
        public static GameObject PauseScreen => Utils.GetGameObject("UI/Pause Screen");
        public static GameObject EscapeMissionButton => Utils.GetGameObject("UI/Pause Screen/Buttons/Escape Button");
        public static LabelInitializer EscapeMissionButtonLabel => (LabelInitializer)Utils.GetComponent("UI/Pause Screen/Buttons/Escape Button/Text", typeof(LabelInitializer));
        public static GameObject Effects => Utils.GetGameObject("UI/Effects");
        public static GameObject ResourceBars => Utils.GetGameObject("UI/Resource Bars");
        public static GameObject UltimateUses => Utils.GetGameObject("UI/Resource Bars/Ultimate Uses");
        public static GameObject MissionInfo => Utils.GetGameObject($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Mission Info");
        public static GameObject Timer => Utils.GetGameObject("UI/Timer");
        public static TextMeshProUGUI TimerText => (TextMeshProUGUI)Utils.GetComponent("UI/Timer", typeof(TextMeshProUGUI));
        public static Slider ChargeBarSlider => (Slider)Utils.GetComponent("UI/Charge Bar", typeof(Slider));
        public static TextMeshProUGUI ChargeBarAbilityText => (TextMeshProUGUI)Utils.GetComponent("UI/Charge Bar/Ability Name", typeof(TextMeshProUGUI));
        public static GameObject InCombatIndicator => Utils.GetGameObject($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/InCombat Indicator");
        public static Image InCombatMaskImage => (Image)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/InCombat Indicator/Mask", typeof(Image));
        public static Image InCombatFillImage => (Image)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/InCombat Indicator/Mask/Red Fill", typeof(Image));
        public static Image AmmoDisplayImage => Settings.Instance.ControlScheme == "Keyboard" ? (Image)Utils.GetComponent("UI/Stance Display Keyboard/Ammo Display", typeof(Image)) : (Image)Utils.GetComponent("UI/Stance Display Gamepad/Ammo Display", typeof(Image));
        public static GameObject Stances => Settings.Instance.ControlScheme == "Keyboard" ? Utils.GetGameObject("UI/Stance Display Keyboard/Stances") : Utils.GetGameObject("UI/Stance Display Gamepad/Stances");
        public static GameObject ItemsKeyboard => Utils.GetGameObject("UI/Stance Display Keyboard/Items");
        public static GameObject ItemsGamepad => Utils.GetGameObject("UI/Stance Display Gamepad/Items");
        public static Image GamepadBindingAbilitiesImage => (Image)Utils.GetComponent("UI/Stance Display Gamepad/Binding Abilities", typeof(Image));
        public static Image GamepadBindingItemsImage => (Image)Utils.GetComponent("UI/Stance Display Gamepad/Binding Gamepad", typeof(Image));
        public static GameObject Items => Settings.Instance.ControlScheme == "Keyboard" ? ItemsKeyboard : ItemsGamepad;
        public static GameObject StanceGaugeContainerKeyboard => Utils.GetGameObject("UI/Stance Display Keyboard/Stance Gauge");
        public static GameObject StanceGaugeContainerGamepad => Utils.GetGameObject("UI/Stance Display Gamepad/Stance Gauge");
        public static GameObject StanceGaugeContainer => Settings.Instance.ControlScheme == "Keyboard" ? StanceGaugeContainerKeyboard : StanceGaugeContainerGamepad;
        public static GameObject StanceDisplayKeyboard => Utils.GetGameObject("UI/Stance Display Keyboard");
        public static GameObject StanceDisplayGamepad => Utils.GetGameObject("UI/Stance Display Gamepad");
        public static GameObject AbilitiesKeyboard => Utils.GetGameObject("UI/Stance Display Keyboard/Abilities");
        public static GameObject AbilitiesGamepad => Utils.GetGameObject("UI/Stance Display Gamepad/Abilities");
        public static GameObject Abilities => Settings.Instance.ControlScheme == "Keyboard" ? AbilitiesKeyboard : AbilitiesGamepad;
        public static GameObject EliteEnemyDisplays => Utils.GetGameObject("UI/Elite Enemy Displays");
        public static GameObject ObjectivesDisplay => Utils.GetGameObject($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Mission Info/Objectives");
        public static GameObject FPSCounter => Utils.GetGameObject("UI/Resource Bars/FPS Counter");
        public static GameObject DebugConsole => Utils.GetGameObject("UI/Debug Console");
        public static TextMeshProUGUI InteractIndicatorText => (TextMeshProUGUI)Utils.GetComponent("UI/Interact Indicator", typeof(TextMeshProUGUI));
        public static Slider CustomGaugeSlider => (Slider)Utils.GetComponent("UI/Custom Gauge", typeof(Slider));
        public static TextMeshProUGUI CustomGaugeAmountText => (TextMeshProUGUI)Utils.GetComponent("UI/Custom Gauge/Amount", typeof(TextMeshProUGUI));
        public static Image CycleDisplayImage => (Image)Utils.GetComponent($"UI/{(Settings.Instance.ShowExtraInfoInUI ? _extraInfoInUI : _extraInfoInPauseScreen)}/Mission Info/Cycle", typeof(Image));
        public static TextMeshProUGUI HealingItemText => Settings.Instance.ControlScheme == "Keyboard" ? HealingItemTextKeyboard : HealingItemTextGamepad;
        public static TextMeshProUGUI HealingItemTextKeyboard => (TextMeshProUGUI)Utils.GetComponent("UI/Stance Display Keyboard/Items/Heal/Upgrade", typeof(TextMeshProUGUI));
        public static TextMeshProUGUI HealingItemTextGamepad => (TextMeshProUGUI)Utils.GetComponent("UI/Stance Display Keyboard/Items/Heal/Upgrade", typeof(TextMeshProUGUI));
        public static LabelInitializer KeyboardAbility1UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Keyboard/Abilities/1/Binding", typeof(LabelInitializer));
        public static LabelInitializer KeyboardAbility2UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Keyboard/Abilities/2/Binding", typeof(LabelInitializer));
        public static LabelInitializer KeyboardAbility3UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Keyboard/Abilities/3/Binding", typeof(LabelInitializer));
        public static LabelInitializer KeyboardAbility4UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Keyboard/Abilities/4/Binding", typeof(LabelInitializer));
        public static LabelInitializer KeyboardItem1UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Keyboard/Items/1/Binding", typeof(LabelInitializer));
        public static LabelInitializer KeyboardItem2UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Keyboard/Items/2/Binding", typeof(LabelInitializer));
        public static LabelInitializer KeyboardItemHealingUIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Keyboard/Items/Heal/Binding", typeof(LabelInitializer));
        public static LabelInitializer GamepadAbility1UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Gamepad/Abilities/1/Binding", typeof(LabelInitializer));
        public static LabelInitializer GamepadAbility2UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Gamepad/Abilities/2/Binding", typeof(LabelInitializer));
        public static LabelInitializer GamepadAbility3UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Gamepad/Abilities/3/Binding", typeof(LabelInitializer));
        public static LabelInitializer GamepadAbility4UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Gamepad/Abilities/4/Binding", typeof(LabelInitializer));
        public static LabelInitializer GamepadItem1UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Gamepad/Items/1/Binding", typeof(LabelInitializer));
        public static LabelInitializer GamepadItem2UIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Gamepad/Items/2/Binding", typeof(LabelInitializer));
        public static LabelInitializer GamepadItemHealingUIText => (LabelInitializer)Utils.GetComponent("UI/Stance Display Gamepad/Items/Heal/Binding", typeof(LabelInitializer));
        public static GameObject PauseScreenConfirmPrompt => Utils.GetGameObject("UI/Pause Screen/Confirm Prompt");
        public static TextMeshProUGUI PauseScreenConfirmPromptDescription => (TextMeshProUGUI)Utils.GetComponent("UI/Pause Screen/Confirm Prompt/Description", typeof(TextMeshProUGUI));
        public static Button PauseScreenConfirmPromptConfirmButton => (Button)Utils.GetComponent("UI/Pause Screen/Confirm Prompt/Confirm Button", typeof(Button));
        public static TextMeshProUGUI PauseScreenConfirmPromptConfirmButtonText => (TextMeshProUGUI)Utils.GetComponent("UI/Pause Screen/Confirm Prompt/Confirm Button/Text", typeof(TextMeshProUGUI));
    }
}