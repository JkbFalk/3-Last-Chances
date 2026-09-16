using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Lethargy : Effect
{
    public Effect_ChangeCompositeStat ASBuffEffect;
    public Effect_Lethargy(float speedup, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        MaxDecayingAmount = 100;
        _initialDecayingAmount = speedup;
        ShowsInUI = true;
        PathToUIGraphic = "UI/AttackSpeed";
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackDecayingAmount;
    }

    public override void ExtraBehaviourOnDecayingAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        if(ASBuffEffect == null) {
            return;
        }
        UIText = Utils.GetFormattedFloat(DecayingAmount, 0);
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
