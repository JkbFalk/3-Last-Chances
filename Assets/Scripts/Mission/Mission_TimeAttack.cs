using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Mission_TimeAttack : Mission
{
    public List<string> Enemies = new List<string>();
    public bool CountTime = false;
    public float SecondsLeft = 0;
    Quest_DefeatSpawningEnemies Quest;
    public int Money;
    public int Experience;
    public Mission_TimeAttack() {
        Type = MissionType.Random;
        Icon = "UI/Hourglass";
        WeeksUntilExpiryMax = UnityEngine.Random.Range(2, 5);
        for(int i = 0; i < 5; i++) {
            Enemies.Add(Constants.RegularEnemies[UnityEngine.Random.Range(0, Constants.RegularEnemies.Count)]);
        } 
        EnemyLevel = SaveFile.Instance.Level + UnityEngine.Random.Range(-3, 4);
        Money = CalculateMoneyReward(EnemyLevel, 0.25f);
        Experience = CalculateExperienceReward(EnemyLevel, 0.25f);
        MusicOnStart = new List<string>() {"Action_25", "Action_35", "Action_40"}[UnityEngine.Random.Range(0, 3)];
        AutoSaveAfterCombat = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, Constants.TimeAttackArenas[UnityEngine.Random.Range(0, Constants.TimeAttackArenas.Count)]);
    }

    public override void OnFinishedLoadingArea() {
        Utils.SpawnUnits(Enemies, EnemyLevel, 2);
        Utils.AllEnemiesAttackPlayer();
        Quest = new Quest_DefeatSpawningEnemies()  {Icon = Icon};
        Quest.StartQuest();
        SecondsLeft = 180;
        CountTime = true;
        CanvasElements.UICanvas.Timer.gameObject.SetActive(true);
        AdvanceTimer();
        ExtraRewards = new() {
            new() {Amount = Experience, Graphic = "UI/Experience", Name = "MissionReward_ExperiencePerEnemy"},
            new() {Amount = Money, Name = "MissionReward_GoldPerEnemy"}
        };
    }

    public void AdvanceTimer() {
        CanvasElements.UICanvas.Timer.GetComponent<TextMeshProUGUI>().text = ((int)SecondsLeft / 60).ToString() + ":" + ((int)SecondsLeft % 60).ToString() + "." + ((SecondsLeft % 1).ToString().Length > 2 ? (SecondsLeft % 1).ToString()[2] : "0");
        if(SecondsLeft <= 0) {
            UIManager.Instance.ShowBlackScreen(0.5f);
            CanvasElements.TransitionScreen.UpperText.GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(0.5f);
            CanvasElements.TransitionScreen.UpperText.GetComponent<TextMeshProUGUI>().text = String.Format(Label.Get("Quest_DefeatSpawningEnemies_Screen"), new string[] {Quest.DefeatedEnemyCount.ToString()});
            MoneyReward = Money * Quest.DefeatedEnemyCount;
            ExperienceReward = Experience * Quest.DefeatedEnemyCount;
            GameController.Instance.WaitAndRunMethod(2, HideText);
            GameController.Instance.WaitAndRunMethod(2, Finish, false);
        }
        else if(CountTime) {
            GameController.Instance.WaitAndRunMethod(0.1f, AdvanceTimer);
            SecondsLeft -= 0.1f;
        }
    }

    public void HideText() {
        CanvasElements.TransitionScreen.UpperText.GetComponent<HideOrShowOverTime>().HideOverTimeFromFull(1.5f);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        CountTime = false;
        CanvasElements.UICanvas.Timer.gameObject.SetActive(false);
        Quest.CompleteQuest();
    }
}