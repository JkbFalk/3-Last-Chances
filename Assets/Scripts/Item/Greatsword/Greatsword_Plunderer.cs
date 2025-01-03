using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Plunderer : Item
{
    public Greatsword_Plunderer(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(110, 130, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_Plunderer(GetFirstModifierEffectValue() * 2.5f, new(this)) {DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 2.5f), "20", "60"}} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.EnergyGain, new(this)) { PercentageAmount = 0.5f}};
    }
}
