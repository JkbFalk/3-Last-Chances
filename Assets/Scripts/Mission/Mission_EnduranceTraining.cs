using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_EnduranceTraining : Mission
{
    public Mission_EnduranceTraining() {
        Type = MissionType.Activity;
        CanExpire = false;
        Icon = "UI/StaggerBar";
        MapMarker = "Home";
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveFile.Instance.StaggerBarGainedFromTraining += SaveFile.Instance.StaggerBarGainPerTraining;
        NotificationController.ShowNotificationWithGraphic("ActivityEnduranceTrainingNotification", "UI/StaggerBar", new List<string> {SaveFile.Instance.StaggerBarGainPerTraining.ToString()});
        if(SaveFile.Instance.StaggerBarGainPerTraining < 200) {
            SaveFile.Instance.StaggerBarGainPerTraining += 10;
        }
        Finish();
    }

    public override List<String> GetDescriptionParameters() { return new List<String> {SaveFile.Instance.StaggerBarGainPerTraining.ToString() }; }
}
