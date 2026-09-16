using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability_ShadowInfusion : Technique
{
    public static float EnergyCost = 25;
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Salutis;
    public static float BaseEffectDuration = 3;
    public static float UltimateEffectDuration = 20;
    public static float InjuryScaling = 250;
    public static float StaggerScaling = 250;
    public static float UltimateInjuryScaling = 150;
    public static float UltimateStaggerScaling = 150;
    public static float UltimateAttackSpeedBuff = 30;
    public static float UpgradeALifestealAmount = 30;
    public static float UpgradeBArmor = 75;
    public static float AttackSpeedBuffAmount = 100;

    public static bool CanBeUsedDuringOtherAbilities
    {
        get
        {
            return Player.Instance.PreparingForUltimate == false;
        }
    }

    public Ability_ShadowInfusion(Unit ability_user) : base(ability_user) {
        if(Is(Property.Ultimate)) {
            NameOfAnimationToAutoPlay = "ShadowInfusion_Ultimate_" + Player.Instance.CurrentWeaponType;
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
        return new List<string> { BaseEffectDuration.ToString(), Utils.GetFormattedFloat(UpgradeBArmor)};
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { UltimateEffectDuration.ToString(), UltimateAttackSpeedBuff.ToString(), (Player.Instance.CurrentWeaponInjury.Current * UltimateInjuryScaling / 100).ToString(), UltimateInjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * UltimateStaggerScaling / 100).ToString(), UltimateStaggerScaling.ToString(), UltimateAttackSpeedBuff.ToString()};
    }

    public override void ActionsToPerformDuringAnotherAbility() {
        if(Player.Instance.Energy.Current < EnergyCost || Player.Instance.TechniqueCooldowns.FirstOrDefault(cooldown => cooldown.Type == GetType()) != null) {
            return;
        }
        ConsumeEnergyAndCooldownForTheAbility();
        Effect_ShadowInfusion effect = new Effect_ShadowInfusion(StaggerScaling, AttackSpeedBuffAmount, User.CurrentWeaponDamageType, new(this)) {MasteryA = SaveFile.Instance.AbilitiesMasteryA.Contains(GetType()), MasteryB = SaveFile.Instance.AbilitiesMasteryB.Contains(GetType())};
        PlayCustomSound("Ability/Ability_SuperCharge_Use" + UnityEngine.Random.Range(1, 6), 0.9f);
        if (Is(Property.UpgradeA))
        {
            effect.LifestealPercentage = UpgradeALifestealAmount;
        }
        User.AddEffect(effect, BaseEffectDuration);
    }

    public override void CallAbilityEvent1()
    {
        Effect_ShadowInfusion_Ultimate effect = new Effect_ShadowInfusion_Ultimate(User.CurrentWeaponDamageType, new(this));
        User.AddEffect(effect, UltimateEffectDuration);
    }
}