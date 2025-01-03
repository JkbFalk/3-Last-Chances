using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_FirstMissionMap : Item
{
    public Quest_FirstMissionMap(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Quest;
        IconPath = "UI/UI Quests";
        RemoveAtEndOfMission = true;
        Grade = ItemGrade.None;
    }
}
