using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_WindRush : Technique
{
    public static float EnergyCost = 20;
    public static float Cooldown = 30;
    public static float InjuryScaling = 300;
    public static float UltimateInjuryScaling = 1000;
    public static float StaggerScaling = 300;
    public static float UltimateStaggerScaling = 300;
    public static float UpgradeABarrierInjuryScaling = 350;
    public static float UpgradeABarrierStaggerScaling = 350;
    public static float UpgradeBStunDuration = 3;
    public static float UpgradeBSleepDuration = 12;
    public static float UltimateStaggerAoEScalingPerSecond = 100;
    public static float UltimateWallDurationInSeconds = 20;
    private Unit _intendedTarget;
    private bool _intendedTargetWasHit = false;
    private AreaOfEffect _ultimateAoe;
    private bool _dealingAoEDamage = false;
    private Unit _targetOfDamage = null;

    public static AbilityFamily Family = AbilityFamily.Anima;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.CurrentWeapon;

    public Ability_WindRush(Unit ability_user) : base(ability_user)
    {
        if(User.CurrentWeaponDamageType == Constants.DamageType.Light) {
            DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
            DamageSources.Add(new DamageSource(Is(Property.Ultimate) ? UltimateInjuryScaling / 2 : InjuryScaling / 2, Is(Property.Ultimate) ? UltimateStaggerScaling / 2 : StaggerScaling / 2, User.CurrentWeaponDamageType));
        }
        else {
            DamageSources.Add(new DamageSource(Is(Property.Ultimate) ? UltimateInjuryScaling : InjuryScaling, Is(Property.Ultimate) ? UltimateStaggerScaling : StaggerScaling, User.CurrentWeaponDamageType));
        }
        DamageSources.Add(new DamageSource(0, UltimateStaggerAoEScalingPerSecond / 2, User.CurrentWeaponDamageType, "WindRush_AoE"));
        AddCustomSound("Start", "Ability/Ability_WindBlast_Use", 0.4f);
        AddCustomSound("WindBlast", "Ability/Ability_WindBlast_Dash", 0.5f);
        NameOfAnimationToAutoPlay = "WindRush_" + User.CurrentWeaponClass;
        _intendedTarget = Player.Instance.CurrentTarget;
    }

    public override void CallAbilityEvent1()
    {
        ConsumeEnergyAndCooldownForTheAbility();
        GameObject vfx = Utils.CreateVisualEffect(new(this), "WindRush");
        vfx.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        vfx.transform.eulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? -90 : 90);
        ChaseCurrentTargetAtGivenDegreeAngle(15, 60);
        if(Is(Property.UpgradeA)) {
            User.AddEffect(new Effect_Barrier(User.CurrentWeaponInjury.Current * UpgradeABarrierInjuryScaling / 100 + User.CurrentWeaponStagger.Current * UpgradeABarrierInjuryScaling / 100, new(this)));
        }
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        if(_intendedTarget != null) {
            projectile.HomingOntoUnit = _intendedTarget;
        }
    }

    public override void CallAbilityEvent2()
    {
        if(_intendedTarget == null && Player.Instance.CurrentTarget == null) {
            _intendedTarget = Player.Instance.GetClosestValidTarget(true);
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> {(Player.Instance.CurrentWeaponInjury.Current * InjuryScaling / 100).ToString(), InjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * StaggerScaling / 100).ToString(), StaggerScaling.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> {(Player.Instance.CurrentWeaponInjury.Current * UpgradeABarrierInjuryScaling / 100 + Player.Instance.CurrentWeaponStagger.Current * UpgradeABarrierStaggerScaling / 100).ToString(), UpgradeABarrierInjuryScaling.ToString(), UpgradeABarrierStaggerScaling.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> {UpgradeBStunDuration.ToString(), UpgradeBSleepDuration.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> {(Player.Instance.CurrentWeaponInjury.Current * UltimateInjuryScaling / 100).ToString(), UltimateInjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * UltimateStaggerScaling / 100).ToString(), UltimateStaggerScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * UltimateStaggerAoEScalingPerSecond / 100).ToString(), UltimateStaggerAoEScalingPerSecond.ToString(), UltimateWallDurationInSeconds.ToString() };
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        if(damage.TargetOfDamage == _intendedTarget && _intendedTargetWasHit == false) {
            AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "WindRush_Knockback");
            aoe.transform.position = damage.TargetOfDamage.transform.position;
            aoe.GetComponent<PushOrPullUnits>().Force = Is(Property.Ultimate) ? 4000 : 1500;
            aoe.GetComponent<PushOrPullUnits>().AffectedUnits.AddRange(new List<Unit> { damage.SourceOfDamage.User, damage.TargetOfDamage });
            if(Is(Property.UpgradeB)) {
                User.AddEffect(new Effect_Stun(new(this)), UpgradeBStunDuration);
            }
            _intendedTargetWasHit = true;
            PlayCustomSound("WindBlast");
            if(Is(Property.Ultimate)) {
                _ultimateAoe = Utils.CreateAreaOfEffect(new(this), "WindRush_Ultimate");
                _ultimateAoe.gameObject.transform.parent.gameObject.SetActive(false);
                GameController.Instance.WaitAndRunMethod(1f, ActivateUltimateWall);
                _targetOfDamage = damage.TargetOfDamage;
            }
        }
        else if(damage.TargetOfDamage != _intendedTarget && Is(Property.UpgradeB)) {
            User.AddEffect(new Effect_Sleep(new(this)), UpgradeBSleepDuration);
        }
    }

    public void ActivateUltimateWall() {
        _ultimateAoe.gameObject.transform.parent.gameObject.SetActive(true);
        _ultimateAoe.transform.parent.position = _targetOfDamage.transform.position;
        _dealingAoEDamage = true;
        EventManager.UnitKnockedOut.AddListener(CheckIfDestroyWall);
        EventManager.OneSecondElapsedInGame.AddListener(PushTarget);
        GameController.Instance.WaitAndRunMethod(20, new System.Action(() => { EventManager.OneSecondElapsedInGame.RemoveListener(PushTarget); }));
        GameController.Instance.WaitAndRunMethod(UltimateWallDurationInSeconds, TurnOffUltimateWall);
    }

    public void CheckIfDestroyWall(Damage damage) {
        if(damage.TargetOfDamage == _targetOfDamage && _ultimateAoe != null && _ultimateAoe.IsDestroyed() == false && _ultimateAoe.gameObject.IsDestroyed() == false) {
            MonoBehaviour.Destroy(_ultimateAoe.gameObject);
        }
    }

    public void TurnOffUltimateWall() {
        if(_ultimateAoe != null && _ultimateAoe.IsDestroyed() == false && _ultimateAoe.gameObject.IsDestroyed() == false) {
            MonoBehaviour.Destroy(_ultimateAoe.gameObject);
        }
        EventManager.UnitKnockedOut.RemoveListener(CheckIfDestroyWall);
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(object_hitting is AreaOfEffect && object_hitting.gameObject.name.Contains("WindRush_AoE") == false) {
            if(unit_getting_attacked != _intendedTarget && Is(Property.UpgradeB)) {
                User.AddEffect(new Effect_Sleep(new(this)), UpgradeBSleepDuration);
            }
            return;
        }
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }

    public void PushTarget()
    {
        ResetPotentialTargets();
        _targetOfDamage.PushIntoPosition(_ultimateAoe.transform.parent.position, this, 1.1f);
    }

    public override bool CheckIfDamageTriggerIsValid(Unit unit_getting_attacked, DamagingObject source_of_hit) {
        if(_dealingAoEDamage == false) {
            return base.CheckIfDamageTriggerIsValid(unit_getting_attacked, source_of_hit);
        }
        return AffectedEnemies.ContainsKey(unit_getting_attacked) == false;
    }
}