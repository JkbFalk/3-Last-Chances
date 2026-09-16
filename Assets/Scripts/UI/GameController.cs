using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : WorldObject
{
    
    public bool ConfirmPromptActive = false;
    public Action FunctionToExecuteOnConfirm;
    public Action<int> ActionToExecuteOnPromptConfirm;
    public Dictionary<string, AudioClip> SpeechBeepClips;
    public bool PlayBossMusicDuringNextCombat = false;
    private bool _interrruptMusicOnDeath = true;
    public bool InterruptMusicOnDeath {
        get => _interrruptMusicOnDeath;
        set {
            _interrruptMusicOnDeath = value;
        }
    }
    public AudioSource AudioSource;
    public List<SortingOrder> DynamicSortingOrders = new();
    private static GameController _instance = null;
    public static GameController Instance
    {
        get
        {
            if (_instance == null && GameObject.FindGameObjectWithTag("Game Controller") != null)
            {
                _instance = GameObject.FindGameObjectWithTag("Game Controller").GetComponent<GameController>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private SaveFile _currentSaveFile;
    public SaveFile CurrentSaveFile
    {
        get
        {
            if (_currentSaveFile == null)
            {
                _currentSaveFile = new SaveFile("Dummy", SaveFile.SaveFileTypeEnum.Challenge);
            }
            return _currentSaveFile;
        }
        set
        {
            Utils.CreateAuditLog("Loading save file: " + value.ToString());
            _currentSaveFile = value;
        }
    }

    public void WaitAndRunMethodRealtime(float seconds, Action nameOfMethodToRun)
    {
        StartCoroutine(WaitAndRunMethodCoroutineRealtime(seconds, () => nameOfMethodToRun()));
    }

    public void WaitAndRunMethodRealtime(float seconds, Action<string> nameOfMethodToRun, string param)
    {
        StartCoroutine(WaitAndRunMethodCoroutineRealtime(seconds, () => nameOfMethodToRun(param)));
    }

    public void WaitAndRunMethodRealtime(float seconds, Action<string[]> nameOfMethodToRun, string[] string_params)
    {
        StartCoroutine(WaitAndRunMethodCoroutineRealtime(seconds, () => nameOfMethodToRun(string_params)));
    }

    public void WaitAndRunMethodRealtime(float seconds, Action<object[]> nameOfMethodToRun, object[] any_params)
    {
        StartCoroutine(WaitAndRunMethodCoroutineRealtime(seconds, () => nameOfMethodToRun(any_params)));
    }


    public void WaitAndRunMethodRealtime(float seconds, Action<int> nameOfMethodToRun, int param)
    {
        StartCoroutine(WaitAndRunMethodCoroutineRealtime(seconds, () => nameOfMethodToRun(param)));
    }

    public void WaitAndRunMethodRealtime(float seconds, Action<GameObject> nameOfMethodToRun, GameObject game_object)
    {
        StartCoroutine(WaitAndRunMethodCoroutineRealtime(seconds, () => nameOfMethodToRun(game_object)));
    }

    public void WaitAndRunMethod(float seconds, Action nameOfMethodToRun)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun()));
    }

    public void WaitAndRunMethod(float seconds, Action<string[]> nameOfMethodToRun, string[] string_params)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(string_params)));
    }

    public void WaitAndRunMethod(float seconds, Action<bool> nameOfMethodToRun, bool param)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(param)));
    }

    public void WaitAndRunMethod(float seconds, Action<int> nameOfMethodToRun, int index)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(index)));
    }

    public void WaitAndRunMethod(float seconds, Action<Unit> nameOfMethodToRun, Unit unit)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(unit)));
    }

    public void WaitAndRunMethod(float seconds, Action<Ability> nameOfMethodToRun, Ability ability)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(ability)));
    }

    public void WaitAndRunMethod(float seconds, Action<GameObject> nameOfMethodToRun, GameObject game_object)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(game_object)));
    }

    public void AdjustSizeBasedOnGradeWaitAndRunMethod(float seconds, Action<string[]> nameOfMethodToRun, string[] string_params)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(string_params)));
    }

    public void WaitAndRunMethod(float seconds, Action<object[]> nameOfMethodToRun, object[] object_params)
    {
        StartCoroutine(WaitAndRunMethodCoroutine(seconds, () => nameOfMethodToRun(object_params)));
    }

    private IEnumerator WaitAndRunMethodCoroutineRealtime(float seconds, Action nameOfMethodToRun)
    {
        yield return new WaitForSecondsRealtime(seconds);
        nameOfMethodToRun();
    }

    private IEnumerator WaitAndRunMethodCoroutineRealtime(float seconds, Action<string> nameOfMethodToRun, string param)
    {
        yield return new WaitForSecondsRealtime(seconds);
        nameOfMethodToRun(param);
    }

    private IEnumerator WaitAndRunMethodCoroutineRealtime(float seconds, Action<string[]> nameOfMethodToRun, string[] string_params)
    {
        yield return new WaitForSecondsRealtime(seconds);
        nameOfMethodToRun(string_params);
    }

    private IEnumerator WaitAndRunMethodCoroutineRealtime(float seconds, Action<object[]> nameOfMethodToRun, object[] any_params)
    {
        yield return new WaitForSecondsRealtime(seconds);
        nameOfMethodToRun(any_params);
    }

    private IEnumerator WaitAndRunMethodCoroutineRealtime(float seconds, Action<int> nameOfMethodToRun, int param)
    {
        yield return new WaitForSecondsRealtime(seconds);
        nameOfMethodToRun(param);
    }

    private IEnumerator WaitAndRunMethodCoroutineRealtime(float seconds, Action<GameObject> nameOfMethodToRun, GameObject game_object)
    {
        yield return new WaitForSecondsRealtime(seconds);
        nameOfMethodToRun(game_object);
    }

    public IEnumerator WaitAndRunMethodCoroutine(float seconds, Action nameOfMethodToRun)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun();
    }

    public IEnumerator WaitAndRunMethodCoroutine(float seconds, Action<Ability> nameOfMethodToRun, Ability ability)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun(ability);
    }

    public IEnumerator WaitAndRunMethodCoroutine(float seconds, Action<bool> nameOfMethodToRun, bool param)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun(param);
    }

    private IEnumerator WaitAndRunMethodCoroutine(float seconds, Action<int> nameOfMethodToRun, int index)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun(index);
    }

    private IEnumerator WaitAndRunMethodCoroutine(float seconds, Action<Unit> nameOfMethodToRun, Unit unit)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun(unit);
    }

    private IEnumerator WaitAndRunMethodCoroutine(float seconds, Action<GameObject> nameOfMethodToRun, GameObject game_object)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun(game_object);
    }

    private IEnumerator WaitAndRunMethodCoroutine(float seconds, Action<string[]> nameOfMethodToRun, string[] string_params)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun(string_params);
    }

    private IEnumerator WaitAndRunMethodCoroutine(float seconds, Action<object[]> nameOfMethodToRun, object[] object_params)
    {
        yield return new WaitForSeconds(seconds);
        nameOfMethodToRun(object_params);
    }

    public Camera Camera;
    public PlayerControls PlayerControls;
    public PlayerInput PlayerInput;
    public bool AutoSaveSettings;

    private float _defaultTimeSpeed = 1;

    public float DefaultTimeSpeed
    {
        get
        {
            return _defaultTimeSpeed;
        }
        set
        {
            _defaultTimeSpeed = value;
            if (GameplayMode == Constants.GameplayMode.Regular)
            {
                Time.timeScale = value;
                Time.fixedDeltaTime = value * 0.02f;
            }
        }
    }

    private bool _loadingNewArea = false;
    public bool LoadingNewArea {
        get => _loadingNewArea;
        set {
            _loadingNewArea = value;
        }
    }

    public bool EnemiesCanBeKilled = true;

    private Constants.GameplayMode _gameplayMode;

    public Constants.GameplayMode GameplayMode
    {
        get => _gameplayMode;
        set
        {
            Constants.GameplayMode previous = _gameplayMode;
            if (_gameplayMode == Constants.GameplayMode.InMenu && value != Constants.GameplayMode.InMenu)
            {
                EventManager.ExitMenu.Invoke();
                MenuManager.Instance?.OnMenuClosed();
            }
            Utils.CreateAuditLog("Changing gameplay mode: " + _gameplayMode + " -> " + value);
            _gameplayMode = value;
            bool isInMissionSelect = SceneManager.GetActiveScene().name == "MissionSelect";
            Utils.SetActiveOnCanvasGroup(Utils.CanvasType.UI, false);
            Utils.SetActiveOnCanvasGroup(Utils.CanvasType.Menu, false);
            Utils.SetActiveOnCanvasGroup(Utils.CanvasType.Shop, false);
            UIManager.Objects.PauseScreen.gameObject.SetActive(false);
            if (_gameplayMode == Constants.GameplayMode.Regular)
            {
                Time.timeScale = DefaultTimeSpeed;
                Utils.SetActiveOnCanvasGroup(Utils.CanvasType.UI, true);
                PlayerInput.SwitchCurrentActionMap("Regular");
                CameraController.Instance.CenteredOnObject = null;
                CameraController.Instance.transform.position = Player.Instance.transform.position;
                ToggleScreenNotifications(true);
            }
            else if (_gameplayMode == Constants.GameplayMode.InCutscene)
            {
                DefaultTimeSpeed = 1;
                Time.timeScale = 1;
                PlayerInput.SwitchCurrentActionMap("Dialogue");
                ToggleScreenNotifications(true);
                Player.Instance.Actions.TryingToMoveInDirection.Clear();
                Player.Instance.Rigidbody2D.linearVelocity = Vector2.zero;
                Player.Instance.GetComponent<NavMeshObstacle>().enabled = false;
                if (Player.Instance?.UnitAI?.NavMeshAgent != null)
                {
                    Player.Instance.UnitAI.NavMeshAgent.enabled = true;
                }
                InterruptMusicOnDeath = true;
            }
            else if (_gameplayMode == Constants.GameplayMode.InMenu)
            {
                Time.timeScale = 0;
                Utils.SetActiveOnCanvasGroup(Utils.CanvasType.Menu, true);
                MenuManager.Instance.OnMenuOpened();
                
                ToggleScreenNotifications(false);
                PlayerInput.SwitchCurrentActionMap("Menu");
                Utils.DestroyAllChildren(MenuManager.Objects.Notifications.transform);
                MenuManager.Instance.transform.Find("Character Window/Effects").gameObject.SetActive(false);
                int count = Player.Instance.CurrentEffects.Where(effect => effect.ShowsInMenu).ToArray().Length;
                MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/ActiveEffects").GetComponent<TextMeshProUGUI>().text = Label.Get("UI_ActiveEffectCount") + count;
                MenuManager.Instance.transform.Find("Character Window/Abilities/UI_DisabledInCombat/ActiveEffects").GetComponent<TextMeshProUGUI>().text = Label.Get("UI_ActiveEffectCount") + count;
            }
            else if (_gameplayMode == Constants.GameplayMode.Paused)
            {
                Time.timeScale = 0;
                PlayerInput.SwitchCurrentActionMap("Menu");
                ToggleScreenNotifications(false);
                if(isInMissionSelect) {
                    Utils.GetSceneRootObject("Mission Select").Find("Pause Screen").gameObject.SetActive(true);
                }
                else {
                    Utils.SetActiveOnCanvasGroup(Utils.CanvasType.UI, true);
                    UIManager.Objects.PauseScreen.gameObject.SetActive(true);
                }
            }
            else if (_gameplayMode == Constants.GameplayMode.InfoPrompt)
            {
                Time.timeScale = 0;
                ToggleScreenNotifications(false);
                PlayerInput.SwitchCurrentActionMap("Menu");
            }
            else if (_gameplayMode == Constants.GameplayMode.Shopping)
            {
                Time.timeScale = 0;
                Utils.SetActiveOnCanvasGroup(Utils.CanvasType.Shop, true);
                ToggleScreenNotifications(false);
                PlayerInput.SwitchCurrentActionMap("Menu");
            }
            else if (_gameplayMode == Constants.GameplayMode.MissionSelect)
            {
                ResetGameplay();
                Time.timeScale = 1;
                if (!isInMissionSelect)
                {
                    EventManager.FinishedLoadingArea.AddListener(FinishEnteringMissionSelect);
                    Utils.MoveIntoArea(true, "MissionSelect");
                }
                else
                {
                    Utils.GetSceneRootObject("Mission Select").Find("Pause Screen").gameObject.SetActive(false);
                    FinishEnteringMissionSelect();
                }
                InterruptMusicOnDeath = true;
            }
            else if (_gameplayMode == Constants.GameplayMode.OnStartScreen)
            {
                ResetGameplay();
                Time.timeScale = 0;
                if (SceneManager.GetActiveScene().name != "StartScreen")
                {
                    EventManager.FinishedLoadingArea.AddListener(FinishEnteringStartScreen);
                    Utils.MoveIntoArea(true, "StartScreen");
                }
                else
                {
                    FinishEnteringStartScreen();
                }
                InterruptMusicOnDeath = true;
            }
            if (Player.Instance.UnitAI != null)
            {
                Player.Instance.UnitAI.enabled = _gameplayMode == Constants.GameplayMode.InCutscene;
                Player.Instance.UnitAI.Start();
                if (_gameplayMode != Constants.GameplayMode.InCutscene)
                {
                    Player.Instance.UnitAI.NavMeshAgent.enabled = false;
                    Player.Instance.GetComponent<NavMeshObstacle>().enabled = Player.Instance.InCombat && Player.Instance.CollisionTurnedOn;
                }
            }
            Button pauseSaveButton = MenuManager.Objects.OtherSaveButton;
            if (pauseSaveButton != null)
            {
                pauseSaveButton.interactable = SaveFile.Instance.CheckIfCurrentlyCanSave();
            }
            CameraController.Instance.Camera.enabled = !isInMissionSelect;
            if(isInMissionSelect) {
                Utils.GetSceneRootObject("Mission Select").Find("Camera").GetComponent<Camera>().enabled = isInMissionSelect;
                Player.Instance.transform.position = new Vector2(-100, -100);
            }
            UIManager.Objects.NotificationList.transform.parent.GetComponent<CanvasGroup>().alpha = (_gameplayMode == Constants.GameplayMode.OnStartScreen ? 0 : 1);
        }
    }

    public void OneTenthSecondElapsedInGame() {
        foreach (SortingOrder so in DynamicSortingOrders)
        {
            so.UpdateSortingOrder();
        }
        EventManager.OneTenthSecondElapsedInGame.Invoke();
        Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
        WaitAndRunMethod(0.1f, OneTenthSecondElapsedInGame);
    }

    public void OneTenthSecondElapsedRealtime() {
        EventManager.OneTenthSecondElapsedRealtime.Invoke();
        WaitAndRunMethodRealtime(0.1f, OneTenthSecondElapsedRealtime);
    }
    
    public void OneSecondElapsedInGame() {
        EventManager.OneSecondElapsedInGame.Invoke();
        WaitAndRunMethod(1.0f, OneSecondElapsedInGame);
    }

    public void OneSecondElapsedRealtime() {
        EventManager.OneSecondElapsedRealtime.Invoke();
        WaitAndRunMethodRealtime(1.0f, OneSecondElapsedRealtime);
    }

    public void ChooseSurvivalType(int option)
    {
        Mission_CompleteSurvival mission = null;
        if (SurvivalController.StoryModeSurvival)
        {
            mission = (Mission_CompleteSurvival)SaveFile.Instance.CurrentMission;
        }
        CurrentSaveFile = new SaveFile(SurvivalController.StoryModeSurvival ? SaveFile.Instance.Id : "StartScreen", SaveFile.SaveFileTypeEnum.Survival);
        if (SurvivalController.StoryModeSurvival)
        {
            SaveFile.Instance.CurrentMission = mission;
        }
        if (option == 0)
        {
            SaveFile.Instance.SkillTreeSurvivalType = false;
        }
        SaveFile.Instance.Difficulty = Constants.Difficulty.Regular;
        if (SurvivalController.StoryModeSurvival)
        {
            Utils.GetSceneRootObject("Mission Select").transform.Find("Survival Type Selection").gameObject.SetActive(false);
        }
        else
        {
            Utils.GetSceneRootObject("Start Screen").transform.Find("Survival Type Selection").gameObject.SetActive(false);
        }
        SurvivalController.StartSurvivalMode();
        CurrentSaveFile.InitializeSaveFile();
    }

    public void FinishEnteringStartScreen()
    {
        ToggleScreenNotifications(false);
        PlayerInput.SwitchCurrentActionMap("Menu");
        Utils.DestroyAllChildren(UIManager.Objects.NotificationList.transform);
        UIManager.Objects.InGameDialogueHideOrShow.gameObject.SetActive(false);
        //Utils.GetSceneRootObject("Start Screen").Find("Screen/Buttons/Continue Survival Mode").GetComponent<Button>().interactable = ES3.FileExists("Survival_StartScreen.es3");
        UIManager.Objects.EscapeMissionButton.gameObject.SetActive(false);
        Utils.GetSceneRootObject("Start Screen").GetComponent<StartScreen>().InitializeStartScreen();
        Utils.SetDefaultMusic("Start Screen");
    }

    public void FinishEnteringMissionSelect()
    {
        Utils.GetSceneRootObject("Mission Select").Find("Level/Left Level").GetComponent<TextMeshProUGUI>().text = SaveFile.Instance.Level.ToString();
        Utils.GetSceneRootObject("Mission Select").Find("Level/Right Level").GetComponent<TextMeshProUGUI>().text = (SaveFile.Instance.Level + 1).ToString();
        Utils.GetSceneRootObject("Mission Select").Find("Level").GetComponent<Slider>().value = (float)SaveFile.Instance.ExperiencePoints / (float)SaveFile.Instance.GetExperiencePointsNeededToLevelUp();
        Utils.GetSceneRootObject("Mission Select").Find("Money").GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedInteger(SaveFile.Instance.Money);
        Utils.GetSceneRootObject("Mission Select").Find("Week").GetComponent<LabelInitializer>().SetLabel(String.Format(Label.Get("SaveFileWeek"), new string[] { SaveFile.Instance.Week.ToString() }));
        UIManager.Objects.WeekDisplayLabel.SetLabel(String.Format(Label.Get("SaveFileWeek"), new string[] { SaveFile.Instance.Week.ToString() }));
        ToggleScreenNotifications(true);
        PlayerInput.SwitchCurrentActionMap("Mission Select");
        MenuManager.Instance.PlayMissionSelectMusic();
        Player.ResetPlayer();
        UIManager.Objects.EscapeMissionButton.gameObject.SetActive(false);
        MenuManager.Instance.UpdateMissionList();
        WaitAndRunMethodRealtime(5f, MenuManager.Instance.SelectTopmostMission);
        SaveFile.Instance.SetCorrectCycle();
    }

    public void ResetAllCoroutinesAndRemoveAllListeners(bool stop_all_coroutines = true)
    {
        if (stop_all_coroutines)
        {
            StopAllCoroutines();
        }
        foreach (FieldInfo unityEvent in typeof(EventManager).GetFields())
        {
            if (unityEvent.Name != "FinishedLoadingArea" && unityEvent.GetValue(null).GetType().IsSubclassOf(typeof(UnityEventBase)))
            {
                ((UnityEventBase)unityEvent.GetValue(null)).RemoveAllListeners();
            }
        }
        OneTenthSecondElapsedRealtime();
        OneTenthSecondElapsedInGame();
        OneSecondElapsedRealtime();
        OneSecondElapsedInGame();
    }

    public void ResetGameplay()
    {
        DefaultTimeSpeed = 1;
        ResetAllCoroutinesAndRemoveAllListeners();
        if (Player.HasInstance())
        {
            Player.Instance.gameObject.tag = "Unit";
            MonoBehaviour.Destroy(Player.Instance.gameObject);
            Player.Instance = null;
        }
        foreach (Transform child in UIManager.Objects.ObjectivesDisplay.transform)
        {
            MonoBehaviour.Destroy(child.gameObject);
        }
    }

    private TextMeshProUGUI counter;
    private int checkIfUpdateFPS = 0;
    private int _counter = 0;
    public void FixedUpdate()
    {
        EventManager.OneFrameElapsedRealtime.Invoke();
        if (Time.timeScale > 0)
        {
            EventManager.OneFrameElapsedInGame.Invoke();
        }
    }

    public MethodInfo CurrentAreaOnUpdateMethod = null;

    public void Update()
    {
        Objects.AudioListener.transform.position = Player.Instance.transform.position;
        if (SaveFile.Instance?.TimePlayedInSeconds != null)
        {
            SaveFile.Instance.TimePlayedInSeconds += Time.unscaledDeltaTime;
        }
        if (UIManager.Objects.FPSCounter.activeSelf)
        {
            if (counter == null)
            {
                counter = UIManager.Objects.FPSCounter.GetComponent<TextMeshProUGUI>();
            }
            if (checkIfUpdateFPS == 50)
            {
                counter.text = Utils.GetFormattedFloat(1.0f / Time.unscaledDeltaTime);
                checkIfUpdateFPS = 0;
            }
            checkIfUpdateFPS++;
        }
        if(CurrentAreaOnUpdateMethod != null) {
            CurrentAreaOnUpdateMethod.Invoke(null, null);
        }
    }

    public void ToggleScreenNotifications(bool show)
    {
        Objects.TransitionBlackScreen.gameObject.SetActive(show);
        Objects.TransitionUpperText.gameObject.SetActive(show);
        Objects.TransitionLowerText.gameObject.SetActive(show);
        Objects.TransitionAreaName.SetActive(show);
    }

    public void SelectObject(GameObject gameobject)
    {
        if (Settings.Instance.ControlScheme == "Gamepad")
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public bool Saving = false;

    public void ToggleSavePanel(bool open)
    {
        transform.Find("Save or Load").gameObject.SetActive(open);
        if (!open)
        {
            if(Settings.Instance.ControlScheme == "Gamepad") {
                PlayerControls.GamepadSelectObjectClosestToCenter();
            }
            return;
        }
        Utils.ScrollToTopOrBottom(transform.Find("Save or Load/Viewport"));
        for (int i = -3; i < Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + 1; i++)
        {
            transform.Find("Save or Load/Viewport/Save Files/" + i + "/Confirm Button/Text").GetComponent<LabelInitializer>().SetLabel("{ButtonSaveFile}");
            if(i < 1) {
                transform.Find("Save or Load/Viewport/Save Files/" + i).gameObject.SetActive(false);
            }
            else{
                transform.Find("Save or Load/Viewport/Save Files/" + i).gameObject.SetActive(true);
                transform.Find("Save or Load/Viewport/Save Files/" + i + "/Delete Button").GetComponent<Button>().interactable = ES3.FileExists("SaveFile_" + i + ".es3");
            }
        }
        transform.Find("Save or Load/Confirm Prompt").gameObject.SetActive(false);
        Instance.transform.Find("Save or Load/Save Title").gameObject.SetActive(true);
        Instance.transform.Find("Save or Load/Load Title").gameObject.SetActive(false);
        Saving = true;
        if(Settings.Instance.ControlScheme == "Gamepad") {
            transform.Find("Save or Load/Viewport/Save Files/1/Background").GetComponent<Button>().Select();
        }
    }

    public void ToggleLoadPanel(bool open)
    {
        transform.Find("Save or Load").gameObject.SetActive(open);
        if (!open)
        {
            return;
        }
        Utils.ScrollToTopOrBottom(transform.Find("Save or Load/Viewport"));
        for (int i = -3; i < Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + 1; i++)
        {
            transform.Find("Save or Load/Viewport/Save Files/" + i + "/Confirm Button/Text").GetComponent<LabelInitializer>().SetLabel("{ButtonLoadFile}");
            if(i == -3) {
                transform.Find("Save or Load/Viewport/Save Files/" + i).gameObject.SetActive(ES3.FileExists("QuickSave.es3"));
            }
            else if(i == -2) {
                transform.Find("Save or Load/Viewport/Save Files/" + i).gameObject.SetActive(ES3.FileExists("PreMissionAutoSave.es3"));
            }
            else if(i == -1) {
                transform.Find("Save or Load/Viewport/Save Files/" + i).gameObject.SetActive(ES3.FileExists("MidMissionAutoSave.es3"));
            }
            else if(i == 0) {
                transform.Find("Save or Load/Viewport/Save Files/" + i).gameObject.SetActive(ES3.FileExists("PostMissionAutoSave.es3"));
            }
            else{
                transform.Find("Save or Load/Viewport/Save Files/" + i).gameObject.SetActive(ES3.FileExists("SaveFile_" + i + ".es3"));
            }
        }
        transform.Find("Save or Load/Confirm Prompt").gameObject.SetActive(false);
        transform.Find("Save or Load/Save Title").gameObject.SetActive(false);
        transform.Find("Save or Load/Load Title").gameObject.SetActive(true);
        Saving = false;
        if(Settings.Instance.ControlScheme == "Gamepad") {
            transform.Find("Save or Load/Viewport/Save Files/-3/Background").GetComponent<Button>().Select();
        }
    }

    public int SaveSlotNumberCurrentlySelected;
    public void ExecuteActionOnPromptConfirm() {
        ActionToExecuteOnPromptConfirm.Invoke(SaveSlotNumberCurrentlySelected);
    }

    public void SaveOrLoadGame(int slot)
    {
        if (Saving)
        {
            SaveFile.Instance.SaveFileNumber = slot.ToString();
            SaveFile.Instance.Save("SaveFile_" + slot.ToString() + ".es3");
        }
        else
        {
            SaveFile sf;
            if (slot == -3)
            {
                sf = SaveFile.RetrieveSaveFile("QuickSave.es3");
            }
            else if (slot == -2)
            {
                sf = SaveFile.RetrieveSaveFile("PreMissionAutoSave.es3");
            }
            else if (slot == -1)
            {
                sf = SaveFile.RetrieveSaveFile("MidMissionAutoSave.es3");
            }
            else if (slot == 0)
            {
                sf = SaveFile.RetrieveSaveFile("PostMissionAutoSave.es3");
            }
            else
            {
                sf = SaveFile.RetrieveSaveFile("SaveFile_" + slot + ".es3");
            }
            sf.Load();
        }
    }

    public void ShowSaveOrLoadModal(int save_slot)
    {
        transform.Find("Save or Load/Confirm Prompt").gameObject.SetActive(true);
        transform.Find("Save or Load/Confirm Prompt/Description").GetComponent<LabelInitializer>().string_params = new List<string> {save_slot == -3 ? Label.Get("SaveSlotQuickSave") : save_slot == -2 ? Label.Get("SaveSlotAutoSaveBeforeMission") : save_slot == -1 ? Label.Get("SaveSlotAutoSaveDuringMission") : save_slot == 0 ? Label.Get("SaveSlotAutoSaveAfterMission") : (Label.Get("SaveSlotRegular") + " " + save_slot.ToString())};
        transform.Find("Save or Load/Confirm Prompt/Description").GetComponent<LabelInitializer>().SetLabel(Saving ? "{OverwriteSaveConfirmation}" : "{LoadSaveConfirmation}");
    }

    public void ShowDeleteSaveModal(int save_slot)
    {
        transform.Find("Save or Load/Confirm Prompt").gameObject.SetActive(true);
        transform.Find("Save or Load/Confirm Prompt/Description").GetComponent<LabelInitializer>().string_params = new List<string> {save_slot == -3 ? Label.Get("SaveSlotQuickSave") : save_slot == -2 ? Label.Get("SaveSlotAutoSaveBeforeMission") : save_slot == -1 ? Label.Get("SaveSlotAutoSaveDuringMission") : save_slot == 0 ? Label.Get("SaveSlotAutoSaveAfterMission") : (Label.Get("SaveSlotRegular") + " " + save_slot.ToString())};
        transform.Find("Save or Load/Confirm Prompt/Description").GetComponent<LabelInitializer>().SetLabel("{DeleteSaveConfirmation}");
    }


    public void DeleteSave(int save_slot)
    {
        if(Settings.Instance.ControlScheme == "Gamepad") {
            transform.Find("Save or Load/Confirm Prompt/UI_Button").GetComponent<Button>().Select();
        }
        if(save_slot == -3 && ES3.FileExists("QuickSave.es3")) {
            ES3.DeleteFile("QuickSave.es3");
            ES3.DeleteFile("QuickSave.png");
            CleanUpSaveSlotInformation(transform.Find("Save or Load/Viewport/Save Files/-3").gameObject);
        }
        else if(save_slot == -2 && ES3.FileExists("PreMissionAutoSave.es3")) {
            ES3.DeleteFile("PreMissionAutoSave.es3");
            ES3.DeleteFile("PreMissionAutoSave.png");
            CleanUpSaveSlotInformation(transform.Find("Save or Load/Viewport/Save Files/-2").gameObject);
        }
        else if(save_slot == -1 && ES3.FileExists("MidMissionAutoSave.es3")) {
            ES3.DeleteFile("MidMissionAutoSave.es3");
            ES3.DeleteFile("MidMissionAutoSave.png");
            CleanUpSaveSlotInformation(transform.Find("Save or Load/Viewport/Save Files/-1").gameObject);
        }
        else if(save_slot == 0 && ES3.FileExists("PostMissionAutoSave.es3")) {
            ES3.DeleteFile("PostMissionAutoSave.es3");
            ES3.DeleteFile("PostMissionAutoSave.png");
            CleanUpSaveSlotInformation(transform.Find("Save or Load/Viewport/Save Files/0").gameObject);
        }
        else if(ES3.FileExists("SaveFile_" + save_slot + ".es3")) {
            ES3.DeleteFile("SaveFile_" + save_slot + ".es3");
            ES3.DeleteFile("SaveFile_" + save_slot + ".png");
            CleanUpSaveSlotInformation(transform.Find("Save or Load/Viewport/Save Files/" + save_slot).gameObject);
        }
        if(SceneManager.GetActiveScene().name == "StartScreen") {
            Utils.GetSceneRootObject("Start Screen").GetComponent<StartScreen>().InitializeStartScreen();
        }
        if(Saving) {
            ToggleSavePanel(false);
            ToggleSavePanel(true);
        }
        else {
            ToggleLoadPanel(false);
            ToggleLoadPanel(true);
        }
    }

    public void CleanUpSaveSlotInformation(GameObject game_object) {
        /*game_object.transform.Find("Delete Button").GetComponent<Button>().interactable = false;
        game_object.transform.Find("Confirm Button").GetComponent<Button>().interactable = true;
        game_object.transform.Find("Screenshot").GetComponent<Image>().color = Color.black;
        game_object.transform.Find("Empty").gameObject.SetActive(true);
        game_object.transform.Find("Cycle, Week, Time/Image").GetComponent<Image>().enabled = false;
        foreach(TextMeshProUGUI text in new List<TextMeshProUGUI>() {game_object.transform.Find("Cycle, Week, Time").GetComponent<TextMeshProUGUI>(), game_object.transform.Find("Level, Money, Skill Trees").GetComponent<TextMeshProUGUI>(), game_object.transform.Find("Difficulty, Mission").GetComponent<TextMeshProUGUI>()}) {
            text.text = "";
        }*/
    }

    public void CancelDeleteSave()
    {
        transform.Find("Save or Load/Confirm Prompt").gameObject.SetActive(false);
    }

    public void MakeAutoSave(bool mid_mission = true, bool show_notification = false) {
        Utils.CreateAuditLog(mid_mission ? "Making Mid-Mission AutoSave" : "Making Post-Mission AutoSave");
        SaveFile.Instance.Save(mid_mission ? "MidMissionAutoSave.es3" : "PostMissionAutoSave.es3", !mid_mission);
        if (show_notification)
        {
            NotificationController.ShowNotificationWithGraphic("SaveMidMission", "UI/Save");
        }
    }

    public void SaveMidMissionInformation(List<string> methods_to_execute_on_load = null)
    {
        Utils.CreateAuditLog("Saving Mid-Mission Information");
        MidMissionInformation mmi = new();
        foreach (InteractableObject inter in Area.Instance.GetComponentsInChildren<InteractableObject>(true))
        {
            SpriteRenderer sr = inter.transform.Find("Graphic")?.GetComponent<SpriteRenderer>();
            string path = Utils.GetGameObjectPath(inter.gameObject);
            if (mmi.GameObjectPathsToInteractables.ContainsKey(path))
            {
                Debug.LogError("Duplicate interactable detected while making a save: " + path);
            }
            else
            {
                mmi.GameObjectPathsToInteractables.Add(path, new() { InteractCount = inter.InteractedCount, Position = inter.transform.position, Sprite = sr?.sprite, IsActive = inter.gameObject.activeSelf });
            }
        }
        foreach (DestructibleEnvironment destr in Area.Instance.GetComponentsInChildren<DestructibleEnvironment>(true))
        {
            string path = Utils.GetGameObjectPath(destr.gameObject);
            if (mmi.GameObjectPathsToDestructibles.ContainsKey(path))
            {
                Debug.LogError("Duplicate destructible detected while making a save: " + path);
            }
            else
            {
                mmi.GameObjectPathsToDestructibles.Add(path, new() { Position = destr.transform.position, IsActive = destr.gameObject.activeSelf });
            }
        }
        foreach (Unit unit in Utils.GetAllUnits(false, false))
        {
            if (unit != null && unit is not Player)
            {
                string path = Utils.GetGameObjectPath(unit.gameObject);
                if (mmi.GameObjectPathsToUnits.ContainsKey(path))
                {
                    Debug.LogError("Duplicate unit detected while making a save: " + path);
                }
                else
                {
                    mmi.GameObjectPathsToUnits.Add(path, new() { Position = unit.transform.position, KnockedOut = unit.KnockedOut, IsFlipped = unit.gameObject.activeInHierarchy ? unit.Actions.IsFlipped : false, IsActive = unit.gameObject.activeSelf });
                }
            }
        }
        mmi.AreaName = Area.Instance.gameObject.name.Replace("(Clone)", "");
        mmi.PlayerIsFlipped = Player.Instance.Actions.IsFlipped;
        mmi.PlayerPosition = Player.Instance.transform.position;
        mmi.ClassesAndMethodsToExecute = methods_to_execute_on_load;
        mmi.PlayerCurrentHealth = Player.Instance.Health.Current;

        SaveFile.Instance.MidMissionInformation = mmi;
    }

    public void LoadMidMissionAutoSave() {

    }

    public void LoadMidMission()
    {
        Utils.MoveIntoArea(true, SaveFile.Instance.MidMissionInformation.AreaName, null, true);
        EventManager.FinishedLoadingArea.AddListener(FinishLoadMidMission);
    }



    public void FinishLoadMidMission()
    {
        foreach (InteractableObject inter in Area.Instance.GetComponentsInChildren<InteractableObject>(true))
        {
            string path = Utils.GetGameObjectPath(inter.gameObject);
            if (SaveFile.Instance.MidMissionInformation.GameObjectPathsToInteractables.ContainsKey(path))
            {
                inter.InteractedCount = SaveFile.Instance.MidMissionInformation.GameObjectPathsToInteractables[path].InteractCount;
                inter.transform.position = SaveFile.Instance.MidMissionInformation.GameObjectPathsToInteractables[path].Position;
                if (inter.GetComponent<SpriteRenderer>() != null)
                {
                    inter.GetComponent<SpriteRenderer>().sprite = SaveFile.Instance.MidMissionInformation.GameObjectPathsToInteractables[path].Sprite;
                }
                inter.gameObject.SetActive(SaveFile.Instance.MidMissionInformation.GameObjectPathsToInteractables[path].IsActive);
            }
            else
            {
                MonoBehaviour.Destroy(inter.gameObject);
            }
        }
        foreach (DestructibleEnvironment destr in Area.Instance.GetComponentsInChildren<DestructibleEnvironment>(true))
        {
            string path = Utils.GetGameObjectPath(destr.gameObject);
            if (SaveFile.Instance.MidMissionInformation.GameObjectPathsToDestructibles.ContainsKey(path))
            {
                destr.transform.position = SaveFile.Instance.MidMissionInformation.GameObjectPathsToDestructibles[path].Position;
                destr.gameObject.SetActive(SaveFile.Instance.MidMissionInformation.GameObjectPathsToDestructibles[path].IsActive);
            }
            else
            {
                MonoBehaviour.Destroy(destr.gameObject);
            }
        }
        foreach (Unit unit in Utils.GetAllUnits())
        {
            string path = Utils.GetGameObjectPath(unit.gameObject);
            if (unit != null && SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits.ContainsKey(path))
            {
                unit.gameObject.SetActive(SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits[path].IsActive);
                unit.transform.position = SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits[path].Position;
                if (SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits[path].IsActive && unit.gameObject.activeSelf && unit.gameObject.IsDestroyed() == false)
                {
                    if(unit.Animator == null || unit.Animator.enabled == false) {
                        WaitAndRunMethod(0.01f, FinishUnitLoad, unit);
                    }
                    else {
                        FinishUnitLoad(unit);
                    }
                }
            }
            else {
                MonoBehaviour.Destroy(unit.gameObject);
            }
        }
        if (SaveFile.Instance.MidMissionInformation.ClassesAndMethodsToExecute != null)
        {
            foreach (string class_and_method in SaveFile.Instance.MidMissionInformation.ClassesAndMethodsToExecute)
            {
                Type type = Type.GetType(class_and_method.Split(".")[0]);
                if (type == null)
                {
                    Debug.LogError("Could not find class: " + type);
                }
                else
                {
                    MethodInfo method = type.GetMethod(class_and_method.Split(".")[1], BindingFlags.Public | BindingFlags.Static);
                    if (method == null)
                    {
                        Debug.LogError("Could not find method: " + method);
                    }
                    else
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
        UIManager.Objects.HealingItemText.text = "+" + SaveFile.Instance.HealUpgrades.ToString();
        if(SaveFile.Instance.CurrentMission.CanBeFinishedByPressingButton) {
            UIManager.Objects.EscapeMissionButton.gameObject.SetActive(true);
            UIManager.Objects.EscapeMissionButtonLabel.SetLabel("{FinishMission}");
        }
        UIManager.Instance.ToggleLoadingScreen(false);
        if(SaveFile.Instance.CurrentObjectiveDisplayed != null) {
            Utils.ShowMissionObjective(SaveFile.Instance.CurrentObjectiveDisplayed);
        }
        NotificationController.ShowNotificationWithGraphic("LoadMidMission", "UI/Save");
        WaitAndRunMethod(0.01f, UpdatePlayerPostLoad);
    }

    public bool ShouldSaveAfterCombat = false;

    public void SaveAfterCombat() {
        if(Player.Instance.InCombat == false && ShouldSaveAfterCombat) {
            ShouldSaveAfterCombat = false;
            MakeAutoSave();
        }
    }

    public void TakeScreenshot(string path) {
        Camera cam = SceneManager.GetActiveScene().name == "MissionSelect" ? Utils.GetSceneRootObject("Mission Select").transform.Find("Camera").GetComponent<Camera>() : CameraController.Instance.Camera;

        if(SceneManager.GetActiveScene().name == "MissionSelect") {
            Utils.GetSceneRootObject("Mission Select").transform.Find("Pause Screen").gameObject.SetActive(false);
        }
        var renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        var texture2D = new Texture2D(Screen.width, Screen.height);

        var target = cam.targetTexture;
        cam.targetTexture = renderTexture;
        cam.Render();
        cam.targetTexture = target;

        var active = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = active;

        texture2D.Apply();
        System.IO.File.WriteAllBytes(path, texture2D.EncodeToJPG());
        FinishTakingScreenshot(path);
    }

    public void FinishTakingScreenshot(string path) {
        EventManager.FinishedTakingScreenshot.Invoke();
        if(SceneManager.GetActiveScene().name == "MissionSelect") {
            Utils.GetSceneRootObject("Mission Select").transform.Find("Pause Screen").gameObject.SetActive(true);
        }
    }

    public void FinishUnitLoad(Unit unit) {
        string path = Utils.GetGameObjectPath(unit.gameObject);
        unit.KnockedOut = SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits[path].KnockedOut;
        if(unit.Actions != null) {
            unit.Actions.IsFlipped = SaveFile.Instance.MidMissionInformation.GameObjectPathsToUnits[path].IsFlipped;
        }
        if (unit.KnockedOut)
        {
            DamageInstance.DeactivateUnit(unit);
        }
        unit.InCombat = false;
    }

    public void UpdatePlayerPostLoad()
    {
        Player.Instance.Actions.IsFlipped = SaveFile.Instance.MidMissionInformation.PlayerIsFlipped;
        Player.Instance.transform.position = SaveFile.Instance.MidMissionInformation.PlayerPosition;
        LoadingNewArea = false;
    }

    public void Start()
    {
        MonoBehaviour.DontDestroyOnLoad(gameObject);
        SpeechBeepClips = new();
        foreach (string vowel in new string[] { "FemaleA", "FemaleI", "FemaleU", "FemaleE", "FemaleO", "MaleA", "MaleI", "MaleU", "MaleE", "MaleO", "PlayerA", "PlayerI", "PlayerU", "PlayerE", "PlayerO" })
        {
            SpeechBeepClips.Add(vowel, Resources.Load("Sounds/Sound Effects/Dialogue/DialogueBeep" + vowel) as AudioClip);
        }
        Camera = transform.Find("Camera Container/Camera").GetComponent<Camera>();
        PlayerInput = GetComponent<PlayerInput>();
        PlayerControls = GetComponent<PlayerControls>();
        AudioSource = GetComponent<AudioSource>();
        GameplayMode = Constants.GameplayMode.OnStartScreen;
        Utils.GetSceneRootObject("Start Screen").Find("Screen/Version").GetComponent<TextMeshProUGUI>().text = "ver " + Application.version;
        AutoSaveSettings = false;
        if (!Settings.Instance.Load())
        {
            Settings.Instance.MasterVolume = 0.5f;
            Settings.Instance.SoundVolume = 0.5f; 
            Settings.Instance.MusicVolume = 0.5f;
            Settings.Instance.DialogueVolume = 0.5f;
            Settings.Instance.FieldOfView = 20f;
            Settings.Instance.DialogueTextSpeed = 10;
            Settings.Instance.Save();
            PlayerControls.SaveDefaultKeybindsToSettings();
        }
        else {
            PlayerControls.LoadSettingsKeybinds();
        }
        AutoSaveSettings = true;
    }


    public void DestroyAllAreas()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.tag = "Untagged";
        }
        foreach (GameObject area in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Where(gameObject => gameObject.CompareTag("Area")))
        {
            area.tag = "Untagged";
            Destroy(area);
        }
        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.CompareTag("Player") == false && obj != this.gameObject)
            {
                Destroy(obj.gameObject);
            }
        }
        Area.Instance = null;
    }

    public void QuitGame() {
        ShowConfirmModal(Label.Get("QuitPlayingConfirmation"), ConfirmedQuitGame);
    }

    public void ConfirmedQuitGame() {
        Application.Quit();
    }

    public void ReturnToTitle() {
        ShowConfirmModal(Label.Get("QuitPlayingConfirmation"), ConfirmedReturnToTitle);
    }

    public void ConfirmedReturnToTitle() {
        GameController.Instance.GameplayMode = Constants.GameplayMode.OnStartScreen;
    }

    public void ShowPauseScreenModal(string description, Action function_to_execute_on_confirm, string special_confirm_button_label = "")
    {
        ConfirmPromptActive = true;
        UIManager.Objects.PauseScreenConfirmPrompt.SetActive(true);
        UIManager.Objects.PauseScreenConfirmPromptDescription.text = description;
        FunctionToExecuteOnConfirm = function_to_execute_on_confirm;
        if(Settings.Instance.ControlScheme == "Gamepad")
        {
            UIManager.Objects.PauseScreenConfirmPromptConfirmButton.Select();
            EventManager.CancelButtonPressed.AddListener(ModalCanceled);
            EventManager.ExitMenu.AddListener(ModalCanceled);
        }
        UIManager.Objects.PauseScreenConfirmPromptConfirmButtonText.text = special_confirm_button_label == "" ? Label.Get("ButtonConfirm") : Label.Get(special_confirm_button_label);
    }

    public void PauseScreenModalConfirmed()
    {
        FunctionToExecuteOnConfirm.Invoke();
        UIManager.Objects.PauseScreenConfirmPrompt.SetActive(false);
        ConfirmPromptActive = false;
    }

    public void PauseScreenModalCanceled()
    {
        EventManager.CancelButtonPressed.RemoveListener(ModalCanceled);
        EventManager.ExitMenu.RemoveListener(ModalCanceled);
        UIManager.Objects.PauseScreenConfirmPrompt.SetActive(false);
        ConfirmPromptActive = false;
    }
    
