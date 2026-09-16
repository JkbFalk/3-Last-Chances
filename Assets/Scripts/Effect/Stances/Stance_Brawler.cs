using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_Brawler : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Molis;
    public float SecondsPlayerSpentInCombat = 0;
    public Effect_ChangeStat MaxHealthBuff;
    public Effect_ChangeStat MaxStaggerBarBuff;
    public static float PercentageOfHealthAndStaggerBarConvertedIntoDamageMultiplier
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.DAMAGE_MULTIPLIER_PER_1000_COMBINED_HEALTH_AND_STAGGER_BAR_PER_PB, new List<float> { });
        }
    }
    public static float IncreaseMaxHealthAndStaggerBarOverTime
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, (PB.MAXIMUM_HEALTH_INCREASE_PER_PB + PB.MAXIMUM_STAGGER_BAR_INCREASE_PER_PB) / 2, new List<float> { PB.REQUIRES__60_SECONDS_IN_COMBAT_FOR_FULL_VALUE });
        }
    }
    public static float Upgrade1ArmorGainedWhileBelowHalfHealth
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.5f, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__PLAYER_BELOW_HALF_HEALTH });
        }
    }
    public static float Upgrade1ArmorGainedWhileAboveHalfStaggerBar
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.5f, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__PLAYER_ABOVE_HALF_STAGGER_BAR });
        }
    }
    public static float Upgrade2PercentageMissingHealthHealedOnBasicAttack
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.5f, PB.PERCENTAGE_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING, PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM, PB.REQUIRES__PLAYER_BELOW_33P_HEALTH });
        }
    }
    public static float Upgrade2PercentageFilledStaggerBarHealedOnBasicAttack
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.5f, PB.PERCENTAGE_STAGGER_BAR_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING, PB.SPECIAL__SCALES_WITH_MISSING_HEALTH_INSTEAD_OF_MAXIMUM, PB.REQUIRES__PLAYER_ABOVE_66P_STAGGER_BAR });
        }
    }
    public static float Upgrade3PercentageOfHealthAndStaggerBarConsumedAndInflictedAsDamage
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.6f, PB.PERCENTAGE_HEALTH_AND_STAGGER_BAR_CONSUMED_TO_EMPOWER_TECHNIQUE_DAMAGE_PER_PB, new List<float> { PB.REQUIRES__10_SECOND_COOLDOWN, PB.AFFECTS_ONLY__TECHNIQUES });
        }
    }
    public static float Upgrade3TenacityWhileAttacking
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.4f, PB.TENACITY_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__PLAYER_ATTACKING, PB.REQUIRES__45_SECOND_COOLDOWN_UPON_BEING_CCED });
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(PercentageOfHealthAndStaggerBarConvertedIntoDamageMultiplier), Utils.GetFormattedFloat(IncreaseMaxHealthAndStaggerBarOverTime), "60" };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1ArmorGainedWhileBelowHalfHealth), Utils.GetFormattedFloat(Upgrade1ArmorGainedWhileAboveHalfStaggerBar) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2PercentageMissingHealthHealedOnBasicAttack), Utils.GetFormattedFloat(Upgrade2PercentageFilledStaggerBarHealedOnBasicAttack) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3PercentageOfHealthAndStaggerBarConsumedAndInflictedAsDamage), Utils.GetFormattedFloat(Upgrade3TenacityWhileAttacking), "45" };
    }

    public Stance_Brawler(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.OneTenthSecondElapsedInGame, EventManager.DamageDealt, EventManager.AbilityUsed, EventManager.EnterCombat, EventManager.EffectStarted };
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance)
        {
            damage.DamageDealtMultiplier += (Player.Instance.Health.Maximum + Player.Instance.StaggerBar.Maximum) * PercentageOfHealthAndStaggerBarConvertedIntoDamageMultiplier / 100;
        }
        if (IsActive && UnlockedUpgrade1 && damage.TargetOfDamage == Player.Instance && Player.Instance.Health.Current < 0.5f * Player.Instance.Health.Maximum)
        {
            damage.ArmorModifier += Upgrade1ArmorGainedWhileBelowHalfHealth;
        }
        if (IsActive && UnlockedUpgrade1 && damage.TargetOfDamage == Player.Instance && Player.Instance.StaggerBar.Current > 0.5f * Player.Instance.StaggerBar.Maximum)
        {
            damage.ArmorModifier += Upgrade1ArmorGainedWhileAboveHalfStaggerBar;
        }
        if (IsActive && UnlockedUpgrade3 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.Technique) && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_Brawler_ExtraTechniqueDamage"))
        {
            float healthConsumed = 0, staggerBarConsumed = 0;
            healthConsumed += Player.Instance.Health.Current * Upgrade3PercentageOfHealthAndStaggerBarConsumedAndInflictedAsDamage / 100;
            damage.Injury += healthConsumed;
            Player.Instance.Health.Current -= healthConsumed;
            staggerBarConsumed += Player.Instance.StaggerBar.Remaining * Upgrade3PercentageOfHealthAndStaggerBarConsumedAndInflictedAsDamage / 100;
            damage.Stagger += staggerBarConsumed;
            Player.Instance.StaggerBar.Current += staggerBarConsumed;
            Player.Instance.AddCooldown(this, 10, "Stance_Brawler_ExtraTechniqueDamage");
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeOneTenthSecondElapsedInGame()
    {
        if (IsActive && MaxHealthBuff != null && MaxStaggerBarBuff != null && Player.Instance.InCombat && SecondsPlayerSpentInCombat < 60)
        {
            SecondsPlayerSpentInCombat += 0.1f;
            MaxHealthBuff.FlatAmount += IncreaseMaxHealthAndStaggerBarOverTime / 600;
            MaxStaggerBarBuff.FlatAmount += IncreaseMaxHealthAndStaggerBarOverTime / 600;
        }
        base.OnInvokeOneTenthSecondElapsedInGame();
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && UnlockedUpgrade2 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && Player.Instance.Health.Current < 0.33f * Player.Instance.Health.Maximum)
        {
            Player.Instance.Health.Current += Player.Instance.Health.Missing * Upgrade2PercentageMissingHealthHealedOnBasicAttack / 100;
        }
        if (IsActive && UnlockedUpgrade2 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && Player.Instance.StaggerBar.Current > 0.66f * Player.Instance.StaggerBar.Maximum)
        {
            Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Current * Upgrade2PercentageFilledStaggerBarHealedOnBasicAttack / 100;
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeEnterCombat(Unit unit)
    {
        if (unit is Player)
        {
            SecondsPlayerSpentInCombat = 0;
        }
        if (unit != Player.Instance || !IsActive)
        {
            return;
        }
        MaxHealthBuff = new Effect_ChangeStat(Player.Instance.Health, new(this))
        {
            FlatAmount = 0
        };
        MaxStaggerBarBuff = new Effect_ChangeStat(Player.Instance.Health, new(this))
        {
            FlatAmount = 0
        };
        Player.Instance.AddEffect(MaxHealthBuff);
        Player.Instance.AddEffect(MaxStaggerBarBuff);
        base.OnInvokeEnterCombat(unit);
    }

    public override void OnInvokeExitCombat(Unit unit)
    {
        if (unit != Player.Instance || !IsActive)
        {
            return;
        }
        if (MaxHealthBuff != null && MaxStaggerBarBuff != null)
        {
            MaxHealthBuff.EndThisEffect();
            MaxStaggerBarBuff.EndThisEffect();
        }
        base.OnInvokeExitCombat(unit);
    }
    
    public override void OnInvokeEffectStarted(Effect effect)
    {
        if (IsActive && UnlockedUpgrade3 && effect.TargetOfEffect == Player.Instance && Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_Brawler_TenacityWhileAttacking"))
        {
            Player.Instance.AddCooldown(this, 45, "Stance_Brawler_TenacityWhileAttacking");
        }
        base.OnInvokeEffectStarted(effect);
    }
}
