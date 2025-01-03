using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_BasicAttacksReduceCooldowns : Effect {
    public float CooldownReductionAmount = 0;

    public Effect_BasicAttacksReduceCooldowns(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        DescriptionParameters.Add(Utils.GetFormattedFloat(CooldownReductionAmount, 1));
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnEffectValueChanged()
    {
        CooldownReductionAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(CooldownReductionAmount, 1) };
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if(damage.InjuryDealt > 0 || damage.StaggerDealt > 0)
        {
            foreach(Cooldown cd in damage.SourceOfDamage.User.AbilityCooldowns.Concat(damage.SourceOfDamage.User.EffectCooldowns).Concat(new List<Cooldown> {damage.SourceOfDamage.User.ItemsCooldown}))
            {
                if(cd != null && cd.RemainingDuration > 0)
                {
                    cd.RemainingDuration -= CooldownReductionAmount;
                }
                else if(cd != null)
                {
                    cd.RemainingDuration = 0;
                }
            }
            base.OnInvokeDamageDealt(damage);
        }
    }
}