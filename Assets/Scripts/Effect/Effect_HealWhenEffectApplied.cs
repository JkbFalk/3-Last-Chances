using System;
using System.Collections.Generic;
using UnityEngine;

public class Effect_HealWhenEffectApplied : Effect {
    public float HealAmount = 0;
    public Func<Effect, bool> ConditionCheck;
    public Action<Effect, Effect_HealWhenEffectApplied> ActionOnDamage;

    public Effect_HealWhenEffectApplied(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.EffectStarted);
    }


    public override void OnInvokeEffectStarted(Effect effect)
    {
        if (ConditionCheck.Invoke(effect))
        {
            ActionOnDamage.Invoke(effect, this);
        }
    }
}