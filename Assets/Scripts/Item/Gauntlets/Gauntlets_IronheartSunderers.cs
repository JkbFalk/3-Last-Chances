using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_IronheartSunderers : Item
{
    public Gauntlets_IronheartSunderers(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Unbreakable;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(80, 80, 1.3f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksRestorePercentageOfStaggerBar")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StaggerBar")};
    }
}
