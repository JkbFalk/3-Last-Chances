using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.Playables;

public class SaveFile
{
    public static SaveFile Instance
    {
        get
        {
            return GameController.Instance.CurrentSaveFile;
        }
    }
    public string GameVersion = Application.version;

    public enum SaveFileTypeEnum { Story, Challenge, Survival};
    public SaveFileTypeEnum SaveFileType = SaveFileTypeEnum.Story;

    public Constants.Difficulty Difficulty;

    public Dictionary<int, int> WeeksAndInvestments = new Dictionary<int, int>();

    public List<string> SurvivalPowerUps = new List<string>();
    public string SaveFileNumber;
    public DateTime SavedTimeStamp;
    public int SurvivalLevel = 1;
    public int SurvivalChancesRemaining = 3;
    public string CurrentSurvivalRunId;
    public string CurrentMissionAttemptId;
    public string Id;
    public bool FinishedPrologue = false;
    public bool SkillTreeSurvivalType = true;
    public bool CurrentAreaLoadedFromSave = false;
    public int IrisTalksCompleted = 0;
    public int IrisMemoryPiecesFound = 0;
    public List<string> ReceivedExperienceFromInteractables = new();

    [DoNotSerialize]
    public float GlobalEnemyDamageModifier {
        get {
            switch(DifficultyLevel) {
                case 0: return 0.8f;
                case 1: return 0.9f;
                case 2: return 1f;
                case 3: return 1.1f;
                default: return 1;
            }
        }
    }

    [DoNotSerialize]
    public float GlobalEnemySurvivabilityModifier {
        get {
            switch(DifficultyLevel) {
                case 0: return 0.9f;
                case 1: return 0.95f;
                case 2: return 1f;
                case 3: return 1.35f;
                default: return 1;
            }
        }
    }

    public List<Type> UnlockedTools = new();
    public Dictionary<Type, Item.ItemGrade> ToolGrades = new() {
        {typeof(Tool_Caltrops), Item.ItemGrade.Regular},
        {typeof(Tool_CrimsonFeather), Item.ItemGrade.Regular},
        {typeof(Tool_ExpertBadge), Item.ItemGrade.Regular},
        {typeof(Tool_FireStarter), Item.ItemGrade.Regular},
        {typeof(Tool_FortificationPotion), Item.ItemGrade.Regular},
        {typeof(Tool_Sanctuary), Item.ItemGrade.Regular},
        {typeof(Tool_SerenityNeedle), Item.ItemGrade.Regular},
        {typeof(Tool_StunGrenade), Item.ItemGrade.Regular},
        {typeof(Tool_VacuumGrenade), Item.ItemGrade.Regular},
        {typeof(Tool_IceCoating), Item.ItemGrade.Regular},
        {typeof(Tool_ThoughtAccelerator), Item.ItemGrade.Regular},
        {typeof(Tool_FirstAidKit), Item.ItemGrade.Regular},
        {typeof(Tool_Decoy), Item.ItemGrade.Regular},
        {typeof(Tool_VeilOfShadows), Item.ItemGrade.Regular},
        {typeof(Tool_VialOfPoison), Item.ItemGrade.Regular}
    };
    public Dictionary<Type, int> ToolMaxAmounts = new() {
        {typeof(Tool_Caltrops), 1},
        {typeof(Tool_CrimsonFeather), 1},
        {typeof(Tool_ExpertBadge), 1},
        {typeof(Tool_FireStarter), 1},
        {typeof(Tool_FortificationPotion), 1},
        {typeof(Tool_Sanctuary), 1},
        {typeof(Tool_SerenityNeedle), 1},
        {typeof(Tool_StunGrenade), 1},
        {typeof(Tool_VacuumGrenade), 1},
        {typeof(Tool_IceCoating), 1},
        {typeof(Tool_ThoughtAccelerator), 1},
        {typeof(Tool_FirstAidKit), 1},
        {typeof(Tool_Decoy), 1},
        {typeof(Tool_VeilOfShadows), 1},
        {typeof(Tool_VialOfPoison), 1}
    };
    public Dictionary<Type, int> ToolRemainingAmounts = new() {
        {typeof(Tool_Caltrops), 1},
        {typeof(Tool_CrimsonFeather), 1},
        {typeof(Tool_ExpertBadge), 1},
        {typeof(Tool_FireStarter), 1},
        {typeof(Tool_FortificationPotion), 1},
        {typeof(Tool_Sanctuary), 1},
        {typeof(Tool_SerenityNeedle), 1},
        {typeof(Tool_StunGrenade), 1},
        {typeof(Tool_VacuumGrenade), 1},
        {typeof(Tool_IceCoating), 1},
        {typeof(Tool_ThoughtAccelerator), 1},
        {typeof(Tool_FirstAidKit), 1},
        {typeof(Tool_Decoy), 1},
        {typeof(Tool_VeilOfShadows), 1},
        {typeof(Tool_VialOfPoison), 1}
    };

    public void IncreaseEnergyLevel(Ability.AbilityFamily family) {

    }

    public void AddPermanentPowerUp(string power_up_name) {
        SaveFile.Instance.PermanentPowerUps.Add(power_up_name);
        List<Effect> effects = PassivePowerUpTile.GetPassivePowerUpEffects(power_up_name);
        foreach(Effect e in effects) {
            Player.Instance.AddEffect(e);
        }
    }

    public string SaveFilePath;
    public List<Mission> Missions = new List<Mission>();
    [SerializeField]
    private Mission _currentMission;
    public Mission CurrentMission {
        get => _currentMission;
        set {
            Utils.CreateAuditLog("Changing current mission: " + _currentMission + " -> " + value);
            _currentMission = value;
        }
    }

    public QuestObjective CurrentObjectiveDisplayed;

    public int GoldFromInvestmentsMinimum = 5000;
    public int GoldFromInvestmentsMaximum = 15000;
    public int ExtraGoldFromInvestmentsStartingNextCycle = 0;

    public List<string> PermanentPowerUps = new List<string>();
    public List<string> PowerUpsRemovedOnNextCycle = new List<string>();
    [SerializeField]
    public MidMissionInformation MidMissionInformation;
    public List<Type> FoundItemTypes = new List<Type>();

    public List<Type> AnimaAbilitiesCountered = new();
    public float IgnisEnergy = 0;
    public int IgnisManorOnFire_VictimsSavedCounter = 0;

    public int MaxHealCharges = 0;
    [SerializeField]
    private int _healUpgrades = 0;
    public int HealUpgrades {
        get => _healUpgrades;
        set {
            int prev_val = _healUpgrades;
            _healUpgrades = value;
            if(prev_val != _healUpgrades) {
                NotificationController.ShowNotificationWithGraphic("HealUpgradeNotification", "UI/Heal", new List<string> {(value-1).ToString(), value.ToString()});
                CanvasElements.UICanvas.Items.transform.Find("Heal/Upgrade").GetComponent<TextMeshProUGUI>().text = "+" + _healUpgrades.ToString();
                Utils.PlaySoundEffect(Player.Instance.AudioSource, "Generic/PowerUp1", 1f);
                if(DifficultyLevel >= 1) {
                    MaxHealCharges = 
                    HealUpgrades >= 15 ? 10 : 
                    HealUpgrades >= 13 ? 9 : 
                    HealUpgrades >= 11 ? 8 : 
                    HealUpgrades >= 9 ? 7 : 
                    HealUpgrades >= 7 ? 6 : 
                    HealUpgrades >= 5 ? 5 : 
                    HealUpgrades >= 3 ? 4 : 
                    HealUpgrades >= 1 ? 3 : 2;
                }
                else {
                    MaxHealCharges = 10;
                }
            }
        }
    }
    [SerializeField]
    private int _healChargesRemaining = 0;
    public int HealChargesRemaining {
        get => _healChargesRemaining;
        set {
            _healChargesRemaining = value < 0 ? 0 : value > SaveFile.Instance.MaxHealCharges ? SaveFile.Instance.MaxHealCharges : value;
            CanvasElements.UICanvas.Items.transform.Find("Heal/Uses/Text").GetComponent<TextMeshProUGUI>().text = _healChargesRemaining.ToString() + "/" + SaveFile.Instance.MaxHealCharges;
            CanvasElements.UICanvas.Items.transform.Find("Heal/Disabled").gameObject.SetActive(_healChargesRemaining <= 0);
        }
    }
    public int DifficultyLevel {
        get {
            return 
            Difficulty == Constants.Difficulty.Story ? 0 :
            Difficulty == Constants.Difficulty.Regular ? 1 :
            Difficulty == Constants.Difficulty.Challenge ? 2 :
            Difficulty == Constants.Difficulty.Ultimate ? 3 : -1;
        }
        set {}
    }

