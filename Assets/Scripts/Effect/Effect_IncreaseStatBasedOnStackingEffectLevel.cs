using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_IncreaseStatBasedOnStackingEffectLevel : Effect
{
    private int _lastCheckedIntensityLevel = 0;
    private Effect_ChangeStat _buff;
    public Type StackingEffectToCheck;
    public Stat IncreasedStat;
    public float PercentageAmount = 0;
    public bool IncreaseBasedOnEffectLevel = true;
    public Effect_IncreaseStatBasedOnStackingEffectLevel(Type stacking_effect_to_check, Stat stat_to_increase, float percentage_amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        StackingEffectToCheck = stacking_effect_to_check;
        IncreasedStat = stat_to_increase;
        PercentageAmount = percentage_amount;
        Type = EffectType.Buff;
        Listeners.AddRange(new List<UnityEngine.Events.UnityEventBase> {EventManager.EffectStarted, EventManager.EffectDecayingAmountChanged, EventManager.EffectEnded});
    }

    public override void OnInvokeEffectStarted(Effect effect_started)
    {
        UpdateEffect(effect_started);
    }

    public override void OnInvokeEffectDecayingAmountChanged(Effect effect_started)
    {
        UpdateEffect(effect_started);
    }

    public override void OnInvokeEffectEnded(Effect effect_started)
    {
        UpdateEffect(effect_started);
    }
    public void UpdateEffect(Effect effect) {
        if(effect.GetType() != StackingEffectToCheck) {
            return;
        }
        else if(effect.EffectEnded && _buff.EffectEnded == false) {
            _buff.EndThisEffect();
        }
        if(IncreaseBasedOnEffectLevel) {
            if(effect.EffectEnded == false && _buff != null && _buff.EffectEnded == false && effect.StackingEffectIntensityLevel != _lastCheckedIntensityLevel) {
                _lastCheckedIntensityLevel = effect.StackingEffectIntensityLevel;
                _buff.PercentageModifier = !IncreaseBasedOnEffectLevel ? effect.DecayingAmount * PercentageAmount / 100 : (_lastCheckedIntensityLevel == 1 ? PercentageAmount * 0.5f :_lastCheckedIntensityLevel == 2 ? PercentageAmount : _lastCheckedIntensityLevel == 3 ? PercentageAmount * 2 : 0);
            }
            else if(effect.EffectEnded == false && _buff == null) {
                _buff = new Effect_ChangeStat(IncreasedStat, SourceOfEffect) {
                    PercentageModifier = !IncreaseBasedOnEffectLevel ? effect.DecayingAmount * PercentageAmount / 100 : (_lastCheckedIntensityLevel == 1 ? PercentageAmount * 0.5f :_lastCheckedIntensityLevel == 2 ? PercentageAmount : _lastCheckedIntensityLevel == 3 ? PercentageAmount * 2 : 0),
                    IsRemovable = false,
                };
                TargetOfEffect.AddEffect(_buff);
            }
        }
    }
}
