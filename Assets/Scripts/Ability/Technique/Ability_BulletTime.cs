using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Ability_BulletTime : Technique
{
    private bool _shootingBullet = false;
    private Unit _enemyMasteryAMarked;
    private int _bullets_shot_counter = 0;
    private List<GameObject> _indicators = new List<GameObject>();

    public static float EnergyCost = 100;
    public static float Cooldown = 30;

    private static float _timeSpeed = 0.2f;
    private static float _maxShots = 12f;
    private static float _injury = 250f;
    private static float _stagger = 500f;

    private static float _markCount = 5;
    private static float _freezeStaggerScaling = 500;
    private static float _masteryAFreezeStaggerScaling = 1500;
    private static float _masteryAOnslaughtApplied = 200;
    private static float _masteryAAnalyzedApplied = 200;
    private static float _masteryBInjuryScaling = 800;

    public static AbilityFamily Family = AbilityFamily.Glacies;
    public static Constants.DamageType TechniqueDamageCategory = Constants.DamageType.Ranged;
    public Dictionary<Unit, int> TimesEachUnitWasHit = new();

    public Ability_BulletTime(Unit ability_user) : base(ability_user) {
        AutoPlayAbilityAnimation = false;
        AddCustomSound("Start", "Ability/Ability_BulletTime_Use", 0.9f);
        AddCustomSound("Shoot", "Ice/Ice_Shot1", 0.5f);
        DamageSources.Add(new DamageSource(UpgradeBUnlocked ? _masteryBInjuryScaling : _injury, UpgradeBUnlocked ? 0 : _stagger, Constants.DamageType.Ranged));
        CanMoveWhileUsing = true;
        User.UnitCanFlipDirection = false;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (_timeSpeed * 100).ToString(), _maxShots.ToString(), (Player.Instance.RangedInjury.Current * _injury / 100).ToString(), _injury.ToString(), (Player.Instance.RangedStagger.Current * _stagger / 100).ToString(), _stagger.ToString(), (Player.Instance.RangedStagger.Current * _freezeStaggerScaling / 100).ToString(), _freezeStaggerScaling.ToString()};
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { "5", (Player.Instance.RangedStagger.Current * _masteryAFreezeStaggerScaling / 100).ToString(), _masteryAFreezeStaggerScaling.ToString(), _masteryAAnalyzedApplied.ToString(), _masteryAOnslaughtApplied.ToString()};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { (Player.Instance.RangedInjury.Current * _masteryBInjuryScaling / 100).ToString(), _masteryBInjuryScaling.ToString() };
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        User.Actions.ConsumeEnergyAndCooldownForTheAbility();
        PlayCustomSound("Start");
        GameController.Instance.DefaultTimeSpeed = _timeSpeed;
        CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Slowed Time") as VolumeProfile;
        ScaleMaxTimeWithCombatSpeed = false;
        ScaleMaxTimeWithAttackSpeed = false;
        StartCountingTime(3f);
    }

    public override void AdditionalActionsOnUpdate()
    {
        base.AdditionalActionsOnUpdate();
        if(PercentageOfMaxTimePassed >= 100)
        {
            User.Actions.EndCurrentAbility();
        }
    }

    public override void OnAbilityEnd() {
        base.OnAbilityEnd();
        GameController.Instance.DefaultTimeSpeed = 1f;
        CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Regular") as VolumeProfile;
        User.UnitCanFlipDirection = true;
        if(_indicators.Count > 0)
        {
            foreach(GameObject item in _indicators)
            {
                MonoBehaviour.Destroy(item);
            }
        }
    }

    public override void OnMainButtonPress() {
        if (_shootingBullet == false) {
            _bullets_shot_counter++;
            User.PlayAnimation(GetType().ToString() + "_" + User.CurrentRangedWeaponClass, 0);
        }
    }

    public override void CallAbilityEvent1() {
        _shootingBullet = true;
    }

    public override void CallAbilityEvent2() {
        _shootingBullet = false;
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile) {
        if (_bullets_shot_counter >= _maxShots) {
            User.Actions.EndCurrentAbility();
        }
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if (_enemyMasteryAMarked != null && _enemyMasteryAMarked == unit_getting_attacked && collider_being_hit.gameObject.name != "VisualEffect_BulletTime_MasteryA")
        {
            return;
        }
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        int indicator_count = _indicators.Count;
        if(collider_being_hit.gameObject.name == "VisualEffect_BulletTime_MasteryA")
        {
            _indicators.Remove(collider_being_hit.gameObject);
            MonoBehaviour.Destroy(collider_being_hit.gameObject);
        }
        if (_enemyMasteryAMarked != null && indicator_count == 1 && _indicators.Count == 0)
        {
            unit_getting_attacked.AddEffect(new Effect_Freeze(Player.Instance.RangedStagger.Current * _masteryAFreezeStaggerScaling / 100, new(this)));
            unit_getting_attacked.AddEffect(new Effect_Analysis(_masteryAAnalyzedApplied, new(this)));
            unit_getting_attacked.AddEffect(new Effect_Onslaught(_masteryAOnslaughtApplied, new(this)));
            EndThisAbility();
        }
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Freeze(Player.Instance.RangedStagger.Current * _freezeStaggerScaling / 100, new(this)));
        if(UpgradeBUnlocked && TimesEachUnitWasHit.ContainsKey(damage.TargetOfDamage)) {
            TimesEachUnitWasHit[damage.TargetOfDamage]++;
        }
        else if(UpgradeBUnlocked){
            TimesEachUnitWasHit.Add(damage.TargetOfDamage, 1);
        }
        if(UpgradeBUnlocked) {
            for(int i = 1; i < TimesEachUnitWasHit[damage.TargetOfDamage]; i++) {
                damage.DamageDealtMultiplier *= 0.75f;
            }
            GameObject ind = Utils.CreateVisualEffect(new(this), "BulletTime_MasteryB", damage.TargetOfDamage.transform.position.x + UnityEngine.Random.Range(-0.25f, 0.25f), damage.TargetOfDamage.transform.position.y + UnityEngine.Random.Range(-0.25f, 0.25f));
            ind.transform.SetParent(damage.TargetOfDamage.SpriteRenderers["Lower Body"].Bone);
            ind.transform.localScale = new Vector2(0.5f, 0.5f);
            _indicators.Add(ind);
        }
        else if(_enemyMasteryAMarked == null && UpgradeAUnlocked)
        {
            _enemyMasteryAMarked = damage.TargetOfDamage;
            for (int i =0; i < _markCount; i++)
            {
                _indicators.Add(Utils.CreateVisualEffect(new(this), "BulletTime_MasteryA", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y));
            }
            _indicators[0].transform.SetParent(damage.TargetOfDamage.SpriteRenderers["Right Hand"].Bone, false);
            _indicators[1].transform.SetParent(damage.TargetOfDamage.SpriteRenderers["Left Hand"].Bone, false);
            _indicators[2].transform.SetParent(damage.TargetOfDamage.SpriteRenderers["Right Foot"].Bone, false);
            _indicators[3].transform.SetParent(damage.TargetOfDamage.SpriteRenderers["Left Foot"].Bone, false);
            _indicators[4].transform.SetParent(damage.TargetOfDamage.SpriteRenderers["Head"].SpriteRenderer.transform, false);
            foreach (GameObject item in _indicators)
            {
                item.transform.position = item.transform.parent.position;
                item.gameObject.name = "VisualEffect_BulletTime_MasteryA";
            }
        }
    }

    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        if(GameController.Instance.DefaultTimeSpeed != 1) {
            NotificationController.ShowTextNotification(Label.Get("SlowedTimeAbilityRestrictionWArning"));
        }
        return GameController.Instance.DefaultTimeSpeed == 1;
    }
}