using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_Helmsplitter : Technique
{
    private static float _maxChargeTime = 6;
    private static float _healthBarrierScaling = 20;
    private static float _staggerBarBarrierScaling = 20;
    private static float _barrierPercentageLostPerSecond = 15;
    private static float _damageDealtPer10BarrierConsumed = 20;
    private static float _upgradeAHealthPercentageConsumed = 50;
    private static float _upgradeAStaggerBarPercentageConsumed = 50;
    private static float _upgradeABarrierGainedPer1HealthOrStaggerBarConsumed = 20;
    private static float _upgradeBFlatArmor = 100;
    private static float _upgradeBArmorScaling = 50;
    private static float _upgradeBProneApplied = 40;
    private static float _ultimateBarrierGainedPer10Damage = 20;
    private List<Unit> _proneAppliedToUnits = new List<Unit>();
    private float _barrierConsumed = 0;
    private bool _canSkip = false;
    private bool _releasedAbilityButton = false;
    private Effect_ChangeStat _upgradeBBarrierBuff;
    private Effect_Unstunnable _upgradeBUnstunnable;
    public static float EnergyCost = 100;
    public static float Cooldown = 180;
    public static AbilityFamily Family = AbilityFamily.Molis;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.None;

    public Ability_Helmsplitter(Unit ability_user) : base(ability_user)
    {
        HitSoundType = Constants.HitSoundTypeEnum.Earth;
        AddCustomSound("Charge", "Explosion/Buildup", 0.4f);
        AddCustomSound("Punch", "Explosion/Explosion7", 0.7f);
        AddCustomSound("Punch_Ultimate", "Explosion/Explosion3", 0.9f);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { _maxChargeTime.ToString(), Utils.GetFormattedFloat(Player.Instance.Health.Maximum * _healthBarrierScaling / 100 + Player.Instance.StaggerBar.Maximum * _staggerBarBarrierScaling / 100), _healthBarrierScaling.ToString(), _staggerBarBarrierScaling.ToString(), _barrierPercentageLostPerSecond.ToString(), _damageDealtPer10BarrierConsumed.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeAHealthPercentageConsumed.ToString(), _upgradeAStaggerBarPercentageConsumed.ToString(), _upgradeABarrierGainedPer1HealthOrStaggerBarConsumed.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(_upgradeBFlatArmor + Player.Instance.Armor.Current * _upgradeBArmorScaling / 100), _upgradeBFlatArmor.ToString(), _upgradeBArmorScaling.ToString(), _upgradeBProneApplied.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { _maxChargeTime.ToString(), _ultimateBarrierGainedPer10Damage.ToString() };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        StartCountingTime(_maxChargeTime);
        ShowChargeBar();
        Player.Instance.AddEffect(new Effect_Barrier(Player.Instance.Health.Maximum * _healthBarrierScaling / 100 + Player.Instance.StaggerBar.Maximum * _staggerBarBarrierScaling / 100, new(this)));
        EventManager.OneTenthSecondElapsedInGame.AddListener(ConsumeBarrier);
        if (Is(Property.UpgradeB))
        {
            _upgradeBBarrierBuff = new Effect_ChangeStat(Player.Instance.Armor, new(this)) { FlatAmount = _upgradeBFlatArmor + Player.Instance.Armor.Current * _upgradeBArmorScaling / 100 };
            _upgradeBUnstunnable = new Effect_Unstunnable(new(this));
            Player.Instance.AddEffect(_upgradeBBarrierBuff);
            Player.Instance.AddEffect(_upgradeBUnstunnable);
            EventManager.DamageDealt.AddListener(AddProne);
        }
        if (Is(Property.Ultimate))
        {
            EventManager.AfterHitDamageCalculation.AddListener(PreventDamageAndAddBarrier);
        }
        if (IsNot(Property.UpgradeA))
        {
            return;
        }
        float healthConsumed, staggerBarConsumed;
        if (Player.Instance.Health.Current < Player.Instance.Health.Maximum * _upgradeAHealthPercentageConsumed / 100)
        {
            healthConsumed = Player.Instance.Health.Current - 1;
            Player.Instance.Health.Current = 1;
        }
        else
        {
            healthConsumed = Player.Instance.Health.Maximum * _upgradeAHealthPercentageConsumed / 100;
            Player.Instance.Health.Current -= healthConsumed;
        }
        if (Player.Instance.StaggerBar.Current + Player.Instance.StaggerBar.Maximum * _upgradeAStaggerBarPercentageConsumed / 100 > Player.Instance.StaggerBar.Maximum)
        {
            staggerBarConsumed = Player.Instance.StaggerBar.Maximum - Player.Instance.StaggerBar.Current - 1;
            Player.Instance.StaggerBar.Current = Player.Instance.StaggerBar.Maximum - 1;
        }
        else
        {
            staggerBarConsumed = Player.Instance.StaggerBar.Maximum * _upgradeAStaggerBarPercentageConsumed / 100;
            Player.Instance.StaggerBar.Current += staggerBarConsumed;
        }
        Player.Instance.AddEffect(new Effect_Barrier((healthConsumed + staggerBarConsumed) * _upgradeABarrierGainedPer1HealthOrStaggerBarConsumed / 10, new(this)));
    }

    private void PreventDamageAndAddBarrier(Damage damage)
    {
        Effect_Barrier barrier = (Effect_Barrier)Player.Instance.GetEffect(typeof(Effect_Barrier));
        if (barrier != null)
        {
            barrier.ChangeDecayingAmount(damage.Injury + damage.Stagger);
        }
        damage.Injury = 0;
        damage.Stagger = 0;
    }

    private void AddProne(Damage damage)
    {
        if (damage.TargetOfDamage == Player.Instance && !_proneAppliedToUnits.Contains(damage.SourceOfDamage.User))
        {
            _proneAppliedToUnits.Add(damage.SourceOfDamage.User);
            damage.SourceOfDamage.User.AddEffect(new Effect_Prone(_upgradeBProneApplied, new(this)));
        }
    }

    public override void OnAbilityButtonRelease()
    {
        base.OnAbilityButtonRelease();
        if (_canSkip)
        {
            PerformAttack();
        }
        else
        {
            _releasedAbilityButton = true;
        }
    }

    public override void CallAbilityEvent1()
    {
        if (_releasedAbilityButton)
        {
            PerformAttack();
        }
        else
        {
            _canSkip = true;
        }
    }

    public override void CallAbilityEvent2()
    {
        _canSkip = false;
        ConsumeEnergyAndCooldownForTheAbility();
        StopCountingTime();
        DamageSources.Add(new DamageSource(_damageDealtPer10BarrierConsumed * _barrierConsumed / 10, _damageDealtPer10BarrierConsumed * _barrierConsumed / 10, Constants.DamageType.None) { KnockbackInMeters = (Is(Property.Ultimate) ? 5 : 1) + PercentageOfMaxTimePassed / 10 });
        PlayCustomSound("Punch" + (Is(Property.Ultimate) ? "_Ultimate" : ""));
        AreaOfEffect _aoe = Utils.CreateAreaOfEffect(new(this), "Helmsplitter" + (Is(Property.Ultimate) ? "_Ultimate" : ""));
        User.PushInTargetDirection(Player.Instance.Actions.IsFlipped ? Vector2.left * 1.5f : Vector2.right * 1.5f, this);
        EventManager.OneTenthSecondElapsedInGame.RemoveListener(ConsumeBarrier);
        CameraController.Instance.ShakeScreen(2, 0.2f * PercentageOfMaxTimePassed / 100, 5f);
        if (Is(Property.UpgradeB))
        {
            _upgradeBBarrierBuff.EndThisEffect();
            _upgradeBUnstunnable.EndThisEffect();
            EventManager.DamageDealt.RemoveListener(AddProne);
        }
        if (Is(Property.Ultimate))
        {
            EventManager.AfterHitDamageCalculation.RemoveListener(PreventDamageAndAddBarrier);
        }
    }

    private void ConsumeBarrier()
    {
        Effect_Barrier barrierEffect = (Effect_Barrier)Player.Instance.GetEffect(typeof(Effect_Barrier));
        if (barrierEffect == null)
        {
            PerformAttack();
            return;
        }
        float barrierAmount = barrierEffect.DecayingAmount * _barrierPercentageLostPerSecond / 100 / 10;
        barrierEffect.ChangeDecayingAmount(-barrierAmount);
        _barrierConsumed += barrierAmount;
        CameraController.Instance.ShakeScreen(6f, 0.005f, -0.1f * PercentageOfMaxTimePassed * (Is(Property.Ultimate) ? 1.75f : 1));
    }

    private void PerformAttack()
    {
        Player.Instance.PlayAnimation("Helmsplitter", 0.02f, 0.79f);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}