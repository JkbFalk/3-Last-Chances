using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_Shatterers : Item
{
    public Gauntlets_Shatterers(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(80, 80, 1.3f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksReducePercentageOfEnemyStaggerBar")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightAttackSpeed")};
    }
}
