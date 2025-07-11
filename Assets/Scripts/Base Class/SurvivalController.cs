using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Constants;
using static Effect;

public class SurvivalController
{
    public static bool StoryModeSurvival = false;
    public static bool LoadStageWithoutRewards = false;
    public static bool SelectingAbilities = false;
    public static List<string> PowerUps = new List<string>();

    public static void StartSurvivalMode()
    {
        SaveFile.Instance.GameType = Constants.GameType.Survival;
        GameObject questDisplay = UIManager.Objects.ObjectivesDisplay;
        Utils.DestroyAllChildren(questDisplay.transform);
        LoadNextLevel();
    }

    public static float PlayerRemainingHealth = 1000;
    public static int RemainingHeals;
    public static float PlayerRemainingAmmo = 6;

    public static void LoadNextLevel()
    {
        if(LoadStageWithoutRewards == false && SaveFile.Instance.SkillTreeSurvivalType && SaveFile.Instance.SurvivalLevel != 1) {
            SaveFile.Instance.ExperiencePoints += SaveFile.Instance.GetExperiencePointsNeededToLevelUp();
        }
        if(Player.Instance != null) {
            PlayerRemainingHealth = Player.Instance.Health.Current;
            RemainingHeals = SaveFile.Instance.HealChargesRemaining;
            PlayerRemainingAmmo = Player.Instance.Ammo;
        }
        if (SaveFile.Instance.SurvivalLevel % 10 == 0)
        {
            Utils.MoveIntoArea(true, BossArenas[UnityEngine.Random.Range(0, BossArenas.Count)]);
            EventManager.FinishedLoadingArea.AddListener(CreateBossStage);
        }
        else if (SaveFile.Instance.SurvivalLevel % 10 == 4 || SaveFile.Instance.SurvivalLevel % 10 == 7) 
        {
            if(UnityEngine.Random.Range(0, 100) < 50) {
                Utils.MoveIntoArea(true, RegularArenas_Small[UnityEngine.Random.Range(0, RegularArenas_Small.Count)]);
            }
            else {
                Utils.MoveIntoArea(true, RegularArenas_Large[UnityEngine.Random.Range(0, RegularArenas_Large.Count)]);
            }
            EventManager.FinishedLoadingArea.AddListener(CreateEliteStage);
        }
        else
        {
            if(UnityEngine.Random.Range(0, 100) < 20) {
                Utils.MoveIntoArea(true, RegularArenas_Small[UnityEngine.Random.Range(0, RegularArenas_Small.Count)]);
            }
            else {
                Utils.MoveIntoArea(true, RegularArenas_Large[UnityEngine.Random.Range(0, RegularArenas_Large.Count)]);
            }
            EventManager.FinishedLoadingArea.AddListener(CreateRegularStage);
        }
        foreach (string power_up in PowerUps)
        {
            /*if(!power_up.Contains("Stance_") && !power_up.Contains("_Unlock") && !power_up.Contains("_UpgradeA") && !power_up.Contains("_UpgradeB")) {
                bool is_percentage = power_up.Contains("%");
                foreach(Effect e in PassivePowerUpTile.GetPassivePowerUpEffects(power_up.Split("~")[0], is_percentage ? int.Parse(power_up.Split("~")[1].Replace("%", "")) : 0, is_percentage ? 0 : int.Parse(power_up.Split("~")[1]))) {
                    Player.Instance.AddEffect(e);
                }
            }*/
        }
        if(LoadStageWithoutRewards == false && (SaveFile.Instance.SurvivalLevel % 2 == 0 || SaveFile.Instance.SurvivalLevel == 1))
        {
            SelectingAbilities = true;
            Utils.ShowLevelUpSelection(Get4RandomAbilities(SaveFile.Instance.SurvivalLevel == 1 ? new List<string> {"Stance_SingularPursuit_Unlock", "Stance_OmniMastery_Unlock", "Stance_PowerWithoutLimit_Unlock", "Stance_MindOverMatter_Unlock"} : null));
        }
        else if (LoadStageWithoutRewards == false)
        {
            Utils.ShowLevelUpSelection(Get4RandomPowerUps());
        }
        if(SaveFile.Instance.SurvivalLevel % 10 == 1) {
            Utils.SetDefaultMusic("Action_10");
        }
        else if(SaveFile.Instance.SurvivalLevel % 10 == 4) {
            Utils.SetDefaultMusic("Action_42");
        }
        else if(SaveFile.Instance.SurvivalLevel % 10 == 5) {
            Utils.SetDefaultMusic("Action_35");
        }
        else if(SaveFile.Instance.SurvivalLevel % 10 == 7) {
            Utils.SetDefaultMusic("Action_50");
        }
        else if(SaveFile.Instance.SurvivalLevel % 10 == 8) {
            Utils.SetDefaultMusic("Action_55");
        }
        else if(SaveFile.Instance.SurvivalLevel % 10 == 0) {
            Utils.SetDefaultMusic("Action_65");
        }
        if(LoadStageWithoutRewards == false && SaveFile.Instance.SurvivalLevel != 1) {
            Player.Instance.Health.Current = PlayerRemainingHealth;
            int maxCharges = SaveFile.Instance.DifficultyLevel == 0 ? 10 : SaveFile.Instance.DifficultyLevel == 1 ? 5 : 2;
            SaveFile.Instance.HealChargesRemaining = RemainingHeals + 1 > maxCharges ? maxCharges : RemainingHeals + 1;
            Player.Instance.Ammo = PlayerRemainingAmmo;
            GameController.Instance.WaitAndRunMethod(0.1f, GiveRandomItems);
        }
        if(LoadStageWithoutRewards) {
            LoadStageWithoutRewards = false;
            Player.Instance.Health.Current = Player.Instance.Health.Maximum;
            GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;
        }
        SaveFile.Instance.SurvivalLevel = SaveFile.Instance.SurvivalLevel;
    }
    
