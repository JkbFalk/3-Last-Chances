using System;
using UnityEngine;

public class Effect_TakeDecreasedDamage : Effect {

    private float _decreaseAmount;
    public Effect_TakeDecreasedDamage(float decrease_amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        _decreaseAmount = decrease_amount;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage == TargetOfEffect) {
            damage.DamageDealtMultiplier = damage.DamageDealtMultiplier * 0.5f;
            base.OnInvokeHitDealt(damage);
        }
    }
}