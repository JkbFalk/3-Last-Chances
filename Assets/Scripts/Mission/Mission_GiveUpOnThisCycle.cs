using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_GiveUpOnThisCycle : Mission
{
    public Mission_GiveUpOnThisCycle() {
        Type = MissionType.MainQuest;
        CanExpire = false;
        NumberOfWeeksConsumed = 0;
        MapMarker = "Penance";
        RemoveOtherMissions = true;
        Icon = "UI/Clock";
    }

    public override void OnStart()
    {
        base.OnStart();
        MenuManager.Instance.MoveToNextCycle();
        Finish();
        SaveFile.Instance.CompletedStoryMissions.Remove(typeof(Mission_GiveUpOnThisCycle));
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Cycle != 3 && SaveFile.Instance.Week == 51;
    }
}
