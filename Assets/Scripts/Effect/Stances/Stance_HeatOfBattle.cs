using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Stance_HeatOfBattle : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Ignis;
    public static float BurnScalingAppliedToEnemiesIn3mRange
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BURN_PER_PB, PB.AFFECTS_ONLY__ENEMIES_WITHIN_3M_RANGE });
        }
    }
    public static float BurnExplosionDamageMultiplier
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__BURN_EXPLOSION_DAMAGE });
        }
    }
    public static float Upgrade1DamageIncreaseBasedOnBurnAmount
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB, PB.DAMAGE_INCREASE_PER_PB, new List<float> { 1 / (PB.EXPECTED_AMOUNT_OF_DAMAGING_STACKING_EFFECT_ON_ENEMY * PB.BURN_PER_PB) });
        }
    }
    public static float Upgrade2ArmorGainedBasedOnBurnOfEnemiesIn3mRange
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB, PB.ARMOR_PER_PB, new List<float> { 1 / (PB.EXPECTED_AMOUNT_OF_DAMAGING_STACKING_EFFECT_ON_ENEMY * PB.BURN_PER_PB), PB.AFFECTS_ONLY__ENEMIES_WITHIN_3M_RANGE });
        }
    }
    public static float Upgrade3LeaveBehindBurnStacksPercentage
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.3f, PB.LEAVE_BEHIND_PERCENTAGE_OF_STACKS_ON_STAGGERING_PER_PB, new List<float> { PB.REQUIRES__BURN });
        }
    }
    public static float Upgrade3ReduceAllCooldowns
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.3f, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__STAGGERING_AN_ENEMY, PB.REQUIRES__BURN });
        }
    }
    public static float Upgrade3HealFromBurnDamageDealt
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.4f, PB.PERCENTAGE_OF_HEAL_DAMAGE_DEALT_PER_PB, new List<float> { PB.AFFECTS_ONLY__BURN_DAMAGE });
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(BurnScalingAppliedToEnemiesIn3mRange), Utils.GetFormattedFloat(1 + BurnExplosionDamageMultiplier, 1) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1DamageIncreaseBasedOnBurnAmount) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2ArmorGainedBasedOnBurnOfEnemiesIn3mRange) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3LeaveBehindBurnStacksPercentage), Utils.GetFormattedFloat(Upgrade3ReduceAllCooldowns), Utils.GetFormattedFloat(Upgrade3HealFromBurnDamageDealt) };
    }

    public Stance_HeatOfBattle(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.OneTenthSecondElapsedInGame, EventManager.EffectStarted, EventManager.DamageDealt };
    }

    public override void OnInvokeOneTenthSecondElapsedInGame()
    {
        if (IsActive)
        {
            float burnTotal = 0;
            foreach (Unit enemy in Utils.GetSpecifiedUnits(unit => unit.IsHostile && Vector2.Distance(unit.transform.position, Player.Instance.transform.position) < 3))
            {
                enemy.AddEffect(new Effect_Burn(Player.Instance.MagicStagger.Current * BurnScalingAppliedToEnemiesIn3mRange / 100 / 10, new(Player.Instance)));
                burnTotal += enemy.GetEffect(typeof(Effect_Burn)).DecayingAmount;
            }
            Player.Instance.CurrentStanceGauge.transform.Find("BurnAmount").GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedFloat(burnTotal, 0);
        }
        base.OnInvokeOneTenthSecondElapsedInGame();
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && damage.TargetOfDamage.IsHostile && damage.Is(DamageInstance.DamageProperty.BurnExplosion))
        {
            damage.DamageDealtMultiplier += BurnExplosionDamageMultiplier;
        }
        if (IsActive && UnlockedUpgrade1 && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Burn)) && damage.TargetOfDamage.IsHostile)
        {
            damage.DamageDealtPercentageModifier += damage.TargetOfDamage.GetEffect(typeof(Effect_Burn)).DecayingAmount * Upgrade1DamageIncreaseBasedOnBurnAmount / 100;
        }
        if (IsActive && UnlockedUpgrade2 && damage.TargetOfDamage == Player.Instance)
        {
            foreach (Unit enemy in Utils.GetSpecifiedUnits(unit => unit.IsHostile && unit.CheckIfUnderEffect(typeof(Effect_Burn)) && Vector2.Distance(unit.transform.position, Player.Instance.transform.position) < 3))
            {
                damage.ArmorModifier += enemy.GetEffect(typeof(Effect_Burn)).DecayingAmount * Upgrade2ArmorGainedBasedOnBurnOfEnemiesIn3mRange / 100;
            }
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && UnlockedUpgrade3 && Vector2.Distance(Player.Instance.transform.position, damage.TargetOfDamage.transform.position) < 15 && damage.TargetOfDamage.IsHostile && (damage.Is(DamageInstance.DamageProperty.BurnExplosion) || damage.Is(DamageInstance.DamageProperty.Burn)))
        {
            Player.Instance.Health.Current += (damage.Injury + damage.Stagger) * Upgrade3HealFromBurnDamageDealt / 100;
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        if (IsActive && UnlockedUpgrade3 && Vector2.Distance(Player.Instance.transform.position, effect.TargetOfEffect.transform.position) < 15 && effect.TargetOfEffect.IsHostile && effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.TargetOfEffect.CheckIfUnderEffect(typeof(Effect_Burn)))
        {
            Player.Instance.ReduceAllRemainingCooldowns(Upgrade3ReduceAllCooldowns);
        }
        base.OnInvokeEffectStarted(effect);
    }
}
