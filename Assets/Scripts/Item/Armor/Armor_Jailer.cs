using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Jailer : Item
{
    public Armor_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_GainDamageAndDamageReductionForEachDebuff(new(this)) {DamageGainedPerDebuff = 0.125f, DamageReductionGainedPerDebuff = 0.0625f, MaxDebuffs = 10} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, GetSecondModifierEffectValue(false) * 0.5f, new(this)) };
    }
}
