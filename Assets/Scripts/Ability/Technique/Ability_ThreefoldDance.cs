using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_ThreefoldDance : Technique
{
    private GameObject _vfx;
    public static float EnergyCost = 10;
    public static float Cooldown = 15;

    private int _slashCounter = 0;
    private bool _holdingAbilityButton = true;
    public static float HeavyHealthScaling = 220;
    public static float HeavyStaggerScaling = 220;
    public static float HeavyHealthScalingMasteryA = 50;
    public static float HeavyStaggerScalingMasteryA = 350;
    public static float HeavyHealthScalingMasteryA3rdWave = 700;
    public static float MasteryBSpeedIncrease = 10;
    public static float MasteryBAnalyzedApplied = 10;
    private List<AbilityInterruptType> _possibleInterruptions = new List<AbilityInterruptType> {AbilityInterruptType.Dodge, AbilityInterruptType.BasicAttack, AbilityInterruptType.Block, AbilityInterruptType.EnergyAbility, AbilityInterruptType.StanceSwitch};

    public static AbilityFamily Family = AbilityFamily.Anima;
    public static Constants.DamageType TechniqueDamageCategory = Constants.DamageType.Heavy;
    public Ability_ThreefoldDance(Unit ability_user) : base(ability_user)
    {
        if(UpgradeAUnlocked) {
            DamageSources.Add(new DamageSource(HeavyHealthScalingMasteryA, HeavyStaggerScalingMasteryA, Constants.DamageType.Heavy, "ThreefoldDance_1"));
            DamageSources.Add(new DamageSource(HeavyHealthScalingMasteryA3rdWave, 0, Constants.DamageType.Heavy, "ThreefoldDance_3") {Knockback = 500});
        }
        else {
            DamageSources.Add(new DamageSource(HeavyHealthScaling, HeavyStaggerScaling, Constants.DamageType.Heavy));
        }
        AddCustomSound("Random1", "Ability/Ability_ThreefoldDance_1", 0.7f);
        AddCustomSound("Random2", "Ability/Ability_ThreefoldDance_2", 0.7f);
        AddCustomSound("Random3", "Ability/Ability_ThreefoldDance_3", 0.7f);
        AddCustomSound("Random4", "Ability/Ability_ThreefoldDance_4", 0.7f);
        AddCustomSound("Shoot1", "Ability/Ability_ThreefoldDance_Shoot1", 0.6f);
        AddCustomSound("Shoot3", "Ability/Ability_ThreefoldDance_Shoot3", 0.6f);
        TransitionIntoAnimationDuration = 0f;
        EffectsAffectingUserDuringAbility = new();
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.HeavyInjury.Current * HeavyHealthScaling / 100).ToString(), HeavyHealthScaling.ToString(), (Player.Instance.HeavyStagger.Current * HeavyStaggerScaling / 100).ToString(), HeavyStaggerScaling.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { (Player.Instance.HeavyInjury.Current * HeavyHealthScalingMasteryA / 100).ToString(), HeavyHealthScalingMasteryA.ToString(), (Player.Instance.HeavyStagger.Current * HeavyStaggerScalingMasteryA / 100).ToString(), HeavyStaggerScalingMasteryA.ToString(), (Player.Instance.HeavyInjury.Current * HeavyHealthScalingMasteryA3rdWave / 100).ToString(), HeavyHealthScalingMasteryA3rdWave.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { MasteryBSpeedIncrease.ToString(), MasteryBAnalyzedApplied.ToString()};
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        _vfx = Utils.CreateVisualEffect(new(this), "ThreefoldDance");
        _vfx.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        _vfx.transform.localPosition = new Vector2(1, 0);
        _vfx.transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityStart();
        MonoBehaviour.Destroy(_vfx);
        User.Actions.TurnOffWeaponCollision("Heavy");
    }

    public override void OnAbilityButtonPress() 
    {
        base.OnAbilityButtonPress();
        _holdingAbilityButton = true;
    }

    public override void OnAbilityButtonRelease()
    {
        base.OnAbilityButtonRelease();
        _holdingAbilityButton = false;
    }


    public override void CallAbilityEvent1()
    {
        if(UpgradeBUnlocked) {
            User.Animator.SetFloat("Technique Speed", 1 + _slashCounter * MasteryBSpeedIncrease / 100);
        }
        CanAlwaysBeInterruptedBy.Clear();
        if(User.Energy.Current >= EnergyCost && (_holdingAbilityButton || _slashCounter == 0) && (UpgradeBUnlocked || _slashCounter < 3)) {
            _slashCounter++;
            User.Actions.ConsumeEnergyAndCooldownForTheAbility();
            User.Actions.TurnOnWeaponCollision("Heavy");
            if(UpgradeAUnlocked && _slashCounter == 3) {
                PlayCustomSound("Shoot" + (_slashCounter != 3 ? "1" : "3"));
                Projectile proj = Utils.CreateProjectile(new(this), "ThreefoldDance_" + (_slashCounter != 3 ? "1" : "3"));
                proj.transform.localEulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? 90 : -90);
                proj.CleanUpAfter();
            }
            else if(UpgradeAUnlocked) {
                GameController.Instance.WaitAndRunMethod(0.1f / User.HeavyAttackSpeed.Current, CreateProjectile);
            }
            else {
                PlayCustomSound("Random" + UnityEngine.Random.Range(1, 5));
            }
        }
        else {
            EndThisAbility();
        }
    }

    public void CreateProjectile() {
        PlayCustomSound("Shoot" + (_slashCounter != 3 ? "1" : "3"));
        Projectile proj = Utils.CreateProjectile(new(this), "ThreefoldDance_" + (_slashCounter != 3 ? "1" : "3"));
        proj.CleanUpAfter();
        proj.transform.eulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? 90 : -90);
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        if(UpgradeBUnlocked) {
            damage.TargetOfDamage.AddEffect(new Effect_Analysis(MasteryBAnalyzedApplied, new(this)));
        }
        Vector3 positionInFront = User.transform.position + (User.Actions.IsFlipped ? Vector3.left : Vector3.right);
        if(damage.AbilityDamageSource.ColliderName == "ThreefoldDance_1") {
            damage.TargetOfDamage.ApplyForce((positionInFront - damage.TargetOfDamage.transform.position) * 100, this);
        }
        else if(damage.AbilityDamageSource.ColliderName == "ThreefoldDance_3"){
            damage.TargetOfDamage.ApplyForce((positionInFront - damage.TargetOfDamage.transform.position) * 70, this);
            damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        }
    }

        public static string GetAbilitySpecificEnergyCostText() {
        return "10-" + (SaveFile.Instance.AbilitiesMasteryB.Contains(typeof(Ability_ThreefoldDance)) ? "90" : "30");
    }
}
