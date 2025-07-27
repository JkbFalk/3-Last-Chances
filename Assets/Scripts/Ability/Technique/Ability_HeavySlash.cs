using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class Ability_HeavySlash : Technique
{
    public static float EnergyCost = 30;
    public static float Cooldown = 40;

    public static AbilityFamily Family = AbilityFamily.Ignis;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.Heavy;

    private bool _canFinishAbility = false;
    private bool _buttonWasReleased = false;
    private bool _performedSlash = false;
    private bool _transitionedAnimation = false;

    private AreaOfEffect _ultimateAoE;
    private AreaOfEffect _ultimateFireWave;

    public static float HeavyInjuryScaling = 350;
    public static float HeavyStaggerScaling = 450;
    public static float MagicWaveInjuryScaling = 150;
    public static float MagicWaveBurnStaggerScaling = 20;
    public static float UltimateHeavyInjuryScaling = 1000;
    public static float UltimateHeavyStaggerScaling = 1000;
    public static float UltimateMagicStaggerBurnScaling = 100;
    public static float ChargeTime = 2;
    public static float UltimateChargeTime = 6;
    public static float MasteryAStunDuration = 5;
    public static float MasteryBChargeSpeed = 400;
    public static float MasteryBHeavyStaggerBurnScaling = 15;

    public Ability_HeavySlash(Unit ability_user) : base(ability_user) {
        AddCustomSound("Explosion", "Explosion/Explosion1", 1f);
        AddCustomSound("HeatWave", "Fire/Fire6", 1f);
        AddCustomSound("Swing", "Fire/FireSwing1", 0.7f);
        DamageSources.Add(new DamageSource(HeavyInjuryScaling, HeavyStaggerScaling, Constants.DamageType.Heavy) {KnockbackInMeters = 5f});
        DamageSources.Add(new DamageSource(MagicWaveInjuryScaling, 0, Constants.DamageType.Magic, "HeavySlash") {KnockbackInMeters = 1f});
        NameOfAnimationToAutoPlay = "HeavySlash_" + (Player.Instance.PreparingForUltimate ? "Ultimate" : User.CurrentWeaponClass);
        if(Player.Instance.PreparingForUltimate) {
            TransitionIntoAnimationDuration = 0.02f;
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { ChargeTime.ToString(), (Player.Instance.HeavyInjury.Current * HeavyInjuryScaling / 100).ToString(), HeavyInjuryScaling.ToString(), (Player.Instance.HeavyStagger.Current * HeavyStaggerScaling / 100).ToString(), HeavyStaggerScaling.ToString(), (Player.Instance.MagicInjury.Current * MagicWaveInjuryScaling / 100).ToString(), MagicWaveInjuryScaling.ToString(), (Player.Instance.MagicStagger.Current * MagicWaveBurnStaggerScaling / 100).ToString(), MagicWaveBurnStaggerScaling.ToString()};
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { MasteryAStunDuration.ToString()};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> {MasteryBChargeSpeed.ToString(), (Player.Instance.HeavyStagger.Current * MasteryBHeavyStaggerBurnScaling / 100).ToString(), MasteryBHeavyStaggerBurnScaling.ToString()};
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { UltimateChargeTime.ToString(), (Player.Instance.HeavyInjury.Current * UltimateHeavyInjuryScaling / 100).ToString(), UltimateHeavyInjuryScaling.ToString(), (Player.Instance.HeavyStagger.Current * UltimateHeavyStaggerScaling / 100).ToString(), UltimateHeavyStaggerScaling.ToString(), (Player.Instance.MagicStagger.Current * UltimateMagicStaggerBurnScaling / 100).ToString(), UltimateMagicStaggerBurnScaling.ToString(),};
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        if(Is(Property.Ultimate)) {
            return;
        }
        ShowChargeBar();
        if(Is(Property.UpgradeB)){
            StartCountingTime(0.33f + (ChargeTime / 4));
            Player.Instance.Animator.SetFloat("Technique Speed", 4);
        }
        else {
            StartCountingTime(0.33f + ChargeTime);
        }
    }

    public override void OnAbilityEnd() {
        base.OnAbilityEnd();
        if(Is(Property.Ultimate) && _ultimateAoE != null && _ultimateAoE.IsDestroyed() == false) {
            _ultimateAoE.MakeObjectDisappear();
        }
        StopCountingTime();
    }

    public override void CallAbilityEvent1()
    {
        if(Is(Property.Ultimate)) {
            _ultimateAoE = Utils.CreateAreaOfEffect(new(this), "HeavySlash_Ultimate");
            _ultimateAoE.GetComponent<AttachObjectToBodyPart>().Initialize(User);
            _ultimateAoE.transform.localEulerAngles = new Vector3(0, 0, 90);
            ScaleAoE();
            _canFinishAbility = true;
            PlayBuildupSound();
            ShowChargeBar();
            StartCountingTime(UltimateChargeTime);
        }
        else {
            ConsumeEnergyAndCooldownForTheAbility();
            if(_buttonWasReleased && _transitionedAnimation == false) {
                Player.Instance.PlayAnimation("HeavySlash_" + User.CurrentWeaponClass, 0.02f, 0.54f);
                _transitionedAnimation = true;
            }
            _canFinishAbility = true;
        }
    }

    public void PlayBuildupSound() {
        if(_transitionedAnimation == false) {
            PlayCustomSound("Fire/Fire" + UnityEngine.Random.Range(1, 15), 0.4f, Player.Instance.AudioSource);
            GameController.Instance.WaitAndRunMethod(0.35f, PlayBuildupSound);
        }
    }

    public override void CallAbilityEvent2()
    {
        if(Is(Property.Ultimate)) {
            if(_buttonWasReleased) {
                FinishChargingUltimate();
            }
        }
        else {
            _performedSlash = true;
            Player.Instance.Animator.SetFloat("Technique Speed", 1);
            StopCountingTime();
        }
    }

    public override void CallAbilityEvent3()
    {
        if(Is(Property.Ultimate)) {
            FinishChargingUltimate();
        }
        else {
            CameraController.Instance.ShakeScreen(GetValueBasedOnPercentageOfTimePassed(0.2f, 0.6f), GetValueBasedOnPercentageOfTimePassed(0.1f, 0.25f));
            if(_transitionedAnimation == false) {
                Utils.CreateAreaOfEffect(new(this), "HeavySlash");
                User.Actions.PlayAbilityCustomSound("HeatWave");
            }
        }
    }

    public override void CallAbilityEvent4()
    {
        _ultimateAoE.DealingDamage = false;
        _ultimateAoE.MakeObjectDisappear();
    }

    public void FinishChargingUltimate() {
        if(_transitionedAnimation) {
            return;
        }
        _transitionedAnimation = true;
        ParticleSystem.MainModule main = _ultimateAoE.GetComponent<ParticleSystem>().main;
        main.prewarm = true;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        _ultimateAoE.GetComponent<ParticleSystem>().Clear();
        User.AudioSource.Stop();
        PlayCustomSound("Swing");
        CameraController.Instance.ShakeScreen(0.3f, 0.07f, 0.1f);
        StopCountingTime();
        User.PlayAnimation("HeavySlash_Ultimate", 0.05f, 0.61f);
        DamageSources.Clear();
        DamageSources.Add(new DamageSource(GetValueBasedOnPercentageOfTimePassed(UltimateHeavyInjuryScaling, UltimateHeavyInjuryScaling * 3), GetValueBasedOnPercentageOfTimePassed(UltimateHeavyStaggerScaling, UltimateHeavyStaggerScaling * 3), Constants.DamageType.Heavy, "HeavySlash_Ultimate") {KnockbackInMeters = GetValueBasedOnPercentageOfTimePassed(300, 900)});
        _ultimateAoE.DealingDamage = true;
        GameController.Instance.WaitAndRunMethod(0.55f, CreateFireWave);
        for(int i = 0; i < 50; i++) {
            GameController.Instance.WaitAndRunMethod(0.55f + 0.02f * i, AdvanceFireWave);
        }
    }

    public void CreateFireWave() {
        _ultimateFireWave = Utils.CreateAreaOfEffect(new(this), "HeavySlash_Ultimate_FireWave", User.transform.position.x + (User.Actions.IsFlipped ? -1 : 1), User.transform.position.y);
        _ultimateFireWave.transform.parent.GetComponent<ParticleSystem>().Stop();
        ParticleSystem.MainModule main = _ultimateFireWave.transform.parent.GetComponent<ParticleSystem>().main;
        main.duration = GetValueBasedOnPercentageOfTimePassed(0.4f, 0.8f);
        _ultimateFireWave.transform.parent.GetComponent<ParticleSystem>().Play();
    }

    public void AdvanceFireWave() {
        if(_ultimateFireWave != null && _ultimateFireWave.transform.parent.IsDestroyed() == false) {
            ParticleSystem.ShapeModule shape = _ultimateFireWave.transform.parent.GetComponent<ParticleSystem>().shape;
            shape.radius += 0.1f;
        }
    }

    public void ScaleAoE() {
        if(_ultimateAoE.IsDestroyed() == false) {
            CameraController.Instance.ShakeScreen(0.1f, 0.01f + 0.04f * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f));
            ParticleSystem.ShapeModule shape = _ultimateAoE.GetComponent<ParticleSystem>().shape;
            shape.scale = new Vector2(4 + 8 * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f), 0.5f + 1.0f * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f));
            ParticleSystem.MainModule main = _ultimateAoE.GetComponent<ParticleSystem>().main;
            main.startSize = 2 + 1  * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f);
            ParticleSystem.ShapeModule shape2 = _ultimateAoE.transform.Find("VisualEffect_ChimneySmoke").GetComponent<ParticleSystem>().shape;
            shape2.radius = 2 + 4 * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f);
            ParticleSystem.ShapeModule shape3 = _ultimateAoE.transform.Find("VisualEffect_HeavySlash_Ultimate_Trail").GetComponent<ParticleSystem>().shape;
            shape3.radius = 2 + 4 * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f);
            _ultimateAoE.transform.localPosition = new Vector2(2.5f + 4 * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f), 0);
            ParticleSystem.EmissionModule emission = _ultimateAoE.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 150 + 300 * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f);
            _ultimateAoE.GetComponent<BoxCollider2D>().size = new Vector3(2f, 2 + 10 * (0.33f + PercentageOfMaxTimePassed / 100 * 0.66f));
            GameController.Instance.WaitAndRunMethod(0.1f / User.HeavyAttackSpeed.Current, ScaleAoE);
        }
        else if(_ultimateAoE.IsDestroyed() == false) {
            FinishChargingUltimate();
        }
    }

    public override void OnAbilityButtonRelease()
    {
        _buttonWasReleased = true;
        if(Is(Property.Ultimate)) {
            if(_canFinishAbility && _transitionedAnimation == false) {
                FinishChargingUltimate();
            }
            return;
        }
        if(_canFinishAbility && !_performedSlash && _transitionedAnimation == false) {
            Player.Instance.PlayAnimation("HeavySlash_" + User.CurrentWeaponClass, 0.02f, 0.54f);
            _transitionedAnimation = true;
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Flinching(new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        if(damage.AbilityDamageSource.ColliderName == "HeavySlash_Ultimate") {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(GetValueBasedOnPercentageOfTimePassed(UltimateMagicStaggerBurnScaling, UltimateMagicStaggerBurnScaling * 3) * User.MagicStagger.Current / 100, new(this)));
        }
        if(damage.AbilityDamageSource.ColliderName == "HeavySlash") {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(MagicWaveBurnStaggerScaling * User.MagicStagger.Current / 100, new(this)));
        }
        if(Is(Property.UpgradeB) && _transitionedAnimation == false && damage.AbilityDamageSource.ColliderName != "HeavySlash") {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(MasteryBHeavyStaggerBurnScaling * User.HeavyStagger.Current / 100, new(this)));
        }
        if(Is(Property.UpgradeA) && _transitionedAnimation == false && damage.AbilityDamageSource.ColliderName != "HeavySlash") {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), MasteryAStunDuration);
        }
    }
}
