using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_SealedBlade : Item
{
    public Longblade_SealedBlade(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        
        SetBaseWeaponStats(100, 150, 0.7f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Chained)},EffectTypeName="ApplyChainedOnAbilityHit", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 1.5f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 1.5f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
            (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.IsTechnique)),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            damage.TargetOfDamage.AddEffect(new Effect_Chained(effect.CustomParam, new(this)));
        })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ApplyChainedOnCounter", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 1.5f).ToString(), (GetFirstModifierEffectValue() * 1.5f * 2).ToString()}, CustomParam = GetFirstModifierEffectValue() * 1.5f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
            (damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.IsRiposte || damage.SourceOfDamage.IsCounter))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            damage.TargetOfDamage.AddEffect(new Effect_Chained(effect.CustomParam * (damage.SourceOfDamage.IsCounter ? 2 : 1), new(this)));
        })}};
    }
}
