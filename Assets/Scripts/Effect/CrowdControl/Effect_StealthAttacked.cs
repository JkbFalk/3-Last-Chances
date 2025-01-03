using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_StealthAttacked : Effect_HardCrowdControl
{
    public Effect_StealthAttacked(SourceOfEffect source_of_effect) : base(source_of_effect) {
        CanBeNegatedByImmunityToCrowdControl = false;
        PriorityLevel = 8;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        List<Effect> HardCrowdControlEffects = TargetOfEffect.CurrentEffects.Where(effect => effect.AutoPlayEffectAnimation && effect != this).ToList();
        Effect HardCrowdControlEffect = null;
        if (HardCrowdControlEffects != null && HardCrowdControlEffects.Count > 0)
        {
            HardCrowdControlEffect = HardCrowdControlEffects.Aggregate((e1, e2) => e1.PriorityLevel > e2.PriorityLevel ? e1 : e2);
        }
        if (TargetOfEffect.Health.Current > 0 && TargetOfEffect.CurrentHealthBars > 0)
        {
            TargetOfEffect.PlayAnimation("StealthAttacked_EndEffect");
        }
    }
}