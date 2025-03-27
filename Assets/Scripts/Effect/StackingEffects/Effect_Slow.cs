using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Slow : Effect
{
    public int SlowLevel = 0;

    public Effect_ChangeStat MSSlowEffect;
    public Effect_Slow(float slow_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialDecayingAmount = slow_amount;
        ShowsInUI = true;
        DefaultDecaySpeed = 0.25f;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        if(MSSlowEffect == null) {
            return;
        }
        float effectiveAmount = DecayingAmount > 80 ? 80 : DecayingAmount;
        EffectIndicatorText = Utils.GetFormattedFloat(effectiveAmount) + "%";
        MSSlowEffect.PercentageAmount = -effectiveAmount;
    }

    public override void OnStart()
    {
        base.OnStart();
        MSSlowEffect = new Effect_ChangeStat(TargetOfEffect.MovementSpeed, SourceOfEffect) {ShowsInMenu=false};
        TargetOfEffect.AddEffect(MSSlowEffect);
        BaseDuration = 5;
        ChangeDecayingAmount(DecayingAmount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        MSSlowEffect.EndThisEffect();
    }
}
