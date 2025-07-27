using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class Ability_Thunderstrike : Technique
{
    private GameObject _masteryBVFX;
    private bool _releasedButton;
    private int _masteryBThunderStrikeCounter = 0;
    private float _intervalBetweenMasteryBThunderstrikes = 0;
    private GameObject _indicator;
    private AreaOfEffect _thunderStrike;
    private bool _executedAttack = false;
    private bool _upgradedThunderStrike = false;
    private float _distanceFromCaster = 2.5f;

    private static float _chargeTime = 2f;
    private static float _minStunDuration = 3f;
    private static float _maxStunDuration = 5f;
    private static float _minInjury = 150;
    private static float _maxInjury = 600;
    private static float _minStagger = 150;
    private static float _maxStagger = 600;

    private static float _masteryAStunDuration = 10;
    private static float _masteryAInjury = 900;
    private static float _masteryAStagger = 1200;

    private static int _masteryBMaxThunderStrikes = 30;
    private static float _masteryBInjury = 150;
    private static float _masteryBStagger = 150;

    public static float EnergyCost = 50;
    public static float Cooldown = 10;

    public static AbilityFamily Family = AbilityFamily.Tonitrui;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.Magic;

    public Ability_Thunderstrike(Unit ability_user) : base(ability_user)
    {
        HitSoundType = Constants.HitSoundTypeEnum.Thunder;
        Properties.Add(Property.Charged);
        TransitionIntoAnimationDuration = 0;
        ScaleMaxTimeWithCombatSpeed = false;
        AddCustomSound("Regular", "Ability/Ability_ThunderStrike_Regular", 0.9f);
        AddCustomSound("Type A", "Ability/Ability_ThunderStrike_A", 0.9f);
        AddCustomSound("Charge", "Ability/Ability_ThunderStrike_Charge", 0.5f);
        DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Magic));
        NameOfAnimationToAutoPlay = "ThunderStrike" + (Is(Property.UpgradeB) ? "_MasteryB" : "");
        if(Is(Property.UpgradeB))
        {
            EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
            DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { _minStunDuration.ToString(), _maxStunDuration.ToString(), (Player.Instance.MagicInjury.Current * _minInjury / 100).ToString(), _minInjury.ToString(), (Player.Instance.MagicInjury.Current * _maxInjury / 100).ToString(), _maxInjury.ToString(), (Player.Instance.MagicStagger.Current * _minStagger / 100).ToString(), _minStagger.ToString(), (Player.Instance.MagicStagger.Current * _maxStagger / 100).ToString(), _maxStagger.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { (Player.Instance.MagicInjury.Current * _masteryAInjury / 100).ToString(), _masteryAInjury.ToString(), (Player.Instance.MagicStagger.Current * _masteryAStagger / 100).ToString(), _masteryAStagger.ToString(), _masteryAStunDuration.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _masteryBMaxThunderStrikes.ToString(), (Player.Instance.MagicInjury.Current * _masteryBInjury / 100).ToString(), _masteryBInjury.ToString(), (Player.Instance.MagicStagger.Current * _masteryBStagger / 100).ToString(), _masteryBStagger.ToString() };
    }

    public override void OnAbilityButtonRelease()
    {
        base.OnAbilityButtonRelease();
        _releasedButton = true;
        if (CountingTime && !_executedAttack && IsNot(Property.UpgradeB))
        {
            ConsumeEnergyAndCooldownForTheAbility();
            CreateRegularThunderStrike();
        }
        if (CountingTime && Is(Property.UpgradeB))
        {
            EndThisAbility();
        }
    }

    public void CreateRegularThunderStrike()
    {
        PlayCustomSound("Type A");
        _executedAttack = true;
        User.PlayAnimation("Ability_ThunderStrike", 0, 0.75f);
        MonoBehaviour.Destroy(_indicator);
        DamageSources[0].InjuryScaling = GetValueBasedOnPercentageOfTimePassed(_minInjury, _maxInjury);
        DamageSources[0].StaggerScaling = GetValueBasedOnPercentageOfTimePassed(_minStagger, _maxStagger);
        _thunderStrike = Utils.CreateAreaOfEffect(new(this), "ThunderStrike");
        float scale = GetValueBasedOnPercentageOfTimePassed(1, 0.35f);
        _thunderStrike.transform.parent.parent.localScale = new Vector3(scale, scale, scale);
        GameController.Instance.WaitAndRunMethod(0.3f, StartDealingDamage);
        StopCountingTime();
    }

    public void CreateMasteryAThunderStrike() {
        PlayCustomSound("Type A");
        _executedAttack = true;
        _upgradedThunderStrike = true;
        User.PlayAnimation("Ability_ThunderStrike", 0, 0.75f);
        MonoBehaviour.Destroy(_indicator);
        DamageSources[0].InjuryScaling = _masteryBInjury;
        DamageSources[0].StaggerScaling = _masteryBStagger;
        _thunderStrike = Utils.CreateAreaOfEffect(new(this), "ThunderStrike_MasteryA");
        GameController.Instance.WaitAndRunMethod(0.3f, StartDealingDamage);
        StopCountingTime();
    }

    public void StartDealingDamage()
    {
        _thunderStrike.GetComponentInChildren<AreaOfEffect>().StartDealingDamage(0.2f);
    }

    public override void CallAbilityEvent1()
    {
        if(Is(Property.UpgradeB))
        {
            DamageSources[0]. CustomHitSound = "Ability/Ability_ThunderStrike_Charge";
            DamageSources[0].InjuryScaling = _masteryBInjury;
            DamageSources[0].StaggerScaling = _masteryBStagger;
            _masteryBVFX = Utils.CreateVisualEffect(new(this), "ThunderStrike_MasteryB");
            _intervalBetweenMasteryBThunderstrikes = 5.5f / User.MagicAttackSpeed.ScaledWithCombatSpeed / _masteryBMaxThunderStrikes;
            StartCountingTime(5f);
            ShowChargeBar();
            CreateMasteryBThunderStrike();
        }
        else
        {
            _indicator = Utils.CreateVisualEffect(new(this), "ThunderStrike_Indicator");
            StartCountingTime(_chargeTime);
            ShowChargeBar();
            if (_releasedButton)
            {
                CallAbilityEvent2();
            }
        }
    }

    public void CreateMasteryBThunderStrike()
    {
        Vector2 random_pos = UnityEngine.Random.insideUnitCircle * 3;
        AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "ThunderStrike_MasteryB", User.transform.position.x + random_pos.x + 1.5f * (User.Actions.IsFlipped ? -_distanceFromCaster : _distanceFromCaster), User.transform.position.y + random_pos.y);
        _masteryBThunderStrikeCounter++;
        if(User.Actions.CurrentAbilityBeingPerformed == this && _masteryBThunderStrikeCounter < _masteryBMaxThunderStrikes)
        {
            GameController.Instance.WaitAndRunMethod(_intervalBetweenMasteryBThunderstrikes, CreateMasteryBThunderStrike);
        }
        else
        {
            StopCountingTime();
        }
    }

    public override void CallAbilityEvent2()
    {
        ConsumeEnergyAndCooldownForTheAbility();
        if(Is(Property.UpgradeA))
        {
            CreateMasteryAThunderStrike();
        }
        else
        {
            CreateRegularThunderStrike();
        }
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        if(_upgradedThunderStrike)
        {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), _masteryAStunDuration);
        }
        else
        {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), GetValueBasedOnPercentageOfTimePassed(_minStunDuration, _maxStunDuration));
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_masteryBVFX != null)
        {
            MonoBehaviour.Destroy(_masteryBVFX.gameObject);
        }
    }
}