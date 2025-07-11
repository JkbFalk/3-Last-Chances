using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public abstract class Mission
{
    public enum MissionType {Activity, MainQuest, SideQuest, Exploration, Random}

    public MissionType Type;

    private int _missionProgress = 0;
    public int MissionProgress {
        get => _missionProgress;
        set {
            int prev_value = _missionProgress;
            _missionProgress = value;
            if(prev_value != value) {
                OnMissionProgressUpdated();
            }
        }
    }
    public string MapMarker= "Town_Capital";
    public int NumberOfWeeksConsumed = 1;
    public int WeeksUntilExpiryRemaining = 0;
    public int WeeksUntilExpiryMax = 9;
    public int FirstAppearsOnWeek = 0;
    public bool CanExpire = true;
    public bool CanBeFinishedByPressingButton = false;
    public string MusicOnStart;
    public bool CanAbandonMission = true;
    public bool RemoveOtherMissions = false;
    public bool AlwaysShow = false;
    public bool AutoSaveAfterCombat = true;
    public GameObject MissionSelectGameObject;
    public string Icon = "UI/UI Quests";
    public bool IsNew {
        get {
            return Type != MissionType.Activity && Type != MissionType.Exploration && WeeksUntilExpiryMax > 0 && WeeksUntilExpiryRemaining == WeeksUntilExpiryMax;
        }
    }
    public int WeekAdded = 0;

    [SerializeField]
    public int _enemyLevel = 0;
    public int EnemyLevel {
        get => _enemyLevel;
        set {
            _enemyLevel = value < 1 ? 1 : value;
        }
    }
    public int MoneyReward = 0;
    public int ExperienceReward = 0;
    public bool IgnoreRemoveAllMissions = false;

    public virtual bool MissionShouldBeAvailable() {
        return true;
    }

    public virtual void OnMissionProgressUpdated() {

    }

    public virtual void OnStart() {
        Utils.DestroyAllChildren(MenuManager.Objects.MenuArchive.transform);
        Utils.DestroyAllChildren(GameController.Objects.DialogueArchive.transform);
        UIManager.Objects.EscapeMissionButton.gameObject.SetActive(CanAbandonMission);
        UIManager.Objects.EscapeMissionButtonLabel.SetLabel(NumberOfWeeksConsumed == 0 ? "{FinishMission}" : "{AbandonMission}");
        if(AutoSaveAfterCombat) {
            EventManager.FinishedLoadingArea.AddListener(SaveOnMissionStart);
        }
    }

    public void SaveOnMissionStart() {
        GameController.Instance.MakeAutoSave();
    }

    public void MakeMissionFinishableUsingButton() {
        UIManager.Objects.EscapeMissionButton.gameObject.SetActive(true);
        UIManager.Objects.EscapeMissionButtonLabel.SetLabel("{FinishMission}");
        SaveFile.Instance.CurrentMission.CanBeFinishedByPressingButton = true;
        SaveFile.Instance.Week += SaveFile.Instance.CurrentMission.NumberOfWeeksConsumed;
        SaveFile.Instance.CurrentMission.NumberOfWeeksConsumed = 0;
    }

    public virtual void OnEnd() {  
        RemoveTemporaryQuestItems();
        RemoveMidMissionSave();
    }

    public void RemoveTemporaryQuestItems() {
        foreach(Item item in SaveFile.Instance.Inventory.Where(item => item.RemoveAtEndOfMission).ToArray()) {
            SaveFile.Instance.RemoveItem(item);
        }
    }

    public void RemoveMidMissionSave() {
        if(ES3.FileExists("MidMission_" + SaveFile.Instance.Id + ".es3")) {
            ES3.DeleteFile("MidMission_" + SaveFile.Instance.Id + ".es3");
        }
        SaveFile.Instance.MidMissionInformation = null;
    }

    public virtual void OnMissionMadeAvailable() {}

    public Action ActionExecutedOnGameOver = new Action(() => {
        if(SaveFile.Instance.MidMissionInformation != null) {
            GameController.Instance.LoadMidMissionAutoSave();
        }
        else {
            SaveFile.Instance.CurrentMission.AbandonMission();
        }
    });

    public void Finish(bool instant_transition = false) {
        if(SaveFile.Instance.CurrentMission.Type == MissionType.MainQuest || SaveFile.Instance.CurrentMission.Type == MissionType.SideQuest) {
            SaveFile.Instance.CompletedStoryMissions.Add(SaveFile.Instance.CurrentMission.GetType());
            SaveFile.Instance.Missions.Remove(SaveFile.Instance.CurrentMission);
        }
        OnEnd();
        if(NumberOfWeeksConsumed > 0) {
            SaveFile.Instance.Week += NumberOfWeeksConsumed;
            if(SaveFile.Instance.Week > 51) {
                SaveFile.Instance.Week = 51;
            }
        }
        MenuManager.Instance.RemoveMission(this);
        SaveFile.Instance.CurrentMission = null;
        MenuManager.Instance.ShowTransitionIntoMissionSelect(instant_transition);
        MenuManager.Instance.Rewards = GetRewards();
        MenuManager.Instance.MoneyReward = MoneyReward;
        MenuManager.Instance.ExperienceReward = ExperienceReward;
        SaveFile.Instance.MidMissionInformation = null;
        GameController.Instance.MakeAutoSave(false);
    }

    public static Type GetRandomMission() {
        List<Type> possibleMissionTypes = new List<Type> {typeof(Mission_EliminateGroup), typeof(Mission_TimeAttack), typeof(Mission_Destructibles)};
        if(SaveFile.Instance.Level > 1) {
            possibleMissionTypes.Add(typeof(Mission_EliminateElites));
            possibleMissionTypes.Add(typeof(Mission_CompleteSurvival));
            //possibleMissionTypes.Add(typeof(Mission_CompleteChallenge));
            //possibleMissionTypes.Add(typeof(Mission_CombatExploration));
            //Arena: fight stronger and stronger enemies, lose everything upon dying (can go back at any point)
        }
        if(SaveFile.Instance.Level > 1 && SaveFile.Instance.Missions.FirstOrDefault(m => m.GetType() == typeof(Mission_EliminateBoss)) == null) {
            possibleMissionTypes.Add(typeof(Mission_EliminateBoss));
            //possibleMissionTypes.Add(typeof(Mission_Race));
        }
        return possibleMissionTypes[UnityEngine.Random.Range(0, possibleMissionTypes.Count)];
    }

    public void AbandonMission() {
        if(CanAbandonMission == false) {
            return;
        }
        if(NumberOfWeeksConsumed == 0) {
            Finish(true);
        }
        else {
            OnEnd();
            float secondsPlayed = SaveFile.Instance.TimePlayedInSeconds;
            SaveFile newestSf = null;
            if(ES3.FileExists("PreMissionAutoSave-" + SaveFile.Instance.CurrentMissionAttemptId + ".es3")) {
                newestSf = SaveFile.RetrieveSaveFile("PreMissionAutoSave-" + SaveFile.Instance.CurrentMissionAttemptId + ".es3");
            }
            if(newestSf != null) {
                newestSf.Load();
                newestSf.TimePlayedInSeconds = secondsPlayed;
            }
            else {
                Debug.LogError("Could not find PreMissionAutoSave for SaveId " + SaveFile.Instance.Id + " and AttemptId " + SaveFile.Instance.CurrentMissionAttemptId);
                GameController.Instance.GameplayMode = Constants.GameplayMode.OnStartScreen;
            }
        }
    }

    public virtual string GetTitleLabel() {
        return GetType() + "_Description";
    }

    public virtual List<String> GetTitleParameters() { return null;}

    public virtual string GetDescriptionLabel() {
        return GetType() + "_Description";
    }

    public virtual List<String> GetDescriptionParameters() { return null;}

    public Color GetClockColor() {
        return (Type == Mission.MissionType.MainQuest && WeeksUntilExpiryRemaining == 1 ? Color.red : Type == Mission.MissionType.MainQuest && WeeksUntilExpiryRemaining < 4 ? Colors.GetColorFromCode("#FFC800") : Type == Mission.MissionType.SideQuest && WeeksUntilExpiryRemaining == 1 ? Colors.GetColorFromCode("#FFC800") : Color.white);
    }

    public List<MissionReward> GetRewards() {
        List<MissionReward> rewards = new();
        if(ExperienceReward > 0) {
            rewards.Add(new MissionReward() {Amount = Utils.GetCalculatedGain(ExperienceReward), Graphic = "UI/Experience", Name = "MissionReward_Experience"});
        }
        if(MoneyReward > 0) {
            rewards.Add(new MissionReward() {Amount = Utils.GetCalculatedGain(MoneyReward)});
        }
        rewards.AddRange(ExtraRewards);
        return rewards;
    }

    public List<MissionReward> ExtraRewards = new();

    public static int CalculateMoneyReward(int level, float multiplier) {
        return (int)(multiplier * UnityEngine.Random.Range(75 + level * 1.5f, 125 + level * 2.5f)) * 100;

    }

    public static int CalculateExperienceReward(int level, float multiplier) {
        return (int)(multiplier * UnityEngine.Random.Range(8 + level * 3, 12 + level * 5)) * 100;
    }

    [Serializable]
    public class MissionReward {
        public int Amount = 1;
        public string Graphic = "UI/Money";
        public string Name = "MissionReward_Gold";
        public Type Item;
        public Item.ItemGrade Rarity;
        public string ClassAndMethodToExecute;
    }

    public static Item.ItemGrade GetItemRarityForLevel(int enemy_level) {
        Dictionary<Item.ItemGrade, int> chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>();
        if(enemy_level < 5) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Regular, 100}, {Item.ItemGrade.Excellent, 10}};
        }
        else if(enemy_level < 10) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Regular, 100}, {Item.ItemGrade.Excellent, 30}, {Item.ItemGrade.Masterful, 5}};
        }
        else if(enemy_level < 15) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Regular, 100}, {Item.ItemGrade.Excellent, 50}, {Item.ItemGrade.Masterful, 15}};
        }
        else if(enemy_level < 20) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Regular, 100}, {Item.ItemGrade.Excellent, 80}, {Item.ItemGrade.Masterful, 30}};
        }
        else if(enemy_level < 25) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Excellent, 100}, {Item.ItemGrade.Masterful, 50}, {Item.ItemGrade.Flawless, 5}};
        }
        else if(enemy_level < 30) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Excellent, 100}, {Item.ItemGrade.Masterful, 80}, {Item.ItemGrade.Flawless, 15}};
        }
        else if(enemy_level < 35) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Masterful, 100}, {Item.ItemGrade.Flawless, 25}};
        }
        else if(enemy_level < 40) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Masterful, 100}, {Item.ItemGrade.Flawless, 40}};
        }
        else if(enemy_level < 45) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Masterful, 100}, {Item.ItemGrade.Flawless, 60}, {Item.ItemGrade.Ultimate, 5}};
        }
        else if(enemy_level < 50) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Masterful, 100}, {Item.ItemGrade.Flawless, 80}, {Item.ItemGrade.Ultimate, 15}};
        }
        else if(enemy_level < 55) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Flawless, 100}, {Item.ItemGrade.Ultimate, 30}};
        }
        else if(enemy_level < 60) {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Flawless, 100}, {Item.ItemGrade.Ultimate, 50}};
        }
        else {
            chance_for_each_rarity = new Dictionary<Item.ItemGrade, int>() {{Item.ItemGrade.Ultimate, 100}};
        }
        int random = UnityEngine.Random.Range(0, 100);
        foreach(Item.ItemGrade rarity in new List<Item.ItemGrade> {Item.ItemGrade.Ultimate, Item.ItemGrade.Flawless, Item.ItemGrade.Masterful, Item.ItemGrade.Excellent, Item.ItemGrade.Regular}) {
            if(chance_for_each_rarity.ContainsKey(rarity) && random < chance_for_each_rarity[rarity]) {
                return rarity;
            }
        }
        return Item.ItemGrade.Regular;
    }

    public virtual void OnFinishedLoadingArea() {

    }
}
