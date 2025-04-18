using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_GainEffectWhileUnderAnotherEffect : Effect
{
    public Type EffectNeededToCreateEffect;
    public Effect CreatedEffect;
    public float PercentageAmount;
    public Effect_GainEffectWhileUnderAnotherEffect(Type effect_needed, SourceOfEffect source_of_effect) : base(source_of_effect) {
        EffectNeededToCreateEffect = effect_needed;
        Type = EffectType.Buff;
        Listeners.Add(EventManager.EffectStarted);
        Listeners.Add(EventManager.EffectEnded);
        TriggersOncePerAbility = true;
    }

    public Func<Effect_GainEffectWhileUnderAnotherEffect, Effect> CreateEffect;

    public void Activate() {
        if(EffectEnded && CreatedEffect != null && CreatedEffect.EffectEnded == false) {
            CreatedEffect.EndThisEffect();
        }
        else if(!EffectEnded && Player.Instance.CheckIfUnderEffect(EffectNeededToCreateEffect)) {
            if(CreatedEffect != null && CreatedEffect.EffectEnded == false) {
                CreatedEffect.EndThisEffect();
            }
            CreatedEffect = CreateEffect.Invoke(this);
            Player.Instance.AddEffect(CreatedEffect);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        Activate();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Activate();
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        if(effect.TargetOfEffect != Player.Instance || effect.GetType() != EffectNeededToCreateEffect) {
            return;
        }
        base.OnInvokeEffectStarted(effect);
        Activate();
    }

    public override void OnInvokeEffectEnded(Effect effect)
    {
        if(effect.TargetOfEffect != Player.Instance || effect.GetType() != EffectNeededToCreateEffect) {
            return;
        }
        base.OnInvokeEffectEnded(effect);
        Activate();
    }
}
