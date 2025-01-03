using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_FirstMissionKeyToChest : Item
{
    public Quest_FirstMissionKeyToChest(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Quest;
        IconPath = "UI/KeySilver";
        RemoveAtEndOfMission = true;
        Grade = ItemGrade.None;
    }
}
