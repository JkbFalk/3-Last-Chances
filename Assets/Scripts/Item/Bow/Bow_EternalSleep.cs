using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_EternalSleep : Item
{
    public Bow_EternalSleep(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(8, 16, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DamageToSleeping", 0.5f), new ItemEffect("RangedTechniqueDamage", 0.5f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ApplySleepWithBasicAttacks")};
    }
}
