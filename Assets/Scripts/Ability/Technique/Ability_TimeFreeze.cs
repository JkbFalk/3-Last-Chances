using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Ability_TimeFreeze : Technique
{
    public static float EnergyCost = 100;
    public static float Cooldown = 120;
    public static AbilityFamily Family = AbilityFamily.Glacies;
    private bool _shootingBullet = false;
    private Unit _enemyMarked;
    private int _shotsCount = 0;
    private GameObject _vfx;
    private List<GameObject> _marks = new List<GameObject>();

    private static float _timeSpeed = 0.2f;
    private static float _rangedInjuryScaling = 150f;
    private static float _magicStaggerFreezeScaling = 15;
    private static float _allMarksHitStaggerRangedScaling = 300;
    private static float _allMarksHitStaggerMagicScaling = 700;
    private static float _masteryAInjuryScaling = 120f;
    private static float _masteryAStaggerScaling = 120f;
    private static float _masteryBEnergyGained = 25f;
    private static float _masteryBCooldownRefundedInSeconds = 40f;
    private static float _ammoGained = 5;
    private static float _upgradeAAmmoGained = 10;
    private static float _markCount = 5;
    private static float _ultimateTechniqueSpeedIncrease = 100;

    public Ability_TimeFreeze(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        AutoPlayAbilityAnimation = false;
        AddCustomSound("Start", "Ability/TimeSlowDown", 0.9f);
        AddCustomSound("Shoot", "Ice/Ice_Shot1", 0.5f);
        CanMoveWhileUsing = true;
        User.UnitCanFlipDirection = false;
        DamageSources.Add(new DamageSource(Is(Property.UpgradeA) ? _masteryAInjuryScaling : _rangedInjuryScaling, Is(Property.UpgradeA) ? _masteryAStaggerScaling : 0, Constants.DamageType.Ranged));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(_timeSpeed * 100), Utils.GetFormattedFloat(_ammoGained), Utils.GetFormattedFloat(_markCount), (Player.Instance.RangedInjury.Current * _rangedInjuryScaling / 100).ToString(), _rangedInjuryScaling.ToString(), (Player.Instance.MagicStagger.Current * _magicStaggerFreezeScaling / 100).ToString(), _magicStaggerFreezeScaling.ToString(),  (Player.Instance.RangedStagger.Current * _allMarksHitStaggerRangedScaling / 100 + Player.Instance.MagicStagger.Current * _allMarksHitStaggerMagicScaling / 100).ToString(), _allMarksHitStaggerRangedScaling.ToString(), _allMarksHitStaggerMagicScaling.ToString()};
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { (Player.Instance.RangedInjury.Current * _masteryAInjuryScaling / 100).ToString(), _masteryAInjuryScaling.ToString(), (Player.Instance.RangedInjury.Current * _masteryAStaggerScaling / 100).ToString(), _masteryAStaggerScaling.ToString()};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _masteryBEnergyGained.ToString(), _masteryBCooldownRefundedInSeconds.ToString() };
    }
    
    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        ConsumeEnergyAndCooldownForTheAbility();
        PlayCustomSound("Start");
        GameController.Instance.DefaultTimeSpeed = _timeSpeed;
        CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Time Freeze") as VolumeProfile;
        ScaleMaxTimeWithCombatSpeed = false;
        ScaleMaxTimeWithAttackSpeed = false;
        _enemyMarked = Player.Instance.CurrentTarget;
        Player.Instance.Ammo += Is(Property.UpgradeA) ? _upgradeAAmmoGained : _ammoGained;
        if (IsNot(Property.Ultimate) && IsNot(Property.UpgradeA))
        {
            if (_enemyMarked == null)
            {
                _enemyMarked = Player.Instance.GetClosestValidTarget();
            }
            if (_enemyMarked == null)
            {
                EndThisAbility();
            }
            for (int i = 0; i < _markCount; i++)
            {
                _marks.Add(Utils.CreateVisualEffect(new(this), "TimeFreezeMark", _enemyMarked.transform.position.x, _enemyMarked.transform.position.y));
            }
            _marks[0].transform.SetParent(_enemyMarked.SpriteRenderers["Right Hand"].Bone, false);
            _marks[1].transform.SetParent(_enemyMarked.SpriteRenderers["Left Hand"].Bone, false);
            _marks[2].transform.SetParent(_enemyMarked.SpriteRenderers["Right Foot"].Bone, false);
            _marks[3].transform.SetParent(_enemyMarked.SpriteRenderers["Left Foot"].Bone, false);
            _marks[4].transform.SetParent(_enemyMarked.SpriteRenderers["Head"].SpriteRenderer.transform, false);
            foreach (GameObject item in _marks)
            {
                item.transform.position = item.transform.parent.position;
                item.gameObject.name = "VisualEffect_TimeFreezeMark";
            }
        }
        if (Is(Property.Ultimate))
        {
            EventManager.AfterHitDamageCalculation.AddListener(ConvertInjuryIntoStagger);
            Player.Instance.Animator.SetFloat("Technique Speed", 1 + _ultimateTechniqueSpeedIncrease / 100);
            GameController.Instance.WaitAndRunMethod(4, FinishAbility);
        }
        _vfx = Utils.CreateVisualEffect(new(this), "TimeFreeze", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
        _vfx.transform.SetParent(Player.Instance.transform);
        UIManager.Objects.AmmoDisplayImage.gameObject.SetActive(true);
        Player.Instance.CurrentTarget = null;
    }

    public void FinishAbility()
    {
        EndThisAbility();
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        GameController.Instance.DefaultTimeSpeed = 1f;
        CameraController.Instance.Camera.GetComponent<Volume>().profile = Resources.Load("Camera Profiles/Regular") as VolumeProfile;
        if (_marks.Count > 0)
        {
            foreach (GameObject item in _marks)
            {
                MonoBehaviour.Destroy(item);
            }
        }
        if (Is(Property.Ultimate))
        {
            EventManager.AfterHitDamageCalculation.RemoveListener(ConvertInjuryIntoStagger);
            Player.Instance.Animator.SetFloat("Technique Speed", 1);
        }
        if (_vfx != null && _vfx.IsDestroyed() == false)
        {
            MonoBehaviour.Destroy(_vfx);
        }
        UIManager.Objects.AmmoDisplayImage.gameObject.SetActive(Player.Instance.CurrentStance == SaveFile.Instance.Stances[2]);
        User.UnitCanFlipDirection = true;
    }

    public override void OnBasicAttackButtonPress() {
        if (_shootingBullet == false)
        {
            _shotsCount++;
            User.PlayAnimation(GetType().ToString() + "_" + User.CurrentRangedWeaponClass, 0);
            if (IsNot(Property.Ultimate))
            {
                Player.Instance.Ammo--;
            }
        }
    }

    public override void OnAbilityButtonPress()
    {
        EndThisAbility();
        base.OnAbilityButtonPress();
    }

    public override void OnBlockButtonPress()
    {
        EndThisAbility();
        base.OnBlockButtonPress();
    }

    public override void OnDodgeButtonPress()
    {
        EndThisAbility();
        base.OnDodgeButtonPress();
    }

    public override void CallAbilityEvent1()
    {
        _shootingBullet = true;
    }

    public override void CallAbilityEvent2()
    {
        _shootingBullet = false;
        if (Is(Property.UpgradeA) && _shotsCount >= 10)
        {
            EndThisAbility();
        }
        else if (Player.Instance.Ammo < 1)
        {
            EndThisAbility();
        }
    }

    public override void CallAbilityEvent3()
    {
        Utils.CreateProjectile(new(this), "TimeFreeze" + (Is(Property.Ultimate) ? "_Ultimate" : ""));
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if ((Is(Property.Ultimate) && unit_getting_attacked.IsStaggered) || (IsNot(Property.Ultimate) && IsNot(Property.UpgradeA) && collider_being_hit.gameObject.name != "VisualEffect_TimeFreezeMark"))
        {
            return;
        }
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        if (IsNot(Property.Ultimate) && IsNot(Property.UpgradeA))
        {
            _marks.Remove(collider_being_hit.gameObject);
            MonoBehaviour.Destroy(collider_being_hit.gameObject);
            unit_getting_attacked.AddEffect(new Effect_Freeze(Player.Instance.MagicStagger.Current * _magicStaggerFreezeScaling / 100, new(this)));
            if (_enemyMarked != null && _marks.Count == 0)
            {
                DamageInstance explosionDamage = new DamageInstance(unit_getting_attacked, this, object_hitting);
                DamageSource source = new DamageSource(null, new Dictionary<Constants.DamageType, float>() { { Constants.DamageType.Ranged, _allMarksHitStaggerRangedScaling }, { Constants.DamageType.Magic, _allMarksHitStaggerMagicScaling } }, Constants.DamageType.Magic);
                explosionDamage.AbilityDamageSource = source;
                explosionDamage.CalculateAndApplyDamage();
                if (Is(Property.UpgradeB))
                {
                    Player.Instance.Energy.GenerateEnergy(_masteryBEnergyGained);
                    Cooldown cd = Player.Instance.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == typeof(Ability_TimeFreeze));
                    if (cd != null)
                    {
                        cd.RemainingDuration -= _masteryBCooldownRefundedInSeconds;
                    }
                }
                EndThisAbility();
            }
        }
        else if (Is(Property.Ultimate))
        {
            unit_getting_attacked.AddEffect(new Effect_Freeze(Player.Instance.MagicStagger.Current * _magicStaggerFreezeScaling / 100, new(this)));
        }
    }

    public void ConvertInjuryIntoStagger(DamageInstance damage)
    {
        if (damage.SourceOfDamage.User == Player.Instance)
        {
            damage.Stagger += damage.Injury;
            damage.Injury = 0;
        }
    }
    
    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        if (GameController.Instance.DefaultTimeSpeed != 1)
        {
            NotificationController.ShowTextNotification(Label.Get("SlowedTimeAbilityRestrictionWArning"));
        }
        return GameController.Instance.DefaultTimeSpeed == 1;
    }
}