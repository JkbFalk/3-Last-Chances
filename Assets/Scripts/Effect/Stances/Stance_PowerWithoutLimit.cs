using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_PowerWithoutLimit : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Proprius;
    public Dictionary<Ability, float> TechniquesAndTheirExtraMultiplier = new();
    public static float DamageMultiplierPer100EnergySpent
    {
        get
        {
            return EffectList.CalculatePB(StancePB, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.SPECIAL__CONSUMES_LEFTOVER_ENERGY_AND_TRANSFORM_INTO_MULTIPLIER });
        }
    }
    public static float Upgrade1PercentageOfMaxEnergyGainedPerCooldown
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB, PB.GAIN_FLAT_ENERGY_AFFECTED_BY_ENERGY_GAIN_PER_PB, new List<float> { PB.REQUIRES__5_SECOND_COOLDOWN, PB.HAPPENS_UPON__GAINING_ENEGY });
        }
    }
    public static float Upgrade2EnergyGainForEach100BelowMax
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB, PB.ENERGY_GAIN_INCREASE_PER_PB, new List<float> { PB.SPECIAL__SCALES_WITH_MISSING_ENERGY });
        }
    }
    public static float Upgrade3PercentageOfEnergySpentTransformedIntoMaxEnergy
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.MAXIMUM_ENERGY_INCREASE_PER_PB, new List<float> { PB.REQUIRES__SPENDING_ENERGY, PB.SPECIAL__SCALES_WITH_ENERGY_SPENT });
        }
    }
    public static float Upgrade3ExtraDamageMultiplierWhileAtMaxEnergy
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.SPECIAL__CONSUMES_LEFTOVER_ENERGY_AND_TRANSFORM_INTO_MULTIPLIER, PB.REQUIRES__BEING_AT_FULL_ENERGY });
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(1 + DamageMultiplierPer100EnergySpent, 1) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1PercentageOfMaxEnergyGainedPerCooldown), "5" };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2EnergyGainForEach100BelowMax) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3PercentageOfEnergySpentTransformedIntoMaxEnergy), Utils.GetFormattedFloat(1 + DamageMultiplierPer100EnergySpent + Upgrade3ExtraDamageMultiplierWhileAtMaxEnergy, 1) };
    }

    public Stance_PowerWithoutLimit(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.AbilityEnergyConsumed, EventManager.HitDealt, EventManager.UnitStatCurrentAmountChanged, EventManager.ExitCombat };
    }

    public override void OnInvokeAbilityEnergyConsumed(Ability ability, float amount, bool was_full_energy)
    {
        base.OnInvokeAbilityEnergyConsumed(ability, amount, was_full_energy);
        if (IsActive && amount > 0)
        {
            if (UnlockedUpgrade3 && was_full_energy)
            {
                TechniquesAndTheirExtraMultiplier.Add(ability, Player.Instance.Energy.Current * Upgrade3ExtraDamageMultiplierWhileAtMaxEnergy / 100);
            }
            else
            {
                TechniquesAndTheirExtraMultiplier.Add(ability, Player.Instance.Energy.Current * DamageMultiplierPer100EnergySpent / 100);
            }
            if (UnlockedUpgrade3)
            {
                Player.Instance.Energy.RemoveFlatModifier(this);
                Player.Instance.Energy.AddFlatModifier(this, amount * Upgrade3PercentageOfEnergySpentTransformedIntoMaxEnergy / 100);
            }
            Player.Instance.Energy.Current = 0;
        }
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        base.OnInvokeHitDealt(damage);
        if (IsActive && damage.SourceOfDamage.Is(Ability.Property.Technique) && TechniquesAndTheirExtraMultiplier.ContainsKey(damage.SourceOfDamage))
        {
            damage.DamageDealtMultiplier += TechniquesAndTheirExtraMultiplier[damage.SourceOfDamage];
        }
    }

    public override void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount)
    {
        if (IsActive && UnlockedUpgrade1 && stat is Energy && stat.Owner == Player.Instance && amount > 0 && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_PowerWithoutLimit_ExtraEnergyGeneration"))
        {
            Player.Instance.Energy.GenerateEnergy(Player.Instance.Energy.Maximum * Upgrade1PercentageOfMaxEnergyGainedPerCooldown / 100);
            Player.Instance.AddCooldown(this, 5, "Stance_PowerWithoutLimit_ExtraEnergyGeneration");
        }
        if (IsActive && UnlockedUpgrade2 && stat is Energy && stat.Owner == Player.Instance)
        {
            Player.Instance.EnergyGain.RemoveFlatModifier(this);
            Player.Instance.EnergyGain.AddFlatRegeneration(this, Player.Instance.Energy.Missing * Upgrade2EnergyGainForEach100BelowMax / 100);
        }
        base.OnInvokeUnitStatCurrentAmountChanged(stat, amount);
    }

    public override void OnInvokeExitCombat(Unit unit)
    {
        Player.Instance.Energy.RemoveFlatModifier(this);
        Player.Instance.EnergyGain.RemoveFlatModifier(this);
        base.OnInvokeExitCombat(unit);
    }
}
