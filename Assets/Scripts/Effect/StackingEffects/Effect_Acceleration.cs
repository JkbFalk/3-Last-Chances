using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Acceleration : Effect
{
    public Effect_ChangeCompositeStat ASBuffEffect;
    public Effect_Acceleration(float speedup, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        MaxDecayingAmount = 100;
        _initialDecayingAmount = speedup;
        ShowsInUI = true;
        PathToEffectGraphic = "UI/AttackSpeed";
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        if(ASBuffEffect == null) {
            return;
        }
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount) + "%";
        ASBuffEffect.PercentageModifier = DecayingAmount;
    }

    public override void OnStart()
    {
        base.OnStart();
        ASBuffEffect = new Effect_ChangeCompositeStat(TargetOfEffect, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect);
        TargetOfEffect.AddEffect(ASBuffEffect);
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        ChangeDecayingAmount(DecayingAmount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ASBuffEffect.EndThisEffect();
    }
}
