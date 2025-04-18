using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_EternalSleep : Item
{
    public Bow_EternalSleep(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(80, 160, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ApplySleepToEnemiesHit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Control")};
    }
}
