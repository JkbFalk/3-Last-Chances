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
        _initialAmount = poison_amount;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        if((TargetOfEffect is Player && Regen == null) || (TargetOfEffect is not Player && CDIncreasedEffect == null)) {
            return;
        }
        if(TargetOfEffect is Player) {
            Regen.Amount = Amount > 10 ? 10 : Amount;
            UIText = Utils.GetFormattedFloat(Regen.Amount, 0);
        }
        else {
            CDIncreasedEffect.PercentageAmount = Amount > 100 ? -100 : -Amount;
            TargetOfEffect.UnitAI.AggressivenessModifier += AggroModifier;
            TargetOfEffect.UnitAI.AggressivenessModifier = Amount > 100 ? -1 : -Amount / 100;
            TargetOfEffect.UnitAI.AggressivenessModifier -= AggroModifier;
            UIText = Utils.GetFormattedFloat(Amount > 100 ? 100 : Amount, 0);
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
            CDIncreasedEffect = new(TargetOfEffect.CooldownReduction, SourceOfEffect) {PercentageAmount = Amount > 100 ? -100 : -Amount};
            TargetOfEffect.AddEffect(CDIncreasedEffect, 30);
            AggroModifier = Amount > 100 ? -1 : -Amount / 100;
            TargetOfEffect.UnitAI.AggressivenessModifier -= AggroModifier;
        }   
        ChangeAmount(Amount);
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
