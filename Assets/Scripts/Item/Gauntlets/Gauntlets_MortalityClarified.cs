using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_MortalityClarified : Item
{
    public Gauntlets_MortalityClarified(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(8, 8, 1.3f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ApplyLethargySlowAndProneOnExitingStaggered")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StaggerToNonStaggered")};
    }
}
