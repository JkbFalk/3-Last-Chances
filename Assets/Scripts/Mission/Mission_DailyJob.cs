using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_DailyJob : Mission
{
    public Mission_DailyJob() {
        Type = MissionType.Activity;
        CanExpire = false;
        Icon = "UI/Money";
        MapMarker = "Home";
    }

    public override void OnStart()
    {
        base.OnStart();
        float min = Utils.GetValueBasedOnMinAndMax(SaveFile.Instance.Level, 1, 51, 100, 200);
        float max = Utils.GetValueBasedOnMinAndMax(SaveFile.Instance.Level, 1, 51, 200, 400);
        float amount = UnityEngine.Random.Range(min, max);
        SaveFile.Instance.Money += (int)amount * 100;
        Finish();
    }

    public override List<String> GetDescriptionParameters() { return new List<String> {Utils.GetFormattedInteger((int)Utils.GetValueBasedOnMinAndMax(SaveFile.Instance.Level, 1, 51, 10000, 20000)), Utils.GetFormattedInteger((int)Utils.GetValueBasedOnMinAndMax(SaveFile.Instance.Level, 1, 51, 20000, 40000)) }; }
}
