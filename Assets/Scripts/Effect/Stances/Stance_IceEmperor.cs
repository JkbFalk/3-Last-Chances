using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_IceEmperor : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Glacies;
    public List<Unit> UnitsNotHitDuringTheirFrozenDuration = new List<Unit>();
    public static float DamageMultiplierToCCedEnemies
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.7f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__CROWD_CONTROLLED });
        }
    }
    public static float IncreasedFrozenDuration
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.3f, PB.FREEZE_DURATION_INCREASE_PER_PB, new List<float> { });
        }
    }
    public static float Upgrade1ArmorPenetrationAgainstCCed
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.5f, PB.PERCENTAGE_ARMOR_PENETRATION_PER_PB, new List<float> { PB.AFFECTS_ONLY__CROWD_CONTROLLED });
        }
    }
    public static float Upgrade1DamageIncreaseAgainstCCedWithNoArmor
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.5f, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__ENEMIES_WITHOUT_ARMOR, PB.AFFECTS_ONLY__CROWD_CONTROLLED });
        }
    }
    public static float Upgrade2DamageMultiplierToFirstHitAgainstFrozen
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__FROZEN, PB.AFFECTS_ONLY__ONCE });
        }
    }
    public static float Upgrade3PercentageOfEnemyStaggerBarRequiredAsFreezeToInstaFrozen
    {
        get
        {
            return 20 - EffectList.CalculatePB(StanceUpgrade3PB * 0.5714f, PB.PERCENTAGE_OF_ENEMY_STAGGER_BAR_AS_FREEZE_REQUIRED_TO_INSTA_FROZEN_PER_PB, new List<float> { PB.HAPPENS_UPON__FREEZING_AN_ENEMY });
        }
    }
    public static float Upgrade3MaxHealthAndStaggerBarReductionOnFreeze
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.4286f, PB.MAXIMUM_HEALTH_AND_STAGGER_BAR_PERMANENTLY_REDUCED_PER_PB, new List<float> { PB.HAPPENS_UPON__FREEZING_AN_ENEMY });
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(1 + DamageMultiplierToCCedEnemies, 1), Utils.GetFormattedFloat(IncreasedFrozenDuration) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1ArmorPenetrationAgainstCCed), Utils.GetFormattedFloat(Upgrade1DamageIncreaseAgainstCCedWithNoArmor) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(1 + Upgrade2DamageMultiplierToFirstHitAgainstFrozen, 1) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3MaxHealthAndStaggerBarReductionOnFreeze) };
    }

    public Stance_IceEmperor(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.EffectAmountChanged, EventManager.EffectStarted, EventManager.EffectEnded };
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && damage.TargetOfDamage.IsHostile && damage.TargetOfDamage.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl)
        {
            damage.DamageDealtMultiplier += DamageMultiplierToCCedEnemies;
            if (UnlockedUpgrade1 && damage.TargetOfDamage.Armor.Current > 0 && damage.TargetOfDamage.IsStaggered == false)
            {
                damage.ArmorPenetrationModifier += Upgrade1ArmorPenetrationAgainstCCed;
            }
            else if (UnlockedUpgrade1)
            {
                damage.DamageDealtPercentageModifier += Upgrade1DamageIncreaseAgainstCCedWithNoArmor;
            }
        }
        if (IsActive && UnlockedUpgrade2 && damage.TargetOfDamage.IsHostile && UnitsNotHitDuringTheirFrozenDuration.Contains(damage.TargetOfDamage))
        {
            damage.DamageDealtMultiplier += Upgrade2DamageMultiplierToFirstHitAgainstFrozen;
            UnitsNotHitDuringTheirFrozenDuration.Remove(damage.TargetOfDamage);
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        if (IsActive && UnlockedUpgrade2 && effect.TargetOfEffect.IsHostile && effect is Effect_Frozen && !UnitsNotHitDuringTheirFrozenDuration.Contains(effect.TargetOfEffect))
        {
            UnitsNotHitDuringTheirFrozenDuration.Add(effect.TargetOfEffect);
        }
        if (IsActive && UnlockedUpgrade3 && effect.TargetOfEffect.IsHostile && effect is Effect_Frozen)
        {
            effect.TargetOfEffect.Health.AddFlatModifier(this, effect.TargetOfEffect.Health.Maximum * -Upgrade3MaxHealthAndStaggerBarReductionOnFreeze / 100);
            effect.TargetOfEffect.StaggerBar.AddFlatModifier(this, effect.TargetOfEffect.StaggerBar.Maximum * -Upgrade3MaxHealthAndStaggerBarReductionOnFreeze / 100);
        }
        base.OnInvokeEffectStarted(effect);
    }

    public override void OnInvokeEffectEnded(Effect effect)
    {
        if (IsActive && UnlockedUpgrade2 && effect.TargetOfEffect.IsHostile && effect is Effect_Frozen && UnitsNotHitDuringTheirFrozenDuration.Contains(effect.TargetOfEffect))
        {
            UnitsNotHitDuringTheirFrozenDuration.Remove(effect.TargetOfEffect);
        }
        base.OnInvokeEffectStarted(effect);
    }

    public override void OnInvokeEffectDecayingAmountChanged(Effect effect, float amount_changed)
    {
        if (IsActive && UnlockedUpgrade3 && effect.TargetOfEffect.IsHostile && effect is Effect_Freeze && (effect.Amount * (1 / Upgrade3PercentageOfEnemyStaggerBarRequiredAsFreezeToInstaFrozen)) > (effect.TargetOfEffect.StaggerBar.Remaining / 100))
        {
            new DamageInstance(TargetOfEffect, SourceOfEffect.SourceAbility, null)
            {
                AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.None),
                Properties = new List<DamageInstance.DamageProperty> { DamageInstance.DamageProperty.Freeze, DamageInstance.DamageProperty.DamageOverTime },
                Stagger = 999999,
                PlaySoundOnEnemyHit = false
            }.CalculateAndApplyDamage();
        }
        base.OnInvokeEffectDecayingAmountChanged(effect, amount_changed);
    }
}
