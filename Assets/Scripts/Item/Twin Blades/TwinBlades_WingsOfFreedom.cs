using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_WingsOfFreedom : Item
{
    public TwinBlades_WingsOfFreedom(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(110, 40, 1.2f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Chained)},EffectTypeName="ConsumeChainedOnBA", DescriptionParameters=new List<String>{new float[5]{12, 14, 16, 18, 20}[GradeIndex].ToString(), (GetFirstModifierEffectValue() * 0.4f).ToString()}, CustomParam = new float[5]{12, 14, 16, 18, 20}[GradeIndex], CustomParam2 = GetFirstModifierEffectValue() * 0.4f, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
            damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Chained))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            Effect_Chained chained = (Effect_Chained)damage.TargetOfDamage.GetEffect(typeof(Effect_Chained));
            chained.AddDecayingAmount(chained.DecayingAmount < effect.CustomParam ? -chained.DecayingAmount : -effect.CustomParam);
            damage.Injury += effect.CustomParam2;
            damage.Stagger += effect.CustomParam2;
        })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ConsumeChainedOnTechnique", DescriptionParameters=new List<String>{new float[5]{24, 28, 32, 36, 40}[GradeIndex].ToString(), (GetFirstModifierEffectValue() * 0.8f).ToString()}, CustomParam = new float[5]{24, 28, 32, 36, 40}[GradeIndex], CustomParam2 = GetFirstModifierEffectValue() * 0.8f, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
            damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Chained))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            Effect_Chained chained = (Effect_Chained)damage.TargetOfDamage.GetEffect(typeof(Effect_Chained));
            chained.AddDecayingAmount(chained.DecayingAmount < effect.CustomParam ? -chained.DecayingAmount : -effect.CustomParam);
            damage.Injury += effect.CustomParam2;
            damage.Stagger += effect.CustomParam2;
        })}};
    }
}

