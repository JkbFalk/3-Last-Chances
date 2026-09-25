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
        _initialAmount = slow_amount;
        ShowsInUI = true;
        DefaultDecaySpeed = 0.25f;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        if (MSSlowEffect == null)
        {
            return;
        }
        UIText = Utils.GetFormattedFloat(Amount, 0);
        MSSlowEffect.FlatAmount = -Amount;
    }

    public override void OnStart()
    {
        base.OnStart();
        MSSlowEffect = new Effect_ChangeStat(TargetOfEffect.MovementSpeed, SourceOfEffect) {
            ShowsInMenu = false
        };
        TargetOfEffect.AddEffect(MSSlowEffect);
        BaseDuration = 5;
        ChangeAmount(Amount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        MSSlowEffect.EndThisEffect();
    }
}
