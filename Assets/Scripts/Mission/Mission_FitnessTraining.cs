using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_FitnessTraining : Mission
{
    public Mission_FitnessTraining() {
        Type = MissionType.Activity;
        CanExpire = false;
        Icon = "UI/Health";
        MapMarker = "Home";
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveFile.Instance.HealthGainedFromTraining += SaveFile.Instance.HealthGainPerTraining;
        NotificationController.ShowNotificationWithGraphic("ActivityFitnessTrainingNotification", "UI/Health", new List<string> {SaveFile.Instance.HealthGainPerTraining.ToString()});
        if(SaveFile.Instance.HealthGainPerTraining < 200) {
            SaveFile.Instance.HealthGainPerTraining += 10;
        }
        Finish();
    }

    public override List<String> GetDescriptionParameters() { return new List<String> {SaveFile.Instance.HealthGainPerTraining.ToString() }; }
}
