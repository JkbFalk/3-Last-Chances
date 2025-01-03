using System;
using UnityEngine;

public class Effect_IncreaseDamageDealt : Effect {
    public Constants.DamageType RequiredCategory = Constants.DamageType.None;
    public float Amount;
    public Effect_IncreaseDamageDealt(float amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Amount = amount;
        DisplayEffectIndicator = true;
        PathToEffectGraphic = "UI/Damage";
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.SourceOfDamage.User == TargetOfEffect && (RequiredCategory == Constants.DamageType.None || (damage.AbilityDamageSource.DamageType != Constants.DamageType.None && damage.AbilityDamageSource.DamageType == RequiredCategory))) {
            damage.Injury = damage.Injury + damage.Injury * Amount / 100;
            damage.Stagger = damage.Stagger + damage.Stagger * Amount / 100;
            base.OnInvokeHitDealt(damage);
        } 
    }
}