using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_Assassin : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Salutis;
    public float TimeSpentWithoutDealingDamage = 0;
    public float AssassinationExtraDurationRemaining = 0;
    public float Upgrade3ExtraMultiplier = 0;
    public bool AssassinationReady = false;
    public static int Upgrade3NumberOfAttacksNeededToGetFullValue = 10;
    public static float AssassinationDamageMultiplier
    {
        get
        {
            return EffectList.CalculatePB(StancePB, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.REQUIRES__NOT_ATTACKING_FOR_5_SECONDS_OR_ENTERING_STEALTH_ONCE_PER_15_SEC });
        }
    }
    public static float Upgrade1BleedAppliedFromBackstabAssassination
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.BLEED_PER_PB, PB.REQUIRES__ASSASSINATION_PREPARED, PB.HAPPENS_UPON__BACKSTABING });
        }
    }
    public static float Upgrade2CooldownOfGainingStealthAndResettingAllCooldownsUponFallingBelow25PHealth
    {
        get
        {
            return 90 - EffectList.CalculatePB(StanceUpgrade2PB, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { PB.HAPPENS_UPON__PLAYER_FALLING_BELOW_25P_HEALTH, PB.SPECIAL__APPLIES_5_SEC_STEALTH, PB.SPECIAL__RESETS_ALL_OTHER_COOLDOWNS });
        }
    }
    public static float Upgrade3SecondsOfProlongedAssassination
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.DURATION_OF_PROLONGING_ASSASSINATION_PER_PB, new List<float> { PB.REQUIRES__NOT_TAKING_DAMAGE });
        }
    }
    public static float Upgrade3ExtraMultiplierAfterAttacking
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.5f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE });
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { "5", Utils.GetFormattedFloat(1 + AssassinationDamageMultiplier, 1), "15" };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1BleedAppliedFromBackstabAssassination) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { "5", Utils.GetFormattedFloat(Upgrade2CooldownOfGainingStealthAndResettingAllCooldownsUponFallingBelow25PHealth) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3SecondsOfProlongedAssassination), Utils.GetFormattedFloat(Upgrade3ExtraMultiplierAfterAttacking, 1), Utils.GetFormattedFloat(1 + AssassinationDamageMultiplier + Upgrade3ExtraMultiplierAfterAttacking * Upgrade3NumberOfAttacksNeededToGetFullValue, 1) };
    }

    public Stance_Assassin(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.OneTenthSecondElapsedInGame, EventManager.HitDealt, EventManager.EffectStarted, EventManager.DamageDealt, EventManager.UnitStatCurrentAmountChanged };
    }

    public override void OnInvokeOneTenthSecondElapsedInGame()
    {
        base.OnInvokeOneTenthSecondElapsedInGame();
        if (IsActive)
        {
            TimeSpentWithoutDealingDamage += 0.1f;
        }
        if (IsActive && TimeSpentWithoutDealingDamage >= 5 && AssassinationReady == false)
        {
            AssassinationReady = true;
        }
        if (UnlockedUpgrade3 && AssassinationExtraDurationRemaining > 0)
        {
            AssassinationExtraDurationRemaining -= 0.1f;
            if (AssassinationExtraDurationRemaining <= 0)
            {
                AssassinationReady = false;
            }
        }
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        base.OnInvokeHitDealt(damage);
        if (IsActive && AssassinationReady && damage.SourceOfDamage.User == Player.Instance)
        {
            damage.DamageDealtMultiplier += AssassinationDamageMultiplier + (UnlockedUpgrade3 ? Upgrade3ExtraMultiplier : 0);
        }
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        base.OnInvokeEffectStarted(effect);
        if (AssassinationReady == false && effect.TargetOfEffect == Player.Instance && effect is Effect_Stealth && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_Assassin_PrepareAssassinationOnEnteringStealth"))
        {
            AssassinationReady = true;
            Player.Instance.AddCooldown(this, 15, "Stance_Assassin_PrepareAssassinationOnEnteringStealth");
        }
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        base.OnInvokeDamageDealt(damage);
        if (IsActive)
        {
            TimeSpentWithoutDealingDamage = 0;
            if (AssassinationReady && damage.SourceOfDamage.Is(Ability.Property.Backstab) && UnlockedUpgrade1)
            {
                damage.TargetOfDamage.AddEffect(new Effect_Bleed(Upgrade1BleedAppliedFromBackstabAssassination, new(this)));
            }
            if (UnlockedUpgrade3 && AssassinationExtraDurationRemaining <= 0 && AssassinationReady)
            {
                AssassinationExtraDurationRemaining = 5;
            }
            else if (UnlockedUpgrade3 && AssassinationExtraDurationRemaining > 0)
            {
                Upgrade3ExtraMultiplier += (Upgrade3ExtraMultiplier >= Upgrade3ExtraMultiplierAfterAttacking * Upgrade3NumberOfAttacksNeededToGetFullValue) ? 0 : Upgrade3ExtraMultiplier;
            }
            else
            {
                AssassinationReady = false;
            }
        }
    }

    public override void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount)
    {
        if (stat is Health && stat.Owner == Player.Instance && Player.Instance.Health.CurrentPercentage <= 25 && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_Assassin_GainStealthUponFallingBelow25PHealth"))
        {
            Player.Instance.RemoveAllCooldowns();
            Player.Instance.AddEffect(new Effect_Stealth(new(this)), 5);
            Player.Instance.AddCooldown(this, Upgrade2CooldownOfGainingStealthAndResettingAllCooldownsUponFallingBelow25PHealth, "Stance_Assassin_GainStealthUponFallingBelow25PHealth");
        }
        base.OnInvokeUnitStatCurrentAmountChanged(stat, amount);
    }
    
}
