using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;

public class Stance_Gunslinger : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Glacies;
    public float AccumulatingMultiplierOfNextRangedTechnique = 0;
    public Ability RangedTechniqueBeingEmpowered;
    public float FinalMultiplierOfNextRangedTechnique = 0;
    public static float RangedTechniqueDamageMultiplierGainedUponBasicAttack
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.6f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__SPECIFIED_WEAPON_TECHNIQUES, PB.HAPPENS_UPON__BASIC_ATTACKING });
        }
    }
    public static float AmmoGainedUponDodgeRiposteOrCounter
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.4f, PB.AMMO_PERCENTAGE_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__DODGING_RIPOSTING_OR_COUNTERING });
        }
    }
    public static float Upgrade1SlowAppliedOnBasicAttacks
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.35f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SLOW_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
        }
    }
    public static float Upgrade1ProneAppliedOnBasicAttacks
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.65f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.PRONE_PER_PB, PB.HAPPENS_UPON__BASIC_ATTACKING });
        }
    }
    public static float Upgrade2KnockbackAppliedToEnemiesCloserThan4mUponDealingRangedDamage
    {
        get
        {
            return EffectList.CalculatePB(10, PB.KNOCK_BACK_METERS_PER_PB, new List<float> { PB.REQUIRES__GIVEN_ENEMY_IS_IN_5M_RANGE, PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE });
        }
    }
    public static float Upgrade2StaggerScalingToEnemiesCloserThan4mUponDealingRangedDamage
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB - 10, PB.STAGGER_SCALING_PER_PB, new List<float> { PB.REQUIRES__GIVEN_ENEMY_IS_IN_5M_RANGE, PB.HAPPENS_UPON__DEALING_SPECIFIC_WEAPON_DAMAGE });
        }
    }
    
    public static float Upgrade3CooldownOfRefillingHalfAmmoUponFallingBelow1
    {
        get
        {
            return 72 - EffectList.CalculatePB(StanceUpgrade3PB * 0.5714f, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { });
        }
    }
    public static float Upgrade3FinalAmmoDamageMultiplier
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.4286f, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__FINAL_AMMO });
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(RangedTechniqueDamageMultiplierGainedUponBasicAttack), Utils.GetFormattedFloat(AmmoGainedUponDodgeRiposteOrCounter) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1SlowAppliedOnBasicAttacks), Utils.GetFormattedFloat(Upgrade1ProneAppliedOnBasicAttacks) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2StaggerScalingToEnemiesCloserThan4mUponDealingRangedDamage) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3CooldownOfRefillingHalfAmmoUponFallingBelow1), Utils.GetFormattedFloat(Upgrade3FinalAmmoDamageMultiplier) };
    }

    public Stance_Gunslinger(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.AbilityUsed, EventManager.AmmoAmountChanged, EventManager.DamageDealt, EventManager.StanceSwitched };
    }
    
    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && RangedTechniqueBeingEmpowered != null && damage.SourceOfDamage == RangedTechniqueBeingEmpowered)
        {
            damage.DamageDealtMultiplier += FinalMultiplierOfNextRangedTechnique;
        }
        if (IsActive && UnlockedUpgrade2 && damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged && Vector2.Distance(Player.Instance.transform.position, damage.TargetOfDamage.transform.position) < 5)
        {
            damage.Stagger += Player.Instance.RangedStagger.Current * Upgrade2StaggerScalingToEnemiesCloserThan4mUponDealingRangedDamage / 100;
            damage.KnockbackInMeters += Upgrade2KnockbackAppliedToEnemiesCloserThan4mUponDealingRangedDamage;
        }
        if (IsActive && UnlockedUpgrade3 && damage.SourceOfDamage.User == Player.Instance && damage.Is(DamageInstance.DamageProperty.FinalAmmo))
        {
            damage.DamageDealtMultiplier += Upgrade3FinalAmmoDamageMultiplier;
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack))
        {
            AccumulatingMultiplierOfNextRangedTechnique += RangedTechniqueDamageMultiplierGainedUponBasicAttack;
        }
        if (IsActive && UnlockedUpgrade1 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Slow(Upgrade1SlowAppliedOnBasicAttacks, new(this)));
            damage.TargetOfDamage.AddEffect(new Effect_Prone(Upgrade1ProneAppliedOnBasicAttacks, new(this)));
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeAmmoAmountChanged()
    {
        RefillAmmo();
        base.OnInvokeAmmoAmountChanged();
    }

    public override void OnInvokeStanceSwitched(Type stance_switched_from, Type stance_switched_to)
    {
        RefillAmmo();
        base.OnInvokeStanceSwitched(stance_switched_from, stance_switched_to);
    }

    public void RefillAmmo()
    {
        if (IsActive && UnlockedUpgrade3 && Player.Instance.Ammo < 1 && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_Gunslinger_AmmoRefill"))
        {
            Player.Instance.Ammo += 4;
            Player.Instance.AddCooldown(this, Upgrade3CooldownOfRefillingHalfAmmoUponFallingBelow1, "Stance_Gunslinger_AmmoRefill");
        }
    }

    public override void OnInvokeDamageWasDodged(DamageInstance damage, Ability dodge)
    {
        if (IsActive && damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            damage.SourceOfDamage.TriggeredEffects.Add(this);
            Player.Instance.Ammo += AmmoGainedUponDodgeRiposteOrCounter;
        }
        base.OnInvokeDamageWasDodged(damage, dodge);
    }

    public override void OnInvokeAbilityUsed(Ability ability)
    {
        FieldInfo fInfo = ability.GetType().GetField("TechniqueDamageType", BindingFlags.Public | BindingFlags.Static);
        if (IsActive && ability.User == Player.Instance && fInfo != null && ((Constants.DamageType)fInfo.GetValue(null) == Constants.DamageType.Ranged))
        {
            FinalMultiplierOfNextRangedTechnique = AccumulatingMultiplierOfNextRangedTechnique;
            AccumulatingMultiplierOfNextRangedTechnique = 0;
            RangedTechniqueBeingEmpowered = ability;
        }
        if (IsActive && ability.User == Player.Instance && ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter))
        {
            Player.Instance.Ammo += AmmoGainedUponDodgeRiposteOrCounter;
        }
        base.OnInvokeAbilityUsed(ability);
    }
}
