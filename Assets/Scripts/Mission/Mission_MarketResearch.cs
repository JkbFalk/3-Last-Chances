using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_MarketResearch : Mission
{
    public Mission_MarketResearch() {
        Type = MissionType.Activity;
        CanExpire = false;
        Icon = "UI/Money";
        MapMarker = "Town_Capital";
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Cycle != 3;
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveFile.Instance.ExtraGoldFromInvestmentsStartingNextCycle += 10000;
        NotificationController.ShowNotificationWithGraphic("ActivityResearchNotification", "UI/Money", new List<string> {SaveFile.Instance.HealthGainPerTraining.ToString()});
        Finish();
    }

    public override List<String> GetDescriptionParameters() { return new List<String> {Utils.GetFormattedInteger(SaveFile.Instance.GoldFromInvestmentsMinimum), Utils.GetFormattedInteger(SaveFile.Instance.GoldFromInvestmentsMaximum), Utils.GetFormattedInteger(SaveFile.Instance.GoldFromInvestmentsMinimum + 5000), Utils.GetFormattedInteger(SaveFile.Instance.GoldFromInvestmentsMaximum + 10000) }; }
}