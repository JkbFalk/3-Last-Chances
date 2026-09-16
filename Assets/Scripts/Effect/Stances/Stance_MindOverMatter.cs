using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_MindOverMatter : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Proprius;
    public List<Ability.AbilityFamily> Upgrade3EmpoweredFamilies = new();
    public Ability.AbilityFamily Upgrade3CurrentlyEmpoweredFamily = Ability.AbilityFamily.None;
    public static float ExtraDamageMultiplierPer100Analysis
    {
        get
        {
            return EffectList.CalculatePB(StancePB, PB.DAMAGE_MULTIPLIER_PER_100_STACKING_EFFECT_PER_PB, new List<float> { 1 / PB.ANALYSIS_PER_PB, PB.AFFECTS_ONLY__BASIC_ATTACKS_RIPOSTES_AND_COUNTERS });
        }
    }

    public static float Upgrade1AnalysisGainedOnBasicAttack
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.4f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ANALYSIS_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
        }
    }
    public static float Upgrade1AnalysisGainedOnDodgeRiposteOrCounter
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.6f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ANALYSIS_PER_PB, PB.HAPPENS_UPON__DODGING_RIPOSTING_OR_COUNTERING });
        }
    }
    public static float Upgrade2CooldownOfEnergyBasedRevive
    {
        get
        {
            return 100 - 100 * EffectList.CalculatePB(StanceUpgrade2PB, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { }) / 100;
        }
    }
    public static float Upgrade3AnalysisExtraEffectivnessForTechniquesFromRandomlySelectedFamily
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.ANALYSIS_PERCENTAGE_EFFECTIVNESS_PER_PB, new List<float> { PB.AFFECTS_ONLY__TECHNIQUE_FROM_RANDOMLY_SELECTED_FAMILY });
        }
    }
    public static float Upgrade3ExtraDamageMultiplierAfterUsing7TechniqueFamilies
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.REQUIRES__USING_TECHNIQUES_FROM_7_RANDOMLY_SELECTED_FAMILIES, PB.AFFECTS_ONLY__TECHNIQUES, PB.AFFECTS_ONLY__ONCE });
        }
    }
    public static float Upgrade3DurationOfMultiplierPersistingAfterSwitchingOutOfStance = 10;
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(1 + ExtraDamageMultiplierPer100Analysis, 1) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1AnalysisGainedOnBasicAttack), Utils.GetFormattedFloat(Upgrade1AnalysisGainedOnDodgeRiposteOrCounter) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2CooldownOfEnergyBasedRevive) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(100 + Upgrade3AnalysisExtraEffectivnessForTechniquesFromRandomlySelectedFamily), Utils.GetFormattedFloat(1 + Upgrade3ExtraDamageMultiplierAfterUsing7TechniqueFamilies, 1) };
    }

    public Stance_MindOverMatter(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.DamageDealt, EventManager.AbilityUsed, EventManager.DamageWasDodged, EventManager.AboutToHandleFatalBlow };
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis)) && (damage.SourceOfDamage.Is(Ability.Property.BasicAttack) || damage.SourceOfDamage.Is(Ability.Property.Riposte) || damage.SourceOfDamage.Is(Ability.Property.Counter)))
        {
            damage.DamageDealtMultiplier += Player.Instance.GetEffect(typeof(Effect_Analysis)).DecayingAmount * ExtraDamageMultiplierPer100Analysis / 100;
        }
        if (IsActive && UnlockedUpgrade3 && damage.SourceOfDamage.User == Player.Instance && Upgrade3CurrentlyEmpoweredFamily != Ability.AbilityFamily.None && damage.SourceOfDamage.Is(Ability.Property.Technique) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis)))
        {
            damage.DamageDealtPercentageModifier += Upgrade3AnalysisExtraEffectivnessForTechniquesFromRandomlySelectedFamily * Player.Instance.GetEffect(typeof(Effect_Analysis)).DecayingAmount;
            Upgrade3EmpoweredFamilies.Add(Upgrade3CurrentlyEmpoweredFamily);
            if (Upgrade3EmpoweredFamilies.Count == 7)
            {
                Upgrade3CurrentlyEmpoweredFamily = Ability.AbilityFamily.None;
            }
            else
            {
                List<Ability.AbilityFamily> notEmpoweredYet = GetFamiliesNotEmpoweredYet();
                Upgrade3CurrentlyEmpoweredFamily = notEmpoweredYet[UnityEngine.Random.Range(0, notEmpoweredYet.Count)];
            }
            UpdateDisplayOfCurrentlyEmpoweredFamily();
            base.OnInvokeHitDealt(damage);
        }
        else if (IsActive && UnlockedUpgrade3 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && Upgrade3EmpoweredFamilies.Count == 7)
        {
            damage.DamageDealtMultiplier += Upgrade3ExtraDamageMultiplierAfterUsing7TechniqueFamilies;
            Upgrade3EmpoweredFamilies.Clear();
            List<Ability.AbilityFamily> allFamilies = Ability.GetAllAbilityFamilies();
            Upgrade3CurrentlyEmpoweredFamily = allFamilies[UnityEngine.Random.Range(0, allFamilies.Count)];
            UpdateDisplayOfCurrentlyEmpoweredFamily();
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && UnlockedUpgrade1 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack))
        {
            Player.Instance.AddEffect(new Effect_Analysis(Upgrade1AnalysisGainedOnBasicAttack, new(this)));
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeAbilityUsed(Ability ability)
    {
        if (IsActive && UnlockedUpgrade1 && ability.User == Player.Instance && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter)))
        {
            Player.Instance.AddEffect(new Effect_Analysis(Upgrade1AnalysisGainedOnDodgeRiposteOrCounter, new(this)));
        }
        base.OnInvokeAbilityUsed(ability);
    }

    public override void OnInvokeDamageWasDodged(DamageInstance damage, Ability dodge)
    {
        if (IsActive && UnlockedUpgrade1 && damage.TargetOfDamage == Player.Instance && dodge.TriggeredEffects.Contains(this) == false)
        {
            Player.Instance.AddEffect(new Effect_Analysis(Upgrade1AnalysisGainedOnDodgeRiposteOrCounter, new(this)));
        }
        base.OnInvokeDamageWasDodged(damage, dodge);
    }

    public override void OnInvokeAboutToHandleFatalBlow(DamageInstance damage)
    {
        if (IsActive && UnlockedUpgrade2 && damage.WillBeFatalBlow && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_MindOverMatter_EnergyBasedRevive"))
        {
            damage.WillBeFatalBlow = false;
            Player.Instance.Health.Current = (Player.Instance.Energy.Current < 1) ? (Player.Instance.Health.Maximum * 0.01f) : (Player.Instance.Energy.Current * Player.Instance.Health.Maximum);
            Player.Instance.Energy.Current = 0;
            Player.Instance.AddCooldown(this, Upgrade2CooldownOfEnergyBasedRevive, "Stance_MindOverMatter_EnergyBasedRevive");
        }
        base.OnInvokeAboutToHandleFatalBlow(damage);
    }

    public override void OnStanceActivated()
    {
        if (IsActive && UnlockedUpgrade3 && Upgrade3CurrentlyEmpoweredFamily == Ability.AbilityFamily.None)
        {
            List<Ability.AbilityFamily> notYetEmpowered = GetFamiliesNotEmpoweredYet();
            Upgrade3CurrentlyEmpoweredFamily = notYetEmpowered[UnityEngine.Random.Range(0, notYetEmpowered.Count)];
            UpdateDisplayOfCurrentlyEmpoweredFamily();
        }
        base.OnStanceActivated();
    }

    public List<Ability.AbilityFamily> GetFamiliesNotEmpoweredYet()
    {
        List<Ability.AbilityFamily> notEmpoweredYet = new();
        foreach (Ability.AbilityFamily family in Ability.GetAllAbilityFamilies())
        {
            if (Upgrade3EmpoweredFamilies.Contains(family) == false)
            {
                notEmpoweredYet.Add(family);
            }
        }
        return notEmpoweredYet;
    }

    public void UpdateDisplayOfCurrentlyEmpoweredFamily()
    {
        
    }
}
