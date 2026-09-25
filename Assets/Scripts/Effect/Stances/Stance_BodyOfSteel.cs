using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_BodyOfSteel : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Molis;
    public static float PercentageOfDamageDealtConvertedToBarrier
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.APPLY_PERCENTAGE_OF_DAMAGE_DEALT_OR_TAKEN_AS_STACKING_EFFECT_PER_PB, new List<float> { PB.BARRIER_PER_PB });
        }
    }
    public static float DamageMultiplierPer1000Barrier
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { 1000 / (PB.EXPECTED_AMOUNT_OF_DAMAGING_STACKING_EFFECT_ON_PLAYER * PB.BARRIER_PER_PB)});
        }
    }
    public static float Upgrade1DamageMultiplierPer100Armor
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { 100 / PB.EXPECTED_AMOUNT_OF_PLAYER_ARMOR});
        }
    }
    public static float Upgrade2PercentageOfCombinedMaximumHealthAndStaggerBarConvertedToNonDecayingBarrierMinimum
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB, PB.AMOUNT_OF_BARRIER_DECAY_MINIMUM_RAISED_BY_1000_HEALTH_OR_STAGGER_BAR_PER_PB, new List<float> { });
        }
    }
    public static float Upgrade3PercentageOfUsedBarrierRestoringHealthAndStaggerBar
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.PERCENTAGE_CONVERSION_OF_USED_STACKING_EFFECT_INTO_HEALTH_AND_STAGGER_BAR_HEAL_PER_PB, new List<float> { 1 / PB.BARRIER_PER_PB});
        }
    }
    public static float Upgrade3PercentageOfDecayedBarrierConvertedIntoEmpowered
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.PERCENTAGE_CONVERSION_OF_DECAYED_STACKING_EFFECT_INTO_ANOTHER_PER_PB, new List<float> { PB.EMPOWERED_PER_PB, 1 / PB.BARRIER_PER_PB });
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(PercentageOfDamageDealtConvertedToBarrier), Utils.GetFormattedFloat(1 + DamageMultiplierPer1000Barrier, 1) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(1 + Upgrade1DamageMultiplierPer100Armor, 1) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2PercentageOfCombinedMaximumHealthAndStaggerBarConvertedToNonDecayingBarrierMinimum * (Player.Instance.Health.Maximum + Player.Instance.StaggerBar.Maximum) / 100), Utils.GetFormattedFloat(Upgrade2PercentageOfCombinedMaximumHealthAndStaggerBarConvertedToNonDecayingBarrierMinimum) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3PercentageOfUsedBarrierRestoringHealthAndStaggerBar), Utils.GetFormattedFloat(Upgrade3PercentageOfDecayedBarrierConvertedIntoEmpowered) };
    }

    public Stance_BodyOfSteel(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.DamageDealt, EventManager.HitDealt };
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Barrier)))
        {
            damage.DamageDealtMultiplier += Player.Instance.GetEffect(typeof(Effect_Barrier)).Amount * DamageMultiplierPer1000Barrier / 1000;
        }
        if (IsActive && UnlockedUpgrade1 && damage.SourceOfDamage.User == Player.Instance && Player.Instance.Armor.Current > 0)
        {
            damage.DamageDealtMultiplier += Player.Instance.Armor.Current * Upgrade1DamageMultiplierPer100Armor / 100;
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && (damage.TargetOfDamage == Player.Instance || damage.SourceOfDamage.User == Player.Instance))
        {
            Player.Instance.AddEffect(new Effect_Barrier(damage.DamageDealt * PercentageOfDamageDealtConvertedToBarrier / 100, new(this)));
        }
        base.OnInvokeDamageDealt(damage);
    }
}
