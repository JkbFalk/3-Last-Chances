using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mission_CompleteChallenge : Mission
{
    [DoNotSerialize]
    public SaveFile OriginalSave;
    public List<string> Enemies = new List<string>();
    Quest_EliminateAllEnemies Quest;
    public Mission_CompleteChallenge() {
        Type = MissionType.Random;
        Icon = "UI/Star";
        WeeksUntilExpiryMax = 5;
        EnemyLevel = SaveFile.Instance.Level;
        MoneyReward = CalculateMoneyReward(EnemyLevel, 2.5f);
        ExperienceReward = CalculateExperienceReward(EnemyLevel, 2.5f);
        AutoSaveAfterCombat = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        OriginalSave = SaveFile.Instance;
        SaveFile save = SaveFile.RetrieveSaveFile("Challenge_CounterMaster.es3");
        save.CurrentMission = this;
        save.Load();
        Utils.MoveIntoArea(true, "Challenge_CounterMaster");
        Quest = new Quest_EliminateAllEnemies() {Icon = Icon};
        Quest.StartQuest();
    }

    public override void OnEnd()
    {
        OriginalSave.Load();
    }
}
