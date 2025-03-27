using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Effect_CustomizableDamageChange : Effect
{
    public float CustomParam = 0;
    public float CustomParam2 = 0;
    public float DamageReductionChange = 0;
    public float PenetrationChange = 0;
    public float MultiplierChange = 0;
    public float InjuryPercentageChange = 0;
    public float StaggerPercentageChange = 0;
    public Func<Damage, Effect_CustomizableDamageChange, bool> ConditionCheckOnHitDealt;
    public Func<Damage, Effect_CustomizableDamageChange, bool> ConditionCheckAfterHitDamageCalculation;
    public Func<Damage, Effect_CustomizableDamageChange, bool> ConditionCheckOnDamageDealt;
    public Action<Damage, Effect_CustomizableDamageChange> Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            if(effect.InjuryPercentageChange != 0) {
                damage.ExtraInjuryDealtPercentage += effect.InjuryPercentageChange;
            }
            if(effect.StaggerPercentageChange != 0) {
                damage.ExtraStaggerDealtPercentage += effect.StaggerPercentageChange;
            }
            if(effect.MultiplierChange != 0) {
                damage.DamageDealtMultiplier += effect.MultiplierChange;
            }
            if(effect.DamageReductionChange != 0) {
                damage.ExtraDamageReduction += effect.DamageReductionChange;
            }
        });

    public Effect_CustomizableDamageChange(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners = new List<UnityEventBase> {EventManager.HitDealt, EventManager.AfterHitDamageCalculation, EventManager.DamageDealt};
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(ConditionCheckOnHitDealt != null && ConditionCheckOnHitDealt.Invoke(damage, this)){
            Action.Invoke(damage, this);
            base.OnInvokeHitDealt(damage);
        }
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage)
    {
        if(ConditionCheckAfterHitDamageCalculation != null && ConditionCheckAfterHitDamageCalculation.Invoke(damage, this)){
            Action.Invoke(damage, this);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }    

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(ConditionCheckOnDamageDealt != null && ConditionCheckOnDamageDealt.Invoke(damage, this)){
            Action.Invoke(damage, this);
            base.OnInvokeDamageDealt(damage);
        }
    }   
}
