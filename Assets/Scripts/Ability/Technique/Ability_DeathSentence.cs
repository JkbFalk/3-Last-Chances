using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class Ability_DeathSentence : Technique
{

    private static float _chargeTime = 5;
    private static float _percentageOfHeavyDamageAsInjuryMinimum = 500;
    private static float _percentageOfHeavyDamageAsInjuryMaximum = 1000;
    private static float _masteryBRegularEnemyMinimumHealth = 50;
    private static float _masteryBRegularEnemyMaximumHealth = 100;
    private static float _masteryBEliteEnemyMinimumHealth = 20;
    private static float _masteryBEliteEnemyMaximumHealth = 40;

    public static float EnergyCost = 75;
    public static float Cooldown = 90;

    private GameObject _visualEffect;

    private List<Unit> _validTargetsForMasteryBExecute = new List<Unit>();
    private List<GameObject> _executeVfx = new List<GameObject>();

    public static AbilityFamily Family = AbilityFamily.Salutis;
    public static Constants.DamageType TechniqueDamageType {
        get {
            return Player.Instance.CurrentStance.DamageType == Constants.DamageType.Light ? Constants.DamageType.Light : Constants.DamageType.Heavy;
        }
    }

    public Ability_DeathSentence(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Shadow;
        Properties.Add(Property.Charged);
        HitSoundVolume = 0.5f;
        DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Heavy));
        DamageSources[0].CustomHitSound = "Ability/DeathSentence";
        AddCustomSound("Charge", "Ability/Ability_DeathSentence_Charge", 0.5f);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { _chargeTime.ToString(), (Player.Instance.HeavyInjury.Current * _percentageOfHeavyDamageAsInjuryMinimum / 100).ToString(), _percentageOfHeavyDamageAsInjuryMinimum.ToString(), (Player.Instance.HeavyInjury.Current * _percentageOfHeavyDamageAsInjuryMaximum / 100).ToString(), _percentageOfHeavyDamageAsInjuryMaximum.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _masteryBRegularEnemyMinimumHealth.ToString(), _masteryBRegularEnemyMaximumHealth.ToString(), _masteryBEliteEnemyMinimumHealth.ToString(), _masteryBEliteEnemyMaximumHealth.ToString(), };
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        StartCountingTime(_chargeTime);
        ShowChargeBar();
        _visualEffect = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/VisualEffect_DeathSentence" + (Is(Property.UpgradeA) ? "_MasteryA" : Is(Property.UpgradeB) ? "_MasteryB" : ""))) as GameObject;
        _visualEffect.transform.SetParent(User.SpriteRenderers["Heavy"].Bone);
        _visualEffect.transform.localPosition = new Vector2(1f, 0);
        _visualEffect.transform.localRotation = Quaternion.Euler(0, 0, -90);
        if(Is(Property.UpgradeB))
        {
            foreach(GameObject item in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Unit unit = item.GetComponent<Unit>();
                if(unit != null && User.CheckIfHostileTowards(unit.Faction))
                {
                    _validTargetsForMasteryBExecute.Add(unit);
                }
            }
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        MonoBehaviour.Destroy(_visualEffect.gameObject);
        if (Is(Property.UpgradeB))
        {
            foreach(GameObject item in _executeVfx)
            {
                MonoBehaviour.Destroy(item.gameObject);
            }
        }
    }

    public override void OnAbilityButtonRelease() {
        base.OnAbilityButtonRelease();
        PerformAttack();
    }

    public void PerformAttack() {
        if (CountingTime) {
            StopCountingTime();
            User.PlayAnimation("DeathSentence", 0, 0.72f);
        }
    }

    public override void CallAbilityEvent1() {
        StopCountingTime();
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit) {
        if (!AffectedEnemies.ContainsKey(unit_getting_attacked)) {
            UpdateAffectedEnemyList(unit_getting_attacked, object_hitting);
            float InjuryAmount = GetValueBasedOnPercentageOfTimePassed(_percentageOfHeavyDamageAsInjuryMinimum, _percentageOfHeavyDamageAsInjuryMaximum) / 100 * User.HeavyInjury.Current;
            Effect_Staggered staggered = (Effect_Staggered)unit_getting_attacked.CurrentEffects.FirstOrDefault(effect => effect.GetType().IsSubclassOf(typeof(Effect_Staggered)));
            if (Is(Property.UpgradeB))
            {
                float execution_range = unit_getting_attacked.IsBoss ? GetValueBasedOnPercentageOfTimePassed(_masteryBEliteEnemyMinimumHealth, _masteryBEliteEnemyMaximumHealth) : GetValueBasedOnPercentageOfTimePassed(_masteryBRegularEnemyMinimumHealth, _masteryBRegularEnemyMaximumHealth);
                if (unit_getting_attacked.Health.Current / unit_getting_attacked.Health.Maximum < execution_range / 100)
                {
                    InjuryAmount = Constants.EXECUTE_DAMAGE_AMOUNT;
                }
            }
            Damage damage = new Damage(unit_getting_attacked, this, object_hitting)
            .SetDamageSource(InjuryAmount, 0, Constants.DamageType.Heavy)
            .CalculateAndApplyDamage();
            if (Is(Property.UpgradeA) && damage.DamageKilledTheTarget) {
                GameObject vfx = Utils.CreateVisualEffect(new(this), "CooldownReset");
                vfx.transform.position = User.transform.position + new Vector3(0, 0.5f);
                User.RemoveCooldown(GetType());
                User.Energy.GenerateEnergy(EnergyCost / 2);
            }
        }
    }

    public override void AdditionalActionsOnUpdate()
    {
        if(Is(Property.UpgradeB))
        {
            List<Unit> targetsLeft = new List<Unit>();
            foreach(Unit target in _validTargetsForMasteryBExecute)
            {
                float execution_range = target.IsBoss? GetValueBasedOnPercentageOfTimePassed(_masteryBEliteEnemyMinimumHealth, _masteryBEliteEnemyMaximumHealth) : GetValueBasedOnPercentageOfTimePassed(_masteryBRegularEnemyMinimumHealth, _masteryBRegularEnemyMaximumHealth);
                if (target.Health.Current / target.Health.Maximum < execution_range / 100)
                {
                    GameObject vfx = Utils.CreateVisualEffect(new(this), "Execution");
                    vfx.transform.SetParent(target.SpriteRenderers["Upper Body"].Bone);
                    vfx.transform.localPosition = new Vector2(0.4f, 0);
                    _executeVfx.Add(vfx);
                }
                else
                {
                    targetsLeft.Add(target);
                }
            }
            _validTargetsForMasteryBExecute = targetsLeft;
        }
    }
}