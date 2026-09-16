using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_PiercingThorns : Item
{
    public Gauntlets_PiercingThorns(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(8, 8, 1.3f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("LightDamageTemporarilyLowersEnemyArmor")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightDamageIgnoresPercentageOfEnemyArmor")};
    }
}
