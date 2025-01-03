using System;
using UnityEngine;

public class Effect_RollSideways : Effect {

    private bool GenerateEnergyOnSuccessfulDodge = true;
    public Effect_RollSideways(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage != TargetOfEffect || damage.CheckIfDamageWorksWithDefensiveAbilities() == false) {
            return;
        }
        if(SaveFile.Instance.DifficultyLevel == 0 || damage.SourceOfDamage.IsCounterable == false)
        {
            float multiplier = 
            !damage.TargetOfDamage.IsStaggered ? 0 : 
            SaveFile.Instance.DifficultyLevel == 0 ? 0 : 
            SaveFile.Instance.DifficultyLevel == 1 ? damage.DamageDealtMultiplier * 0.1f : 
            damage.DamageDealtMultiplier * 0.5f;

            damage.Injury *= multiplier;
            damage.Stagger *= multiplier;

            damage.DecreaseProjectileDurability = false;

            if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.GeneratedEnergy == false && GenerateEnergyOnSuccessfulDodge)
            {
                UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Dodge, damage.SourceOfDamage.User.IsBoss);
            }
            GenerateEnergyOnSuccessfulDodge = false;
            EventManager.DamageWasDodged.Invoke(damage);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
        else if (SaveFile.Instance.DifficultyLevel > 0 && damage.SourceOfDamage.IsCounterable)
        {
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;

            TargetOfEffect.AddEffect(new Effect_Stun(SourceOfEffect), SaveFile.Instance.DifficultyLevel < 2 ? 1 : 2);
            TargetOfEffect.AddEffect(new Effect_TakeDecreasedDamage(50, SourceOfEffect) {Type = EffectType.Debuff}, SaveFile.Instance.DifficultyLevel < 2 ? 1 : 2);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }
}