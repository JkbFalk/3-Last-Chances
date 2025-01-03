using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_FarEnemiesTakeExtraDamage : Effect
{
    public float FlatDamageAmount = 0;
    public float FlatStaggerAmount = 0;
    public float MinimumDistance = 3;

    public Effect_FarEnemiesTakeExtraDamage(float flat_damage_amount, float flat_stagger_amount, float min_distance, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        FlatDamageAmount = flat_damage_amount;
        FlatStaggerAmount = flat_stagger_amount;
        MinimumDistance = min_distance;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        FlatDamageAmount *= LinearEffectValue;
        FlatStaggerAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(MinimumDistance), Utils.GetFormattedFloat(FlatDamageAmount), Utils.GetFormattedFloat(FlatStaggerAmount) };
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if(Vector2.Distance(damage.TargetOfDamage.transform.position, damage.SourceOfDamage.User.transform.position) > MinimumDistance)
        {
            if (FlatDamageAmount > 0)
            {
                damage.Injury += FlatDamageAmount;
            }
            if (FlatStaggerAmount > 0)
            {
                damage.Stagger += FlatStaggerAmount;
            }
            base.OnInvokeHitDealt(damage);
        }    
    }
}
