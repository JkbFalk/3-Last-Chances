using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Settings;

public class StartScreen : MonoBehaviour {

    public void InitializeStartScreen()
    {
        List<string> referencedSurvivalRuns = new();
        List<string> referencedMissionAttempts = new();
        transform.Find("Screen/Version").GetComponent<TextMeshProUGUI>().text = "ver " + Application.version;
        bool saveFileExists = false;
        bool quickSaveExists = ES3.FileExists("QuickSave.es3");
        if(quickSaveExists) {
            SaveFile sf = SaveFile.RetrieveSaveFile("QuickSave.es3");
            sf.SetSaveSlotValues(GameController.Instance.transform.Find("Save or Load/Viewport/Save Files/-3"));
            referencedSurvivalRuns.Add(sf.CurrentSurvivalRunId);
            referencedMissionAttempts.Add(sf.CurrentMissionAttemptId);
        }
        bool preMissionAutoSaveExists = ES3.FileExists("PreMissionAutoSave.es3");
        if(preMissionAutoSaveExists) {
            SaveFile sf = SaveFile.RetrieveSaveFile("PreMissionAutoSave.es3");
            sf.SetSaveSlotValues(GameController.Instance.transform.Find("Save or Load/Viewport/Save Files/-2"));
            referencedSurvivalRuns.Add(sf.CurrentSurvivalRunId);
            referencedMissionAttempts.Add(sf.CurrentMissionAttemptId);
        }
        bool midMissionAutoSaveExists = ES3.FileExists("MidMissionAutoSave.es3");
        if(midMissionAutoSaveExists) {
            SaveFile sf = SaveFile.RetrieveSaveFile("MidMissionAutoSave.es3");
            sf.SetSaveSlotValues(GameController.Instance.transform.Find("Save or Load/Viewport/Save Files/-1"));
            referencedSurvivalRuns.Add(sf.CurrentSurvivalRunId);
            referencedMissionAttempts.Add(sf.CurrentMissionAttemptId);
        }
        bool postMissionAutoSaveExists = ES3.FileExists("PostMissionAutoSave.es3");
        if(postMissionAutoSaveExists) {
            SaveFile sf = SaveFile.RetrieveSaveFile("PostMissionAutoSave.es3");
            sf.SetSaveSlotValues(GameController.Instance.transform.Find("Save or Load/Viewport/Save Files/0"));
            referencedSurvivalRuns.Add(sf.CurrentSurvivalRunId);
            referencedMissionAttempts.Add(sf.CurrentMissionAttemptId);
        }
        for(int i = 1; i < Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + 1; i++) {
            bool fileExists = ES3.FileExists("SaveFile_" + i + ".es3");
            if(fileExists) {
                saveFileExists = true;
                SaveFile sf = SaveFile.RetrieveSaveFile("SaveFile_" + i + ".es3");
                sf.SetSaveSlotValues(GameController.Instance.transform.Find("Save or Load/Viewport/Save Files/" + i));
                referencedSurvivalRuns.Add(sf.CurrentSurvivalRunId);
                referencedMissionAttempts.Add(sf.CurrentMissionAttemptId);
            }
        }
        RemovePreMissionSavesAndSurvivalRuns(referencedSurvivalRuns, referencedMissionAttempts);
        if(saveFileExists || postMissionAutoSaveExists || midMissionAutoSaveExists) {
            transform.Find("Screen/Buttons/Load").GetComponent<Button>().interactable = true;
            transform.Find("Screen/Buttons/Continue").GetComponent<Button>().interactable = true;
        }
        Settings.Instance.ControlScheme = Settings.Instance.ControlScheme;
    }

    public void OpenLoadPanel() {
        GameController.Instance.ToggleLoadPanel(true);
    }

    public void OpenStartScreenOptions() {
        MenuManager.Instance.transform.Find("Options Window/StartScreenOptions").gameObject.SetActive(true);
        MenuManager.Instance.transform.Find("Options Window/Options/Gameplay/Items/Difficulty").gameObject.SetActive(false);
        Utils.SetActiveOnCanvasGroup(Utils.CanvasType.StartScreen, false);
        GameController.Instance.PlayerControls.OnOpenOptionsMenuButtonPress();
    }

