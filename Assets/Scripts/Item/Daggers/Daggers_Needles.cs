using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_Needles : Item
{
    public Daggers_Needles(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(120, 0, 1.25f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_InflictBleedOnBAs(0.05f, new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Bleed)}} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeEffectPower(typeof(Effect_Bleed), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -GetSecondModifierEffectValue() / 2, new(this)) };
    }
}
