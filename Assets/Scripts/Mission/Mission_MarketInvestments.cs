using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_MarketInvestments : Mission
{
    public Mission_MarketInvestments() {
        Type = MissionType.Activity;
        CanExpire = false;
        Icon = "UI/Money";
        MapMarker = "Town_Capital";
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Money >= 10000;
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveFile.Instance.WeeksAndInvestments.Add(SaveFile.Instance.Week + 10, UnityEngine.Random.Range(SaveFile.Instance.GoldFromInvestmentsMinimum / 200, SaveFile.Instance.GoldFromInvestmentsMaximum / 200) * 100);
        NotificationController.ShowTextNotification(String.Format(Label.Get("ActivityInvestmentNotification"), new string[] {}));
        Finish();
    }

    public override List<String> GetDescriptionParameters() { return new List<String> {"10", (SaveFile.Instance.Week + 10).ToString(), Utils.GetFormattedInteger(SaveFile.Instance.GoldFromInvestmentsMinimum), Utils.GetFormattedInteger(SaveFile.Instance.GoldFromInvestmentsMaximum).ToString() }; }
}
