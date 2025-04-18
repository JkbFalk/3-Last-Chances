using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Jailer : Item
{
    public Armor_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageAndDamageReductionForEachDebuff")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ChainedDamageReduction")};
    }
}
