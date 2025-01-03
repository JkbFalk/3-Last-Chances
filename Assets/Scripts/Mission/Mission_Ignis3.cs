using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Ignis3 : Mission
{
    public Mission_Ignis3() {
        Type = MissionType.MainQuest;
        NumberOfWeeksConsumed = 1;
        WeeksUntilExpiryMax = 9;
        FirstAppearsOnWeek = 2;
        Icon = "UI/Ignis";
        EnemyLevel = 35;
        MapMarker = "Town_IgnisManor";
        AutoSaveAfterCombat = true;
        ExtraRewards = new List<MissionReward>() {new MissionReward() { Item = typeof(Greatsword_Plunderer), Rarity = Item.ItemGrade.Flawless}};
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Cycle == 3 && SaveFile.Instance.CompletedStoryMissions.Contains(typeof(Mission_Ignis2));
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter = 0;
        Utils.MoveIntoArea(true, "IgnisManor_Graves");
        //Utils.MoveIntoArea(true, "IgnisVolcano_Summit");
        //Utils.MoveIntoArea(true, "IgnisManorOnFire");
    }
}
