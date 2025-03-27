using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_GainPortionOfStackingEffectAsStat : Effect {
    public float StatGainedPer10StackingEffectAmount;
    public Stat AffectedStat;
    public Effect_ChangeStat StatBuff;
    public Type StackingEffectToTakeFrom;
    public float MaxAmount;

    public Effect_GainPortionOfStackingEffectAsStat(Stat affected_stat, Type stacking_effect, float stat_per_10_stacking_amount, float max_amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        StatGainedPer10StackingEffectAmount = stat_per_10_stacking_amount;
        AffectedStat = affected_stat;
        StackingEffectToTakeFrom = stacking_effect;
        MaxAmount = max_amount;
        Type = EffectType.Buff;
        Listeners.Add(EventManager.EffectStarted);
        Listeners.Add(EventManager.EffectEnded);
        Listeners.Add(EventManager.EffectDecayingAmountChanged);
        TriggersOncePerAbility = true;
    }

    public override void OnEffectValueChanged()
    {
        StatGainedPer10StackingEffectAmount *= NonLinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(StatGainedPer10StackingEffectAmount, 1), Label.Get("Effect_" + AffectedStat), Label.Get( StackingEffectToTakeFrom.ToString()), Utils.GetFormattedFloat(MaxAmount)};
    }

    public void Activate() {
        if(EffectEnded && StatBuff != null && StatBuff.EffectEnded == false) {
            StatBuff.EndThisEffect();
        }
        else if(!EffectEnded && Player.Instance.CheckIfUnderEffect(StackingEffectToTakeFrom)) {
            if(StatBuff != null && StatBuff.EffectEnded == false) {
                StatBuff.EndThisEffect();
            }
            Effect stacking_effect = Player.Instance.GetEffect(StackingEffectToTakeFrom);
            float amount = stacking_effect.DecayingAmount / 10 * StatGainedPer10StackingEffectAmount > MaxAmount ? MaxAmount : stacking_effect.DecayingAmount / 10 * StatGainedPer10StackingEffectAmount;
            StatBuff = new Effect_ChangeStat(AffectedStat, SourceOfEffect) {ShowsInUI=true, PathToEffectGraphic="UI/" + AffectedStat.ToString(), EffectIndicatorText=Utils.GetFormattedFloat(amount) + "%", PercentageAmount = amount};
            Player.Instance.AddEffect(StatBuff);
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
        if(effect.TargetOfEffect != Player.Instance || effect.GetType() != StackingEffectToTakeFrom) {
            return;
        }
        base.OnInvokeEffectStarted(effect);
        Activate();
    }

    public override void OnInvokeEffectEnded(Effect effect)
    {
        Debug.Log($"OnInvokeEffectEnded {effect.TargetOfEffect}, {effect.GetType()}");
        if(effect.TargetOfEffect != Player.Instance || effect.GetType() != StackingEffectToTakeFrom) {
            return;
        }
        base.OnInvokeEffectEnded(effect);
        Activate();
    }

    public override void OnInvokeEffectDecayingAmountChanged(Effect effect)
    {
        Debug.Log($"OnInvokeEffectDecayingAmountChanged {effect.TargetOfEffect}, {effect.GetType()}, {effect.DecayingAmount}");
        if(effect.TargetOfEffect != Player.Instance || effect.GetType() != StackingEffectToTakeFrom) {
            return;
        }
        base.OnInvokeEffectDecayingAmountChanged(effect);
        Activate();
    }
}