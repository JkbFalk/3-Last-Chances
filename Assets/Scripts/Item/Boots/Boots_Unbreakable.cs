using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Unbreakable : Item
{
    public Boots_Unbreakable(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Unbreakable;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertXPercentOfInjuryDealtToYouIntoStagger")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StaggerBar")};
    }
}