public void ShowConfirmModal(string description, Action function_to_execute_on_confirm, string special_confirm_button_label = "")
    {
        ConfirmPromptActive = true;
        GameObject modal = Objects.ConfirmPrompt;
        if (modal == null)
        {
            Debug.LogError("ConfirmPrompt modal GameObject could not be found!");
            return;
        }
        modal.transform.SetAsLastSibling();
        modal.SetActive(true);
        Objects.ConfirmPromptDescription.text = description;
        FunctionToExecuteOnConfirm = function_to_execute_on_confirm;
        Button confirmBtn = Objects.ConfirmPromptConfirmButton;
        if (confirmBtn != null)
        {
            confirmBtn.onClick.RemoveAllListeners();
            confirmBtn.onClick.AddListener(ModalConfirmed);
            confirmBtn.Select();
        }
        Button cancelBtn = modal.transform.Find("Cancel Button")?.GetComponent<Button>()
                        ?? modal.transform.Find("Cancel")?.GetComponent<Button>()
                        ?? modal.GetComponentsInChildren<Button>(true).FirstOrDefault(b => b != confirmBtn);
        if (cancelBtn != null)
        {
            cancelBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.AddListener(ModalCanceled);
        }
        EventManager.CancelButtonPressed.RemoveListener(ModalCanceled);
        EventManager.CancelButtonPressed.AddListener(ModalCanceled);
        EventManager.ExitMenu.RemoveListener(ModalCanceled);
        EventManager.ExitMenu.AddListener(ModalCanceled);
        Objects.ConfirmPromptConfirmButtonText.text = special_confirm_button_label == "" 
            ? Label.Get("ButtonConfirm") 
            : Label.Get(special_confirm_button_label);
    }

    public void ModalConfirmed()
    {
        EventManager.CancelButtonPressed.RemoveListener(ModalCanceled);
        EventManager.ExitMenu.RemoveListener(ModalCanceled);
        Action actionToExecute = FunctionToExecuteOnConfirm;
        FunctionToExecuteOnConfirm = null;

        if (Objects.ConfirmPrompt != null)
        {
            Objects.ConfirmPrompt.SetActive(false);
        }
        ConfirmPromptActive = false;
        actionToExecute?.Invoke();
    }

    public void ModalCanceled()
    {
        EventManager.CancelButtonPressed.RemoveListener(ModalCanceled);
        EventManager.ExitMenu.RemoveListener(ModalCanceled);
        FunctionToExecuteOnConfirm = null;
        if (Objects.ConfirmPrompt != null)
        {
            Objects.ConfirmPrompt.SetActive(false);
        }
        ConfirmPromptActive = false;
    }

    public void AbandonMission() {
        if (SaveFile.Instance.CurrentMission != null && SaveFile.Instance.CurrentMission.NumberOfWeeksConsumed == 0) {
            SaveFile.Instance.CurrentMission.AbandonMission();
        }
        else {
            ShowConfirmModal(Label.Get("AbandonMissionConfirmation"), ConfirmedAbandonMission);
        }  
    }

    public void ConfirmedAbandonMission() {
        SaveFile.Instance.CurrentMission.AbandonMission();
    }

    public static class Objects
    {
        public static GameObject SaveAndLoadPanel => Utils.GetGameObject("Save or Load");
        public static GameObject DialogueLinesContainer => Utils.GetGameObject("Dialogue Window/Window/Scroll Rect/Viewport/Lines");
        public static GameObject DialogueBoxLeft => Utils.GetGameObject("Dialogue Window/Window/Left Portrait");
        public static GameObject DialogueBoxRight => Utils.GetGameObject("Dialogue Window/Window/Right Portrait");
        public static TextMeshProUGUI DialogueMoneyDisplayText => (TextMeshProUGUI)Utils.GetComponent("Dialogue Window/Money", typeof(TextMeshProUGUI));
        public static Slider DialogueExperienceBarSlider => (Slider)Utils.GetComponent("Dialogue Window/Level", typeof(Slider));
        public static TextMeshProUGUI DialogueExperienceBarLeftLevel => (TextMeshProUGUI)Utils.GetComponent("Dialogue Window/Level/Left Level", typeof(TextMeshProUGUI));
        public static TextMeshProUGUI DialogueExperienceBarRightLevel => (TextMeshProUGUI)Utils.GetComponent("Dialogue Window/Level/Right Level", typeof(TextMeshProUGUI));
        public static GameObject DialogueNotifications => Utils.GetGameObject("Dialogue Window/Window/List");
        public static GameObject DialogueArchive => Utils.GetGameObject("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content");
        public static GameObject TransitionScreen => Utils.GetGameObject("Transition Screen");
        public static CanvasGroup TransitionBlackScreen => (CanvasGroup)Utils.GetComponent("Transition Screen/Black Screen", typeof(CanvasGroup));
        public static GameObject TransitionLoadingScreen => Utils.GetGameObject("Transition Screen/Loading Screen");
        public static Slider TransitionLoadProgress => (Slider)Utils.GetComponent("Transition Screen/Loading Screen/Progress", typeof(Slider));
        public static TextMeshProUGUI TransitionUpperText => (TextMeshProUGUI)Utils.GetComponent("Transition Screen/Upper Text", typeof(TextMeshProUGUI));
        public static TextMeshProUGUI TransitionLowerText => (TextMeshProUGUI)Utils.GetComponent("Transition Screen/Lower Text", typeof(TextMeshProUGUI));
        public static GameObject TransitionAreaName => Utils.GetGameObject("Transition Screen/Area Name");
        public static GameObject Shop => Utils.GetGameObject("Shop");
        public static GameObject ShopItems => Utils.GetGameObject("Shop/Items/Viewport/Items");
        public static TextMeshProUGUI ShopMoneyText => (TextMeshProUGUI)Utils.GetComponent("Shop/Money", typeof(TextMeshProUGUI));
        public static AudioSource Music => (AudioSource)Utils.GetComponent("Music", typeof(AudioSource));
        public static AudioSource AudioListener => (AudioSource)Utils.GetComponent("Audio Listener", typeof(AudioSource));
        public static GameObject ConfirmPrompt => GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu ? Utils.GetGameObject("Menu/Confirm Prompt") : Utils.GetGameObject("UI/Pause Screen/Confirm Prompt");
        public static TextMeshProUGUI ConfirmPromptDescription => GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu ? (TextMeshProUGUI)Utils.GetComponent("Menu/Confirm Prompt/Description", typeof(TextMeshProUGUI)) : (TextMeshProUGUI)Utils.GetComponent("UI/Pause Screen/Confirm Prompt/Description", typeof(TextMeshProUGUI));
        public static Button ConfirmPromptConfirmButton => GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu ? (Button)Utils.GetComponent("Menu/Confirm Prompt/Confirm Button", typeof(Button)) : (Button)Utils.GetComponent("UI/Pause Screen/Confirm Prompt/Confirm Button", typeof(Button));
        public static TextMeshProUGUI ConfirmPromptConfirmButtonText => GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu ? (TextMeshProUGUI)Utils.GetComponent("Menu/Confirm Prompt/Confirm Button/Text", typeof(TextMeshProUGUI)) : (TextMeshProUGUI)Utils.GetComponent("UI/Pause Screen/Confirm Prompt/Confirm Button/Text", typeof(TextMeshProUGUI));
    }
}
