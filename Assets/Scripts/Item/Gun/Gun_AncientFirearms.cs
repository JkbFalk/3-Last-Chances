using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_AncientFirearms : Item
{
    public Gun_AncientFirearms(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(130, 100, 0.75f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AncientFirearmsDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("AncientFirearmsDamageReduction")};
    }
}
