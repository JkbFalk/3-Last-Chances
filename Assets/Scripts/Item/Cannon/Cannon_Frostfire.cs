using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_Frostfire : Item
{
    public Cannon_Frostfire(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(7, 14, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RangedDamageAppliesBurnOrFreezeToEqualize")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BurnAmount", 0.5f), new ItemEffect("FreezeAmount", 0.5f)};
    }
}
