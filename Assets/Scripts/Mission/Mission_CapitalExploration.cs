using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_CapitalExploration : Mission
{
    public Mission_CapitalExploration() {
        Type = MissionType.Exploration;
        CanExpire = false;
        Icon = "UI/HouseYellow";
    }

    public override bool MissionShouldBeAvailable() {
        return false;
    }
}
