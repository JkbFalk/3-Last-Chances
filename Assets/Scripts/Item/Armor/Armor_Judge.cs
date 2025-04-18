using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Judge : Item
{
    public Armor_Judge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("PushAwayAndFreezeUponFallingBelowHalfHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DamageReductionWhileAbove50PHealth")};
    }
}
