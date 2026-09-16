using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Impetus : Item
{
    public Bow_Impetus(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(8, 16, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealIncreasedDamageBasedOnFlightTime")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorPenetrationBasedOnFlightTime")};
    }
}
