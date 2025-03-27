using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_FracturedCleavers : Item
{
    public Daggers_FracturedCleavers(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(95, 80, 1.1f);
    }
    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="GainAccelerationOnWeaponTypeHit", CustomParam = GetFirstModifierEffectValue() * 0.05f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.05f, 1)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Light && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.InjuryDealt > 0 || damage.StaggerDealt > 0) {
                        Player.Instance.AddEffect(new Effect_Acceleration(effect.CustomParam, new(this)));
                    }
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
    /*public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_GainASForDistancelTravelled(GetFirstModifierEffectValue() * 0.05f, new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Acceleration)},DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.05f, 5)}}};
    }

    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="GainMSOnBA", CustomParam = GetSecondModifierEffectValue() * 0.1f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 0.1f, 5)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
            (damage.SourceOfDamage?.User is Player && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            damage.SourceOfDamage.User.AddEffect(new Effect_ChangeStat(damage.SourceOfDamage.User.MovementSpeed, new(this)) {PercentageAmount = GetSecondModifierEffectValue() * 0.1f}, 5);
        })}};
    }*/
}

