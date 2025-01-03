using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class Mission_CompleteSurvival : Mission
{
    [DoNotSerialize]
    public SaveFile OriginalSave;
    public List<string> Enemies = new List<string>();
    public Mission_CompleteSurvival() {
        Type = MissionType.Random;
        Icon = "UI/Skull";
        WeeksUntilExpiryMax = 5;
        NumberOfWeeksConsumed = 3;
        EnemyLevel = SaveFile.Instance.Level;
        MoneyReward = CalculateMoneyReward(EnemyLevel, 6f);
        ExperienceReward = CalculateExperienceReward(EnemyLevel, 6f);
        AutoSaveAfterCombat = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        OriginalSave = SaveFile.Instance;
        SurvivalController.StoryModeSurvival = true;
        if(SaveFile.Instance.CurrentSurvivalRunId != null && ES3.FileExists("Survival_" + SaveFile.Instance.CurrentSurvivalRunId + ".es3")) {
            SaveFile sf = SaveFile.RetrieveSaveFile("Survival_" + SaveFile.Instance.CurrentSurvivalRunId + ".es3");
            sf.Load();
            sf.CurrentMission = this;
            SurvivalController.StartSurvivalMode();
        }
        else {
            UIManager.Instance.HideBlackScreen(0);
            Utils.GetSceneRootObject("Mission Select").transform.Find("Survival Type Selection").gameObject.SetActive(true);
        }
    }

    public override List<string> GetDescriptionParameters() {
        return new List<string> { (SaveFile.Instance.SurvivalLevel - 1).ToString(), "50"};
    }

    public override void OnEnd()
    {
        SaveFile.Instance.Id = Guid.NewGuid().ToString();
        SaveFile.Instance.Save("Survival_" + SaveFile.Instance.Id + ".es3");
        OriginalSave.CurrentSurvivalRunId = SaveFile.Instance.Id;
        OriginalSave.SurvivalLevel = SaveFile.Instance.SurvivalLevel;
        OriginalSave.Load();
        if(SaveFile.Instance.SurvivalLevel == 50) {
            ES3.DeleteFile("Survival_" + OriginalSave.CurrentSurvivalRunId + ".es3");
            SaveFile.Instance.CurrentSurvivalRunId = null;
            SaveFile.Instance.SurvivalLevel = 0;
        }
    }
}