    public static void GiveRandomItems() {
        /*int range = UnityEngine.Random.Range(3, 6);
        for (int i = 0; i < range; i++)
        {
            Type random_item_type = Constants.PossibleItemDrops[UnityEngine.Random.Range(0, Constants.PossibleItemDrops.Count)];
            Item random_item = (Item)Activator.CreateInstance(random_item_type, new object[] { Item.GetRandomizedGradeForGivenLevel(SaveFile.Instance.SurvivalLevel % 10 == 1 ? SaveFile.Instance.SurvivalLevel + 6 : (SaveFile.Instance.SurvivalLevel % 10 == 5 || SaveFile.Instance.SurvivalLevel % 10 == 8) ? SaveFile.Instance.SurvivalLevel + 1 : SaveFile.Instance.SurvivalLevel - 4) });
            if(random_item.Category == ItemType.Tool) {
                random_item.Amount = UnityEngine.Random.Range(3, 6);
            }
            SaveFile.Instance.AddItem(random_item);
        }
        SaveFile.Instance.Save();*/
    }

    public static List<string> Get4RandomAbilities(List<string> predefined_choices = null)
    {
        return null;
        /*System.Random rng = new System.Random();
        List<string> potential_choices = new List<string>();
        foreach (string unlockable in Unlockables)
        {
            if (!PowerUps.Contains(unlockable) 
            && 
            ((!unlockable.Contains("_UpgradeA") && !unlockable.Contains("_UpgradeB")) || 
            (SaveFile.Instance.UnlockedAbilities.Contains(Type.GetType(unlockable.Replace("_UpgradeA", "").Replace("_UpgradeB", ""))) &&
             !SaveFile.Instance.AbilitiesMasteryA.Contains(Type.GetType(unlockable.Replace("_UpgradeA", "").Replace("_UpgradeB", ""))) &&
             !SaveFile.Instance.AbilitiesMasteryB.Contains(Type.GetType(unlockable.Replace("_UpgradeA", "").Replace("_UpgradeB", "")))))
            &&
            ((!unlockable.Contains("_Upgrade1") && !unlockable.Contains("_Upgrade2") && !unlockable.Contains("_Upgrade3")) || 
            SaveFile.Instance.UnlockedStances.Contains(Type.GetType(unlockable.Replace("_Upgrade1", "").Replace("_Upgrade2", "").Replace("_Upgrade3", ""))))
            )
            {
                potential_choices.Add(unlockable);
            }
        }
        List<string> chosen_choices = predefined_choices != null ? predefined_choices : new List<string>();
        foreach (string choice in potential_choices.OrderBy(a => rng.Next()).ToList())
        {
            if (chosen_choices.Count < 4 && chosen_choices.Contains(choice) == false)
            {
                chosen_choices.Add(choice);
            }
        }
        return chosen_choices;*/
    }

