using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_LifestealBelowHealthThreshold : Effect
{
    public float LifestealAmount = 0;  
    public float HealthThreshold = 0;
    public Constants.DamageType DamageCategory;

    public Effect_LifestealBelowHealthThreshold(float health_threshold, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        HealthThreshold = health_threshold;
        Listeners.Add(EventManager.HitDealt);
        RemainsActiveInOtherStances = true;
    }


    public override void OnEffectValueChanged()
    {
        LifestealAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(HealthThreshold), Label.Get("WeaponCategory_" + DamageCategory), Utils.GetFormattedFloat(LifestealAmount) };
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User == TargetOfEffect && TargetOfEffect.Health.Current / TargetOfEffect.Health.Maximum < HealthThreshold / 100) {
            damage.Injury += LifestealAmount;
            TargetOfEffect.Health.Current += LifestealAmount;
            base.OnInvokeHitDealt(damage);
        }
    }
}
 