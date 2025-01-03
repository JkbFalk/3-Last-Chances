using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Lethargy : Effect
{
    public Effect_ChangeCompositeStat ASSlowEffect;
    public Effect_Lethargy(float slow, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        MaxDecayingAmount = 100;
        _initialDecayingAmount = slow;
        DisplayEffectIndicator = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        DescriptionLabel = "Effect_Lethargy_Explanation";
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        if(ASSlowEffect == null) {
            return;
        }
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount) + "%";
        ASSlowEffect.PercentageAmount = -DecayingAmount / 2;
    }

    public override void OnStart()
    {
        base.OnStart();
        ASSlowEffect = new Effect_ChangeCompositeStat(TargetOfEffect, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect);
        TargetOfEffect.AddEffect(ASSlowEffect);
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        AddDecayingAmount(DecayingAmount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ASSlowEffect.EndThisEffect();
    }
}