    public static List<string> Get4RandomPowerUps()
    {
        return null;
        /*List<string> power_ups = new List<string>();
        System.Random rng = new System.Random();
        foreach (string power_up in PossiblePassivePowerUps.OrderBy(a => rng.Next()).ToList())
        {
            if (!power_ups.Contains(power_up) && power_ups.Count < 4)
            {
                if (power_up.Contains("-") && power_up.Contains("~"))
                {
                    bool percentage = power_up.Contains("%");
                    string split_power_up = power_up.Split("~")[1];
                    string[] values = split_power_up.Split("-");
                    string randomized_value = ((int)UnityEngine.Random.Range(float.Parse(values[0].Replace("%", "")), float.Parse(values[1].Replace("%", "")))).ToString();    
                    power_ups.Add(power_up.Split("~")[0] + "~" + randomized_value + (percentage ? "%" : ""));
                }
                else
                {
                    power_ups.Add(power_up);
                }
            }
        }
        return power_ups;*/
    }

    public static void CreateRegularStage()
    {
        int random = UnityEngine.Random.Range(0, 100);
        int enemy_count;
        int level_adjustment;
        if (random < 25)
        {
            enemy_count = 3;
            level_adjustment = 5;
        }
        else if (random < 45)
        {
            enemy_count = 4;
            level_adjustment = 3;
        }
        else if (random < 65)
        {
            enemy_count = 5;
            level_adjustment = 0;
        }
        else if (random < 80)
        {
            enemy_count = 6;
            level_adjustment = -2;
        }
        else if (random < 85)
        {
            enemy_count = 7;
            level_adjustment = -4;
        }
        else if (random < 90)
        {
            enemy_count = 8;
            level_adjustment = -6;
        }
        else if (random < 95)
        {
            enemy_count = 9;
            level_adjustment = -8;
        }
        else
        {
            enemy_count = 10;
            level_adjustment = -9;
        }
        List<string> enemies = new List<string>();
        for(int i = 0; i < enemy_count; i++) {
            enemies.Add(Constants.RegularEnemies[UnityEngine.Random.Range(0, Constants.RegularEnemies.Count)]);
        }
        Area.ComponentInstance.Level = SaveFile.Instance.SurvivalLevel;
        Utils.SpawnUnits(enemies, SaveFile.Instance.SurvivalLevel + level_adjustment, 2.5f / enemies.Count + 0.5f);
        FinishCreatingStage();
    }

    public static void CreateEliteStage()
    {
        int random = UnityEngine.Random.Range(0, 100);
        int enemy_count;
        int level_adjustment;
        if (random < 40)
        {
            enemy_count = 1;
            level_adjustment = 0;
        }
        else if (random < 70)
        {
            enemy_count = 2;
            level_adjustment = -2;
        }
        else
        {
            enemy_count = 5;
            level_adjustment = -4;
        }
        Area.ComponentInstance.Level = SaveFile.Instance.SurvivalLevel;
        Transform boss_spawns = GameObject.FindGameObjectWithTag("Area").transform.Find("Boss Spawns");
        for (int j = 0; j < (enemy_count == 2 ? 2 : 1); j++)
        {
            Utils.SpawnUnit(EliteEnemies[UnityEngine.Random.Range(0, EliteEnemies.Count)], boss_spawns, SaveFile.Instance.SurvivalLevel + level_adjustment, enemy_count == 1 ? 3 : 2);
        }
        if(enemy_count == 5)
        {
            Transform enemy_spawns = GameObject.FindGameObjectWithTag("Area").transform.Find("Enemy Spawns");
            for (int j = 0; j < 4; j++)
            {
                Utils.SpawnUnit(RegularEnemies[UnityEngine.Random.Range(0, RegularEnemies.Count)], enemy_spawns, SaveFile.Instance.SurvivalLevel + level_adjustment, 2.5f / enemy_count + 0.5f);
            }
        }
        FinishCreatingStage();
    }

