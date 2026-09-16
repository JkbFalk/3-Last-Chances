using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Sage : Item
{
    public Boots_Sage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Sage;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealingMagicDamageGivesBarrier")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("MagicDamage")};
    }
}
