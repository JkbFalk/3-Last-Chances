using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_BasicAttacksInflictStun : Effect
{
    public float StunDuration = 0.03125f;
    public int MaxKnockbackVsLevel = 1;

    public Effect_BasicAttacksInflictStun(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        StunDuration *= NonLinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(StunDuration) };
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if (damage.SourceOfDamage.IsBasicAttack)
        {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(SourceOfEffect), StunDuration);
            base.OnInvokeHitDealt(damage);
        }
    }
}
