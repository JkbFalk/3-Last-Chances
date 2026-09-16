using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mission_EliminateGroup : Mission
{
    public List<string> Enemies = new List<string>();
    public Mission_EliminateGroup() {
        Type = MissionType.Random;
        Icon = "UI/Sword01";
        WeeksUntilExpiryMax = UnityEngine.Random.Range(2, 5);
        for(int i = 0; i < UnityEngine.Random.Range(5, 11); i++) {
            Enemies.Add(Constants.RegularEnemies[UnityEngine.Random.Range(0, Constants.RegularEnemies.Count)]);
        }
        EnemyLevel = SaveFile.Instance.Level + UnityEngine.Random.Range(-5, 11);
        MoneyReward = CalculateMoneyReward(EnemyLevel, 1f);
        ExperienceReward = CalculateExperienceReward(EnemyLevel, 1f);
        MusicOnStart = new List<string>() {"Action_17", "Action_22", "Action_25"}[UnityEngine.Random.Range(0, 3)];
        AutoSaveAfterCombat = false;
    }

    public override List<string> GetDescriptionParameters() {
        string desc = "";
        Dictionary<string, int> grouped = Utils.GetGroupedListOfUnits(Enemies);
        foreach(string enemy in grouped.OrderByDescending(e => grouped[e.Key]).ToDictionary(e => e.Key, e => e.Value).Keys) {
            desc += "\n• " + grouped[enemy] + "x " +  Label.Get("Title_" + enemy.Replace("Unit_", "")) + ",";
        }
        return new List<string> {desc.Substring(0, desc.Length - 1) + "."};
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, Constants.RegularArenas_Small[UnityEngine.Random.Range(0, Constants.RegularArenas_Small.Count)]);
    }

    public override void OnFinishedLoadingArea() {
        Utils.SpawnUnits(Enemies, CombatMath.GetLevelAdjustmentBasedOnUnitCount(Enemies.Count), CombatMath.GetAggresivenessBasedOnUnitCount(Enemies.Count));
        Quest_EliminateAllEnemies quest = new Quest_EliminateAllEnemies() {Icon = Icon};
        quest.StartQuest();
    }
}