    public static void CreateBossStage()
    {
        Area.ComponentInstance.Level = SaveFile.Instance.SurvivalLevel;
        Transform boss_spawns = GameObject.FindGameObjectWithTag("Area").transform.Find("Boss Spawns");
        Utils.SpawnUnit(BossEnemies[UnityEngine.Random.Range(0, BossEnemies.Count)], boss_spawns, SaveFile.Instance.SurvivalLevel - 5, 3);
        FinishCreatingStage();
    }

    public static void FinishCreatingStage() {
        EventManager.UnitKnockedOut.AddListener(CheckEnemyDefeatedCount);
        Transform player_spawns = Area.Instance.transform.Find("Player Start Positions");
        Transform random_spawn = player_spawns.GetChild(UnityEngine.Random.Range(0, player_spawns.childCount));
        Player.Instance.transform.position = random_spawn.position;
        if (random_spawn.gameObject.name.Contains("(Flipped)"))
        {
            Player.Instance.Actions.IsFlipped = true;
        }
        GameController.Instance.WaitAndRunMethod(0.01f, StartMission);
    }

    public static void StartMission() {
        CheckEnemyDefeatedCount(null);
    }

    public static void EndSurvivalMode()
    {
        EventManager.UnitKnockedOut.RemoveListener(CheckEnemyDefeatedCount);
    }

    public static void CheckEnemyDefeatedCount(Damage damage)
    {
        Utils.DestroyAllChildren(UIManager.Objects.ObjectivesDisplay.transform);
        int enemyCount = Utils.GetAllUnits(true, true).Count;
        if (enemyCount > 0)
        {
            Utils.ShowMissionObjective(Label.Get("SurvivalLevelDisplay") + " " + SaveFile.Instance.SurvivalLevel.ToString(), Label.Get("SurvivalEnemiesRemaining") + ": " + enemyCount.ToString(), "UI/Skull");
        }
        else if (!StoryModeSurvival && SaveFile.Instance.SurvivalLevel == 50) {
            Utils.SetDefaultMusic("Victory");
            Utils.ShowMissionObjective(Label.Get("SurvivalLevelDisplay") + " " + (SaveFile.Instance.SurvivalLevel - 1).ToString(), Label.Get("SurvivalVictory"), "UI/Skull");
        }
        else if (StoryModeSurvival == true && SaveFile.Instance.SurvivalLevel % 10 == 0) {
            SaveFile.Instance.SurvivalLevel++;
            Utils.ShowMissionObjective(Label.Get("SurvivalLevelDisplay") + " " + (SaveFile.Instance.SurvivalLevel - 1).ToString(), Label.Get("ReturningToRealWorld"), "UI/Skull");
            UIManager.Instance.ShowBlackScreen();
            GameController.Instance.WaitAndRunMethod(1f, FinishMission);
        }
        else {
            SaveFile.Instance.SurvivalLevel++;
            Utils.ShowMissionObjective(Label.Get("SurvivalLevelDisplay") + " " + (SaveFile.Instance.SurvivalLevel - 1).ToString(), Label.Get("SurvivalLoadingNextStage"), "UI/Skull");
            UIManager.Instance.ShowBlackScreen();
            GameController.Instance.WaitAndRunMethod(1f, LoadNextLevel);
        }
    }

    public static void FinishMission() {
        SaveFile.Instance.CurrentMission.Finish();
    }
}
