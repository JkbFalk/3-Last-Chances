using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_DoubleDown : Item
{
    public Gun_DoubleDown(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(125, 125, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="DealExtraDamageWithOtherDamage", CustomParam = GetFirstModifierEffectValue() * 0.75f, DescriptionParameters = new List<String> {"10", "[RI]", "[RS]", Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.75f)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.DamageType == Constants.DamageType.Ranged)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.InjuryDealt > 0 && !(damage.SourceOfDamage is Ability_SourcelessDamage)) {
                        new Damage(damage.TargetOfDamage, new Ability_SourcelessDamage(damage.SourceOfDamage.User), null).SetDamageSource(0, damage.InjuryDealt * 0.1f > effect.CustomParam ? effect.CustomParam : damage.InjuryDealt * 0.1f, Constants.DamageType.None).CalculateDamage();
                    }
                })}, 
            new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="DealExtraDamageWithOtherDamage", CustomParam = GetFirstModifierEffectValue() * 0.75f, DescriptionParameters = new List<String> {"10", "[RS]", "[RI]", Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.75f)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.DamageType == Constants.DamageType.Ranged)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.StaggerDealt > 0 && !(damage.SourceOfDamage is Ability_SourcelessDamage)) {
                        new Damage(damage.TargetOfDamage, new Ability_SourcelessDamage(damage.SourceOfDamage.User), null).SetDamageSource(damage.StaggerDealt * 0.1f > effect.CustomParam ? effect.CustomParam : damage.StaggerDealt * 0.1f, 0, Constants.DamageType.None).CalculateDamage();
                    }
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.RangedInjury, new(this)) { PercentageAmount = 0.75f }, new Effect_ChangeStat(Player.Instance.RangedStagger, new(this)) { PercentageAmount = 0.75f }};
    }
}
