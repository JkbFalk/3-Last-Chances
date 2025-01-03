using System;
using UnityEngine;

public class Effect_IncreaseDamageFromGivenAbilityType : Effect {

    public float Amount;
    public Func<Damage, bool> CheckIfShouldIncrease;
    public Type AbilityType;

    public Effect_IncreaseDamageFromGivenAbilityType(float amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Amount = amount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnInvokeHitDealt(Damage damage) {
        if(damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.GetType().IsSubclassOf(AbilityType) && (CheckIfShouldIncrease == null || CheckIfShouldIncrease(damage))) {
            damage.ExtraInjuryDealtPercentage += Amount;
            damage.ExtraStaggerDealtPercentage += Amount;
            base.OnInvokeHitDealt(damage);
        }
    }
}