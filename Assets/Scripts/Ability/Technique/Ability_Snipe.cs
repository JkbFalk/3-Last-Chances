using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability_Snipe : Technique
{
    private bool _releasedButton = false;

    private static float _chargeTime = 4;
    private static float _masteryAChargeTime = 1;
    private static float _minimumInjury = 300;
    private static float _maximumInjury = 600;
    private static float _minimumFreezeStaggerScaling = 200;
    private static float _maximumFreezeStaggerScaling = 400;
    private static float _minimumStagger = 200;
    private static float _maximumStagger = 400;
    private static int _masteryAMaxEnemiesHit = 3;
    public static float Cooldown = 45;
    public static float GetEnergyCost()
    {
        return SaveFile.Instance.AbilitiesMasteryB.Contains(typeof(Ability_Snipe)) ? 30 : 50;
    }

    private int _attackNumber = 1;

    public static AbilityFamily Family = AbilityFamily.Glacies;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.Ranged;

    public Ability_Snipe(Unit ability_user) : base(ability_user) {
        Properties.Add(AbilityProperty.Charged);
        AddCustomSound("Shoot", "Ice/Ice_Shot1", 0.9f);
        AddCustomSound("Charge", "Ability/Ability_Snipe_Charge", 0.5f);
        DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Ranged));
        if (UpgradeAUnlocked)
        {
            DamageSources[0].Knockback = 500;
        }
        NameOfAnimationToAutoPlay = "Snipe_" + (UpgradeAUnlocked ? "MasteryA_" : "") + User.CurrentRangedWeaponClass;
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.RangedInjury.Current * _minimumInjury / 100).ToString(), _minimumInjury.ToString(), (Player.Instance.RangedStagger.Current * _minimumStagger / 100).ToString(), _minimumStagger.ToString(), (Player.Instance.RangedStagger.Current * _minimumFreezeStaggerScaling / 100).ToString(), (Player.Instance.RangedInjury.Current * _maximumInjury / 100).ToString(), _maximumInjury.ToString(), (Player.Instance.RangedStagger.Current * _maximumStagger / 100).ToString(), _maximumStagger.ToString(), (Player.Instance.RangedStagger.Current * _maximumFreezeStaggerScaling / 100).ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _masteryAChargeTime.ToString(), _masteryAMaxEnemiesHit.ToString(),};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> {};
    }

    public override void OnAbilityButtonRelease() {
        base.OnAbilityButtonRelease();
        _releasedButton = true;
        PerformAttack();
    }

    public void PerformAttack() {
        if(CountingTime)
        {
            StopCountingTime();
            DamageSources[0].InjuryScaling = GetValueBasedOnPercentageOfTimePassed(_minimumInjury, _maximumInjury);
            DamageSources[0].StaggerScaling = GetValueBasedOnPercentageOfTimePassed(_minimumStagger, _maximumStagger);
            User.PlayAnimation("Snipe_" + (UpgradeAUnlocked ? "MasteryA_" : "") + User.CurrentRangedWeaponClass, 0, UpgradeAUnlocked ? 0.49f : 0.73f);
            float force = GetValueBasedOnPercentageOfTimePassed(100, 200);
            User.ApplyForce(User.Actions.IsFlipped ? Vector2.right * force : Vector2.left * force, this);
            Utils.CreateVisualEffect(new(this), "Snipe_Flash" + (UpgradeAUnlocked ? "_MasteryA" : (UpgradeBUnlocked ? "_MasteryB" : "")));
        }

    }

    public override void CallAbilityEvent1()
    {
        StartCountingTime(UpgradeAUnlocked ? _masteryAChargeTime : _chargeTime);
        ShowChargeBar();
    }

    public override void CallAbilityEvent2() {
        if (_releasedButton) {
            PerformAttack();
        }
    }

    public override void CallAbilityEvent3() {
        PerformAttack();
    }

    public override void CallAbilityEvent4()
    {
        if(UpgradeBUnlocked && Player.Instance.Energy.Current >= GetEnergyCost() && _attackNumber < 3 && (HoldingMainButton || HoldingTechniqueButton)) {
            Player.Instance.Actions.ConsumeEnergyAndCooldownForTheAbility();
            _attackNumber++;
            Player.Instance.PlayAnimation("Snipe_" + User.CurrentRangedWeaponClass, 0.1f, 0.67f);
        }
        
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        if (UpgradeAUnlocked)
        {
            projectile.DisappearsAfterNHits = _masteryAMaxEnemiesHit;
        }
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        if(UpgradeBUnlocked) {
            damage.TargetOfDamage.AddEffect(new Effect_Slow(50, new(this)));
        }
        else {
            damage.TargetOfDamage.AddEffect(new Effect_Freeze(GetValueBasedOnPercentageOfTimePassed(_minimumFreezeStaggerScaling, _maximumFreezeStaggerScaling) * User.RangedStagger.Current * (UpgradeAUnlocked ? 1.5f : 1), new(this)));
        }
        Utils.CreateVisualEffect(new(this), "Snipe_Impact" + (UpgradeAUnlocked ? "_MasteryA" : (UpgradeBUnlocked ? "_MasteryB" : "")), damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }

    public static bool CheckIfAbilityUsableDependingOnCombat(bool in_combat)
    {
        if(SaveFile.Instance.AbilitiesMasteryB.Contains(typeof(Ability_Snipe)))
        {
            return in_combat == false;
        }
        return true;
    }
}