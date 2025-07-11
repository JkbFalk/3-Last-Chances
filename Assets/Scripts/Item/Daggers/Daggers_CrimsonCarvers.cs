using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_CrimsonCarvers : Item
{
    public Daggers_CrimsonCarvers(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(120, 0, 1.25f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksApplyIncision")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("IncisionArmor")};
    }
}
