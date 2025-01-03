using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Anima3 : Mission
{
    public Mission_Anima3() {
        Type = MissionType.MainQuest;
        NumberOfWeeksConsumed = 1;
        WeeksUntilExpiryMax = 9;
        FirstAppearsOnWeek = 2;
        Icon = "UI/Anima";
        EnemyLevel = 8;
        MapMarker = "AnimaIsland";
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Cycle == 3;
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, "AnimaIsland");
        //Utils.MoveIntoArea(true, "AnimaIsland_FloodedCaverns");
    }
}
