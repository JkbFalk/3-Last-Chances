using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_TectonicPull : Technique
{
    public static float EnergyCost = 40;
    public static float Cooldown = 60;
    public static AbilityFamily Family = AbilityFamily.Molis;
    private AreaOfEffect _aoe;
    private GameObject _ultimateWall;
    private Vector2 _wallPosition;
    private bool _wallFlipped;
    private bool _releasedTechniqueButton = false;
    private bool _charging = false;
    private bool _finished = false;
    private static float _chargeTime = 2;
    private static float _upgradeBChargeTime = 4;
    private static float _injuryHeavyScaling = 200;
    private static float _staggerHeavyScaling = 200;
    private static float _injuryMagicScaling = 200;
    private static float _staggerMagicScaling = 200;
    private static float _upgradeAStunDuration = 5;
    private static float _upgradeBMaxChargeDamageMultiplier = 2.5f;
    private static float _ultimateWallDuration = 20;

    public Ability_TectonicPull(Unit ability_user) : base(ability_user)
    {
        Properties.Add(Property.Charged);
        HitSoundType = Constants.HitSoundTypeEnum.Earth;
        DamageSources.Add(new DamageSource(DamageType == Constants.DamageType.Heavy ? _injuryHeavyScaling : _injuryMagicScaling, DamageType == Constants.DamageType.Heavy ? _staggerHeavyScaling : _staggerMagicScaling, DamageType));
        AddCustomSound("Use", "Earth/Earth_PreCrack1", 0.4f);
        AddCustomSound("Stomp", "Earth/Earth_Crack6", 0.4f);
        AddCustomSound("Wall", "Earth/Earth_Crack3", 0.4f);
        NameOfAnimationToAutoPlay = "TectonicPull_" + DamageType.ToString();
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { _chargeTime.ToString(), Utils.GetFormattedFloat(DamageType == Constants.DamageType.Heavy ? (Player.Instance.HeavyInjury.Current * _injuryHeavyScaling / 100) : (Player.Instance.MagicInjury.Current * _injuryMagicScaling / 100)) + (DamageType == Constants.DamageType.Heavy ? "[HI]" : "[MI]"), Utils.GetFormattedFloat(DamageType == Constants.DamageType.Heavy ? (Player.Instance.HeavyStagger.Current * _staggerHeavyScaling / 100) : (Player.Instance.MagicStagger.Current * _staggerMagicScaling / 100)) + (DamageType == Constants.DamageType.Heavy ? "[HS]" : "[MS]"), _injuryHeavyScaling.ToString(), _staggerHeavyScaling.ToString(), _injuryMagicScaling.ToString(), _staggerMagicScaling.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeAStunDuration.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _upgradeBChargeTime.ToString(), _chargeTime.ToString(), _upgradeBMaxChargeDamageMultiplier.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { _chargeTime.ToString(), _ultimateWallDuration.ToString() };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if (IsNot(Property.Ultimate) && IsNot(Property.UpgradeA))
        {
            StartCountingTime(Is(Property.UpgradeB) ? _upgradeBChargeTime : _chargeTime);
            ShowChargeBar();
        }
        ConsumeEnergyAndCooldownForTheAbility();
        if (Is(Property.UpgradeB))
        {
            Player.Instance.Animator.SetFloat("Technique Speed", 0.5f);
        }
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        if (Is(Property.UpgradeA) || (_releasedTechniqueButton && !_finished))
        {
            SkipToFinish();
        }
        else
        {
            _charging = true;
        }
        _aoe = Utils.CreateAreaOfEffect(new(this), "TectonicPull");
        Utils.Apply2DFlip(_aoe.transform.parent != null ? _aoe.transform.parent.gameObject : _aoe.gameObject, Player.Instance.Actions.IsFlipped);
        if (Is(Property.UpgradeA))
        {
            _aoe.transform.parent.Find("Indicator").gameObject.SetActive(false);
        }
    }

    public override void OnAbilityButtonRelease()
    {
        base.OnAbilityButtonRelease();
        if (_charging && !_finished)
        {
            SkipToFinish();
        }
        else
        {
            _releasedTechniqueButton = true;
        }
    }

    public void SkipToFinish()
    {
        Player.Instance.PlayAnimation("TectonicPull_" + DamageType.ToString(), 0.02f, 0.56f);
    }

    public override void CallAbilityEvent2()
    {
        base.CallAbilityEvent2();
        Player.Instance.Animator.SetFloat("Technique Speed", 1f);
        _finished = true;
        foreach (Transform child in _aoe.transform.parent)
        {
            if (child.gameObject.name.Contains("Spike"))
            {
                child.transform.gameObject.SetActive(true);
                float scale = Is(Property.UpgradeA) ? 1 : (2 - PercentageOfMaxTimePassed / 100);
                child.transform.localScale = new Vector2(scale, scale);
            }
        }
        if (Is(Property.UpgradeA))
        {
            _aoe.transform.parent.localScale = new Vector2(1, 1);
        }
        _aoe.transform.parent.Find("Indicator").gameObject.SetActive(false);
        _aoe.gameObject.SetActive(true);
        _aoe.transform.parent.GetComponent<ChangeTransformOverTime>().enabled = false;
        GameController.Instance.WaitAndRunMethod(0.1f, TurnOffAoE);
        if (Is(Property.Ultimate))
        {
            _wallPosition = Player.Instance.ProjectileSpawnLocation.transform.position;
            _wallFlipped = Player.Instance.Actions.IsFlipped;
            GameController.Instance.WaitAndRunMethod(0.4f, CreateWall);
            PlayCustomSound("Wall");
        }
    }

    private void TurnOffAoE()
    {
        _aoe.gameObject.SetActive(false);
    }

    private void CreateWall()
    {
        _ultimateWall = Utils.CreateVisualEffect(new(this), "TectonicPullWall", _wallPosition.x, _wallPosition.y);
        Utils.Apply2DFlip(_ultimateWall, _wallFlipped);
        GameController.Instance.WaitAndRunMethod(1, PauseWall);
    }

    private void PauseWall()
    {
        if (Is(Property.Ultimate))
        {
            _ultimateWall.transform.Find("Collider").gameObject.SetActive(true);
            foreach (ParticleSystem ps in _ultimateWall.GetComponentsInChildren<ParticleSystem>())
            {
                if (ps.gameObject.name == "Spikes")
                {
                    ps.Pause();
                }
            }
            GameController.Instance.WaitAndRunMethod(_ultimateWallDuration, DestroyWall);
        }
    }

    private void DestroyWall()
    {
        foreach (ParticleSystem ps in _ultimateWall.GetComponentsInChildren<ParticleSystem>())
        {
            if (ps.gameObject.name == "Spikes")
            {
                ps.Play();
            }
        }
        _ultimateWall.GetComponent<DestroyGameObjectAfterGivenTime>().enabled = true;
    }

    public static Constants.DamageType DamageType
    {
        get
        {
            float heavyTotal = Player.Instance.HeavyInjury.Current * _injuryHeavyScaling / 100 + Player.Instance.HeavyStagger.Current * _staggerHeavyScaling / 100;
            float magicTotal = Player.Instance.MagicInjury.Current * _injuryMagicScaling / 100 + Player.Instance.MagicStagger.Current * _staggerMagicScaling / 100;
            return heavyTotal > magicTotal ? Constants.DamageType.Heavy : Constants.DamageType.Magic;
        }
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        base.ExtraBehaviourOnHit(damage);
        if (Is(Property.UpgradeB))
        {
            damage.DamageDealtMultiplier = GetValueBasedOnPercentageOfTimePassed(1, _upgradeBMaxChargeDamageMultiplier);
        }
        
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        Vector2 targetPosition = new Vector2(Player.Instance.transform.position.x + (Player.Instance.Actions.IsFlipped ? -1 : 1), Player.Instance.transform.position.y);
        damage.TargetOfDamage.PushIntoPosition(targetPosition, this, 1.2f);
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        if (Is(Property.UpgradeA))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), _upgradeAStunDuration);
        }
    }
}