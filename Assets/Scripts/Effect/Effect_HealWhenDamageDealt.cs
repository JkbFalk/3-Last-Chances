using System;
using System.Collections.Generic;
using UnityEngine;

public class Effect_HealWhenDamageDealt : Effect {
    public float HealAmount = 0;
    public Func<Damage, bool> ConditionCheck;
    public Action<Damage, Effect_HealWhenDamageDealt> ActionOnDamage;

    public Effect_HealWhenDamageDealt(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if (ConditionCheck.Invoke(damage))
        {
            ActionOnDamage.Invoke(damage, this);
            base.OnInvokeDamageDealt(damage);
        }
    }
}