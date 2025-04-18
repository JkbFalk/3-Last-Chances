using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_ZephyrsTalons : Item
{
    public Daggers_ZephyrsTalons(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(95, 80, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainAlacrityBasedOnDistanceTravelledInLast5Seconds")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightDamage")};
    }
}

