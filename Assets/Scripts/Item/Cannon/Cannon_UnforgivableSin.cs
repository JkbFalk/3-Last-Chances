using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cannon_UnforgivableSin : Item
{
    public Cannon_UnforgivableSin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(7, 14, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainBurnOnBasicAttackAndIncreaseDamageBasedOnBurn")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("CleanseBurnOnWeaponSwitch")};
    }
}
