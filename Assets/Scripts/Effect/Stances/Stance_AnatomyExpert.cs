using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_AnatomyExpert : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Salutis;
    public Effect_Id Upgrade3PersistingMultiplier;
    public static float EffectivnessOfStaggerToInjuryConversion = 50;
    public static float EffectivnessOfInjuryToBleedConversion
    {
        get
        {
            return EffectList.CalculatePB(StancePB, PB.PERCENTAGE_EFFECTIVNESS_OF_CONVERTING_ALL_INJURY_INTO_STACKING_EFFECT_PER_PB, new List<float> { PB.BLEED_PER_PB });
        }
    }
    public static float Upgrade1EmpoweredGainedUponSwitchingOutOfTheStance
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.EMPOWERED_PER_PB, PB.REQUIRES__10_SECOND_COOLDOWN, PB.HAPPENS_UPON__SWITCHING_OUT_OF_THE_STANCE });
        }
    }
    public static float Upgrade2PercentageConversionOfBurnFreezeAndBleedIntoProne
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STOLEN_TECHNIQUES, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static float Upgrade3DamageMultiplierToEnemiesWhoseBleedIsEnoughToFinishThemOff
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMIES_WITH_ENOUGH_BLEED_TO_FINISH_THEM_OFF_OVER_ITS_DURATION, PB.SPECIAL__PERSISTS_FOR_10_SECONDS_AFTER_SWITCHING_OUT_OF_STANCE });
        }
    }
    public static float Upgrade3DurationOfMultiplierPersistingAfterSwitchingOutOfStance = 10;
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(EffectivnessOfStaggerToInjuryConversion), Utils.GetFormattedFloat(EffectivnessOfInjuryToBleedConversion) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1EmpoweredGainedUponSwitchingOutOfTheStance), "10" };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2PercentageConversionOfBurnFreezeAndBleedIntoProne) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(1 + Upgrade3DamageMultiplierToEnemiesWhoseBleedIsEnoughToFinishThemOff, 1), "10" };
    }

    public Stance_AnatomyExpert(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.AfterHitDamageCalculation, EventManager.StanceSwitched, EventManager.EffectAmountChanged, EventManager.HitDealt };
    }

    public override void OnInvokeAfterHitDamageCalculation(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance && damage.Damage > 0 && damage.IsNot(DamageInstance.DamageProperty.Bleed))
        {
            damage.Injury += damage.Stagger * EffectivnessOfStaggerToInjuryConversion / 100;
            damage.Stagger = 0;
            damage.TargetOfDamage.AddEffect(new Effect_Bleed(damage.Injury * EffectivnessOfInjuryToBleedConversion / 100, new(this)));
            damage.Injury = 0;
        }
        base.OnInvokeAfterHitDamageCalculation(damage);
    }

    public override void OnInvokeStanceSwitched(Type stance_switched_from, Type stance_switched_to)
    {
        base.OnInvokeStanceSwitched(stance_switched_from, stance_switched_to);
        if (UnlockedUpgrade2 && stance_switched_from == GetType() && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_AnatomyExpert_EmpoweredWhenSwitchingOutOf"))
        {
            Player.Instance.AddEffect(new Effect_Empowered(Upgrade1EmpoweredGainedUponSwitchingOutOfTheStance, new(this)));
            Player.Instance.AddCooldown(this, 10, "Stance_AnatomyExpert_EmpoweredWhenSwitchingOutOf");
        }
        if (UnlockedUpgrade3 && stance_switched_from == GetType() && Upgrade3PersistingMultiplier != null && Upgrade3PersistingMultiplier.EffectEnded == false)
        {
            Upgrade3PersistingMultiplier.RemainingDuration = Upgrade3DurationOfMultiplierPersistingAfterSwitchingOutOfStance;
        }
        else if (UnlockedUpgrade3 && stance_switched_from == GetType())
        {
            Upgrade3PersistingMultiplier = new Effect_Id("Stance_AnatomyExpert_PersistingMultiplier", new(this));
            Player.Instance.AddEffect(Upgrade3PersistingMultiplier, Upgrade3DurationOfMultiplierPersistingAfterSwitchingOutOfStance);
        }
    }

    public override void OnInvokeEffectDecayingAmountChanged(Effect effect, float amount_changed)
    {
        base.OnInvokeEffectDecayingAmountChanged(effect, amount_changed);
        if (IsActive && UnlockedUpgrade2 && effect.SourceOfEffect.User == Player.Instance && (effect is Effect_Burn || effect is Effect_Freeze || effect is Effect_Bleed) && amount_changed > 0)
        {
            effect.TargetOfEffect.AddEffect(new Effect_Prone(amount_changed * Upgrade2PercentageConversionOfBurnFreezeAndBleedIntoProne / 100, effect.SourceOfEffect));
        }
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        base.OnInvokeHitDealt(damage);
        if (UnlockedUpgrade3 && damage.SourceOfDamage.User == Player.Instance && (IsActive || Player.Instance.CheckIfUnderEffectWithGivenId("Stance_AnatomyExpert_PersistingMultiplier")) && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Bleed)) && damage.TargetOfDamage.GetEffect(typeof(Effect_Bleed)).Amount * 15 > damage.TargetOfDamage.Health.Current)
        {
            damage.DamageDealtMultiplier += Upgrade3DamageMultiplierToEnemiesWhoseBleedIsEnoughToFinishThemOff;
        }
    }
}
