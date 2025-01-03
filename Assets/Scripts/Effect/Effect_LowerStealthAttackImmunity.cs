using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_LowerStealthAttackImmunity : Effect
{
    public float ImmunityDurationReductionInSeconds;

    public Effect_LowerStealthAttackImmunity(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
    }

    public override void OnEffectValueChanged()
    {
        ImmunityDurationReductionInSeconds = LinearEffectValue * 0.25f;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(ImmunityDurationReductionInSeconds) };
    }

    public override void OnStart()
    {
        base.OnStart();
        Player.Instance.StealthAttackCooldown -= ImmunityDurationReductionInSeconds;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Player.Instance.StealthAttackCooldown += ImmunityDurationReductionInSeconds;
    }
}
