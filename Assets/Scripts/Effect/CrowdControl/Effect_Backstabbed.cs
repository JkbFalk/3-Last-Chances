using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Effect_Backstabbed : Effect_HardCrowdControl
{
    private const float END_ANIMATION_LEAD_TIME = 0.5f;
    private bool _playedEndAnimation = false;

    public Effect_Backstabbed(SourceOfEffect source_of_effect) : base(source_of_effect) 
    {
        CanBeNegatedByImmunityToCrowdControl = false;
        PriorityLevel = 8;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_playedEndAnimation && RemainingDuration <= END_ANIMATION_LEAD_TIME && !EffectEnded) {
            _playedEndAnimation = true;
            if (TargetOfEffect != null && !TargetOfEffect.KnockedOut && TargetOfEffect.gameObject.activeInHierarchy)
            {
                TargetOfEffect.PlayAnimation("Backstabbed_EndEffect", 0.05f);
            }
        }
    }
}