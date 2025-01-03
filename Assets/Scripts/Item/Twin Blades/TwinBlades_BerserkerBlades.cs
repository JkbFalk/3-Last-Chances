using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_BerserkerBlades : Item
{
    public TwinBlades_BerserkerBlades(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(150, 30, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_LifestealBelowHealthThreshold(50, new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Acceleration)},LifestealAmount = 0.375f, DamageCategory = Constants.DamageType.Light } };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="GainAccelerationOnWeaponTypeHit", CustomParam = GetSecondModifierEffectValue() * 0.1f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 0.1f, 1)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
            (damage.SourceOfDamage?.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Light && damage.SourceOfDamage.IsBasicAttack)),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            if(damage.InjuryDealt > 0 || damage.StaggerDealt > 0) {
                Player.Instance.AddEffect(new Effect_Acceleration(effect.CustomParam, new(this)));
            }
        })}};
    }
}

