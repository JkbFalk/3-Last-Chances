using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_DawnAndDusk : Item
{
    public TwinBlades_DawnAndDusk(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(95, 95, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealMoreInjuryOrStaggerAndCanSwitchUsingBlock")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightTechniqueDamage")};
    }
}

