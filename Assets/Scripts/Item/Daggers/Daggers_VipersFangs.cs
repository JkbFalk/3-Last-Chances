using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_VipersFangs : Item
{
    public Daggers_VipersFangs(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(60, 70, 1.35f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksApplyPoison")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightAttackSpeed")};
    }
}

