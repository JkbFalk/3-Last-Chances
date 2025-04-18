using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_TrimvineKeyToLaurasLab : Item
{
    public Quest_TrimvineKeyToLaurasLab(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Quest;
        IconPath = "UI/KeySilver";
        RemoveAtEndOfMission = false;
        Grade = ItemGrade.None;
    }
}
