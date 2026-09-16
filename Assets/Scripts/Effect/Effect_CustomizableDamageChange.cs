using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Effect_CustomizableDamageChange : Effect
{
    public float ArmorModifier = 0;
    public float ArmorPenetrationModifier = 0;
    public float DamagePercentageModifier = 0;
    public float InjuryPercentageModifier = 0;
    public float StaggerPercentageModifier = 0;
    public Func<DamageInstance, Effect_CustomizableDamageChange, bool> ConditionCheckOnHitDealt;
    public Func<DamageInstance, Effect_CustomizableDamageChange, bool> ConditionCheckAfterHitDamageCalculation;
    public Func<DamageInstance, Effect_CustomizableDamageChange, bool> ConditionCheckOnAboutToHandleFatalBlow;
    public Func<DamageInstance, Effect_CustomizableDamageChange, bool> ConditionCheckOnDamageDealt;
    public Action<DamageInstance, Effect_CustomizableDamageChange> Action = new Action<DamageInstance, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            if(effect.DamagePercentageModifier != 0) {
                damage.InjuryDealtPercentageModifier += effect.DamagePercentageModifier;
                damage.StaggerDealtPercentageModifier += effect.DamagePercentageModifier;
            }
            if(effect.InjuryPercentageModifier != 0) {
                damage.InjuryDealtPercentageModifier += effect.InjuryPercentageModifier;
            }
            if(effect.StaggerPercentageModifier != 0) {
                damage.StaggerDealtPercentageModifier += effect.StaggerPercentageModifier;
            }
            if(effect.ArmorModifier != 0) {
                damage.ArmorModifier += effect.ArmorModifier;
            }
            if(effect.ArmorPenetrationModifier != 0) {
                damage.ArmorPenetrationModifier += effect.ArmorPenetrationModifier;
            }
        });

    public Effect_CustomizableDamageChange(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners = new List<UnityEventBase> {EventManager.HitDealt, EventManager.AfterHitDamageCalculation, EventManager.DamageDealt};
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if(ConditionCheckOnHitDealt != null && ConditionCheckOnHitDealt.Invoke(damage, this)){
            Action.Invoke(damage, this);
            base.OnInvokeHitDealt(damage);
        }
    }

    public override void OnInvokeAfterHitDamageCalculation(DamageInstance damage)
    {
        if(ConditionCheckAfterHitDamageCalculation != null && ConditionCheckAfterHitDamageCalculation.Invoke(damage, this)){
            Action.Invoke(damage, this);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }    

    public override void OnInvokeAboutToHandleFatalBlow(DamageInstance damage)
    {
        if(ConditionCheckOnAboutToHandleFatalBlow != null && ConditionCheckOnAboutToHandleFatalBlow.Invoke(damage, this)){
            Action.Invoke(damage, this);
            base.OnInvokeAboutToHandleFatalBlow(damage);
        }
    }    

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if(ConditionCheckOnDamageDealt != null && ConditionCheckOnDamageDealt.Invoke(damage, this)){
            Action.Invoke(damage, this);
            base.OnInvokeDamageDealt(damage);
        }
    }   
}
