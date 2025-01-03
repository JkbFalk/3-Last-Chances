using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Skip3Weeks : Mission
{
    public Mission_Skip3Weeks() {
        Type = MissionType.Activity;
        NumberOfWeeksConsumed = 0;
        CanExpire = false;
        Icon = "UI/Clock";
    }

    public override void OnStart()
    {
        base.OnStart();
        if(SaveFile.Instance.Week < 48) {
            SaveFile.Instance.Week += 3;
        }
        else {
            SaveFile.Instance.Week = 51;
        }
        Finish();
    }

    public override bool MissionShouldBeAvailable()
    {
        return Settings.Instance.ConsoleEnabled;
    }
}
