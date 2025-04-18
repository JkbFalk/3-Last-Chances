using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_BattleBorn : Item
{
    public Boots_BattleBorn(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageReductionBasedOnMissingHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health")};
    }
}
