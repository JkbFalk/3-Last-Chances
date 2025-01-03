using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mission_EliminateElites : Mission
{
    public List<string> BossEnemies = new List<string>();
    public List<string> RegularEnemies = new List<string>();
    public Mission_EliminateElites() {
        Type = MissionType.Random;
        Icon = "UI/Sword02";
        WeeksUntilExpiryMax = UnityEngine.Random.Range(3, 7);
        int random = UnityEngine.Random.Range(0, 100);
        if (random < 100)
        {
            BossEnemies.Add(Constants.EliteEnemies[UnityEngine.Random.Range(0, Constants.EliteEnemies.Count)]);
            EnemyLevel = SaveFile.Instance.Level + UnityEngine.Random.Range(-2, 8);
            EnemyLevel = EnemyLevel < 4 ? 4 : EnemyLevel;
            MoneyReward = CalculateMoneyReward(EnemyLevel, 1.5f);
            ExperienceReward = CalculateExperienceReward(EnemyLevel, 1.5f);
        }
        else if (random < 70)
        {
            BossEnemies.Add(Constants.EliteEnemies[UnityEngine.Random.Range(0, Constants.EliteEnemies.Count)]);
            BossEnemies.Add(Constants.EliteEnemies[UnityEngine.Random.Range(0, Constants.EliteEnemies.Count)]);
            EnemyLevel = SaveFile.Instance.Level + UnityEngine.Random.Range(-5, 5);
            EnemyLevel = EnemyLevel < 4 ? 4 : EnemyLevel;
            MoneyReward = CalculateMoneyReward(EnemyLevel, 2);
            ExperienceReward = CalculateExperienceReward(EnemyLevel, 2);
        }
        else
        {
            BossEnemies.Add(Constants.EliteEnemies[UnityEngine.Random.Range(0, Constants.EliteEnemies.Count)]);
            for(int i = 0; i < 5; i++) {
                RegularEnemies.Add(Constants.RegularEnemies[UnityEngine.Random.Range(0, Constants.RegularEnemies.Count)]);
            }
            EnemyLevel = SaveFile.Instance.Level + UnityEngine.Random.Range(-5, 5);
            EnemyLevel = EnemyLevel < 4 ? 4 : EnemyLevel;
            MoneyReward = CalculateMoneyReward(EnemyLevel, 1.7f);
            ExperienceReward = CalculateExperienceReward(EnemyLevel, 1.7f);
        }
        MusicOnStart = new List<string>() {"Action_33", "Action_30", "Action_45"}[UnityEngine.Random.Range(0, 3)];
        AutoSaveAfterCombat = false;
    }

    public override List<string> GetDescriptionParameters() {
        string desc = "";
        Dictionary<string, int> grouped = Utils.GetGroupedListOfUnits(BossEnemies);
        foreach(string enemy in grouped.OrderByDescending(e => grouped[e.Key]).ToDictionary(e => e.Key, e => e.Value).Keys) {
            desc += "\n• " + grouped[enemy] + "x " +  Label.Get("Title_" + enemy.Replace("Unit_", "")) + " (" + Label.Get("UnitType_Boss") + "),";
        }
        Dictionary<string, int> grouped2 = Utils.GetGroupedListOfUnits(RegularEnemies);
        foreach(string enemy in grouped2.OrderByDescending(e => grouped2[e.Key]).ToDictionary(e => e.Key, e => e.Value).Keys) {
            desc += "\n• " + grouped2[enemy] + "x " +  Label.Get("Title_" + enemy.Replace("Unit_", "")) + ",";
        }
        return new List<string> {desc.Substring(0, desc.Length - 1) + "."};
    }
    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, Constants.RegularArenas_Small[UnityEngine.Random.Range(0, Constants.RegularArenas_Small.Count)]);
    }

    public override void OnFinishedLoadingArea() {
        foreach(Unit u in Utils.SpawnUnits(BossEnemies, EnemyLevel, Utils.GetAggresivenessBasedOnUnitCount(BossEnemies.Count), true)) {
            u.IsBoss = true;
        }
        if(RegularEnemies.Count > 0) {
            Utils.SpawnUnits(RegularEnemies, EnemyLevel - 5, Utils.GetAggresivenessBasedOnUnitCount(RegularEnemies.Count));
        }
        Quest_EliminateAllEnemies quest = new Quest_EliminateAllEnemies() {Icon = Icon};
        quest.StartQuest();
    }
}