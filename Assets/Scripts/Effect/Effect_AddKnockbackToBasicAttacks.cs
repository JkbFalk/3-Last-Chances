using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_AddKnockbackToBasicAttacks : Effect
{
    public float KnockbackAmount = 0;
    public Effect_AddKnockbackToBasicAttacks(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.HitDealt);
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnEffectValueChanged()
    {
        KnockbackAmount = 5 * NonLinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(KnockbackAmount / 200, 1) };
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsBasicAttack)
        {
            damage.Knockback += KnockbackAmount;
            base.OnInvokeHitDealt(damage);
        }
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsBasicAttack)
        {
            damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, SourceOfEffect));
            base.OnInvokeDamageDealt(damage);
        }
    }
}
