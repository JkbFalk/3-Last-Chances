using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_Stiletto : Item
{
    public Daggers_Stiletto(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(90, 80, 1.1f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Acceleration)}, EffectTypeName="GainAccelerationOnWeaponTypeHit", CustomParam = GetFirstModifierEffectValue() * 0.05f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.05f, 1)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Light && damage.SourceOfDamage.IsBasicAttack)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.InjuryDealt > 0 || damage.StaggerDealt > 0) {
                        Player.Instance.AddEffect(new Effect_Acceleration(effect.CustomParam, new(this)));
                    }
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Lethargy)}, EffectTypeName="ApplyLethargyOnWeaponTypeHit", CustomParam = GetSecondModifierEffectValue() * 0.05f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 0.05f, 1)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Light)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.InjuryDealt > 0 || damage.StaggerDealt > 0) {
                        damage.TargetOfDamage.AddEffect(new Effect_Lethargy(effect.CustomParam, new(this)));
                    }
                })}};
    }
}

