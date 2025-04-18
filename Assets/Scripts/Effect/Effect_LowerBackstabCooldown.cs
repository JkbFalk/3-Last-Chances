using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_LowerBackstabImmunity : Effect
{
    public float ImmunityDurationReductionInSeconds;

    public Effect_LowerBackstabImmunity(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
    }

    public override void OnStart()
    {
        base.OnStart();
        Player.Instance.BackstabCooldown -= ImmunityDurationReductionInSeconds;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Player.Instance.BackstabCooldown += ImmunityDurationReductionInSeconds;
    }
}
