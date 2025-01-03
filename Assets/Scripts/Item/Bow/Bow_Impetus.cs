using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Impetus : Item
{
    public Bow_Impetus(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(80, 160, 0.6f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_GainSpeedAndDamageWithDistanceTravelled(new(this))};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AddKnockbackToBasicAttacks(new(this))};
    }
}
