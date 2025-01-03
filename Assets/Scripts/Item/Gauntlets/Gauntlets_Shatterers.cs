using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_Shatterers : Item
{
    public Gauntlets_Shatterers(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(80, 80, 1.3f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Control, new(this)) {PercentageAmount = 0.5f}};
    }
}
