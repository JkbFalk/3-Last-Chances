using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using System.Linq;

public class Ability_Quickdraw : Technique
{
    public static float EnergyCost = 50;
    public static float Cooldown = 50;

    public static AbilityFamily Family = AbilityFamily.Anima;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.CurrentWeapon;
    private bool _canFinishAbility = false;
    private bool _buttonWasReleased = false;
    private bool _preparedForHit = false;
    private bool _createdWindSlash = false;
    private bool _counteredAnAttack = false;
    private bool _transitionedAnimation = false;

    public static float TimeSpeed = 0.2f;
    public static float InjuryScaling = 400;
    public static float StaggerScaling = 400;
    public static float CounterInjuryScaling = 1000;
    public static float CounterStaggerScaling = 1000;
    public static float UpgradeBProneApplied = 80;
    public static float UpgradeBProneAppliedToCountered = 160;
    public static float UltimateInjuryScaling = 2500;
    public static float UltimateStaggerScaling = 2500;
    public static float ChargeTime = 5;
    

    public Ability_Quickdraw(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.Charged);
        AddCustomSound("TimeSlow", "Ability/TimeSlowDown", 0.5f);
        AddCustomSound("Sheathe", "Longblade/Longblade_Sheathe1", 1f);
        AddCustomSound("Release", "Ability/Ability_Quickdraw_Release", 1f);
        AddCustomSound("WindSlash", "Wind/WindSlash", 1f);
        AddCustomSound("Counter", "Ability/Ability_Quickdraw_Counter", 1f);
        TransitionIntoAnimationDuration = 0.01f;
        EffectsAffectingUserDuringAbility = new List<Effect> {new Effect_Unstunnable(new(this))};
        EventManager.HitDealt.AddListener(CheckHitDealt);
        NameOfAnimationToAutoPlay = "Quickdraw_" + User.CurrentWeaponClass;
        DamageSources.Add(new DamageSource(Is(Property.Ultimate) ? UltimateInjuryScaling : CounterInjuryScaling, Is(Property.Ultimate) ? UltimateStaggerScaling : CounterStaggerScaling, User.CurrentWeaponDamageType) {KnockbackInMeters = 0.2f});
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { ChargeTime.ToString(), (Player.Instance.CurrentWeaponInjury.Current * InjuryScaling / 100).ToString(), InjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * StaggerScaling / 100).ToString(), StaggerScaling.ToString(), (Player.Instance.CurrentWeaponInjury.Current * CounterInjuryScaling / 100).ToString(), CounterInjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * CounterStaggerScaling / 100).ToString(), CounterStaggerScaling.ToString()  };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { UpgradeBProneApplied.ToString(), UpgradeBProneAppliedToCountered.ToString()};
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { ChargeTime.ToString(), (TimeSpeed * 100).ToString(), (Player.Instance.CurrentWeaponInjury.Current * UltimateInjuryScaling / 100).ToString(), UltimateInjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * UltimateStaggerScaling / 100).ToString(), UltimateStaggerScaling.ToString()  };
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        ConsumeEnergyAndCooldownForTheAbility();
        ShowChargeBar();
        if(Is(Property.Ultimate)) {
            PlayCustomSound("TimeSlow");
            GameController.Instance.DefaultTimeSpeed = TimeSpeed;
            CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Slowed Time") as VolumeProfile;
            Player.Instance.Animator.SetFloat("Technique Speed", 5);
            StartCountingTime(1f);
        }
        else {
            StartCountingTime(5f);
        }
    }

    public override void OnAbilityEnd() {
        base.OnAbilityEnd();
        StopCountingTime();
        EventManager.HitDealt.RemoveListener(CheckHitDealt);
        if(Is(Property.Ultimate)) {
            GameController.Instance.DefaultTimeSpeed = 1f;
            CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Regular") as VolumeProfile;
            Player.Instance.Animator.SetFloat("Technique Speed", 1);
        }
    }

    public override void CallAbilityEvent1()
    {
        if(_buttonWasReleased && _transitionedAnimation == false) {
            Player.Instance.PlayAnimation("Quickdraw_" + User.CurrentWeaponClass, 0.01f, 0.75f);
            _transitionedAnimation = true;
        }
        _canFinishAbility = true;
    }

    public override void CallAbilityEvent2()
    {
        if(Is(Property.Ultimate)) {
            Player.Instance.Animator.SetFloat("Technique Speed", 1);
            GameController.Instance.DefaultTimeSpeed = 1f;
            CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Regular") as VolumeProfile;
        }
        StopCountingTime();
        CreateWindSlash();
    }

    public override void OnAbilityButtonRelease()
    {
        _buttonWasReleased = true;
        if(Is(Property.UpgradeA)) {
            _preparedForHit = true;
            GameController.Instance.WaitAndRunMethod(0.2f, TurnOffPreparedForHit);
        }
        if(_canFinishAbility && !_createdWindSlash && _transitionedAnimation == false) {
            Player.Instance.PlayAnimation("Quickdraw_" + User.CurrentWeaponClass, 0.01f, 0.75f);
            _transitionedAnimation = true;
        }
    }

    public void TurnOffPreparedForHit() {
        _preparedForHit = false;
    }

    public void CheckHitDealt(Damage damage) {
        if(damage.TargetOfDamage == User && _counteredAnAttack == false && damage.DamagingObject is UnitWeapon) {
            if(Is(Property.Ultimate)) {
                GameController.Instance.DefaultTimeSpeed = 1f;
                CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Regular") as VolumeProfile;
            }
            StopCountingTime();
            CounterEnemyAttack(damage);
        }
    }

    public void CreateWindSlash() {
        if((_createdWindSlash || _counteredAnAttack) && !(_preparedForHit && _createdWindSlash == false)) {
            return;
        }
        _createdWindSlash = true;
        PlayCustomSound("WindSlash");
        if(Is(Property.Ultimate)) {
            DamageSources.Add(new DamageSource(GetValueBasedOnPercentageOfTimePassed(UltimateInjuryScaling / 4, UltimateInjuryScaling), GetValueBasedOnPercentageOfTimePassed(UltimateStaggerScaling / 4, UltimateStaggerScaling), User.CurrentWeaponDamageType, "Quickdraw_Ultimate_WindSlash") {KnockbackInMeters = 6.5f});
        }
        else {
            DamageSources.Add(new DamageSource(GetValueBasedOnPercentageOfTimePassed(InjuryScaling / 4, InjuryScaling), GetValueBasedOnPercentageOfTimePassed(StaggerScaling / 4, StaggerScaling), User.CurrentWeaponDamageType, "Quickdraw_WindSlash") {KnockbackInMeters = 2.5f});
        }
        Projectile proj = Utils.CreateProjectile(new(this), Is(Property.Ultimate) ? "Quickdraw_Ultimate_WindSlash" : "Quickdraw_WindSlash");
        proj.transform.eulerAngles = new Vector3(0, 0, -90);
    }

    public void CounterEnemyAttack(Damage damage) {
        if((_createdWindSlash || _counteredAnAttack) && !(_preparedForHit && _counteredAnAttack == false)) {
            return;
        }
        _counteredAnAttack = true;
        PlayCustomSound("Counter");
        if(Is(Property.Ultimate)) {
            Player.Instance.Animator.SetFloat("Technique Speed", 1);
            Player.Instance.Health.Current += damage.Injury;
            Player.Instance.StaggerBar.Current -= damage.Stagger;
            for(int i = 0; i < 10; i++) {
                GameController.Instance.WaitAndRunMethod(0.1f * i, PlayUltimateCounterHitSound, new string[] {"Hit/LongSharp_CriticalHit" + UnityEngine.Random.Range(1, 4).ToString()});
            }
        }
        else {
            for(int i = 0; i < 4; i++) {
                GameController.Instance.WaitAndRunMethod(0.1f * i, PlayUltimateCounterHitSound, new string[] {"Hit/LongSharp_Hit" + UnityEngine.Random.Range(1, 4).ToString()});
            }
        }
        damage.Injury = 0;
        damage.Stagger = 0;
        Player.Instance.AddEffect(new Effect_Invincible(new(this)), 1);
        if(_transitionedAnimation == false) {
            Player.Instance.PlayAnimation("Quickdraw_" + User.CurrentWeaponClass, 0.01f, 0.75f);
        }
        _transitionedAnimation = true;
        Unit target = damage.SourceOfDamage.User;
        GameObject vfx = Utils.CreateVisualEffect(new(this), Is(Property.Ultimate) ? "Quickdraw_Ultimate_Counter" : "Quickdraw_Counter");
        vfx.transform.SetParent(target.SpriteRenderers["Upper Body"].Bone);
        vfx.transform.localPosition = Vector2.zero;
        vfx.gameObject.name = vfx.gameObject.name + "_PersistsOnDeath";
        Properties.Add(Property.Counter);
        HandleEnemyHit(target, User.SpriteRenderers[User.CurrentWeaponDamageType == Constants.DamageType.Light ? "Light Right" : User.CurrentWeaponDamageType.ToString()].Bone.GetComponent<UnitWeapon>(), null);
        if(Is(Property.UpgradeB)){
            target.AddEffect(new Effect_Prone(GetValueBasedOnPercentageOfTimePassed(UpgradeBProneAppliedToCountered / 4, UpgradeBProneAppliedToCountered), new(this)));
        }
    }

    public void PlayUltimateCounterHitSound(string[] sound_number) {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, sound_number[0], 1);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        if(Is(Property.UpgradeB)) {
            damage.TargetOfDamage.AddEffect(new Effect_Prone(GetValueBasedOnPercentageOfTimePassed(UpgradeBProneApplied / 4, UpgradeBProneApplied), new(this)));
        }
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}
