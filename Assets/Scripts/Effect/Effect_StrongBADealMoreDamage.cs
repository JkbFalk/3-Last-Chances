using System;
using System.Collections.Generic;
using UnityEngine;

public class Effect_StrongBADealMoreDamage : Effect
{
    public float DamageIncrease = 0;
    public Effect_StrongBADealMoreDamage(float percentage_increase,  SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        DamageIncrease = percentage_increase;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        DamageIncrease *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(DamageIncrease) };
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsStrongBasicAttack)
        {
            damage.ExtraInjuryDealtPercentage += DamageIncrease;
            damage.ExtraStaggerDealtPercentage += DamageIncrease;
            base.OnInvokeHitDealt(damage);
        }
    }
}