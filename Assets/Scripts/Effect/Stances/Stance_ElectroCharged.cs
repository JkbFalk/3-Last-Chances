using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Security.Cryptography;

public class Stance_ElectroCharged : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Tonitrui;
    public Effect_ChangeStat Upgrade2MovementSpeedBuff;
    public static float SuperchargeDamageMultiplier
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__SUPERCHARGE_DAMAGE });
        }
    }
    public static float SuperchargeGainedOnDodgeOrBasicAttack
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SUPERCHARGE_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING_OR_DODGING });
        }
    }
    public static float Upgrade1CooldownOfSuperchargeBounces
    {
        get
        {
            return 15 - 15 * EffectList.CalculatePB(StanceUpgrade1PB, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { }) / 100;
        }
    }
    public static float Upgrade2SuperchargeGainedPer1MTravelled
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.5f, PB.STACKING_EFFECT_GAINED_PER_1M_TRAVELLED_PER_PB, new List<float> { PB.SUPERCHARGE_PER_PB });
        }
    }
    public static float Upgrade2ConversionEffectivnessOfSuperchargeIntoMovementSpeed
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.5f, PB.PERCENTAGE_EFFECTIVNESS_OF_CONVERSION_OF_STACKING_EFFECT_TO_STAT_PER_PB, new List<float> { PB.MOVEMENT_SPEED_INCREASE_PER_PB, 1 / PB.SUPERCHARGE_PER_PB });
        }
    }
    public static float Upgrade3PercentageOfSuperchargeAsInjuryReduction
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.6666f, PB.MAXIMUM_HEALTH_AND_STAGGER_BAR_PERMANENTLY_STOLEN_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static float Upgrade3CooldownOfBlockingAndThunderboltEffect
    {
        get
        {
            return 40 - 40 * EffectList.CalculatePB(StanceUpgrade3PB * 0.3333f, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { }) / 100;
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(1 + SuperchargeDamageMultiplier, 1), Utils.GetFormattedFloat(SuperchargeGainedOnDodgeOrBasicAttack) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1CooldownOfSuperchargeBounces) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2SuperchargeGainedPer1MTravelled), Utils.GetFormattedFloat(Upgrade2ConversionEffectivnessOfSuperchargeIntoMovementSpeed) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3PercentageOfSuperchargeAsInjuryReduction), Utils.GetFormattedFloat(Upgrade3CooldownOfBlockingAndThunderboltEffect) };
    }

    public Stance_ElectroCharged(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.DamageDealt, EventManager.DamageWasDodged, EventManager.EffectDecayingAmountChanged, EventManager.OneTenthSecondElapsedInGame, EventManager.AfterHitDamageCalculation, EventManager.StanceSwitched };
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance && damage.Is(DamageInstance.DamageProperty.Supercharge))
        {
            damage.DamageDealtMultiplier += SuperchargeDamageMultiplier;
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false && damage.TriggersOnHitEffects)
        {
            Player.Instance.AddEffect(new Effect_Supercharge(SuperchargeGainedOnDodgeOrBasicAttack, new(this)));
        }
        if (IsActive && UnlockedUpgrade1 && damage.SourceOfDamage.User == Player.Instance && damage.Is(DamageInstance.DamageProperty.Supercharge) && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_ElectroCharged_SuperchargeBounces"))
        {
            List<Unit> unitsAffected = new List<Unit>() { damage.TargetOfDamage };
            Unit previousTarget = damage.TargetOfDamage;
            for (int bounceNumber = 1; bounceNumber < 4; bounceNumber++)
            {
                Unit chosenTarget = Utils.GetSpecifiedUnits(new Func<Unit, bool>(unit => unit.IsHostile && !unitsAffected.Contains(unit) && Vector2.Distance(unit.transform.position, previousTarget.transform.position) < 5)).OrderBy(unit => Vector2.Distance(unit.transform.position, previousTarget.transform.position)).FirstOrDefault();
                if (chosenTarget == null)
                {
                    chosenTarget = Utils.GetSpecifiedUnits(new Func<Unit, bool>(unit => unit.IsHostile && Vector2.Distance(unit.transform.position, previousTarget.transform.position) < 5)).OrderBy(unit => Vector2.Distance(unit.transform.position, previousTarget.transform.position)).FirstOrDefault();
                }
                if (chosenTarget == null)
                {
                    chosenTarget = previousTarget;
                }
                new DamageInstance(TargetOfEffect, SourceOfEffect.SourceAbility, null)
                {
                    AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.None),
                    Properties = new List<DamageInstance.DamageProperty> { DamageInstance.DamageProperty.ExtraDamage },
                    Injury = (chosenTarget == previousTarget) ? (damage.InjuryDealt * 2f - (0.5f * bounceNumber)) : (damage.InjuryDealt * 4f - (1f * bounceNumber)),
                    PlaySoundOnEnemyHit = false
                }.CalculateAndApplyDamage();
                previousTarget = chosenTarget;
            }
            Player.Instance.AddCooldown(this, Upgrade1CooldownOfSuperchargeBounces, "Stance_ElectroCharged_SuperchargeBounces");
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeDamageWasDodged(DamageInstance damage, Ability dodge)
    {
        if (IsActive && damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false && damage.TriggersOnHitEffects)
        {
            Player.Instance.AddEffect(new Effect_Supercharge(SuperchargeGainedOnDodgeOrBasicAttack, new(this)));
        }
        base.OnInvokeDamageWasDodged(damage, dodge);
    }

    public override void OnInvokeEffectDecayingAmountChanged(Effect effect, float amount_changed)
    {
        base.OnInvokeEffectDecayingAmountChanged(effect, amount_changed);
    }

    public override void OnInvokeOneTenthSecondElapsedInGame()
    {
        if (IsActive && UnlockedUpgrade2 && Player.Instance.PlayerSavedPosition != Player.Instance.transform.position) {
            float metersTravelled = Vector2.Distance(Player.Instance.PlayerSavedPosition, Player.Instance.transform.position);
            Player.Instance.AddEffect(new Effect_Supercharge(metersTravelled * Upgrade2SuperchargeGainedPer1MTravelled / 100, new(this)));
        }
        if (IsActive && UnlockedUpgrade2 && Player.Instance.PlayerSavedPosition != Player.Instance.transform.position) {
            Upgrade2MovementSpeedBuff.FlatAmount = Player.Instance.CheckIfUnderEffect(typeof(Effect_Supercharge)) ? Player.Instance.GetEffect(typeof(Effect_Supercharge)).DecayingAmount * Upgrade2ConversionEffectivnessOfSuperchargeIntoMovementSpeed / 100 : 0;
        }
        base.OnInvokeOneTenthSecondElapsedInGame();
    }

    public override void OnInvokeStanceSwitched(Type stance_switched_from, Type stance_switched_to)
    {
        if (IsActive)
        {
            Upgrade2MovementSpeedBuff = new Effect_ChangeStat(Player.Instance.MovementSpeed, new(this))
            {
                FlatAmount = Player.Instance.CheckIfUnderEffect(typeof(Effect_Supercharge)) ? Player.Instance.GetEffect(typeof(Effect_Supercharge)).DecayingAmount * Upgrade2ConversionEffectivnessOfSuperchargeIntoMovementSpeed / 100 : 0
            };
            Player.Instance.AddEffect(Upgrade2MovementSpeedBuff);
        }
        else if (Upgrade2MovementSpeedBuff != null && Upgrade2MovementSpeedBuff.EffectEnded == false)
        {
            Upgrade2MovementSpeedBuff.EndThisEffect();
        }
        base.OnInvokeStanceSwitched(stance_switched_from, stance_switched_to);
    }

    public override void OnInvokeAfterHitDamageCalculation(DamageInstance damage)
    {
        if (IsActive && UnlockedUpgrade3 && damage.TargetOfDamage == Player.Instance && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_ElectroCharged_RetaliationThunderbolt") && damage.Injury > Player.Instance.Health.Maximum * 0.2f && Player.Instance.CheckIfUnderEffect(typeof(Effect_Supercharge)))
        {
            float amountReduced = damage.Injury * Player.Instance.GetEffect(typeof(Effect_Supercharge)).DecayingAmount * Upgrade3PercentageOfSuperchargeAsInjuryReduction / 100;
            damage.Injury -= amountReduced;
            new DamageInstance(damage.SourceOfDamage.User, new Ability_DamagingEffect(Player.Instance), null)
            {
                AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.Magic),
                Injury = amountReduced,
                PlaySoundOnEnemyHit = false
            }.CalculateAndApplyDamage();
            Player.Instance.AddCooldown(this, Upgrade3CooldownOfBlockingAndThunderboltEffect, "Stance_ElectroCharged_RetaliationThunderbolt");
        }
        base.OnInvokeAfterHitDamageCalculation(damage);
    }
}
