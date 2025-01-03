using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_HealMissingHealth : Effect {
    public float HealAmount = 0;

    public Effect_HealMissingHealth(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if(TargetOfEffect.Health.Current == TargetOfEffect.Health.Maximum) {
            return;
        }
        TargetOfEffect.Health.Current += (TargetOfEffect.Health.Maximum - TargetOfEffect.Health.Current) / 50 * HealAmount / 100;
    }
}
