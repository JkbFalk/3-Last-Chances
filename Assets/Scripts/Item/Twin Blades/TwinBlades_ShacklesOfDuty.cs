using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwinBlades_ShacklesOfDuty : Item
{
    public TwinBlades_ShacklesOfDuty(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(12, 4, 1.25f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertCurrentHealthIntoChained")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksRestoreHealth")};
    }
}