    public void RemovePreMissionSavesAndSurvivalRuns(List<string> survival_runs_not_to_delete, List<string> mission_attempts_not_to_delete) {
        var dir = new DirectoryInfo(Application.persistentDataPath);
        /*foreach (var file in dir.EnumerateFiles("PreMissionAutoSave-*.es3")) {
            if(mission_attempts_not_to_delete.Contains(file.Name.Replace("PreMissionAutoSave-", "").Replace(".es3", "")) == false) {
                file.Delete();
            }
        }*/
        foreach (var file in dir.EnumerateFiles("Survival_*.es3")) {
            if(file.Name != "Survival_StartScreen.es3" && survival_runs_not_to_delete.Contains(file.Name.Replace("Survival_", "").Replace(".es3", "")) == false) {
                file.Delete();
            }
        }
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ChooseSurvivalType(int option) {
        GameController.Instance.ChooseSurvivalType(option);
    }

    public void StartSurvivalMode()
    {
        SurvivalController.StoryModeSurvival = false;
        transform.Find("Survival Type Selection").gameObject.SetActive(true);
    }

    public void ContinueSurvivalMode()
    {
        GameController.Instance.CurrentSaveFile = SaveFile.RetrieveSaveFile("Survival_StartScreen.es3");
        SurvivalController.LoadStageWithoutRewards = true;
        SurvivalController.StartSurvivalMode();
        GameController.Instance.CurrentSaveFile.Load();
    }

    public void CancelSurvivalMode()
    {
        if(SurvivalController.StoryModeSurvival) {
            MenuManager.Instance.ShowTransitionIntoMissionSelect();
            Utils.GetSceneRootObject("Mission Select").transform.Find("Survival Type Selection").gameObject.SetActive(false);
        }
        else {
            transform.Find("Survival Type Selection").gameObject.SetActive(false);
        }
    }

    public void CancelStoryMode()
    {
        Utils.GetSceneRootObject("First-time Launch").gameObject.SetActive(false);
    }

    public void ToggleArenaSandboxSelection(bool open_arena_selection)
    {
        transform.Find("Screen").gameObject.SetActive(!open_arena_selection);
        transform.Find("Sandbox Arena").gameObject.SetActive(open_arena_selection);
        if (Settings.Instance.ControlScheme == "Gamepad" && open_arena_selection)
        {
            transform.Find("Sandbox Arena/Back Button").GetComponent<Button>().Select();
        }
        else if (Settings.Instance.ControlScheme == "Gamepad")
        {
            transform.Find("Screen/Buttons/SandboxArena").GetComponent<Button>().Select();
        }
    }

    public void ContinueGame() {
        SaveFile newestSf = null;
        if(ES3.FileExists("AutoSave.es3")) {
            newestSf = SaveFile.RetrieveSaveFile("AutoSave.es3");
        }
        for(int i = 1; i < Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + 1; i++) {
            if(ES3.FileExists("SaveFile" + i + ".es3")) {
                SaveFile sf = SaveFile.RetrieveSaveFile("SaveFile" + i + ".es3");
                if(newestSf == null || newestSf.SavedTimeStamp < sf.SavedTimeStamp) {
                    newestSf = sf;
                }
            }
        }
        if(newestSf != null) {
            newestSf.Load();
        }
    }

    public void StartGame() {
        Utils.GetSceneRootObject("First-time Launch").gameObject.SetActive(true);
        if(Settings.Instance.ControlScheme == "Gamepad") {
            Utils.GetSceneRootObject("First-time Launch").Find("Combat Type/Choices/Choices/Challenge/Select").GetComponent<UnityEngine.UI.Button>().Select();
        }
    }

    public void StartChallenge(string stage_name) {
        /*
        if(generate_new_save_file) {
            GameController.Instance.CurrentSaveFile = new SaveFile();
        }
        ((Quest_TenStages)SaveFile.Instance.GetQuest(typeof(Quest_TenStages))).ShowUnlockInfo = show_selection;
        Quest_TenStages quest = (Quest_TenStages)SaveFile.Instance.GetQuest(typeof(Quest_TenStages));
        quest.StartQuest((stage_number - 1) * 10);
        for (int i = 0; i < stage_number - 2; i++)
        {
            Utils.AddPassive(Settings.Instance.Passives[i].Passive1);
            Utils.AddPassive(Settings.Instance.Passives[i].Passive2);
        }
        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities)
        {
            ability.AbilityGraphic.transform.Find("Disabled").gameObject.SetActive(!SaveFile.Instance.UnlockedAbilities.Contains(ability.Type));
        }
        Player.Instance.transform.position = Utils.GetPlayerStartPositionForArea(Player.Instance.CurrentArea);
        if(generate_new_save_file) {
            SaveFile.Instance.InitializeSaveFile();
            SaveFile.Instance.Level = 5;
        }
        SaveFile.Instance.GameType = Constants.GameType.Challenge;
        SaveFile.Instance.GetItem(typeof(Tool_HealthPotion), Item.ItemGrade.Regular).Amount = SaveFile.Instance.ExtraPotions ? 5 : 3;
        SaveFile.Instance.GetItem(typeof(Tool_VacuumGrenade), Item.ItemGrade.Regular).Amount = SaveFile.Instance.ExtraGrenades ? 4 : 2;
        GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;*/
    }

    public void SelectDifficultyAndStartGame(int index)
    {
        GameController.Instance.CurrentSaveFile = new SaveFile("Story", SaveFile.SaveFileTypeEnum.Story);
        SaveFile.Instance.GameType = Constants.GameType.Story;
        SaveFile.Instance.Difficulty = index == 0 ? Constants.Difficulty.Story : index == 1 ? Constants.Difficulty.Regular : index == 2 ? Constants.Difficulty.Challenge : Constants.Difficulty.Ultimate;
        if(!Settings.Instance.SkipPrologue) {
            EventManager.FinishedLoadingArea.AddListener(Mission_FirstReturn.OnEnterArea);
            Utils.MoveIntoArea(true, "FirstReturn");
            SaveFile.Instance.CurrentMission = new Mission_FirstReturn();
            SaveFile.Instance.CurrentMission.OnStart();
        }        
        SaveFile.Instance.InitializeSaveFile(false);
        if(Settings.Instance.SkipPrologue) {
            SaveFile.Instance.FinishedPrologue = true;
            MenuManager.Instance.ShowTransitionIntoMissionSelect();
        }
        Utils.GetSceneRootObject("First-time Launch").gameObject.SetActive(false);
    }
}