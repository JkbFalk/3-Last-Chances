using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_CombatTraining : Mission
{
    public Mission_CombatTraining() {
        Type = MissionType.Activity;
        CanExpire = false;
        Icon = "UI/Experience";
        MapMarker = "Home";
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveFile.Instance.ExperiencePoints += 1000 + 200 * SaveFile.Instance.Level;
        Finish();
    }

    public override List<String> GetDescriptionParameters() { return new List<String> {(1000 + 200 * SaveFile.Instance.Level).ToString() }; }
}
