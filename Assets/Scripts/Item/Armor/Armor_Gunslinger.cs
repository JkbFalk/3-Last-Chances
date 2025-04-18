using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Gunslinger : Item
{
    public Armor_Gunslinger(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("WhileInRangedStanceDecreaseDamageReductionButIncreaseDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedDamage")};
    }
}
