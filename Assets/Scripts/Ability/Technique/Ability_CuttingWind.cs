using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability_CuttingWind : Technique
{
    public static float EnergyCost = 25;
    public static float Cooldown = 15;

    public static float BaseEffectDuration = 3;
    public static float UltimateEffectDuration = 20;
    public static float InjuryScaling = 250;
    public static float StaggerScaling = 250;
    public static float UltimateInjuryScaling = 150;
    public static float UltimateStaggerScaling = 150;
    public static float UltimateAttackSpeedBuff = 30;
    public static float UpgradeALifestealAmount = 30;
    public static float UpgradeBDamageReduction = 75;
    public static float AttackSpeedBuffAmount = 100;

    public static AbilityFamily Family = AbilityFamily.Anima;
    public static Constants.DamageType TechniqueDamageCategory {
        get {
            return Player.Instance.CurrentStance.DamageCategory;
        }
    }

    public static bool CanBeUsedDuringOtherAbilities = true;

    public Ability_CuttingWind(Unit ability_user) : base(ability_user) {
        if(Player.Instance.PreparingForUltimate) {
            NameOfAnimationToAutoPlay = "CuttingWind_Ultimate_" + Player.Instance.CurrentWeaponCategory;
        }
        else {
            AutoPlayAbilityAnimation = false;
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { BaseEffectDuration.ToString(), ((Player.Instance.CurrentWeaponInjury.Current * InjuryScaling / 100) + (Player.Instance.CurrentWeaponStagger.Current * StaggerScaling / 100)).ToString(), InjuryScaling.ToString(), StaggerScaling.ToString(), AttackSpeedBuffAmount.ToString(), };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { UpgradeALifestealAmount.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { BaseEffectDuration.ToString(), Utils.GetFormattedFloat(UpgradeBDamageReduction)};
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { UltimateEffectDuration.ToString(), UltimateAttackSpeedBuff.ToString(), (Player.Instance.CurrentWeaponInjury.Current * UltimateInjuryScaling / 100).ToString(), UltimateInjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * UltimateStaggerScaling / 100).ToString(), UltimateStaggerScaling.ToString(), UltimateAttackSpeedBuff.ToString()};
    }

    public override void ActionsToPerformDuringAnotherAbility() {
        if(Player.Instance.Energy.Current < EnergyCost || Player.Instance.TechniqueCooldowns.FirstOrDefault(cooldown => cooldown.Type == GetType()) != null) {
            return;
        }
        AddOrUpdateCooldown();
        Player.Instance.Energy.Current -= EnergyCost;
        Effect_CuttingWind effect = new Effect_CuttingWind(StaggerScaling, AttackSpeedBuffAmount, User.CurrentWeaponDamageCategory, new(this)) {MasteryA = SaveFile.Instance.AbilitiesMasteryA.Contains(GetType()), MasteryB = SaveFile.Instance.AbilitiesMasteryB.Contains(GetType())};
        PlayCustomSound("Ability/Ability_SuperCharge_Use" + UnityEngine.Random.Range(1, 6), 0.9f);
        if (UpgradeAUnlocked)
        {
            effect.LifestealPercentage = UpgradeALifestealAmount;
        }
        User.AddEffect(effect, BaseEffectDuration);
    }

        public override void CallAbilityEvent1()
    {
        Effect_CuttingWind_Ultimate effect = new Effect_CuttingWind_Ultimate(User.CurrentWeaponDamageCategory, new(this));
        User.AddEffect(effect, UltimateEffectDuration);
    }
}