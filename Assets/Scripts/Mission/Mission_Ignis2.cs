using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Ignis2 : Mission
{
    public Mission_Ignis2() {
        Type = MissionType.MainQuest;
        NumberOfWeeksConsumed = 1;
        WeeksUntilExpiryMax = 9;
        FirstAppearsOnWeek = 2;
        Icon = "UI/Ignis";
        EnemyLevel = 20;
        MapMarker = "Town_IgnisManor";
        AutoSaveAfterCombat = true;
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Cycle == 2 && SaveFile.Instance.CompletedStoryMissions.Contains(typeof(Mission_Ignis1));
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, "IgnisManor_Graves");
        //Utils.MoveIntoArea(true, "IgnisVolcano");
    }
}
