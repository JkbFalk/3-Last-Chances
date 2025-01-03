using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mission_Destructibles : Mission
{
    public List<string> Enemies = new List<string>();
    Quest_DestroyDestructibles Quest;
    public Mission_Destructibles() {
        Type = MissionType.Random;
        Icon = "UI/Hammer";
        WeeksUntilExpiryMax = UnityEngine.Random.Range(2, 5);
        for(int i = 0; i < 7; i++) {
            Enemies.Add(Constants.RegularEnemies[UnityEngine.Random.Range(0, Constants.RegularEnemies.Count)]);
        } 
        for(int i = 0; i < 3; i++) {
            Enemies.Add(Constants.EliteEnemies[UnityEngine.Random.Range(0, Constants.EliteEnemies.Count)]);
        } 
        EnemyLevel = SaveFile.Instance.Level + UnityEngine.Random.Range(-3, 4);
        MoneyReward = CalculateMoneyReward(EnemyLevel, 1.5f);
        ExperienceReward = CalculateExperienceReward(EnemyLevel, 1.5f);
        MusicOnStart = new List<string>() {"Action_25", "Action_35", "Action_40"}[UnityEngine.Random.Range(0, 3)];
        AutoSaveAfterCombat = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, Constants.DestructibleArenas[UnityEngine.Random.Range(0, Constants.DestructibleArenas.Count)]);
    }

    public override void OnFinishedLoadingArea() {
        Utils.SpawnUnits(Enemies, EnemyLevel + 15, 2.5f);
        Utils.AllEnemiesAttackPlayer();
        Quest = new Quest_DestroyDestructibles() {Icon = Icon};
        Quest.StartQuest();
    }
}