    public float TimePlayedInSeconds;

    public List<string> Flags = new List<string>();

    public void AddFlag(string flag) {
        string formatted_flag = Utils.GetFormattedFlag(flag);
        if(Flags.Contains(formatted_flag)) {
            return;
        }
        Flags.Add(formatted_flag);
        Utils.CreateAuditLog("Added flag: " + formatted_flag);
        RefreshFlagBehaviours();
        if(DebugController.WorldspaceDebugEnabled) {
            NotificationController.ShowTextNotification("Added Flag: " + formatted_flag);
        }
    }
    public void RemoveFlag(string flag) {
        string formatted_flag = Utils.GetFormattedFlag(flag);
        if(Flags.Contains(formatted_flag)) {
            return;
        }
        Flags.Remove(formatted_flag);
        Utils.CreateAuditLog("Removed flag: " + formatted_flag);
        RefreshFlagBehaviours();
        if(DebugController.WorldspaceDebugEnabled) {
            NotificationController.ShowTextNotification("Removed Flag: " + formatted_flag);
        }
    }

    public void RefreshFlagBehaviours() {
        List<GameObject> refreshedObjects = new();
        if(Area.Instance == null) {
            return;
        }
        foreach(FlagBehaviour fb in Area.Instance.GetComponentsInChildren<FlagBehaviour>(true)) {
            if(!refreshedObjects.Contains(fb.gameObject)) {
                refreshedObjects.Add(fb.gameObject);
                FlagBehaviour[] fbs = fb.GetComponents<FlagBehaviour>().OrderBy(fb => fb.FlagPriority).ToArray();
                foreach(FlagBehaviour fb2 in fbs) {
                    fb2.PerformFlagBehaviour();
                }
            }
        }
    }

    public bool HasFlag(string flag) {
        return Flags.Contains(Utils.GetFormattedFlag(flag));
    }

    public bool DoesNotHaveFlag(string flag) {
        return !Flags.Contains(Utils.GetFormattedFlag(flag));
    }

    public int Cycle = 1;
    [SerializeField]
    private int _week = 1;
    public int Week {
        get => _week;
        set {
            Utils.CreateAuditLog("Changing game week (Cycle " + SaveFile.Instance.Cycle + ") from " + SaveFile.Instance._week + " to " + value );
            int weeksPassed = value - _week;
            bool hasSurvival = SaveFile.Instance.Missions.FirstOrDefault(m => m.GetType() == typeof(Mission_CompleteSurvival)) != null;
            for(int i = 0; i < weeksPassed; i++) {
                Type mission1 = Mission.GetRandomMission();
                MenuManager.Instance.AddMission(mission1);
                Type mission2;
                do {
                    mission2 = Mission.GetRandomMission();
                } while (mission2 == mission1 || (hasSurvival && mission2.GetType() == typeof(Mission_CompleteSurvival)));
                MenuManager.Instance.AddMission(mission2);
                foreach(Mission mission in SaveFile.Instance.Missions) {
                    if(mission.WeeksUntilExpiryRemaining > 0) {
                        mission.WeeksUntilExpiryRemaining --;
                    }
                }
            }
            _week = value;
            MenuManager.Instance.UpdateMissionList();
            if(GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect && SceneManager.GetActiveScene().name == "MissionSelect") {
                Utils.GetSceneRootObject("Mission Select").Find("Week").GetComponent<LabelInitializer>().SetLabel(String.Format(Label.Get("SaveFileWeek"), new string[] {_week.ToString()}));
            }
        }
    }

    public void CheckIfShouldPerformTimeSensitiveEvent() {
        for(int i = 10; i < 51; i+=10) {
            if(Week >= i && DoesNotHaveFlag("ReceivedLauraMoney_Week" + i + "_" + Cycle)) {
                AddFlag("ReceivedLauraMoney_Week" + i + "_" + Cycle);
                Money += 15000 + 750 * i;
                NotificationController.ShowNotificationWithGraphic("LauraPocketMoneyNotification", "UI/Money", new() {Utils.GetFormattedFloat(15000 + 750 * i)});
            }
        }
        if(Cycle == 1 && Week > 40 && SaveFile.Instance.CompletedStoryMissions.Contains(typeof(Mission_Ignis1)) && SaveFile.Instance.DoesNotHaveFlag("Ignis_LearnedAboutVolcanoEruption")) {
            UIManager.Instance.StartDialogue(Mission_Ignis1.LearnAboutVolcanoEruption());
        }
    }

