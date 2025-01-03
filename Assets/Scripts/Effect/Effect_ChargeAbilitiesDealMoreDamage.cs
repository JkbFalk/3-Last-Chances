using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ChargeAbilitiesDealMoreDamage : Effect
{
    public float MoreInjuryPercentage = 0;
    public float MoreStaggerPercentage = 0;

    public Effect_ChargeAbilitiesDealMoreDamage(float health_damage_percentage, float stagger_damage_percentage, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        MoreInjuryPercentage = health_damage_percentage;
        MoreStaggerPercentage = stagger_damage_percentage;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        MoreInjuryPercentage = 1.25f * LinearEffectValue;
        MoreStaggerPercentage = 1.25f * LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(MoreInjuryPercentage), Utils.GetFormattedFloat(MoreInjuryPercentage) };
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if (damage.SourceOfDamage.IsChargeAbility && MoreInjuryPercentage > 0)
        {
            damage.ExtraInjuryDealtPercentage += MoreInjuryPercentage;
            base.OnInvokeHitDealt(damage);
        }
        if (damage.SourceOfDamage.IsChargeAbility && MoreStaggerPercentage > 0)
        {
            damage.ExtraStaggerDealtPercentage += MoreStaggerPercentage;
            base.OnInvokeHitDealt(damage);
        }
    }
}
