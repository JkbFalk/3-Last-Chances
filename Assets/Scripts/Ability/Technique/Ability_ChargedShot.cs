using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability_ChargedShot : Technique
{

    public static float EnergyCost = 50;
    public static AbilityFamily Family = AbilityFamily.Glacies;
    private static float _chargeTime = 4;
    private static float _ultimateChargeTime = 8;
    private static float _minimumInjury = 300;
    private static float _minimumFreezeStaggerScaling = 30;
    private static float _percentagePowerIncreaseAtMaxCharge = 120;
    private static float _ultimatePercentagePowerIncreaseAtMaxCharge = 240;
    private static float _ultimateFreezeStaggerScalingPerSecond = 5;
    private static float _upgradeASlowAmount = 250;
    private static float _upgradeBProneAmount = 50;
    private static Unit _validTarget;

    public Ability_ChargedShot(Unit ability_user) : base(ability_user)
    {
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        Properties.Add(Property.Charged);
        AddCustomSound("Shoot", "Ice/Ice_Shot1", 0.9f);
        AddCustomSound("Charge", "Ability/Ability_ChargedShot_Charge", 0.5f);
        DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Ranged));
        NameOfAnimationToAutoPlay = "ChargedShot_" + (Is(Property.Ultimate) ? "Ultimate_" : "") + User.CurrentRangedWeaponClass;
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.RangedInjury.Current * _minimumInjury / 100).ToString(), _minimumInjury.ToString(), (Player.Instance.RangedStagger.Current * _minimumFreezeStaggerScaling / 100).ToString(), _minimumFreezeStaggerScaling.ToString(), _percentagePowerIncreaseAtMaxCharge.ToString(), _chargeTime.ToString(), (Player.Instance.RangedInjury.Current * _minimumInjury / 100 * (1 + _percentagePowerIncreaseAtMaxCharge / 100)).ToString(), (Player.Instance.RangedStagger.Current * _minimumFreezeStaggerScaling / 100 * (1 + _percentagePowerIncreaseAtMaxCharge / 100)).ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeASlowAmount.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _upgradeBProneAmount.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { (Player.Instance.RangedInjury.Current * _minimumInjury / 100).ToString(), _minimumInjury.ToString(), (Player.Instance.RangedStagger.Current * _minimumFreezeStaggerScaling / 100).ToString(), _minimumFreezeStaggerScaling.ToString(), _ultimateChargeTime.ToString(), _ultimatePercentagePowerIncreaseAtMaxCharge.ToString(), (Player.Instance.RangedInjury.Current * _minimumInjury / 100 * (1 + _ultimatePercentagePowerIncreaseAtMaxCharge / 100)).ToString(), (Player.Instance.RangedStagger.Current * _minimumFreezeStaggerScaling / 100 * (1 + _ultimatePercentagePowerIncreaseAtMaxCharge / 100)).ToString(), (Player.Instance.RangedStagger.Current * _ultimateFreezeStaggerScalingPerSecond / 100).ToString(), _ultimateFreezeStaggerScalingPerSecond.ToString() };
    }

    public override void OnAbilityButtonRelease()
    {
        base.OnAbilityButtonRelease();
        PerformAttack();
    }

    public void PerformAttack()
    {
        if (CountingTime)
        {
            EventManager.OneTenthSecondElapsedInGame.RemoveListener(ApplyUltimateFreeze);
            StopCountingTime();
            DamageSources[0].InjuryScaling = GetValueBasedOnPercentageOfTimePassed(_minimumInjury, _minimumInjury * (1 + (Is(Property.Ultimate) ? _ultimatePercentagePowerIncreaseAtMaxCharge : _percentagePowerIncreaseAtMaxCharge) / 100));
            User.PlayAnimation("ChargedShot_" + (Is(Property.Ultimate) ? "Ultimate_" : "") + User.CurrentRangedWeaponClass, 0, Is(Property.Ultimate) ? 0.84f : 0.725f);
            float force = GetValueBasedOnPercentageOfTimePassed(1, 3);
            Vector2 shotDirection;
            if (User.CurrentTarget != null)
            {
                shotDirection = CombatMath.GetDirectionVector(User.transform.position, User.CurrentTarget.transform.position, User.Actions.IsFlipped, 60);
            }
            else
            {
                shotDirection = CombatMath.GetDirectionVector(Vector2.zero, User.Actions.SavedAimDirection != Vector2.zero ? User.Actions.SavedAimDirection : User.Actions.GetCurrentAimVector(), User.Actions.IsFlipped, 60);
            }
            User.PushInTargetDirection(-1 * force * shotDirection, this);
            Utils.CreateVisualEffect(new(this), "ChargedShot_Flash");
        }

    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        StartCountingTime(Is(Property.Ultimate) ? _ultimateChargeTime : _chargeTime);
        ShowChargeBar();
        ConsumeEnergyAndCooldownForTheAbility();
        if (Is(Property.Ultimate))
        {
            EventManager.OneTenthSecondElapsedInGame.AddListener(ApplyUltimateFreeze);
        }
    }

    public void ApplyUltimateFreeze()
    {
        if (_validTarget == null)
        {
            return;
        }
        _validTarget.AddEffect(new Effect_Freeze(_ultimateFreezeStaggerScalingPerSecond / 10 / 100 * User.MagicStagger.Current, new(this)));
    }

    public override void CallAbilityEvent1()
    {
        _validTarget = User.CurrentTarget != null ? User.CurrentTarget : User.GetClosestValidTarget();
        if (Is(Property.UpgradeA))
        {
            _validTarget.AddEffect(new Effect_Slow(_upgradeASlowAmount, new(this)));
        }
        if (HoldingTechniqueButton == false)
        {
            PerformAttack();
        }
    }

    public override void CallAbilityEvent2()
    {
        PerformAttack();
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        if (Is(Property.UpgradeB))
        {
            projectile.DisappearsAfterNHits = 0;
        }
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        float freezeScaling = GetValueBasedOnPercentageOfTimePassed(_minimumFreezeStaggerScaling, _minimumFreezeStaggerScaling * (1 + (Is(Property.Ultimate) ? _ultimatePercentagePowerIncreaseAtMaxCharge : _percentagePowerIncreaseAtMaxCharge) / 100));
        damage.TargetOfDamage.AddEffect(new Effect_Freeze(freezeScaling / 100 * User.MagicStagger.Current, new(this)));
        Utils.CreateVisualEffect(new(this), "ChargedShot_Impact", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        if (Is(Property.UpgradeB))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Prone(_upgradeBProneAmount, new(this)));
        }
    }
}