using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class Effect_Poison : Effect
{
    public float EnergyLossPerSecond;
    public Stat.StatModifier Regen;
    public Effect_ChangeStat CDIncreasedEffect;
    public float AggroModifier;
    public Effect_Poison(float poison_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        ShowsInUI = true;
        _initialDecayingAmount = poison_amount;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackDecayingAmount;
    }

    public override void ExtraBehaviourOnDecayingAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        if((TargetOfEffect is Player && Regen == null) || (TargetOfEffect is not Player && CDIncreasedEffect == null)) {
            return;
        }
        if(TargetOfEffect is Player) {
            Regen.Amount = DecayingAmount > 10 ? 10 : DecayingAmount;
            UIText = Utils.GetFormattedFloat(Regen.Amount, 0);
        }
        else {
            CDIncreasedEffect.PercentageAmount = DecayingAmount > 100 ? -100 : -DecayingAmount;
            TargetOfEffect.UnitAI.AggressivenessModifier += AggroModifier;
            TargetOfEffect.UnitAI.AggressivenessModifier = DecayingAmount > 100 ? -1 : -DecayingAmount / 100;
            TargetOfEffect.UnitAI.AggressivenessModifier -= AggroModifier;
            UIText = Utils.GetFormattedFloat(DecayingAmount > 100 ? 100 : DecayingAmount, 0);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        if(TargetOfEffect is Player) {
            TargetOfEffect.Energy.AddPercentageRegeneration(this, -EnergyLossPerSecond);
            Regen = TargetOfEffect.Energy.PercentageRegeneration.FirstOrDefault(regen => regen.Source == this && regen.Amount == -EnergyLossPerSecond);
        }
        else {
            CDIncreasedEffect = new(TargetOfEffect.CooldownReduction, SourceOfEffect) {PercentageAmount = DecayingAmount > 100 ? -100 : -DecayingAmount};
            TargetOfEffect.AddEffect(CDIncreasedEffect, 30);
            AggroModifier = DecayingAmount > 100 ? -1 : -DecayingAmount / 100;
            TargetOfEffect.UnitAI.AggressivenessModifier -= AggroModifier;
        }   
        ChangeDecayingAmount(DecayingAmount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        if(TargetOfEffect is Player) {
            TargetOfEffect.Energy.RemovePercentageRegeneration(this, -EnergyLossPerSecond);
        }
        else {
            CDIncreasedEffect.EndThisEffect();
            TargetOfEffect.UnitAI.AggressivenessModifier += AggroModifier;
        }
    }
}
