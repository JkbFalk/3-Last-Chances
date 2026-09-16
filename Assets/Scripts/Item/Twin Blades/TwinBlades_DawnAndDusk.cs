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
        SetBaseWeaponStats(9, 9, 1.2f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealMoreLightInjuryOrStaggerAndCanSwitchUsingBlock")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightTechniqueDamage")};
    }
}