    [SerializeField]
    private Constants.GameType _gameType;
    public Constants.GameType GameType {
        get => _gameType;
        set {
            Utils.CreateAuditLog("Game Type updated: " + _gameType + " -> " + value);
            _gameType = value;
            MenuManager.Instance.transform.Find("Skill Tree Window/Disabled").gameObject.SetActive((SkillTreeSurvivalType == false && _gameType == Constants.GameType.Survival) || _gameType == Constants.GameType.Challenge);
        }
    }
    public class SavedItem {
        public Type Type;
        public int Amount;
        public Item.ItemGrade Rarity;
        public bool IsEquipped;
        public bool IsEquippedToSlot1;
        public bool IsEquippedToSlot2;
    }
    [SerializeField]
    public List<SavedItem> SavedItems = new List<SavedItem>();
    [NonSerialized]
    public List<Item> Inventory = new List<Item>();
    public List<Quest> Quests = new List<Quest>();
    public List<Stance> Stances;
    [SerializeField]
    private int _maxUpgradePoints = 0;
    public int MaxUpgradePoints {
        get => _maxUpgradePoints;
        set {
            _maxUpgradePoints = value;
            MenuManager.Instance.transform.Find("Skill Tree Window/Counters/Displays/Technique Upgrades/Amount").GetComponent<TextMeshProUGUI>().text = SaveFile.Instance.MaxUpgradePoints.ToString();
        }
    }
    public int UsedUpgradePoints = 0;
    [SerializeField]
    private int _maxPassivePowerUps = 0;
    public int MaxPassivePowerUps {
        get => _maxPassivePowerUps;
        set {
            _maxPassivePowerUps = value;
            MenuManager.Instance.transform.Find("Skill Tree Window/Counters/Displays/Power-up Points/Amount").GetComponent<TextMeshProUGUI>().text = (SaveFile.Instance.MaxPassivePowerUps - UsedPassivePowerUps).ToString();
        }
    }
    public int UsedPassivePowerUps = 0;
    [SerializeField]
    private int _maxUltimateUsesPerCombat = 0;
    public int MaxUltimateUsesPerCombat {
        get => _maxUltimateUsesPerCombat;
        set {
            _maxUltimateUsesPerCombat = value;
            MenuManager.Instance.transform.Find("Skill Tree Window/Counters/Displays/Ultimate Uses/Amount").GetComponent<TextMeshProUGUI>().text = _maxUltimateUsesPerCombat.ToString();
        }
    }
    [SerializeField]
    private int _ultimatesUsedInCurrentCombat;
    public int UltimatesUsedInCurrentCombat {
        get => _ultimatesUsedInCurrentCombat;
        set {
            _ultimatesUsedInCurrentCombat = value;
            int regularTiles = MaxUltimateUsesPerCombat - UltimatesUsedInCurrentCombat;
            for(int i = 0; i < Constants.MAX_ULTIMATE_USES_POSSIBLE; i++) {
                CanvasElements.UICanvas.UltimateUses.transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (i < regularTiles ? "UltimateCanBeUsed" : "UltimateCannotBeUsed"), typeof(Sprite)) as Sprite; 
            }
        }
    }

    [SerializeField]
    private int _level = 1;
    public int Level {
        get => _level;
        set {
            _level = value; 
            if(Player.Instance != null) {
                Player.Instance.Level = value > 100 ? 100 : value;
            }
            MenuManager.Instance.transform.Find("Overview Window/Stats/Stats/Level/Label").GetComponent<TextMeshProUGUI>().text = Label.Get("UIPlayerLevel") + " " + _level.ToString();
            MaxPassivePowerUps = Level;
            MaxUpgradePoints = Level >= 63 ? 21 : Level / 3;
            MaxUltimateUsesPerCombat = Level >= 65 ? 5 : Level >= 55 ? 4 : Level >= 45 ? 3 : Level >= 35 ? 2 : Level >= 25 ? 1 : 0;
            int regularTiles = MaxUltimateUsesPerCombat - UltimatesUsedInCurrentCombat;
            for(int i = 0; i < Constants.MAX_ULTIMATE_USES_POSSIBLE; i++) {
                CanvasElements.UICanvas.UltimateUses.transform.GetChild(i).gameObject.SetActive(MaxUltimateUsesPerCombat > i);
                CanvasElements.UICanvas.UltimateUses.transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (i < regularTiles ? "UltimateCanBeUsed" : "UltimateCannotBeUsed"), typeof(Sprite)) as Sprite; 
            }
            MakeSureAllCorrectTechniquesAndStancesAreUnlocked();
            UpdateSkillTrees();
            MenuManager.Instance.UpdateLoadoutUpgradePoints();
            CanvasElements.UICanvas.ExperienceBar.transform.Find("Left Level").GetComponent<TextMeshProUGUI>().text = _level.ToString();
            CanvasElements.UICanvas.ExperienceBar.transform.Find("Right Level").GetComponent<TextMeshProUGUI>().text = (_level + 1).ToString();
            CanvasElements.DialogueExperienceBar.transform.Find("Left Level").GetComponent<TextMeshProUGUI>().text = _level.ToString();
            CanvasElements.DialogueExperienceBar.transform.Find("Right Level").GetComponent<TextMeshProUGUI>().text = (_level + 1).ToString();
            if(GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect) {
                Utils.GetSceneRootObject("Mission Select").Find("Level/Left Level").GetComponent<TextMeshProUGUI>().text = _level.ToString();
                Utils.GetSceneRootObject("Mission Select").Find("Level/Right Level").GetComponent<TextMeshProUGUI>().text = (_level + 1).ToString();
            }
        }
    }

    public void MakeSureAllCorrectTechniquesAndStancesAreUnlocked() {
        UnlockedAbilities.Clear();
        UnlockedStances.Clear();
        foreach(Type ability in new List<Type> {typeof(Ability_WindRush), typeof(Ability_HeavySlash)}) {
            SaveFile.Instance.UnlockAbility(ability);
        }
        foreach(Type stance in new List<Type> {typeof(Stance_SingularPursuit), typeof(Stance_OmniMastery), typeof(Stance_HeatOfBattle), typeof(Stance_PlunderingFlame), typeof(Stance_PowerWithoutLimit), typeof(Stance_MindOverMatter)}) {
            SaveFile.Instance.UnlockStance(stance);
        }
        if(Level < 10) {
            return;
        }
        foreach(Type ability in new List<Type> {typeof(Ability_TempestStrikes), typeof(Ability_Fireball)}) {
            SaveFile.Instance.UnlockAbility(ability);
        }
        if(Level < 20) {
            return;
        }
        foreach(Type ability in new List<Type> {typeof(Ability_Quickdraw), typeof(Ability_Flamethrower)}) {
            SaveFile.Instance.UnlockAbility(ability);
        }
        if(Level < 30) {
            return;
        }
        foreach(Type ability in new List<Type> {typeof(Ability_CuttingWind), typeof(Ability_Eruption)}) {
            SaveFile.Instance.UnlockAbility(ability);
        }
    }

    public void UpdateSkillTrees() {
        foreach(PassivePowerUpTile tile in MenuManager.Instance.PowerUpTiles) {
            tile.CheckIfRowIsActive(tile.transform.parent.Find("Active"));
        }
        foreach(StanceUnlockTile tile in MenuManager.Instance.StanceUnlockTiles) {
            tile.UpdateUnlockedStatus();
        }
        foreach(AbilityUnlockTile tile in MenuManager.Instance.AbilityUnlockTiles) {
            tile.UpdateUnlockedStatus();
        }
        foreach(MasteryUnlockTile tile in MenuManager.Instance.MasteryUnlockTiles) {
            tile.UpdateUnlockedStatus();
        }
    }

    public void UnlockTool(Type tool_type) {
        if(UnlockedTools.Contains(tool_type)) {
            return;
        }
        UnlockedTools.Add(tool_type);
        Item item_to_add = (Item)Activator.CreateInstance(tool_type, new object[] { SaveFile.Instance.ToolGrades[tool_type] });
        item_to_add.Amount = SaveFile.Instance.ToolMaxAmounts[tool_type];
        AddItem(item_to_add, false);
    }

    public void UpgradeTool(Type tool_type) {
        SaveFile.Instance.ToolGrades[tool_type] = 
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Regular ? Item.ItemGrade.Excellent :
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Excellent ? Item.ItemGrade.Masterful :
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Masterful ? Item.ItemGrade.Flawless :
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Flawless ? Item.ItemGrade.Ultimate : Item.ItemGrade.Ultimate;

        Inventory.FirstOrDefault(item => item is Quest_ToolMaterials).Amount -= 
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Excellent ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_EXCELLENT :
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Masterful ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_MASTERFUL :
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Flawless ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_FLAWLESS :
        SaveFile.Instance.ToolGrades[tool_type] == Item.ItemGrade.Ultimate ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_ULTIMATE :
        0;

        UpdateToolInventoryTile(tool_type);
    }

    public void IncreaseMaxToolUses(Type tool_type) {
        SaveFile.Instance.ToolMaxAmounts[tool_type] = SaveFile.Instance.ToolMaxAmounts[tool_type] == 6 ? 6 : SaveFile.Instance.ToolMaxAmounts[tool_type] + 1;

        Inventory.FirstOrDefault(item => item is Quest_ToolMaterials).Amount -= 
        SaveFile.Instance.ToolMaxAmounts[tool_type] == 2 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_2 :
        SaveFile.Instance.ToolMaxAmounts[tool_type] == 3 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_3 :
        SaveFile.Instance.ToolMaxAmounts[tool_type] == 4 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_4 :
        SaveFile.Instance.ToolMaxAmounts[tool_type] == 5 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_5 :
        SaveFile.Instance.ToolMaxAmounts[tool_type] == 6 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_6 :
        0;

        UpdateToolInventoryTile(tool_type);
    }

    
    public void UpdateToolInventoryTile(Type tool_type) {
        foreach(Transform child in CanvasElements.MenuCanvasObject.transform.Find("Inventory Window/Inventory/Viewport/Items/Tool/Items").transform) {
            InventoryTile tile = child.GetComponent<InventoryTile>();
            if(tile.Item.GetType() == tool_type) {
                tile.InitializeOptions();
                tile.AmountDisplay.text = tile.Item.Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[tool_type];
            }
        }
    }


    public void ChangeIgnisEnergy(int amount) {
        SaveFile.Instance.IgnisEnergy += amount;
        SaveFile.Instance.GetQuest("Ignis").CurrentObjective.DescriptionParameters = new() {SaveFile.Instance.IgnisEnergy.ToString()};
        if(amount >= 10 && GameController.Instance.InterruptMusicOnDeath) {
            NotificationController.ShowNotificationWithGraphic("GainedIgnisEnergyNotification", "UI/Ignis", new List<string>{SaveFile.Instance.IgnisEnergy.ToString()});
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Generic/PowerUp1", 0.65f);
        }
        if(SaveFile.Instance.IgnisEnergy >= 100 && SaveFile.Instance.GetQuest("Ignis").CurrentObjective.Number == 40) {
            SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(50, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        }
        if(SaveFile.Instance.IgnisEnergy >= 100 && Utils.GetUnit("Blaine3") != null) {
            if(Utils.GetUnit("Blaine3").transform.Find("Dialogue1") != null) {
                Utils.GetUnit("Blaine3").transform.Find("Dialogue1").gameObject.SetActive(false);
            }
            if(Utils.GetUnit("Blaine3").transform.Find("BlaineWrapUp1") != null) {
                Utils.GetUnit("Blaine3").transform.Find("BlaineWrapUp1").gameObject.SetActive(true);
            }
        }
        if(GameController.Instance.InterruptMusicOnDeath == false) {
            CanvasElements.UICanvasObject.transform.Find("Ignis Energy/Amount").GetComponent<TextMeshProUGUI>().text = SaveFile.Instance.IgnisEnergy.ToString();
            CanvasElements.UICanvasObject.transform.Find("Ignis Energy").GetComponent<Slider>().value = SaveFile.Instance.IgnisEnergy / 2000;
            if(Area_IgnisManorOnFire.PlayerBuffEffect != null) {
                Area_IgnisManorOnFire.PlayerBuffEffect.Amount = SaveFile.Instance.IgnisEnergy / 10;
                Area_IgnisManorOnFire.PlayerBuffEffect.EffectIndicatorText = Utils.GetFormattedFloat(Area_IgnisManorOnFire.PlayerBuffEffect.Amount) + "%";
            }
        }
    }

    [SerializeField]
    private int _experiencePoints = 0;
    public int ExperiencePoints {
        get => _experiencePoints;
        set {
            int prevValue = _experiencePoints;
            int prevLevel = _level;
            Utils.CreateAuditLog("Gained experience (Level " + Level + "): " + _experiencePoints + " -> " + value);
            _experiencePoints = value;
            if(_experiencePoints >= GetExperiencePointsNeededToLevelUp()) {
                int lvl_ups = 0;
                do {
                    _experiencePoints -= GetExperiencePointsNeededToLevelUp(Level + lvl_ups);
                    lvl_ups++;
                } while (_experiencePoints >= GetExperiencePointsNeededToLevelUp(Level + lvl_ups));
                Level += lvl_ups;
                Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/LevelUp", 0.6f);
                NotificationController.ShowNotificationWithGraphic("LevelUpNotification", "UI/Experience", new List<string> {Level.ToString()});
                if(Level % 2 == 0) {
                    NotificationController.ShowNotificationWithGraphic("GainedTechniquePoint", "UI/Experience");
                }
                if(Level >= 10 && (Level - 10) % 4 == 0) {
                    NotificationController.ShowNotificationWithGraphic("GainedMasteryPoint", "UI/Experience");
                }
            }
            int expNeeded = GetExperiencePointsNeededToLevelUp();
            MenuManager.Instance.transform.Find("Overview Window/Stats/Stats/ExperiencePointsSlider/Amount").GetComponent<TextMeshProUGUI>().text = _experiencePoints + " / " + expNeeded;
            MenuManager.Instance.transform.Find("Overview Window/Stats/Stats/ExperiencePointsSlider").GetComponent<Slider>().value = (float)_experiencePoints / (float)expNeeded;
            CanvasElements.UICanvas.ExperienceBar.GetComponent<Slider>().value = (float)_experiencePoints / (float)expNeeded;
            CanvasElements.DialogueExperienceBar.GetComponent<Slider>().value = (float)_experiencePoints / (float)expNeeded;
            if(GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect) {
                Utils.GetSceneRootObject("Mission Select").Find("Level").GetComponent<Slider>().value = (float)_experiencePoints / (float)expNeeded;;
            }
            int gainedExp = Math.Abs(value - prevValue);
            if(prevValue != value || prevLevel != Level) {
                Utils.PlaySoundEffect(null, "UI/ExperienceGained", 0.3f);
                GameObject notification = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ExperienceNotification")) as GameObject;
                notification.transform.SetParent(GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect ? Utils.GetSceneRootObject("Mission Select").Find("Level").transform : GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene ? CanvasElements.DialogueExperienceBar.transform : CanvasElements.UICanvas.ExperienceBar.transform);
                notification.transform.localPosition = new Vector2(0, 10);
                notification.transform.localScale = Vector3.one;
                notification.GetComponent<TextMeshProUGUI>().text = "+<sprite name=\"Experience\"/>" + Utils.GetFormattedInteger(gainedExp).ToString();
                if(GameController.Instance.GameplayMode == Constants.GameplayMode.Regular) {
                    GameObject notification2 = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ExperienceNotificationWorldspace")) as GameObject;
                    notification2.transform.SetParent(Player.Instance.WorldSpaceCanvas.transform);
                    notification2.transform.localScale = new Vector2(0.01f, 0.01f);
                    notification2.transform.localPosition = new Vector2(-0.5f, 0.5f);
                    notification2.GetComponent<TextMeshProUGUI>().text = "+<sprite name=\"Experience\"/>" + Utils.GetFormattedInteger(gainedExp).ToString();
                }
            }
            if(gainedExp > 0) {
                MenuManager.Instance.AddHistoryEntry(string.Format(Label.Get("GetExperienceHistoryEntry"), new string[] {Utils.GetFormattedInteger(gainedExp).ToString()}), Resources.Load("Sprites/UI/Experience", typeof(Sprite)) as Sprite);
            }
        }
    }

    public float HealthGainPerTraining = 100;
    public float HealthGainedFromTraining = 0;
    public float StaggerBarGainPerTraining = 100;
    public float StaggerBarGainedFromTraining = 0;

    [NonSerialized]
    public Item EquippedHeavyWeapon;
    [NonSerialized]
    public Item EquippedLightWeapon;
    [NonSerialized]
    public Item EquippedRangedWeapon;
    [NonSerialized]
    public Item EquippedGloves;
    [NonSerialized]
    public Item EquippedHelmet;
    [NonSerialized]
    public Item EquippedArmor;
    [NonSerialized]
    public Item EquippedBoots;
    [NonSerialized]
    public Item EquippedItem1;
    [NonSerialized]
    public Item EquippedItem2;
    public List<Type> CompletedStoryMissions = new List<Type>();
    
    public List<string> ReadDialogueLines = new List<string>();

    public Dictionary<string, int> PointsPutIntoEachSkillTree = new Dictionary<string, int>() {
        {"Ignis", 0},
        {"Anima", 0},
        {"Glacies", 0},
        {"Molis", 0},
        {"Salutis", 0},
        {"Tonitrui", 0},
        {"Proprius", 0},
    };
    public List<string> ActiveUpgrades = new List<string>();
    public List<string> StanceUpgrades = new List<string>();
    public List<Type> UnlockedStances = new List<Type>();
    public List<Type> UnlockedAbilities = new List<Type>();
    public List<Type> UnlockedUltimates = new List<Type>();
    public List<Type> AbilitiesMasteryA = new List<Type>();
    public List<Type> AbilitiesMasteryB = new List<Type>();
    public List<string> UnlockedPowerUps = new List<string>();

    private TextMeshProUGUI _moneyTextDisplay;
    [SerializeField]
    private int _money;
    public int Money
    {
        get
        {
            return _money;
        }
        set
        {
            int prevValue = _money;
            Utils.CreateAuditLog("Gained money: " + _money + " -> " + value);
            _money = value < -9999999 ? -9999999 : value > 9999999 ? 9999999 : value;
            if(Instance != this) {
                return;
            }
            if(_moneyTextDisplay == null)
            {
                _moneyTextDisplay = CanvasElements.MenuCanvasObject.transform.Find("Inventory Window/Equipment/Money/Amount").GetComponent<TextMeshProUGUI>();
            }
            _moneyTextDisplay.text = Utils.GetFormattedInteger(_money);
            CanvasElements.UICanvas.MoneyDisplay.GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedInteger(_money);
            CanvasElements.DialogueMoneyDisplay.GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedInteger(_money);
            CanvasElements.ShopMoneyDisplay.GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedInteger(_money);
            if(GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect) {
                Utils.GetSceneRootObject("Mission Select").Find("Money").GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedInteger(_money);
            }
            if(prevValue != _money) {
                Utils.PlaySoundEffect(null, "UI/ItemPickedUp", 0.3f);
                GameObject notification = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_MoneyNotification")) as GameObject;
                notification.transform.SetParent(GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect ? Utils.GetSceneRootObject("Mission Select").Find("Money") : GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene ? CanvasElements.DialogueMoneyDisplay.transform : CanvasElements.UICanvas.MoneyDisplay.transform);
                notification.transform.localPosition = new Vector2(0, 30);
                notification.transform.localScale = Vector3.one;
                notification.GetComponent<TextMeshProUGUI>().text = (prevValue - _money > 0 ? "- " : "+ ") + Utils.GetFormattedInteger(Math.Abs(prevValue - _money)).ToString();
                if(GameController.Instance.GameplayMode == Constants.GameplayMode.Regular) {
                    GameObject notification2 = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_MoneyNotificationWorldspace")) as GameObject;
                    notification2.transform.SetParent(Player.Instance.WorldSpaceCanvas.transform);
                    notification2.transform.localScale = new Vector2(0.01f, 0.01f);
                    notification2.transform.localPosition = new Vector2(2f, 0.5f);
                    notification2.GetComponent<TextMeshProUGUI>().text = (prevValue - _money > 0 ? "-" : "+") + "<sprite name=\"Money\"/>" + Utils.GetFormattedInteger(Math.Abs(prevValue - _money)).ToString();
                }
            }
            MenuManager.Instance.AddHistoryEntry(string.Format(Label.Get(prevValue - _money > 0 ? "SpentMoneyHistoryEntry" : "GetMoneyHistoryEntry"), new string[] {Utils.GetFormattedInteger(Math.Abs(prevValue - _money)).ToString()}), Resources.Load("Sprites/UI/Money", typeof(Sprite)) as Sprite);
        }
    }

    public SaveFile(string save_number, SaveFileTypeEnum save_file_type) {
        SaveFileType = save_file_type;
        SaveFileNumber = save_number;
        if(save_number != "Dummy") {
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Quest))))
            {
                Quests.Add((Quest)Activator.CreateInstance(type));
            }
        }
        if(save_file_type == SaveFileTypeEnum.Story) {
            SaveFilePath = "SaveFile_" + save_number.ToString() + ".es3";
        }
        else if(save_file_type == SaveFileTypeEnum.Challenge) {
            SaveFilePath = "Challenge_" + save_number.ToString() + ".es3";
        }
        else if(save_file_type == SaveFileTypeEnum.Survival) {
            SaveFilePath = "Survival_" + save_number.ToString() + ".es3";
        }
    }
    
 
    public void InitializeSaveFile(bool give_tools = true) {
        Id = Guid.NewGuid().ToString();
        Player.Instance.InitializeStances();
        Level = 1;
        ExperiencePoints = 0;
        Money = 0;
        MenuManager.Instance.ClearInventory();
        AddDefaultItems();
        RefreshMenuDisplays();
        TimePlayedInSeconds = 0;
        Week = 1;
        MenuManager.Instance.InitializeMissionList();
        MenuManager.Instance.UpdateMissionList();
        MenuManager.Instance.UpdateDifficultyDisplay();
    }

    public void RefreshMenuDisplays() {
        foreach(AbilitySelect item in MenuManager.Instance.AbilityOverview) {
            item.UpdateUnlockedStatus();
        }
        foreach(StanceSelect item in MenuManager.Instance.StanceOverview) {
            item.UpdateUnlockedStatus();
        }
        foreach(AbilityUnlockTile item in MenuManager.Instance.AbilityUnlockTiles) {
            item.UpdateUnlockedStatus();
        }
        foreach(MasteryUnlockTile item in MenuManager.Instance.MasteryUnlockTiles) {
            item.UpdateUnlockedStatus();
        }
        foreach(StanceUnlockTile item in MenuManager.Instance.StanceUnlockTiles) {
            item.UpdateUnlockedStatus();
        }
        foreach(PassivePowerUpTile item in MenuManager.Instance.PowerUpTiles) {
            item.UpdateUnlockedStatus();
        }
    }

    public void AddDefaultItems() {
        List<Item> default_items = new List<Item>
        {
        new Greatsword_Retribution(Item.ItemGrade.Regular),
        new Daggers_ZephyrsTalons(Item.ItemGrade.Regular),
        new Bow_Barrage(Item.ItemGrade.Regular),
        };
        foreach(Item item in default_items)
        {
            AddItem(item, false);
        }
        MenuManager.Instance.HeavyEquipmentSlot.EquipItem(default_items[0]);
        MenuManager.Instance.LightEquipmentSlot.EquipItem(default_items[1]);
        MenuManager.Instance.RangedEquipmentSlot.EquipItem(default_items[2]);
        UnlockTool(typeof(Tool_VacuumGrenade));
        UnlockTool(typeof(Tool_FortificationPotion));
    }

    public void AddItem(Type item_type, Item.ItemGrade grade = Item.ItemGrade.None, bool show_notification = true) {
        Item item = (Item)Activator.CreateInstance(item_type, new object[] {grade});
        AddItem(item, show_notification);
    }

    public void AddItem(Item item, bool show_notification = true) {
        Utils.CreateAuditLog("Acquired item (" + item.GetType() + "): " + item.Grade + " , amount: " + item.Amount);
        int sellPrice = CheckSellPriceIfItemIsDuplicate(item);
        if(show_notification) {
            NotificationController.ShowItemDropNotification(item, sellPrice);
        }
        if(sellPrice > 0) {
            Money += sellPrice;
            return;
        }
        else if(item is Quest_UpgradeMaterials || item is Quest_ToolMaterials) {
            Item existing_item = Inventory.FirstOrDefault(i => i.GetType() == item.GetType() && i.Grade == item.Grade);
            if(existing_item != null) {
                existing_item.Amount += item.Amount;
                return;
            }
        }
        if(item.Category != Constants.ItemCategory.Tool && item.Category != Constants.ItemCategory.Quest && !FoundItemTypes.Contains(item.GetType())) {
            FoundItemTypes.Add(item.GetType());
        }
        Inventory.Add(item);
        Inventory.OrderBy(item => item.GetType()).ThenBy(item => item.Grade);
        MenuManager.Instance.AddItemToGrid(item);
    }

    public bool HasKey(string key_type_name) {
        Type type = Type.GetType(key_type_name);
        return Inventory.FirstOrDefault(item => item.GetType() == type) != null;
    }

    public int CheckSellPriceIfItemIsDuplicate(Item item) {
        Item existing_item = Inventory.FirstOrDefault(i => i.GetType() == item.GetType() && i.Grade == item.Grade);
        if(item.Category == Constants.ItemCategory.Tool || item.Category == Constants.ItemCategory.Quest)
        {
            return 0;
        }
        if(existing_item != null)
        {
            return item.SellPrice;
        }
        return 0;
    }

    public void RemoveItem(Item item) {
        Utils.CreateAuditLog("Removed item (" + item.GetType() + "): " + item.Grade + " , amount: " + item.Amount);
        Inventory.Remove(item);
        if(EquippedItem1 == item) {
            MenuManager.Instance.Item1EquipmentSlot.UnequipItem(1);
        }
        if(EquippedItem2 == item) {
            MenuManager.Instance.Item2EquipmentSlot.UnequipItem(2);
        }
        if(item.IsEquipped) {
            item.TileInInventory.UnequipItem();
        }
        MonoBehaviour.Destroy(item.TileInInventory.gameObject);
    }

    public bool CheckIfQuestIsInProgress(string quest_name)
    {
        return Quests.FirstOrDefault(quest => quest.Status == Quest.QuestStatus.InProgress && quest.GetType().Name == quest_name) != null;
    }

    public bool CheckIfQuestIsInProgress(Type quest_type)
    {
        return Quests.FirstOrDefault(quest => quest.Status == Quest.QuestStatus.InProgress && quest.GetType() == quest_type) != null;
    }

    public Quest GetQuest(string quest_name)
    {
        return Quests.FirstOrDefault(quest => quest.GetType().Name == quest_name || quest.GetType().Name.Replace("Quest_", "") == quest_name || ("Quest_" + quest.GetType().Name) == quest_name);
    }

    public Quest GetQuest(Type quest_type)
    {
        return Quests.FirstOrDefault(quest => quest.GetType() == quest_type);
    }

    public Mission GetMission(Type mission_type)
    {
        return Missions.FirstOrDefault(mission => mission.GetType() == mission_type);
    }

    
    public Item GetItem(Type item_type) {
        return Inventory.FirstOrDefault(item => item.GetType() == item_type);
    }

    public Item GetItem(Type item_type, Item.ItemGrade grade) {
        return Inventory.FirstOrDefault(item => item.GetType() == item_type && item.Grade == grade);
    }

    public Item GetEquipmentForCategory(Constants.ItemCategory category)
    {
        switch (category)
        {
            case Constants.ItemCategory.Heavy: return SaveFile.Instance.EquippedHeavyWeapon;
            case Constants.ItemCategory.Light: return SaveFile.Instance.EquippedLightWeapon;
            case Constants.ItemCategory.Ranged: return SaveFile.Instance.EquippedRangedWeapon;
            case Constants.ItemCategory.Gloves: return SaveFile.Instance.EquippedGloves;
            case Constants.ItemCategory.Helmet: return SaveFile.Instance.EquippedHelmet;
            case Constants.ItemCategory.Armor: return SaveFile.Instance.EquippedArmor;
            case Constants.ItemCategory.Boots: return SaveFile.Instance.EquippedBoots;
            default: return null;
        }
    }

    public void AcquireItem(string item_type, int amount, Item.ItemGrade grade) {
        if(String.IsNullOrEmpty(item_type)) {
            return;
        }
        if(item_type == "Money") {
            Money += amount;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
        else if(item_type.EndsWith("_Unlock") && SaveFile.Instance.UnlockedTools.Contains(Type.GetType(item_type.Replace("_Unlock", "")))) {
            Item item = (Item)Activator.CreateInstance(typeof(Quest_ToolMaterials), new object[] { Item.ItemGrade.None });
            item.Amount = 10;
            SaveFile.Instance.AddItem(item, false);
            Item item2 = (Item)Activator.CreateInstance(Type.GetType(item_type.Replace("_Unlock", "")), new object[] { Item.ItemGrade.Regular });
            NotificationController.ShowNotificationWithGraphic(Label.Get("ToolDuplicateMessage"), item2.GetIcon(), new List<string> {Label.Get(item_type.Replace("_Unlock", "")), "10"});
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
        else if(item_type.EndsWith("_Unlock")) {
            Item item = (Item)Activator.CreateInstance(Type.GetType(item_type.Replace("_Unlock", "")), new object[] { Item.ItemGrade.Regular });
            NotificationController.ShowNotificationWithGraphic(Label.Get("NewToolUnlockMessage"), item.GetIcon(), new List<string> {Label.Get(item_type.Replace("_Unlock", ""))});
            SaveFile.Instance.UnlockTool(Type.GetType(item_type.Replace("_Unlock", "")));
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
        else if(!string.IsNullOrWhiteSpace(item_type)) {
            Item item = (Item)Activator.CreateInstance(Type.GetType(item_type), new object[] { grade });
            item.Amount = amount;
            SaveFile.Instance.AddItem(item);
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
    }

    public void UnlockStance(Type ability_type, string upgrade_number = "0") {
        StanceUnlockTile unlock_tile = MenuManager.Instance.StanceUnlockTiles.FirstOrDefault(tile => (tile.Ability.Replace("1", "").Replace("2", "").Replace("3", "") == ability_type.ToString() && (upgrade_number == "0" || tile.Ability.Contains(upgrade_number))));
        if(unlock_tile.IsUpgrade) {
            StanceUpgrades.Add(unlock_tile.Ability);
            SaveFile.Instance.RefreshStances();
        }
        else {
            UnlockedStances.Add(ability_type);
        }
        foreach(Stance s in SaveFile.Instance.Stances) {
            s.StanceEffect.RefreshStance();
        }
        unlock_tile.transform.Find("Disabled").gameObject.SetActive(false);
        MenuManager.Instance.StanceOverview.FirstOrDefault(select => select.Stance == ability_type.ToString()).UpdateUnlockedStatus();
        foreach(StanceSelect stanceSelect in MenuManager.Instance.StanceLoadout.Where(stanceSel => stanceSel.StanceType == ability_type)) {
            stanceSelect.UpdateUnlockedStatus();
        }
        MenuManager.Instance.UpdateLoadoutUpgradePoints();
    }

    public void UnlearnStance(Type ability_type, string upgrade_number = "0") {
        StanceUnlockTile unlock_tile = MenuManager.Instance.StanceUnlockTiles.FirstOrDefault(tile => (tile.Ability.Replace("1", "").Replace("2", "").Replace("3", "")  == ability_type.ToString() && (upgrade_number == "0" || tile.Ability.Contains(upgrade_number))));
        if(unlock_tile.IsUpgrade) {
            StanceUpgrades.Remove(unlock_tile.Ability);
            SaveFile.Instance.RefreshStances();
        }
        else {
            UnlockedStances.Remove(ability_type);
        }
        unlock_tile.transform.Find("Disabled").gameObject.SetActive(true);
        MenuManager.Instance.StanceOverview.FirstOrDefault(select => select.Stance == ability_type.ToString()).UpdateUnlockedStatus();
        foreach(StanceSelect stanceSelect in MenuManager.Instance.StanceLoadout.Where(stanceSel => stanceSel.StanceType == ability_type)) {
            stanceSelect.UpdateUnlockedStatus();
        }
        MenuManager.Instance.UpdateLoadoutUpgradePoints();
        if(upgrade_number != "0") {
            return;
        }
        for(int i = 0; i < 3; i++) {
            if(SaveFile.Instance.Stances[i].StanceEffect.GetType() == ability_type) {
                MenuManager.Instance.StanceBeingChanged = i.ToString();
                MenuManager.Instance.EquipStance(typeof(Stance_None));
            }
            SaveFile.Instance.Stances[i].StanceEffect.RefreshStance();
        }
    }

    public void UnlockAbility(Type ability_type) {
        UnlockedAbilities.Add(ability_type);
        AbilityUnlockTile unlock_tile = MenuManager.Instance.AbilityUnlockTiles.FirstOrDefault(tile => tile.Ability == ability_type.ToString());
        if(unlock_tile != null) {
            unlock_tile.UpdateUnlockedStatus();;
        }
        MenuManager.Instance.AbilityOverview.FirstOrDefault(select => select.Ability == ability_type.ToString()).UpdateUnlockedStatus();
        foreach(AbilitySelect abilitySelect in MenuManager.Instance.AbilityLoadout.Where(abilitySel => abilitySel.AbilityType == ability_type)) {
            abilitySelect.UpdateUnlockedStatus();
        }
        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            ability.RefreshDisplayForEquippedAbility();
        }
        MenuManager.Instance.UpdateLoadoutUpgradePoints();
    }

    public void UnlearnAbility(Type ability_type) {
        if(UnlockedAbilities.Contains(ability_type)) {
            UnlockedAbilities.Remove(ability_type);
            AbilityUnlockTile unlock_tile = MenuManager.Instance.AbilityUnlockTiles.FirstOrDefault(tile => tile.Ability == ability_type.ToString());
            if(unlock_tile != null) {
                unlock_tile.UpdateUnlockedStatus();;
            }
            MenuManager.Instance.AbilityOverview.FirstOrDefault(select => select.Ability == ability_type.ToString()).UpdateUnlockedStatus();
            foreach(AbilitySelect abilitySelect in MenuManager.Instance.AbilityLoadout.Where(abilitySel => abilitySel.AbilityType == ability_type)) {
                abilitySelect.UpdateUnlockedStatus();
            }
            foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
                ability.RefreshDisplayForEquippedAbility();
            }
        }
        MenuManager.Instance.UpdateLoadoutUpgradePoints();
    }

    private void UnlockAbilityMastery(Type ability_type, string mastery) {
        MasteryUnlockTile unlock_tile = MenuManager.Instance.MasteryUnlockTiles.FirstOrDefault(tile => (tile.Ability == ability_type.ToString() && tile.Mastery == mastery));
        if(unlock_tile != null) {
            unlock_tile.UpdateUnlockedStatus();
        }
        SaveFile.Instance.UsedUpgradePoints++;
        MenuManager.Instance.AbilityOverview.FirstOrDefault(select => select.Ability == ability_type.ToString()).UpdateUnlockedStatus();
        foreach(AbilitySelect abilitySelect in MenuManager.Instance.AbilityLoadout.Where(abilitySel => abilitySel.AbilityType == ability_type)) {
            abilitySelect.UpdateUnlockedStatus();
        }
        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            ability.RefreshDisplayForEquippedAbility();
        }
        MenuManager.Instance.UpdateLoadoutUpgradePoints();
        foreach(Stance.EquippedAbility equippedAbility in SaveFile.Instance.Stances[0].Abilities.Concat(SaveFile.Instance.Stances[1].Abilities).Concat(SaveFile.Instance.Stances[2].Abilities)) {
            MethodInfo methodInfo = ability_type.GetMethod("OnUnequip", BindingFlags.Public | BindingFlags.Static);
            if(methodInfo != null) {
                methodInfo.Invoke(null, null);
            }
            MethodInfo methodInfo2 = ability_type.GetMethod("OnEquip", BindingFlags.Public | BindingFlags.Static);
            if(methodInfo2 != null) {
                methodInfo2.Invoke(null, null);
            }
        }
    }

    private void UnlearnAbilityMastery(Type ability_type, string mastery) {
        MasteryUnlockTile unlock_tile = MenuManager.Instance.MasteryUnlockTiles.FirstOrDefault(tile => (tile.Ability == ability_type.ToString() && tile.Mastery == mastery));
        if(unlock_tile != null) {
            unlock_tile.UpdateUnlockedStatus();
        }
        SaveFile.Instance.UsedUpgradePoints--;
        MenuManager.Instance.AbilityOverview.FirstOrDefault(select => select.Ability == ability_type.ToString()).UpdateUnlockedStatus();
        foreach(AbilitySelect abilitySelect in MenuManager.Instance.AbilityLoadout.Where(abilitySel => abilitySel.AbilityType == ability_type)) {
            abilitySelect.UpdateUnlockedStatus();
        }
        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            ability.RefreshDisplayForEquippedAbility();
        }
        MenuManager.Instance.UpdateLoadoutUpgradePoints();
    }

    public void UnlockAbilityMasteryA(Type ability_type) {
        AbilitiesMasteryA.Add(ability_type);
        UnlockAbilityMastery(ability_type, "A");
    }

    public void UnlearnAbilityMasteryA(Type ability_type) {
        AbilitiesMasteryA.Remove(ability_type);
        UnlearnAbilityMastery(ability_type, "A");
    }

    public void UnlockAbilityMasteryB(Type ability_type) {
        AbilitiesMasteryB.Add(ability_type);
        UnlockAbilityMastery(ability_type, "B");
    }

    public void UnlearnAbilityMasteryB(Type ability_type) {
        AbilitiesMasteryB.Remove(ability_type);
        UnlearnAbilityMastery(ability_type, "B");
    }

    public int GetExperiencePointsNeededToLevelUp(int level = 0) {
        return level == 0 ? (_level <= 0 ? 1000 : _level * 1000) : (level <= 0 ? 1000 : level * 1000);
    }

    public bool CheckIfCurrentlyCanSave() {
        return 
        SaveFile.Instance != null && 
        SaveFile.Instance.GameType == Constants.GameType.Story && 
        Player.Instance.InCombat == false && 
        GameController.Instance.GameplayMode != Constants.GameplayMode.InCutscene && 
        (SceneManager.GetActiveScene().name == "MissionSelect" || 
        (SaveFile.Instance.CurrentMission != null && 
        (SaveFile.Instance.CurrentMission.Type == Mission.MissionType.MainQuest || SaveFile.Instance.CurrentMission.Type == Mission.MissionType.SideQuest)));
    }

    public bool CheckIfCurrentlyCanLoad() {
        return true;
    }

    public void Save(string savefile_path = null, bool is_post_mission_autosave = false) {
        if(savefile_path != null) {
            SaveFilePath = savefile_path;
        }
        Utils.CreateAuditLog("Saving save file: " + SaveFilePath);
        SavedTimeStamp = DateTime.Now;
        SavedItems.Clear();
        foreach(Item item in Inventory) {
            SavedItems.Add(new SavedItem() {Type = item.GetType(), Amount = item.Amount, Rarity = item.Grade, IsEquipped = item.IsEquipped, IsEquippedToSlot1 = EquippedItem1 == item, IsEquippedToSlot2 = EquippedItem2 == item});
        }
        if(CurrentMission != null) {
            GameController.Instance.SaveMidMissionInformation();
        }
        SaveFileNumber = (savefile_path.Contains("PreMissionAutoSave") ? "-2" : savefile_path.Contains("PostMissionAutoSave") ? "-1" : savefile_path.Contains("MidMissionAutoSave") ? "0" : SaveFileNumber);
        ES3.Save("Save", Instance, SaveFilePath);
        if(savefile_path.Contains("PreMissionAutoSave")) {
            return;
        }
        EventManager.FinishedTakingScreenshot.AddListener(FinishSavingAfterScreenshot);
        GameController.Instance.TakeScreenshot(Application.persistentDataPath + "/" + SaveFilePath.Replace(".es3", "") + ".png");
    }

    public void FinishSavingAfterScreenshot() {
        SetSaveSlotValues(GameController.Instance.transform.Find("Save or Load/Viewport/Save Files/" + SaveFileNumber), false);
    }

    public void SetSaveSlotValues(Transform game_object, bool is_post_mission_autosave = false) {
        int hours_played = (int)(TimePlayedInSeconds / 3600);
        int minutes_played = (int)((TimePlayedInSeconds - 3600 * hours_played) / 60);
        int seconds_played = (int)(TimePlayedInSeconds - hours_played * 3600 - minutes_played * 60);
        game_object.Find("Left-side Info/Time Played").GetComponent<TextMeshProUGUI>().text = String.Format(Label.Get("SaveFileTimePlayed"), new string[] {hours_played.ToString(), minutes_played.ToString(), seconds_played.ToString()} );
        game_object.Find("Left-side Info/Cycle and Week").GetComponent<TextMeshProUGUI>().text = String.Format(Label.Get("SaveFileCycleAndWeek"), new string[] {Week.ToString()} );
        game_object.Find("Left-side Info/Cycle and Week/Image").GetComponent<Image>().enabled = true;
        game_object.Find("Left-side Info/Cycle and Week/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/Cycle" + Cycle, typeof(Sprite)) as Sprite;
        game_object.Find("Left-side Info/Current Mission").GetComponent<TextMeshProUGUI>().text = String.Format(Label.Get("SaveFileCurrentMission"), new string[] {CurrentMission == null ? Label.Get("CurrentMissionIsNull") : Label.Get(CurrentMission.ToString())} );
        
        game_object.Find("Right-side Info/Level and Money").GetComponent<TextMeshProUGUI>().text = String.Format(Label.Get("SaveFileLevelAndMoney"), new string[] {Level.ToString(), Money.ToString()} );
        if(PointsPutIntoEachSkillTree != null) {
            game_object.Find("Right-side Info/Skill Trees").GetComponent<TextMeshProUGUI>().text = String.Format(Label.Get("SaveFileSkillTrees"), new string[] {PointsPutIntoEachSkillTree["Ignis"].ToString(), PointsPutIntoEachSkillTree["Anima"].ToString(), PointsPutIntoEachSkillTree["Glacies"].ToString(), PointsPutIntoEachSkillTree["Molis"].ToString(), PointsPutIntoEachSkillTree["Salutis"].ToString(), PointsPutIntoEachSkillTree["Tonitrui"].ToString(), PointsPutIntoEachSkillTree["Proprius"].ToString()} );
        }
        else {
            game_object.Find("Right-side Info/Skill Trees").GetComponent<TextMeshProUGUI>().text = String.Format(Label.Get("SaveFileSkillTrees"), new string[] {"0", "0", "0", "0", "0", "0", "0"} );
        }
        game_object.Find("Right-side Info/Difficulty").GetComponent<TextMeshProUGUI>().text = Label.Get("SaveFileDifficulty") + Label.Get(Difficulty.ToString() + "CombatType");
        game_object.Find("Empty").gameObject.SetActive(false);
        UpdateScreenShot(new string[] {SaveFilePath.Replace(".es3", ""), SaveFileNumber, is_post_mission_autosave ? "IS_POST_MISSION_AUTOSAVE" : "NO"});
    }

    public static void UpdateScreenShot(string[] path) {
        GameController.Instance.transform.Find("Save or Load").GetComponent<CanvasGroup>().alpha = 1;
        if(GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu) {
            MenuManager.Instance.GetComponent<CanvasGroup>().alpha = 1;
        }
        if(SaveFile.Instance.CurrentMission == null) {
            UIManager.Instance.HideBlackScreen(2);
        }
        Transform transform = GameController.Instance.transform.Find("Save or Load/Viewport/Save Files/" + path[1]);
        transform.Find("Screenshot").GetComponent<Image>().color = Color.white;
        transform.Find("Screenshot").GetComponent<Image>().sprite = Utils.LoadSaveFileScreenshot(Application.persistentDataPath + "/" + path[0] + ".png");
    }

    public static SaveFile RetrieveSaveFile(string path)
    {
        if (ES3.FileExists(path))
        {
            return (SaveFile)ES3.Load("Save", path);
        }
        else
        {
            Debug.LogError("Could not find save file to load: " + path);
            return null;
        }
    }

    public void Load() {
        Utils.CreateAuditLog("Loading save file: " + SaveFilePath);
        GameController.Instance.CurrentSaveFile = this;
        foreach (PropertyInfo field in SaveFile.Instance.GetType().GetProperties())
        {
            if(field.Name != "Instance") {
                try {
                    field.SetValue(SaveFile.Instance, field.GetValue(SaveFile.Instance));
                }
                catch (Exception e) {
                    Debug.LogError("Error while loading savefile field: " + field.Name + ": " + e.Message + " , " + e.StackTrace + " , " + Utils.GetStackTrace());
                }
            }
        }
        ReloadItems();
        if(SaveFileType == SaveFileTypeEnum.Story && MidMissionInformation == null) {
            List<Mission> randomMissions = SaveFile.Instance.Missions.Where(m => m.Type == Mission.MissionType.Random).ToList();
            MenuManager.Instance.InitializeMissionList();
            foreach(Mission m in randomMissions) {
                SaveFile.Instance.Missions.Add(m);
            }
            MenuManager.Instance.UpdateMissionList();
            MenuManager.Instance.ShowTransitionIntoMissionSelect(true);
        }
        if(MidMissionInformation != null) {
            Utils.CreateAuditLog("Loading mid-mission save");
            UIManager.Instance.ToggleLoadingScreen();
            GameController.Instance.WaitAndRunMethodRealtime(0.01f, GameController.Instance.LoadMidMission);
        }
        GameController.Instance.transform.Find("Save or Load").gameObject.SetActive(false);
        EventManager.FinishedLoadingArea.AddListener(SetCorrectCycle);
    }

    public void SetCorrectCycle() {
        if(SceneManager.GetActiveScene().name == "MissionSelect") {
            Utils.GetSceneRootObject("Mission Select").Find("Cycle").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/Cycle" + SaveFile.Instance.Cycle, typeof(Sprite)) as Sprite;
        }
        CanvasElements.UICanvasObject.transform.Find("Cycle").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/Cycle" + SaveFile.Instance.Cycle, typeof(Sprite)) as Sprite;
    }

    public void ReloadItems() {
        MenuManager.Instance.ClearInventory();
        foreach(SavedItem item in SavedItems) {
            Item item_to_add = (Item)Activator.CreateInstance(item.Type, new object[] { item.Rarity });
            item_to_add.Amount = item.Amount;
            AddItem(item_to_add, false);
            if(item.IsEquipped && item_to_add.Category == Constants.ItemCategory.Heavy) {
                EquippedHeavyWeapon = item_to_add;
                MenuManager.Instance.HeavyEquipmentSlot.EquipItem(EquippedHeavyWeapon);
            }
            else if(item.IsEquipped && item_to_add.Category == Constants.ItemCategory.Light) {
                EquippedLightWeapon = item_to_add;
                MenuManager.Instance.LightEquipmentSlot.EquipItem(EquippedLightWeapon);
            }
            else if(item.IsEquipped && item_to_add.Category == Constants.ItemCategory.Ranged) {
                EquippedRangedWeapon = item_to_add;
                MenuManager.Instance.RangedEquipmentSlot.EquipItem(EquippedRangedWeapon);
            }
            else if(item.IsEquipped && item_to_add.Category == Constants.ItemCategory.Gloves) {
                EquippedGloves = item_to_add;
                MenuManager.Instance.GlovesEquipmentSlot.EquipItem(EquippedGloves);
            }
            else if(item.IsEquipped && item_to_add.Category == Constants.ItemCategory.Helmet) {
                EquippedHelmet = item_to_add;
                MenuManager.Instance.HelmetEquipmentSlot.EquipItem(EquippedHelmet);
            }
            else if(item.IsEquipped && item_to_add.Category == Constants.ItemCategory.Armor) {
                EquippedArmor = item_to_add;
                MenuManager.Instance.ArmorEquipmentSlot.EquipItem(EquippedArmor);
            }
            else if(item.IsEquipped && item_to_add.Category == Constants.ItemCategory.Boots) {
                EquippedBoots = item_to_add;
                MenuManager.Instance.BootsEquipmentSlot.EquipItem(EquippedBoots);
            }
            /*else if(item.IsEquippedToSlot1 && (item_to_add.Category == Constants.ItemCategory.Tool)) {
                EquippedItem1 = item_to_add;
                MenuManager.Instance.Item1EquipmentSlot.EquipItem(EquippedItem1);
            }
            else if(item.IsEquippedToSlot2 && (item_to_add.Category == Constants.ItemCategory.Tool)) {
                EquippedItem2 = item_to_add;
                MenuManager.Instance.Item2EquipmentSlot.EquipItem(EquippedItem2);
            }*/
        }
        foreach(Type unlockedTool in SaveFile.Instance.UnlockedTools) {
            Item item_to_add = (Item)Activator.CreateInstance(unlockedTool, new object[] { SaveFile.Instance.ToolGrades[unlockedTool] });
            item_to_add.Amount = SaveFile.Instance.ToolMaxAmounts[unlockedTool];
            AddItem(item_to_add, false);
        }
    }

    public void ReloadAbilities() {
        if(Stances == null) {
            Player.Instance.InitializeStances();
        }
        foreach (Stance stance in Stances)
        {
            for(int i = 0; i < 4; i++)
            {
                stance.Abilities[i].Reload();
            }
        }
        RefreshStances();
        RefreshMenuDisplays();
    }

    public void RefreshStances() {
        foreach(Stance s in SaveFile.Instance.Stances) {
            if(s == Player.Instance.CurrentStance && s.StanceEffect != null) {
                Player.Instance.ReplaceStanceDisplay();
                s.StanceEffect.RefreshStance();
            }
            if(s.StanceEffect == null) {
                Effect_Stance stance_effect = (Effect_Stance)Activator.CreateInstance(s.StanceEffectType, new object[] {null});
                s.StanceEffect = stance_effect;
            }
            Image img = MenuManager.Instance.transform.Find("Overview Window/Abilities/Stances/" + s.WeaponCategory + "/UI_StanceTile").GetComponent<Image>();
            string family = s.StanceEffect.GetType().GetField("Family",  BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();
            if(img.GetComponent<Button>() != null) {
                img.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + s.StanceEffect.GetType().ToString(), typeof(Sprite)) as Sprite;
                img.GetComponent<Image>().color = s.StanceEffect.GetType() == typeof(Stance_None) ? Color.grey : Colors.GetFamilyColor(family);
            }
            else {
                img.transform.Find("Stance Border").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + s.StanceEffect.ToString(), typeof(Sprite)) as Sprite;
                img.color = Colors.GetFamilyColor(family);
            }
            img.transform.Find("Border Upper").GetComponent<Image>().color = Colors.GetFamilyColor(family);
            img.transform.Find("Border Lower").GetComponent<Image>().color = Colors.GetFamilyColor(family);
        }
    }
}
