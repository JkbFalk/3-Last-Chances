using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Gunslinger : Item
{
    public Outfit_Gunslinger(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("WhileInRangedStanceDecreaseArmorButIncreaseDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedDamage")};
    }
}
