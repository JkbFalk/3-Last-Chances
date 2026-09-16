using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_Shattershield : Item
{
    public Polearm_Shattershield(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Unbreakable;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(2, 13, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("HeavyDamageStealsStaggerBarPerCooldown")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyAttackSpeed")};
    }
}
