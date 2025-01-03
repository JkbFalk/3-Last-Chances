using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_ExtraDamageToUndamagedEnemies : Effect
{
    public float FlatDamageAmount = 0;
    public float FlatStaggerAmount = 0;

    public Effect_ExtraDamageToUndamagedEnemies(float flat_damage_amount, float flat_stagger_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        FlatDamageAmount = flat_damage_amount;
        FlatStaggerAmount = flat_stagger_amount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        FlatDamageAmount *= LinearEffectValue;
        FlatStaggerAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(FlatDamageAmount), Utils.GetFormattedFloat(FlatStaggerAmount) };
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if(FlatDamageAmount > 0 && damage.TargetOfDamage.Health.Current == damage.TargetOfDamage.Health.Maximum)
        {
            damage.Injury += FlatDamageAmount;
            base.OnInvokeHitDealt(damage);
        }
        if (FlatStaggerAmount > 0 && damage.TargetOfDamage.StaggerBar.Current == 0)
        {
            damage.Stagger += FlatStaggerAmount;
            base.OnInvokeHitDealt(damage);
        }
    }
}
