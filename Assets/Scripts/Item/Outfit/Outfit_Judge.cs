using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Judge : Item
{
    public Outfit_Judge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("PushAwayAndFreezeUponFallingBelowHalfHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorWhileAbove50PHealth")};
    }
}
