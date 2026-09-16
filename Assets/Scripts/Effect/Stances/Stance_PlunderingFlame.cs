using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Unity.Collections;
using System;
using Unity.Mathematics;

public class Stance_PlunderingFlame : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Ignis;
    public float AccumulatedFlame = 0;
    public float BurnAppliedByFireWave = 0;
    public Effect_CustomizableDamageChange StolenTechniqueDamageBuff;
    public static float StolenTechniqueDamageBuffDuration = 15;
    public static Ability.AbilityFamily CurrentlyEmpoweredFamily;
    public Effect_ChangeStat MaxHealthIncrease;
    public Effect_ChangeStat MaxStaggerIncrease;
    public static float PercentageOfDamageDealtConvertedToBurn
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.6f, PB.APPLY_PERCENTAGE_OF_DAMAGE_DEALT_AS_STACKING_EFFECT_PER_PB, new List<float> { PB.BURN_PER_PB, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static float PercentageOfAccumulatedFlameConvertedIntoHeal
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.4f, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.REQUIRES__90_SECOND_COOLDOWN, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static float Upgrade1PercentageOfFlameConvertedIntoBurnAppliedByFireWave
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.REQUIRES__90_SECOND_COOLDOWN, PB.AFFECTS_ONLY__ENEMIES_WITHIN_3M_RANGE, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static float Upgrade2PercentageOfFlameIncreasingDamageDealtWithStolenTechniques
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__STOLEN_TECHNIQUES, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static float Upgrade3PercentageOfFlameStolenStatsUponHitting
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.35f, PB.MAXIMUM_HEALTH_AND_STAGGER_BAR_PERMANENTLY_STOLEN_PER_PB, new List<float> { PB.HAPPENS_UPON__DEALING_DAMAGE, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static float Upgrade3PercentageOfFlameStolenStatsUponStaggering
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.65f, PB.MAXIMUM_HEALTH_AND_STAGGER_BAR_PERMANENTLY_STOLEN_PER_PB, new List<float> { PB.HAPPENS_UPON__STAGGERING_AN_ENEMY, PB.SPECIAL__SCALES_WITH_ACCUMULATED_PLUNDERING_FLAME });
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(PercentageOfDamageDealtConvertedToBurn), Utils.GetFormattedFloat(PercentageOfAccumulatedFlameConvertedIntoHeal) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1PercentageOfFlameConvertedIntoBurnAppliedByFireWave) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2PercentageOfFlameIncreasingDamageDealtWithStolenTechniques), Utils.GetFormattedFloat(StolenTechniqueDamageBuffDuration) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3PercentageOfFlameStolenStatsUponHitting), Utils.GetFormattedFloat(Upgrade3PercentageOfFlameStolenStatsUponStaggering) };
    }
    public Image ReviveCooldownIndicator;

    public Stance_PlunderingFlame(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.DamageDealt, EventManager.AboutToHandleFatalBlow, EventManager.AbilityUsed, EventManager.EffectStarted, EventManager.ExitCombat };
    }

    public override void OnInvokeAboutToHandleFatalBlow(DamageInstance damage)
    {
        if (IsActive && damage.WillBeFatalBlow && AccumulatedFlame > 0 && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_PlunderingFlame - Revive"))
        {
            damage.WillBeFatalBlow = false;
            Player.Instance.Health.Current += AccumulatedFlame * PercentageOfAccumulatedFlameConvertedIntoHeal / 100;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Ability/Revival", 0.55f);
            if (UnlockedUpgrade1)
            {
                BurnAppliedByFireWave = AccumulatedFlame * Upgrade1PercentageOfFlameConvertedIntoBurnAppliedByFireWave / 100;
                Ability flameWave = new Ability_DamagingEffect(Player.Instance)
                {
                    DamageSources = new List<Ability.DamageSource>() {
                        new Ability.DamageSource(0, 10, Constants.DamageType.Magic)
                    }
                };
                AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(Player.Instance), "PlunderingFlame", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
                aoe.SourceAbility = flameWave;
                Utils.PlaySoundEffect(Player.Instance.AudioSource, "Fire/FireSwing1");
            }
            AccumulatedFlame = 0;
            Player.Instance.AddCooldown(new Cooldown(typeof(Effect), 90, Player.Instance, "Stance_PlunderingFlame - Revive") { CooldownIndicator = ReviveCooldownIndicator });
        }
        base.OnInvokeAboutToHandleFatalBlow(damage);
    }

    public override void OnInvokeAbilityUsed(Ability ability)
    {
        if (IsActive && UnlockedUpgrade2 && ability.User.IsHostile && Ability.GetFamily(ability.GetType()) != Ability.AbilityFamily.None)
        {
            if (StolenTechniqueDamageBuff != null && StolenTechniqueDamageBuff.EffectEnded == false)
            {
                StolenTechniqueDamageBuff.EndThisEffect();
            }
            CurrentlyEmpoweredFamily = Ability.GetFamily(ability.GetType());
            StolenTechniqueDamageBuff = new Effect_CustomizableDamageChange(new(Player.Instance))
            {
                DamagePercentageModifier = Upgrade2PercentageOfFlameIncreasingDamageDealtWithStolenTechniques,
                ConditionCheckOnHitDealt = new Func<DamageInstance, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    damage.SourceOfDamage.User == Player.Instance && Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_PlunderingFlame) && SaveFile.Instance.ActiveUpgrades.Contains("Stance_PlunderingFlame2") && Ability.GetFamily(damage.SourceOfDamage.GetType()) == CurrentlyEmpoweredFamily
                ),
            };
            Player.Instance.AddEffect(StolenTechniqueDamageBuff, StolenTechniqueDamageBuffDuration);
        }
        base.OnInvokeAbilityUsed(ability);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && damage.TargetOfDamage.IsHostile && damage.SourceOfDamage.User == Player.Instance && damage.IsNot(DamageInstance.DamageProperty.DamageOverTime))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Burn((damage.InjuryDealt + damage.StaggerDealt) * PercentageOfDamageDealtConvertedToBurn / 100, new(Player.Instance)));
        }
        if (IsActive && damage.TargetOfDamage.IsHostile && (damage.Is(DamageInstance.DamageProperty.Burn) || damage.Is(DamageInstance.DamageProperty.BurnExplosion)))
        {
            AccumulatedFlame += damage.DamageDealt;
        }
        if (IsActive && damage.SourceOfCollision.gameObject.name.Contains("PlunderingFlame"))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(BurnAppliedByFireWave, new(Player.Instance)));
        }
        if (IsActive && UnlockedUpgrade3 && damage.TriggersOnHitEffects && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false && damage.TargetOfDamage.IsHostile && damage.SourceOfDamage.User == Player.Instance)
        {
            float amountStolen = AccumulatedFlame * Upgrade3PercentageOfFlameStolenStatsUponHitting / 100;
            if (EnemyHealthHigherThanStaggerBar(damage.TargetOfDamage))
            {
                MaxHealthIncrease.FlatAmount += amountStolen;
                Player.Instance.Health.Current += amountStolen;
                damage.TargetOfDamage.Health.AddFlatModifier(this, -amountStolen);
            }
            else
            {
                MaxStaggerIncrease.FlatAmount += amountStolen;
                damage.TargetOfDamage.StaggerBar.AddFlatModifier(this, -amountStolen);
            }
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        if (IsActive && UnlockedUpgrade3 && effect.TargetOfEffect.IsHostile && effect.GetType().IsSubclassOf(typeof(Effect_Staggered)))
        {
            
            float amountStolen = AccumulatedFlame * Upgrade3PercentageOfFlameStolenStatsUponStaggering / 100;
            if (EnemyHealthHigherThanStaggerBar(effect.TargetOfEffect))
            {
                MaxHealthIncrease.FlatAmount += amountStolen;
                Player.Instance.Health.Current += amountStolen;
                effect.TargetOfEffect.Health.AddFlatModifier(this, -amountStolen);
            }
            else
            {
                MaxStaggerIncrease.FlatAmount += amountStolen;
                effect.TargetOfEffect.StaggerBar.AddFlatModifier(this, -amountStolen);
            }
        }
        base.OnInvokeEffectStarted(effect);
    }

    public override void OnStart()
    {
        base.OnStart();
        MaxHealthIncrease = new Effect_ChangeStat(Player.Instance.Health, new(Player.Instance));
        MaxStaggerIncrease = new Effect_ChangeStat(Player.Instance.StaggerBar, new(Player.Instance));
        Player.Instance.AddEffect(MaxHealthIncrease);
        Player.Instance.AddEffect(MaxStaggerIncrease);
    }

    public override void OnInvokeExitCombat(Unit unit)
    {
        if (IsActive && UnlockedUpgrade3 && MaxHealthIncrease != null && MaxStaggerIncrease != null)
        {
            MaxHealthIncrease.FlatAmount = 0;
            MaxStaggerIncrease.FlatAmount = 0;
        }
        base.OnInvokeExitCombat(unit);
    }

    public bool EnemyHealthHigherThanStaggerBar(Unit unit)
    {
        return unit.Health.Maximum > unit.StaggerBar.Maximum * 2;
    }
}
