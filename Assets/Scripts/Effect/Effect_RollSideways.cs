using System;
using UnityEngine;

public class Effect_RollSideways : Effect {
    public Effect_RollSideways(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage != TargetOfEffect || damage.CheckIfDamageWorksWithDefensiveAbilities() == false) {
            return;
        }
        if(SaveFile.Instance.DifficultyLevel == 0 || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter) == false)
        {
            float multiplier = 
            !damage.TargetOfDamage.IsStaggered ? 0 : 
            SaveFile.Instance.DifficultyLevel == 0 ? 0 : 
            SaveFile.Instance.DifficultyLevel == 1 ? damage.DamageDealtMultiplier * 0.1f : 
            damage.DamageDealtMultiplier * 0.5f;

            damage.Injury *= multiplier;
            damage.Stagger *= multiplier;

            damage.DecreaseProjectileDurability = false;

            if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.AbilityProperty.AlreadyGeneratedEnergy))
            {
                UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Dodge, damage.SourceOfDamage.User.IsBoss);
                UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.AlreadyGeneratedEnergy);
            }
            EventManager.DamageWasDodged.Invoke(damage, SourceOfEffect.SourceAbility);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
        else if (SaveFile.Instance.DifficultyLevel > 0 && damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))
        {
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;

            TargetOfEffect.AddEffect(new Effect_Stun(SourceOfEffect), SaveFile.Instance.DifficultyLevel < 2 ? 1.5f : 3);
            TargetOfEffect.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {PercentageModifier = 50, Type = EffectType.Debuff}, SaveFile.Instance.DifficultyLevel < 2 ? 1.5f : 3);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }
}