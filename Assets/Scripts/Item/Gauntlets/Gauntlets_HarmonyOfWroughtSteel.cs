using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_HarmonyOfWroughtSteel : Item
{
    public Gauntlets_HarmonyOfWroughtSteel(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
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
