using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_BlastDash : Technique
{
    public static float EnergyCost = 25;
    public static AbilityFamily Family = AbilityFamily.Molis;
    private static float _maxChargeTime = 2;
    private static float _maxDashRangeInMeters = 10;
    private static float _ultimateDashRangeInMeters = 25;
    private bool _canSkip = false;
    private bool _finishedAbility = false;
    private bool _releasedAbilityButton = false;
    private bool _collidedWithAbilityEndingObject = false;
    private List<Unit> _enemiesHit = new List<Unit>();
    private List<AreaOfEffect> _aoes = new List<AreaOfEffect>();
    private static float _percentageOfMaxHealthConsumed = 20;
    private static float _percentageOfMaxStaggerBarConsumed = 20;
    private static float _stunDuration = 4;
    private static float _ultimateStunDuration = 8;
    private static float _upgradeAProneApplied = 50;
    private static float _upgradeARangeIncrease = 30;
    private static float _upgradeBAreaOfEffectIncrease = 30;
    private float _healthConsumed;
    private float _staggerBarConsumed;

    public Ability_BlastDash(Unit ability_user) : base(ability_user)
    {
        HitSoundType = Constants.HitSoundTypeEnum.Earth;
        DamageSources.Add(new DamageSource(1, 0, Constants.DamageType.None));
        AddCustomSound("Start", "Explosion/Explosion1", 0.8f);
        AddCustomSound("Hit", "Hit/LargeBlunt_Hit2", 0.4f);
        AddCustomSound("Collide", "Hit/LargeBlunt_CriticalHit1", 0.8f);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { _maxChargeTime.ToString(), _maxDashRangeInMeters.ToString(), Utils.GetFormattedFloat(_percentageOfMaxHealthConsumed / 100 * Player.Instance.Health.Maximum), _percentageOfMaxHealthConsumed.ToString(), Utils.GetFormattedFloat(_percentageOfMaxHealthConsumed / 100 * Player.Instance.Health.Maximum), _percentageOfMaxStaggerBarConsumed.ToString(), _stunDuration.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeARangeIncrease.ToString(), _upgradeAProneApplied.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _upgradeBAreaOfEffectIncrease.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { _ultimateDashRangeInMeters.ToString(), _ultimateStunDuration.ToString() };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if (IsNot(Property.Ultimate))
        {
            StartCountingTime(_maxChargeTime);
            ShowChargeBar();   
        }
    }

    public override void CallAbilityEvent1()
    {
        if (_releasedAbilityButton || Is(Property.Ultimate))
        {
            PerformDash();
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
        if (Player.Instance.Health.Current < Player.Instance.Health.Maximum * _percentageOfMaxHealthConsumed / 100)
        {
            _healthConsumed = Player.Instance.Health.Current - 1;
            Player.Instance.Health.Current = 1;
        }
        else
        {
            _healthConsumed = Player.Instance.Health.Maximum * _percentageOfMaxHealthConsumed / 100;
            Player.Instance.Health.Current -= _healthConsumed;
        }
        if (Player.Instance.StaggerBar.Current + Player.Instance.StaggerBar.Maximum * _percentageOfMaxStaggerBarConsumed / 100 > Player.Instance.StaggerBar.Maximum)
        {
            _staggerBarConsumed = Player.Instance.StaggerBar.Remaining - 1;
            Player.Instance.StaggerBar.Current = Player.Instance.StaggerBar.Maximum - 1;
        }
        else
        {
            _staggerBarConsumed = Player.Instance.StaggerBar.Maximum * _percentageOfMaxStaggerBarConsumed / 100;
            Player.Instance.StaggerBar.Current += _staggerBarConsumed;
        }
        StopCountingTime();
        PlayCustomSound("Start");
        CreateAoE(Player.Instance);
        Vector2 dashDirection;
        if (User.CurrentTarget != null)
        {
            dashDirection = CombatMath.GetDirectionVector(User.transform.position, User.CurrentTarget.transform.position, User.Actions.IsFlipped, 60);
        }
        else
        {
            dashDirection = CombatMath.GetDirectionVector(Vector2.zero, User.Actions.SavedAimDirection != Vector2.zero ? User.Actions.SavedAimDirection : User.Actions.GetCurrentAimVector(), User.Actions.IsFlipped, 60);
        }
        if (Is(Property.Ultimate))
        {
            Player.Instance.PushInTargetDirectionOverTime(dashDirection * _ultimateDashRangeInMeters, this, 1);
        }
        else
        {
            Player.Instance.PushInTargetDirection(dashDirection * _maxDashRangeInMeters * GetValueBasedOnPercentageOfTimePassed(0.25f, 1) * (Is(Property.UpgradeA) ? 1 + (_upgradeARangeIncrease / 100) : 1), this);
        }
    }

    private void CreateAoE(Unit parent)
    {
        AreaOfEffect _aoe = Utils.CreateAreaOfEffect(new(this), "BlastDash");
        _aoe.transform.SetParent(parent.transform);
        _aoe.transform.localPosition = Vector2.zero;
        if (Is(Property.UpgradeB))
        {
            _aoe.transform.localScale = new Vector2(1 + _upgradeBAreaOfEffectIncrease / 100, 1 + _upgradeBAreaOfEffectIncrease / 100);
        }
        _aoes.Add(_aoe);
    }

    public override void CallAbilityEvent3()
    {
        _finishedAbility = true;
        foreach (AreaOfEffect aoe in _aoes)
        {
            aoe.GetComponent<DestroyGameObjectAfterGivenTime>().enabled = true;
            aoe.DealingDamage = false;
        }
        if (_collidedWithAbilityEndingObject && _enemiesHit.Count > 0)
        {
            PlayCustomSound("Collide");
            foreach (Unit enemy in _enemiesHit)
            {
                DamageInstance d = new DamageInstance(enemy, this, null);
                d.InjuryDealtFlatModifier = _healthConsumed;
                d.StaggerDealtFlatModifier = _staggerBarConsumed;
                if (Is(Property.Ultimate))
                {
                    d.DamageDealtMultiplier = _enemiesHit.Count;
                }
                d.CalculateAndApplyDamage();
                enemy.AddEffect(new Effect_Stun(new(this)), _stunDuration * (Is(Property.Ultimate) ? _enemiesHit.Count : 1));
                if (Is(Property.UpgradeA))
                {
                    enemy.AddEffect(new Effect_Prone(_upgradeAProneApplied, new(this)));
                }
            }
        }
        else if (_collidedWithAbilityEndingObject)
        {
            PlayCustomSound("Collide");
            Player.Instance.AddEffect(new Effect_Stun(new(this)), _stunDuration * (Is(Property.Ultimate) ? _enemiesHit.Count : 1));
        }
    }

    public override void OnAbilityButtonRelease()
    {
        base.OnAbilityButtonRelease();
        if (_canSkip)
        {
            PerformDash();
        }
        else
        {
            _releasedAbilityButton = true;
        }
    }

    private void PerformDash()
    {
        Player.Instance.PlayAnimation("BlastDash", 0f, 0.57f);
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        _enemiesHit.Add(damage.TargetOfDamage);
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        PlayCustomSound("Hit");
        CreateAoE(damage.TargetOfDamage);
        if (Is(Property.UpgradeB))
        {
            Player.Instance.Health.Current += _healthConsumed;
            Player.Instance.StaggerBar.Current -= _staggerBarConsumed;
        }
    }

    public override void AdditionalActionsOnUpdate()
    {
        base.AdditionalActionsOnUpdate();
        if (_finishedAbility == false && _enemiesHit.Count > 0 && _finishedAbility == false)
        {
            foreach (Unit u in _enemiesHit)
            {
                u.transform.position = User.Actions.IsFlipped ? (User.transform.position + new Vector3(-2 - _enemiesHit.IndexOf(u) * 0.3f, 0)) : (User.transform.position + new Vector3(2 + _enemiesHit.IndexOf(u) * 0.3f, 0));
            }
        }
    }

    public void HandleEnvironmentCollision()
    {
        if (_collidedWithAbilityEndingObject == false)
        {
            Player.Instance.PlayAnimation("BlastDash", 0.02f, 0.76f);
            PlayCustomSound("Collide");
            _collidedWithAbilityEndingObject = true;
        }
    }
}