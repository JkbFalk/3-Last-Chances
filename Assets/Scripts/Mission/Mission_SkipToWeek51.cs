using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_SkipToWeek51 : Mission
{
    public Mission_SkipToWeek51() {
        Type = MissionType.Activity;
        NumberOfWeeksConsumed = 0;
        CanExpire = false;
        Icon = "UI/Clock";
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveFile.Instance.Week = 51;
        Finish();
    }

    public override bool MissionShouldBeAvailable()
    {
        return Settings.Instance.ConsoleEnabled;
    }
}
