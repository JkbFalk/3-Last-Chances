using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_StilettoSeriesMark3 : Item
{
    public Daggers_StilettoSeriesMark3(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(90, 80, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainAlacrityOnLightDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ApplyLethargyOnLightDamage")};
    }
}

