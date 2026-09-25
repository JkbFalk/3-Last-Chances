// FILE: Assets\Scripts\Effect\Effect_TempestStrikesAutoCounter.cs
using System;
using UnityEngine;

public class Effect_TempestStrikesAutoCounter : Effect 
{
    public Effect_TempestStrikesAutoCounter(SourceOfEffect source) : base(source) 
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(DamageInstance damage) 
    {
        // Ensure the hit is actually directed at the player and comes from the front
        if (damage.TargetOfDamage != TargetOfEffect) return;
        if (!CombatMath.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User)) return;
        
        // Cannot deflect Unstoppable attacks
        if (damage.SourceOfDamage.Is(Ability.Property.Unstoppable)) return;

        TargetOfEffect.AddEffect(new Effect_Sharp(1, new SourceOfEffect(this)));
        if (damage.DamagingObject != null && damage.DamagingObject is Projectile && damage.DamagingObject.CanBeRiposted) 
        {
            damage.DestroyProjectileAfterDamageCalcuation = true;
            Utils.SendProjectileBackTowardsSource(damage, TargetOfEffect, SourceOfEffect.SourceAbility, true);
            base.OnInvokeAfterHitDamageCalculation(damage);
        } 
        // 2. Counter Melee Attacks
        else if (damage.SourceOfDamage.User != null && !damage.SourceOfDamage.Is(Ability.Property.Counter)) 
        {
            damage.DamageWasRiposted = true;
            damage.DamageWasBlocked = true;
            damage.Injury = 0;
            damage.Stagger = 0;

            Type riposteType = AbilityTypeRegistry.GetRiposteCounter(TargetOfEffect.CurrentWeaponClass);
            if (riposteType != null) 
            {
                Counter counter = (Counter)Activator.CreateInstance(riposteType, new object[] { TargetOfEffect });
                counter.NameOfAnimationToAutoPlay = TargetOfEffect.CurrentWeaponClass.ToString() + "_RiposteCounter" + UnityEngine.Random.Range(1, 4);
                counter.Target = damage.SourceOfDamage.User;
                counter.OriginalRipostedAbility = damage.SourceOfDamage;
                TargetOfEffect.Actions.CurrentAbilityBeingPerformed = counter;

                if (TargetOfEffect.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.Property.AlreadyGeneratedEnergy)) 
                {
                    TargetOfEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Counter, damage.SourceOfDamage.User.IsBoss);
                    TargetOfEffect.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.Property.AlreadyGeneratedEnergy);
                }

                EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, true);

                float totalStagger = Constants.STAGGER_PERCENTAGE_FROM_COUNTER;
                if (SaveFile.Instance.ActiveUpgrades.Contains("Ability_TempestStrikes_UpgradeB")) {
                    totalStagger += Ability_TempestStrikes.MasteryBExtraStaggerScaling;
                }

                new DamageInstance(damage.SourceOfDamage.User, counter, null)
                    .SetDamageSource(0, totalStagger, TargetOfEffect.CurrentWeaponDamageType)
                    .CalculateAndApplyDamage();
            }
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }
}