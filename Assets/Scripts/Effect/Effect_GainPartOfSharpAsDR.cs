using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_GainPartOfSharpAsDR : Effect {
    public float DRGainedPer10Sharp = 0.03125f;
    public Effect_ChangeStat DamageReductionBuff;

    public Effect_GainPartOfSharpAsDR(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.EffectStarted);
        Listeners.Add(EventManager.EffectEnded);
        Listeners.Add(EventManager.EffectDecayingAmountChanged);
        TriggersOncePerAbility = true;
    }

    public override void OnEffectValueChanged()
    {
        DRGainedPer10Sharp *= NonLinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(DRGainedPer10Sharp, 1)};
    }

    public void Activate() {
        if(EffectEnded && DamageReductionBuff != null && DamageReductionBuff.EffectEnded == false) {
            Debug.Log($"Activate1 {EffectEnded}, {DamageReductionBuff}, {DamageReductionBuff?.EffectEnded}");
            DamageReductionBuff.EndThisEffect();
        }
        else if(!EffectEnded && Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))) {
            if(DamageReductionBuff != null && DamageReductionBuff.EffectEnded == false) {
                DamageReductionBuff.EndThisEffect();
            }
            Effect_Sharp sharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
            DamageReductionBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {DisplayEffectIndicator=true, PathToEffectGraphic="UI/DamageReduction", EffectIndicatorText=Utils.GetFormattedFloat(sharp.DecayingAmount / 10 * DRGainedPer10Sharp) + "%", PercentageAmount = sharp.DecayingAmount / 10 * DRGainedPer10Sharp};
            Player.Instance.AddEffect(DamageReductionBuff);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        Activate();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Activate();
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        Debug.Log($"OnInvokeEffectStarted {effect.TargetOfEffect}, {effect.GetType()}");
        if(effect.TargetOfEffect != Player.Instance || effect is not Effect_Sharp) {
            return;
        }
        base.OnInvokeEffectStarted(effect);
        Activate();
    }

    public override void OnInvokeEffectEnded(Effect effect)
    {
        Debug.Log($"OnInvokeEffectEnded {effect.TargetOfEffect}, {effect.GetType()}");
        if(effect.TargetOfEffect != Player.Instance || effect is not Effect_Sharp) {
            return;
        }
        base.OnInvokeEffectEnded(effect);
        Activate();
    }

    public override void OnInvokeEffectDecayingAmountChanged(Effect effect)
    {
        Debug.Log($"OnInvokeEffectDecayingAmountChanged {effect.TargetOfEffect}, {effect.GetType()}, {effect.DecayingAmount}");
        if(effect.TargetOfEffect != Player.Instance || effect is not Effect_Sharp) {
            return;
        }
        base.OnInvokeEffectDecayingAmountChanged(effect);
        Activate();
    }
}