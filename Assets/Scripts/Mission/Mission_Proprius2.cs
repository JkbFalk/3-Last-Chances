using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Proprius2 : Mission
{
    public Mission_Proprius2() {
        Type = MissionType.MainQuest;
        NumberOfWeeksConsumed = 1;
        WeeksUntilExpiryMax = 9;
        FirstAppearsOnWeek = 2;
        Icon = "UI/Proprius";
        EnemyLevel = 8;
        MapMarker = "AnimaIsland";
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Cycle == 2;
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, "AnimaIsland");
        //Utils.MoveIntoArea(true, "AnimaIsland_FloodedCaverns");
    }
}
