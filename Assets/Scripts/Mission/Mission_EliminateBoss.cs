using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_EliminateBoss : Mission
{
    public string Boss;
    public Mission_EliminateBoss() {
        Type = MissionType.Random;
        EnemyLevel = SaveFile.Instance.Level + UnityEngine.Random.Range(-5, 6);
        EnemyLevel = EnemyLevel < 7 ? 7 : EnemyLevel;
        ExperienceReward = CalculateExperienceReward(EnemyLevel, 4);
        Icon = "UI/Sword03";
        Boss = Constants.BossEnemies[UnityEngine.Random.Range(0, Constants.BossEnemies.Count)];
        MusicOnStart = new List<string>() {"Action_42", "Action_50", "Action_55"}[UnityEngine.Random.Range(0, 3)];
        ExtraRewards = GetItemRewardsForBoss(Boss, EnemyLevel);
        AutoSaveAfterCombat = false;
    }

    public override List<string> GetTitleParameters() {
        return new List<string> { Label.Get("Name_" + Boss.Replace("Unit_", ""))};
    }

    public override List<string> GetDescriptionParameters() {
        return new List<string> { Label.Get("Name_" + Boss.Replace("Unit_", ""))};
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, GetArenaForBoss(Boss));
    }

    public override void OnFinishedLoadingArea() {
        Utils.SpawnUnits(new List<string> {Boss}, EnemyLevel, 2, true);
        Quest_EliminateAllEnemies quest = new Quest_EliminateAllEnemies() {Icon = Icon};
        quest.StartQuest();
    }

    public static string GetArenaForBoss(string boss) {
        return boss switch
        {
            "Unit_Berserker" => "Warehouse_Arena",
            "Unit_Ryker" => "Outside_RockBackAlley",
            _ => "Warehouse_FightClub",
        };
    }

    public static List<MissionReward> GetItemRewardsForBoss(string boss, int level) {
        return boss switch
        {
            "Unit_Berserker" => new List<MissionReward> { new MissionReward() { Item = typeof(TwinBlades_BerserkerBlades), Rarity = GetItemRarityForLevel(level) } },
            "Unit_Ryker" => new List<MissionReward> { new MissionReward() { Item = typeof(Helmet_WeaponMaster), Rarity = GetItemRarityForLevel(level) } },
            _ => new List<MissionReward>(),
        };
    }
}
