using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_BattleBorn : Item
{
    public Armor_BattleBorn(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("TakingDamageGivesBarrierBasedOnMissingHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("FlatHealth", 0.5f), new ItemEffect("Health", 0.5f)};
    }
}
