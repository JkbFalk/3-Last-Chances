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
        MaxAmount = 100;
        _initialAmount = speedup;
        ShowsInUI = true;
        PathToUIGraphic = "UI/AttackSpeed";
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        if(ASBuffEffect == null) {
            return;
        }
        UIText = Utils.GetFormattedFloat(Amount, 0);
        ASBuffEffect.PercentageModifier = Amount;
    }

    public override void OnStart()
    {
        base.OnStart();
        ASBuffEffect = new Effect_ChangeCompositeStat(TargetOfEffect, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect);
        TargetOfEffect.AddEffect(ASBuffEffect);
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        ChangeAmount(Amount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ASBuffEffect.EndThisEffect();
    }
}
