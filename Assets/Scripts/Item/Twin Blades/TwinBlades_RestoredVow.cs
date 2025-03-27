using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_RestoredVow : Item
{
    public TwinBlades_RestoredVow(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(80, 80, 1.4f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ApplyAnalyzedOnBAs", CustomParam = GetFirstModifierEffectValue() * 0.1875f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.1875f)}, ConditionCheckAfterHitDamageCalculation = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && damage.AbilityDamageSource.DamageType == Constants.DamageType.Light)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Analysis(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}

