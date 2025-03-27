using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_Flamethrower : Technique
{
    private int _cycle = 0;
    private bool _techniqueEnded = false;
    public static float EnergyCost = 1;
    public static float Cooldown = 1f;
    private AreaOfEffect _aoe;
    private GameObject _ultimateVFX;
    private bool _ultimateHitATarget = false;
    public static float MagicInjuryScaling = 200;
    public static float MagicStaggerBurnScaling = 20;
    public static float MasteryBMagicStaggerBurnScaling = 30;
    public static float UltimateMagicStaggerScaling = 200;
    public static float UltimateMagicInjuryPerBurn = 30;
    private bool _cyclesStarted = false;
    private bool _createdFlamethrower = false;
    public static bool IsStacksBasedTechnique = true;
    public static int MaxStacks {
        get {
            return SaveFile.Instance.ActiveUpgrades.Contains("Ability_Flamethrower_UpgradeA") ? 150 : 100;
        }
    }
    public static int UltimateMaxStacks {
        get {
            return 100;
        }
    }
    private List<Unit> _alreadyAffectedEnemies = new();
    private Dictionary<Unit, float> UltimateEnemiesAndBurn = new();

    public static AbilityFamily Family = AbilityFamily.Ignis;
    public static Constants.DamageType TechniqueDamageCategory = Constants.DamageType.Magic;

    public Ability_Flamethrower(Unit ability_user) : base(ability_user)
    {
        AddCustomSound("Use", "Ability/Ability_Flamethrower", 0.2f);
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        if(Player.Instance.PreparingForUltimate) {
            NameOfAnimationToAutoPlay = "Flamethrower_Ultimate";
            DamageSources.Add(new DamageSource(0, UltimateMagicStaggerScaling, Constants.DamageType.Magic, "SmallCircleAoE") {Knockback = 600});
            DamageSources.Add(new DamageSource(1, 0, Constants.DamageType.Magic, "Ultimate AoE"));
            return;
        }
        DamageSources.Add(new DamageSource(MagicInjuryScaling, 0, Constants.DamageType.Magic));
        NameOfAnimationToAutoPlay = (SaveFile.Instance.ActiveUpgrades.Contains("Ability_Flamethrower_UpgradeB") && Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.Moving) ? "Flamethrower_Moving" : "Flamethrower"; 
        if(SaveFile.Instance.ActiveUpgrades.Contains("Ability_Flamethrower_UpgradeB")) {
            CanMoveWhileUsing = true;
        }
    }

    public void AdvanceCycle() {
        if(_techniqueEnded) {
            return;
        }
        if(Player.Instance.CurrentTechniqueStacks[typeof(Ability_Flamethrower)] > 0 && Player.Instance.Energy.Current >= 1 && HoldingTechniqueButton) {
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_Flamethrower), Player.Instance.CurrentTechniqueStacks[typeof(Ability_Flamethrower)] - 1);
            Player.Instance.Energy.Current -= 1;
        }
        else {
            _techniqueEnded = true;
            Player.Instance.PlayAnimation("Flamethrower", 0.05f, 0.6f);
            EndThisAbility();
            return;
        }
        _cycle++;
        if(_cycle % 10 == 0) {
            ResetPotentialTargets();
        }
        if(UpgradeAUnlocked && _cycle <= 100) {
            _aoe.transform.localScale = new Vector2(1 + _cycle * 0.01f, 1 + _cycle * 0.01f);
            ParticleSystem.MainModule main = _aoe.GetComponent<ParticleSystem>().main;
            var startSize = main.startSize;
            startSize.constantMin = 3 - 1.5f * _cycle / 100f;
            startSize.constantMax = 4 - 2f * _cycle / 100f;
            var emissionModule = _aoe.GetComponent<ParticleSystem>().emission;
            emissionModule.rateOverTime = 100 + 3f * _cycle;
            var velocityOverTimeModule = _aoe.GetComponent<ParticleSystem>().velocityOverLifetime;
            ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
            curve.mode = ParticleSystemCurveMode.TwoConstants;
            curve.constantMin = -1 - 1 * _cycle / 100f;
            curve.constantMax = 1 + 1 * _cycle / 100f;
            velocityOverTimeModule.x = curve;
            Player.Instance.Animator.SetFloat("Technique Speed", 1 + _cycle * 0.02f);
        }
        GameController.Instance.WaitAndRunMethod(0.1f / Player.Instance.Animator.GetFloat("Technique Speed"), AdvanceCycle);
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        _ultimateVFX = Utils.CreateVisualEffect(new(this), "Flamethrower_Ultimate_Marker", 0, 0);
        _ultimateVFX.GetComponent<AttachObjectToBodyPart>().BodyPartName = "Right Hand";
        _ultimateVFX.GetComponent<AttachObjectToBodyPart>().Initialize(Player.Instance);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        _techniqueEnded = true;
        if(_aoe != null && _aoe.IsDestroyed() == false ) {
            _aoe.MakeObjectDisappear();
        }
        if(_ultimateVFX != null && _ultimateVFX.IsDestroyed() == false) {
            MonoBehaviour.Destroy(_ultimateVFX);
        }
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(Is(AbilityProperty.Ultimate) && object_hitting.gameObject.name == "SmallCircleAoE" && (unit_getting_attacked != Target || _ultimateHitATarget)) {
            return;
        }
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        if(Is(AbilityProperty.Ultimate)) {
            if((damage.AbilityDamageSource.ColliderName != "SmallCircleAoE" || !_ultimateHitATarget) && !UltimateEnemiesAndBurn.ContainsKey(damage.TargetOfDamage)) {
                _ultimateHitATarget = true;
                Effect_Burn appliedBurn = (Effect_Burn)damage.TargetOfDamage.GetEffect(typeof(Effect_Burn));
                if(appliedBurn != null) {
                    appliedBurn.ChangeDecayingAmount(appliedBurn.DecayingAmount * Player.Instance.CurrentUltimateTechniqueStacks[typeof(Ability_Flamethrower)] / 100 + appliedBurn.DecayingAmount * Player.Instance.Energy.Current / 100);
                    UltimateEnemiesAndBurn.Add(damage.TargetOfDamage, appliedBurn.DecayingAmount);
                    appliedBurn.EndThisEffect();
                    GameObject vfx = Utils.CreateVisualEffect(new(this), "Flamethrower_Ultimate_Marker", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
                    vfx.GetComponent<AttachObjectToBodyPart>().Initialize(damage.TargetOfDamage);
                    GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(0.75f, 1.75f), MakeEnemyExplode, damage.TargetOfDamage);
                }
                if(damage.AbilityDamageSource.ColliderName == "SmallCircleAoE") {
                    Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_Flamethrower), 0, true);
                    User.Actions.ConsumeEnergyAndCooldownForTheAbility();
                    PlayCustomSound("Hit/Fire_Hit" + UnityEngine.Random.Range(1, 7), 1, damage.TargetOfDamage.AudioSource);
                    MonoBehaviour.Destroy(_ultimateVFX);
                }
                else {
                    PlayCustomSound("Fire/FireExplosion1", 1, damage.TargetOfDamage.AudioSource);
                }
            }
            return;
        }
        if(UpgradeBUnlocked && _alreadyAffectedEnemies.Contains(damage.TargetOfDamage) == false) {
            _alreadyAffectedEnemies.Add(damage.TargetOfDamage);
            damage.TargetOfDamage.AddEffect(new Effect_Burn((MagicStaggerBurnScaling + MasteryBMagicStaggerBurnScaling) * User.MagicStagger.Current / 100, new(this)));
        }
        else {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(MagicStaggerBurnScaling * User.MagicStagger.Current / 100, new(this)));
        }
    }

    public void MakeEnemyExplode(Unit enemy) {
        if(enemy.SpriteRenderers["Upper Body"].Bone.transform.Find("Flamethrower_Ultimate_Marker") != null) {
            MonoBehaviour.Destroy(enemy.SpriteRenderers["Upper Body"].Bone.transform.Find("Flamethrower_Ultimate_Marker").gameObject);
        }
        AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "Flamethrower_Ultimate", enemy.transform.position.x, enemy.transform.position.y);
        Damage d = new Damage(enemy, this, aoe) {AbilityDamageSource = new(0, 0, Constants.DamageType.Magic), Injury = UltimateMagicInjuryPerBurn * UltimateEnemiesAndBurn[enemy]};
        d.CalculateDamage();
    }

    public static void OnEquip()
    {
        EventManager.EffectStarted.AddListener(AddStacks);
        EventManager.EffectEmpowered.AddListener(AddStacksForAlreadyBurningEnemy);
    }

    public static void OnUnequip()
    {
        EventManager.EffectStarted.RemoveListener(AddStacks);
        EventManager.EffectEmpowered.RemoveListener(AddStacksForAlreadyBurningEnemy);
    }

    public static void AddStacks(Effect effect) {
        if(effect?.SourceOfEffect?.User != null && effect.SourceOfEffect.User is Player && effect?.TargetOfEffect != null && effect.TargetOfEffect.IsHostile && effect is Effect_Burn) {
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_Flamethrower), Player.Instance.CurrentTechniqueStacks[typeof(Ability_Flamethrower)] + 1);
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_Flamethrower), Player.Instance.CurrentTechniqueStacks[typeof(Ability_Flamethrower)] + 1, true);
        }
    }

    public static void AddStacksForAlreadyBurningEnemy(Effect effect1, Effect effect2) {
        if(effect2?.SourceOfEffect?.User != null && effect2.SourceOfEffect.User is Player && effect2?.TargetOfEffect != null && effect2.TargetOfEffect.IsHostile && effect2 is Effect_Burn) {
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_Flamethrower), Player.Instance.CurrentTechniqueStacks[typeof(Ability_Flamethrower)] + 1);
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_Flamethrower), Player.Instance.CurrentTechniqueStacks[typeof(Ability_Flamethrower)] + 1, true);
        }
    }

    public override void CallAbilityEvent1()
    {
        if(_createdFlamethrower) {
            return;
        }
        _createdFlamethrower = true;
        _aoe = Utils.CreateAreaOfEffect(new(this), "Flamethrower");
        _aoe.transform.SetParent(User.SpriteRenderers["Lower Body"].Bone);
        _aoe.transform.eulerAngles = new Vector3(0, 0, 90 * (User.Actions.IsFlipped ? 1 : -1));
        _cyclesStarted = true;
        if(IsNot(AbilityProperty.Ultimate)) {
            AdvanceCycle();
        }
    }

    public override void CallAbilityEvent2()
    {
        if(_techniqueEnded == false) {
            Player.Instance.PlayAnimation("Flamethrower", 0, 0.2f);
        }
    }

    public override void CallAbilityEvent3()
    {
        Target = Player.Instance.CurrentTarget == null ? User.GetClosestValidTarget(true) : Player.Instance.CurrentTarget;
        ChaseCurrentTargetAtGivenDegreeAngle(200, 45, 20, Target);
    }

    public override void OnAbilityButtonRelease()
    {
        if(_cyclesStarted) {
            _techniqueEnded = true;
            Player.Instance.PlayAnimation("Flamethrower", 0.05f, 0.6f);
            EndThisAbility();
        }
    }

    public override void AdditionalAbilitySpecificActionsOnTryingToMove() {
        if(UpgradeBUnlocked == false || _cyclesStarted == false) {
            return;
        }
        if(User.Actions.TryingToMoveInDirection.Count > 0) {
            User.PlayAnimation("Flamethrower_Moving");
        }
        else if(User.Actions.TryingToMoveInDirection.Count == 0){
            User.PlayAnimation("Flamethrower", 0.1f, 0.25f);
        }
    }
    public static string GetAbilitySpecificEnergyCostText() {
        return GetEnergyCost(typeof(Ability_Flamethrower)) * 10 + "/" + Label.Get("CostPerSecond");
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.MagicInjury.Current * MagicInjuryScaling / 100).ToString(), MagicInjuryScaling.ToString(), (Player.Instance.MagicStagger.Current * MagicStaggerBurnScaling / 100).ToString(), MagicStaggerBurnScaling.ToString()};
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> {};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { };
    }
        public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { UltimateMagicInjuryPerBurn.ToString() };
    }
}