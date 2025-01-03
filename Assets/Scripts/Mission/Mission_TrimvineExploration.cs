using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_TrimvineExploration : Mission
{
    public Mission_TrimvineExploration() {
        Type = MissionType.Exploration;
        CanExpire = false;
        Icon = "UI/HouseYellow";
        MapMarker = "Town_Trimvine";
        NumberOfWeeksConsumed = 0;
        CanAbandonMission = true;
    }

    public override void OnStart()
    {
        base.OnStart();
        Quest_Exploration exploration = new Quest_Exploration() {Icon = "UI/HouseYellow"};
        Utils.ShowMissionObjective(exploration.Objectives[0]);
        Utils.MoveIntoArea(true, "Trimvine");
    }
}
