using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Damage {
    public bool DamageKilledTheTarget = false;
    public float SoundVolume = 1;
    public bool PlaySoundOnEnemyHit = true;
    public bool DamageWasBlocked = false;
    public bool DamageWasRiposted = false;
    public string CustomHitSound;
    public bool DestroyProjectileAfterDamageCalcuation = false;
    public bool DecreaseProjectileDurability = true;
    public DamagingObject DamagingObject { get; set; }
    public bool IsDamageOverTime { get; set; } = false;
    public bool IsExtraDamage { get; set; } = false;
    public Unit TargetOfDamage { get; set; }
    public Ability SourceOfDamage { get; set; }
    public enum DamageProperty { Burn, Freeze, Incision, CannotKill, CannotStagger, Supercharge, BurnExplosion, IsFinalAmmo };
    public List<DamageProperty> Properties = new List<DamageProperty>();
    public bool Is(DamageProperty property) {
        return Properties.Contains(property);
    }

    public bool IsNot(DamageProperty property) {
        return !Properties.Contains(property);
    }
    public Constants.DamageType DamageType;
    public Ability.DamageSource AbilityDamageSource = new(0, 0, Constants.DamageType.None);
    public float MaxDamage {get; set;} = 9999999;

    public Collider2D SourceOfCollision;
    public float Knockback { get; set; } = 0;
    public bool InjuryWasHigherThan0 = false;
    public bool StaggerWasHigherThan0  = false;
    public bool IsCriticalInjury = false;
    public bool IsCriticalStagger = false;
    public float ExtraDamageReduction = 0;
    public float RetainedDamageReductionPercentageWhileStaggered = 0;

    public float OverkillInjury {get; set;} = 0;
    public bool WillBeFatalBlow = false;
    public float Injury {get; set;} = 0;
    public float Stagger { get; set; } = 0;
    public float InjuryDealt { get; set; } = 0;
    public float StaggerDealt { get; set; } = 0;
    public bool ShouldHit { get; set; } = true;
    public bool IsExecute = false;
    public bool CanCauseFlinching = true;
    public float HealthLost { get; set; } = 0;
    public float DamageDealtMultiplier = 1;
    public float ExtraDamageDealtPercentage = 0;
    public float ExtraInjuryDealtPercentage = 0;
    public float ExtraStaggerDealtPercentage = 0;
    public float ExtraDamageDealtFlat = 0;
    public float ExtraInjuryDealtFlat = 0;
    public float ExtraStaggerDealtFlat = 0;

    public string Id;

    public Damage(Unit damage_target, Ability damaging_ability, DamagingObject source_of_collision) {
        Id = Guid.NewGuid().ToString();
        TargetOfDamage = damage_target;
        SourceOfDamage = damaging_ability;
        DamagingObject = source_of_collision;
        if(source_of_collision != null && (source_of_collision is Projectile || source_of_collision.GetType().IsSubclassOf(typeof(Projectile))))
        {
            Projectile proj = (Projectile)source_of_collision;
            DestroyProjectileAfterDamageCalcuation = DecreaseProjectileDurability && proj.DisappearsAfterNHits == 1;
        }
    }

    public bool IsWeaponDamage {
        get {
            return AbilityDamageSource != null && (DamageType == Constants.DamageType.Heavy || DamageType == Constants.DamageType.Light || DamageType == Constants.DamageType.Ranged);
        }
    }

    public Damage SetKnockback(float amount) {
        Knockback = amount;
        return this;
    }

    public Damage SetCanCauseFlinching(bool can_cause_flinching) {
        CanCauseFlinching = can_cause_flinching;
        return this;
    }
 
    public Damage SetDamageSource(Ability.DamageSource source) {
        AbilityDamageSource = source;
        return this;
    }

    public Damage SetDamageSource(float health_scaling, float stagger_scaling, Constants.DamageType damage_type = Constants.DamageType.None) {
        AbilityDamageSource = new Ability.DamageSource(health_scaling, stagger_scaling, damage_type);
        return this;
    }


    public Damage SetSoundVolume(float sound_volume) {
        SoundVolume = sound_volume;
        return this;
    }

    public Damage CalculateDamage() {
        if(TargetOfDamage.KnockedOut) {
            return this;
        }
        ActivateUnitBehaviourOnHit();
        if(DamagingObject is Projectile && ((Projectile)DamagingObject).IsFinalAmmo) {
            Properties.Add(DamageProperty.IsFinalAmmo);
        }
        DamageType = SourceOfDamage.ScalesWith == Constants.DamageType.CurrentWeapon ? Player.Instance.CurrentWeaponDamageType : SourceOfDamage.ScalesWith;
        EventManager.HitDealt.Invoke(this);
        CalculateDamageValues();
        EventManager.AfterHitDamageCalculation.Invoke(this);
        DecreaseTargetStaggerBar();
        DecreaseTargetHealth();
        if (InjuryDealt > 0 || StaggerDealt > 0) {
            CheckIfTargetShouldFlinch();
            CalculateKnockback();
            EnsureDistanceFromTarget();
            CalculateEnergyGeneration();
            ActivatePlayerBehaviourOnDamage();
            DisplayDamageAmount();
            UpdateInCombatStatus();
            if(WillBeFatalBlow) {
                EventManager.AboutToHandleFatalBlow.Invoke(this);
            }
            if(WillBeFatalBlow) {
                HandleFatalDamage();
            }
            PlaySound();
            PlayGroan();
            SourceOfDamage.User.MostRecentEnemyHit = TargetOfDamage;
            ProjectileSpecificActions();
            if(TargetOfDamage is Player == false && TargetOfDamage.Actions.CurrentAbilityBeingPerformed is AI_Observe)
            {
                TargetOfDamage.UnitAI.SkipObserve = true;
                TargetOfDamage.Actions.EndCurrentAbility();
            }
            EventManager.DamageDealt.Invoke(this);
        }
        else if(DamageWasBlocked)
        {
            ActivatePlayerBehaviourOnDamage();
            SourceOfDamage.User.MostRecentEnemyHit = TargetOfDamage;
            UpdateInCombatStatus();
            ProjectileSpecificActions();
            DisplayDamageAmount();
        }
        else {
            DisplayDamageAmount();
        }
        return this;
    }

    private void CalculateDamageValues() {
        float globalDamageModifier = 1;
        if(SourceOfDamage.User.IsHostile) {
            globalDamageModifier = SaveFile.Instance.GlobalEnemyDamageModifier;
        }
        float initialInjury = Injury;
        float initialStagger = Stagger;

        InjuryWasHigherThan0 = Injury > 0;
        StaggerWasHigherThan0 = Stagger > 0;

        float damageReduction = (TargetOfDamage.DamageReduction.Current + ExtraDamageReduction / 100 - SourceOfDamage.User.Penetration.Current) < 0 ? 1 : (1/(TargetOfDamage.DamageReduction.Current + ExtraDamageReduction / 100 - SourceOfDamage.User.Penetration.Current + 1));
        if(TargetOfDamage.IsStaggered) {
            damageReduction = 1 - ((1 - damageReduction) * RetainedDamageReductionPercentageWhileStaggered / 100);
        }

        Injury += GetCalculatedEffectiveWeaponDamage();

        float preModificationInjury = Injury;

        Injury = Injury * damageReduction * DamageDealtMultiplier * globalDamageModifier + ExtraInjuryDealtFlat + ExtraDamageDealtFlat;

        Stagger += GetCalculatedEffectiveWeaponDamage(false);

        float preModificationStagger= Stagger;

        Stagger = Stagger * damageReduction * DamageDealtMultiplier * globalDamageModifier + ExtraStaggerDealtFlat + ExtraDamageDealtFlat;

        Utils.CreateAuditLog(
            $"{Utils.GetFormattedFloat(Injury + Stagger)}D ({Utils.GetFormattedFloat(Injury)}I {Utils.GetFormattedFloat(Stagger)}S) dealt to {TargetOfDamage} by {SourceOfDamage.User} using {SourceOfDamage.GetType()} ({AbilityDamageSource.DamageType})" +

            $"\n\n{Utils.GetFormattedFloat(Injury)} Injury (Precalculation: {preModificationInjury}I, Stat: {SourceOfDamage.User.GetInjuryStatForGivenDamageType(AbilityDamageSource.DamageType).Current} + Extra {ExtraInjuryDealtPercentage + ExtraDamageDealtPercentage}% + Extra {ExtraInjuryDealtFlat + ExtraDamageDealtFlat} = {SourceOfDamage.User.GetInjuryStatForGivenDamageType(AbilityDamageSource.DamageType).Current + SourceOfDamage.User.GetInjuryStatForGivenDamageType(AbilityDamageSource.DamageType).Current * (ExtraInjuryDealtPercentage + ExtraDamageDealtPercentage) / 100} Injury, {AbilityDamageSource.InjuryScaling}% Base Scaling, {initialInjury} Extra Injury)" +

            $"\n{Utils.GetFormattedFloat(Stagger)} Stagger (Precalculation: {preModificationStagger}S, Stat: {SourceOfDamage.User.GetStaggerStatForGivenDamageType(AbilityDamageSource.DamageType).Current} + Extra {ExtraStaggerDealtPercentage + ExtraDamageDealtPercentage}% + Extra {ExtraStaggerDealtFlat + ExtraDamageDealtFlat} = {SourceOfDamage.User.GetStaggerStatForGivenDamageType(AbilityDamageSource.DamageType).Current + SourceOfDamage.User.GetStaggerStatForGivenDamageType(AbilityDamageSource.DamageType).Current * (ExtraStaggerDealtPercentage + ExtraDamageDealtPercentage) / 100} Stagger, {AbilityDamageSource.StaggerScaling}% Base Scaling, {initialStagger} Extra Stagger)" +

            $"\n{(TargetOfDamage.DamageReduction.Current - 1)* 100}% + {ExtraDamageReduction}% Damage Reduction vs {(SourceOfDamage.User.Penetration.Current - 1) * 100}% Penetration -> {damageReduction * 100}% Damage Dealt, DamageDealtMultiplier: {DamageDealtMultiplier}");

        InjuryWasHigherThan0 = InjuryWasHigherThan0 || Injury > 0;
        StaggerWasHigherThan0 = StaggerWasHigherThan0 || Stagger > 0;
    }

    public float GetCalculatedEffectiveWeaponDamage(bool is_injury = true) {
        float effectiveWeaponInjury = 0, effectiveWeaponStagger = 0, total = 0;
        if (is_injury && AbilityDamageSource.HybridInjurySource != null) {
            foreach(Constants.DamageType damage_type in AbilityDamageSource.HybridInjurySource.Keys) {
                effectiveWeaponInjury = SourceOfDamage.User.GetInjuryStatForGivenDamageType(damage_type).Current + SourceOfDamage.User.GetInjuryStatForGivenDamageType(damage_type).Current * (ExtraInjuryDealtPercentage + ExtraDamageDealtPercentage) / 100;
                total += AbilityDamageSource.HybridInjurySource[damage_type] / 100 * effectiveWeaponInjury;
            }
        }
        else if (is_injury){
            effectiveWeaponInjury = SourceOfDamage.GetInjuryStatForDamageSource(AbilityDamageSource).Current + SourceOfDamage.GetInjuryStatForDamageSource(AbilityDamageSource).Current * (ExtraInjuryDealtPercentage + ExtraDamageDealtPercentage) / 100;
            total += AbilityDamageSource.InjuryScaling / 100 * effectiveWeaponInjury;
        }
        else if (AbilityDamageSource.HybridStaggerSource != null) {
            foreach(Constants.DamageType damage_type in AbilityDamageSource.HybridStaggerSource.Keys) {
                effectiveWeaponStagger = SourceOfDamage.User.GetStaggerStatForGivenDamageType(damage_type).Current + SourceOfDamage.User.GetStaggerStatForGivenDamageType(damage_type).Current * (ExtraStaggerDealtPercentage + ExtraDamageDealtPercentage) / 100;
                total += AbilityDamageSource.HybridStaggerSource[damage_type] / 100 * effectiveWeaponStagger;
            }
        }
        else {
            effectiveWeaponStagger = SourceOfDamage.GetStaggerStatForDamageSource(AbilityDamageSource).Current + SourceOfDamage.GetStaggerStatForDamageSource(AbilityDamageSource).Current * (ExtraStaggerDealtPercentage + ExtraDamageDealtPercentage) / 100;
            total += AbilityDamageSource.StaggerScaling / 100 * effectiveWeaponStagger;
        }
        return total;
    }

    private void DecreaseTargetHealth() {
        float enemyHealthBarMaximum = TargetOfDamage.Health.Maximum;
        float currentHealth = TargetOfDamage.Health.Current;
        InjuryWasHigherThan0 = InjuryWasHigherThan0 || Injury > 0;
        if (Injury > 0) {
            if(Injury > 100000) {
                IsExecute = true;
            }
            Injury = Injury > MaxDamage ? MaxDamage : Injury;
            OverkillInjury = Injury - TargetOfDamage.Health.Current < 0 ? 0 : Injury - TargetOfDamage.Health.Current;
            InjuryDealt = Injury - TargetOfDamage.Health.Current > 0 ? TargetOfDamage.Health.Current + 0.1f : Injury;
            if(TargetOfDamage.Health.Current - Injury <= 0 && Properties.Contains(DamageProperty.CannotKill)) {
                TargetOfDamage.Health.Current = 1f;
            }
            else {
                TargetOfDamage.Health.Current -= InjuryDealt;
                if(TargetOfDamage.Health.Current <= 0) {
                    WillBeFatalBlow = true;
                }
            }
        }
        HealthLost = currentHealth - TargetOfDamage.Health.Current; 
        if((TargetOfDamage.IsBoss && InjuryDealt > enemyHealthBarMaximum * 0.25f) || (!TargetOfDamage.IsBoss && InjuryDealt > enemyHealthBarMaximum * 0.5f)) {
            IsCriticalInjury = true;
        }
    }

    public Damage DisableSoundOnEnemyHit() {
        PlaySoundOnEnemyHit = false;
        return this;
    }

    public Damage SetCustomHitSound(string sound_on_hit) {
        CustomHitSound = sound_on_hit;
        return this;
    }

    private void DecreaseTargetStaggerBar() {
        float enemyStaggerBarMaximum = TargetOfDamage.StaggerBar.Maximum;
        StaggerWasHigherThan0 = StaggerWasHigherThan0 || Stagger > 0;
        if (!TargetOfDamage.IsStaggered && Stagger > 0) {
            TargetOfDamage.StaggerBar.DealStaggerDamage(this);
        }
        else if(TargetOfDamage is not Player && TargetOfDamage.IsStaggered) {
            Injury += Stagger / 2;
        }
        if((TargetOfDamage.IsBoss && StaggerDealt > enemyStaggerBarMaximum * 0.4f) || (!TargetOfDamage.IsBoss && StaggerDealt > enemyStaggerBarMaximum * 0.7f)) {
            IsCriticalStagger = true;
        }
    }

    private void ActivateUnitBehaviourOnHit() {
        if (TargetOfDamage.CurrentTarget == null && !(TargetOfDamage is Player)) {
            TargetOfDamage.CurrentTarget = SourceOfDamage.User;
        }
    }

    private void CalculateKnockback() {
        if (Knockback != 0) {
            Transform source = DamagingObject != null ? DamagingObject.transform : SourceOfDamage.User.transform;
            bool source_to_the_left_of_target = SourceOfDamage.User.transform.position.x > TargetOfDamage.transform.position.x ? false : true;
            Vector2 direction_vector_towards_target = (TargetOfDamage.transform.position - (source.position + (source_to_the_left_of_target ? Vector3.left : Vector3.right))).normalized;
            TargetOfDamage.ApplyForce(direction_vector_towards_target * Knockback, SourceOfDamage);
        }
    }

    private void EnsureDistanceFromTarget() {
        if(SourceOfDamage.User is Player && TargetOfDamage is not Player && IsDamageOverTime == false) {
            Utils.KnockbackEnemyBasedOnMeleeWeaponDistance(this, AbilityDamageSource.KnockbackIntoRange != 0 ? AbilityDamageSource.KnockbackIntoRange : GetMinimumDistanceFromTarget(Utils.GetPlayerWeaponClassForDamageType(AbilityDamageSource.DamageType)));
        }
    }

    private float GetMinimumDistanceFromTarget(Constants.WeaponClass weapon) {
        return weapon switch
        {
            Constants.WeaponClass.Greatsword => 1.7f,
            Constants.WeaponClass.Longblade => 1.8f,
            Constants.WeaponClass.Polearm => 2.0f,
            Constants.WeaponClass.TwinBlades => 1.4f,
            Constants.WeaponClass.Daggers => 1.3f,
            Constants.WeaponClass.Gauntlets => 1.1f,
            Constants.WeaponClass.Gun => 1.5f,
            Constants.WeaponClass.Bow => 2.0f,
            Constants.WeaponClass.Cannon => 3.0f,
            Constants.WeaponClass.Magic => Player.Instance.CurrentStance.StanceEffect is Stance_MindOverMatter ? 1.3f : 2.0f,
            _ => 0,
        };
    }
    
    private void CalculateEnergyGeneration() {
        if (SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && SourceOfDamage.IsNot(Ability.AbilityProperty.AlreadyGeneratedEnergy)) {
            SourceOfDamage.User.Energy.GenerateEnergy(Constants.EnergyGainSource.BasicAttack, TargetOfDamage.IsBoss);
            SourceOfDamage.Properties.Add(Ability.AbilityProperty.AlreadyGeneratedEnergy);
        }
        else if (SourceOfDamage.EnergyGainedOnHit > 0) {
            SourceOfDamage.User.Energy.GenerateEnergy(SourceOfDamage.EnergyGainedOnHit);
            SourceOfDamage.Properties.Add(Ability.AbilityProperty.AlreadyGeneratedEnergy);
        }
        if (HealthLost > 0 && TargetOfDamage is Player) {
            TargetOfDamage.Energy.GenerateEnergy(Constants.EnergyGainSource.HealthLost, TargetOfDamage.IsBoss, HealthLost);
        }
    }

    private void CheckIfTargetShouldFlinch()
    {
        if (CanCauseFlinching && !TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Block)) && TargetOfDamage.Actions.CurrentActionBeingPerformed != Constants.ActionType.UnderHardCrowdControl && 
            ((TargetOfDamage is Player && StaggerDealt >= TargetOfDamage.StaggerBar.Maximum * Constants.PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_PLAYER_FLINCH / 100) || (TargetOfDamage is not Player && !TargetOfDamage.IsBoss && StaggerDealt >= TargetOfDamage.StaggerBar.Maximum * Constants.PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_REGULAR_FLINCH / 100) || (TargetOfDamage is not Player && TargetOfDamage.IsBoss &&  StaggerDealt >= TargetOfDamage.StaggerBar.Maximum * Constants.PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_BOSS_FLINCH / 100)) &&
            (TargetOfDamage.Actions.CurrentAbilityBeingPerformed == null ||
            (TargetOfDamage.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.AbilityProperty.ImmuneToFlinch) && !TargetOfDamage.Actions.CurrentAbilityBeingPerformed.Is(Ability.AbilityProperty.Counter) && !TargetOfDamage.Actions.CurrentAbilityBeingPerformed.Is(Ability.AbilityProperty.Unstoppable))))
        {
            Effect_ProtectFromFlinchingOnce protection = (Effect_ProtectFromFlinchingOnce)TargetOfDamage.GetEffect(typeof(Effect_ProtectFromFlinchingOnce));
            if (protection != null && TargetOfDamage.EffectCooldowns.FirstOrDefault(cooldown => cooldown.Type == typeof(Effect_ProtectFromFlinchingOnce)) == null) {
                TargetOfDamage.AddCooldown(protection, protection.Cooldown);
            }
            else
            {
                TargetOfDamage.AddEffect(new Effect_Flinching(new(SourceOfDamage)), Constants.DEFAULT_FLINCHING_DURATION);
            }
        }
    }

    private void ActivatePlayerBehaviourOnDamage() {
        if (TargetOfDamage is Player && IsDamageOverTime == false) {
            CameraController.Instance.ShakeScreen();
        }
        if(DamagingObject is UnitWeapon) {
            if(SourceOfDamage.User is Player && SourceOfDamage.Is(Ability.AbilityProperty.Technique)) {
                Player.Instance.Animator.SetFloat("Technique Speed", Player.Instance.TechniqueSpeed * 0.15f);
            }
            else {
                SourceOfDamage.User.Animator.SetFloat(AbilityDamageSource.DamageType.ToString() + " Attack Speed", SourceOfDamage.User.GetAttackSpeedStatForGivenDamageType(AbilityDamageSource.DamageType).ScaledWithCombatSpeed * 0.15f);
            }
            SourceOfDamage.User.HitStopFramesRemaining = AbilityDamageSource.DamageType == Constants.DamageType.Light ? Constants.MELEE_HITSTOP_DURATION_IN_FIXED_FRAMES / 2 : Constants.MELEE_HITSTOP_DURATION_IN_FIXED_FRAMES;
            CameraController.Instance.ShakeScreen(AbilityDamageSource.DamageType == Constants.DamageType.Light ? 0.05f : 0.08f, AbilityDamageSource.DamageType == Constants.DamageType.Light ? 0.03f : 0.05f, 1);
        }
    }

    private void HandleFatalDamage() {
        if ((TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Unkillable)) || TargetOfDamage.CheckIfUnderEffect(typeof(Effect_CannotBeDefeated))) && TargetOfDamage.Health.Current <= 0 && TargetOfDamage.CurrentHealthBars == 1) {
            TargetOfDamage.Health.Current = 0.1f;
            if(TargetOfDamage.CheckIfUnderEffect(typeof(Effect_CannotBeDefeated))) {
                TargetOfDamage.Actions.EndCurrentAbility();
                TargetOfDamage.Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
                TargetOfDamage.PlayAnimation("HeavilyWounded");
                if(TargetOfDamage is not Player && TargetOfDamage.IsHostile && ((Effect_CannotBeDefeated)TargetOfDamage.GetEffect(typeof(Effect_CannotBeDefeated))).GrantsExperienceOnDefeat) {
                    ((Effect_CannotBeDefeated)TargetOfDamage.GetEffect(typeof(Effect_CannotBeDefeated))).GrantsExperienceOnDefeat = false;
                    GiveResourcesAfterDefeat();
                }
                TargetOfDamage.InCombat = false;
                TargetOfDamage.AddEffect(new Effect_CannotContinueCombat(new(TargetOfDamage)));
                TargetOfDamage.AddEffect(new Effect_CannotEnterCombat(new(TargetOfDamage)), 2);
                EventManager.UnitWouldBeDefeated.Invoke(this);
            }
            return;
        }
        if (TargetOfDamage.Health.Current <= 0 && GameController.Instance.EnemiesCanBeKilled && TargetOfDamage.CurrentHealthBars > 1) {
            TargetOfDamage.CurrentHealthBars--;
            TargetOfDamage.Health.Maximum = TargetOfDamage.HealthBars[TargetOfDamage.HealthBars.Count - TargetOfDamage.CurrentHealthBars] * (TargetOfDamage.IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1);
            TargetOfDamage.Health.Current = TargetOfDamage.Health.Maximum;
            EventManager.HealthBarBroken.Invoke(this);
            if (TargetOfDamage.CurrentHealthBars == TargetOfDamage.HealthBars.Count - 1 && TargetOfDamage.UnitAI.ActionsAfter1HBarBroken.Count > 0)
            {
                TargetOfDamage.UnitAI.AvailableActions = TargetOfDamage.UnitAI.ActionsAfter1HBarBroken;
                TargetOfDamage.UnitAI.InitializeAvailableActions();
            }
            if (TargetOfDamage.CurrentHealthBars == TargetOfDamage.HealthBars.Count - 2 && TargetOfDamage.UnitAI.ActionsAfter2HBarsBroken.Count > 0)
            {
                TargetOfDamage.UnitAI.AvailableActions = TargetOfDamage.UnitAI.ActionsAfter2HBarsBroken;
                TargetOfDamage.UnitAI.InitializeAvailableActions();
            }
            TargetOfDamage.AddEffect(new Effect_HealthBarBroken(new(SourceOfDamage)), 3);
            TargetOfDamage.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(SourceOfDamage)) {PercentageModifier = 200});
        }
        else if (TargetOfDamage.Health.Current <= 0 && GameController.Instance.EnemiesCanBeKilled) {
            EventManager.UnitWouldBeDefeated.Invoke(this);
            if(TargetOfDamage.Health.Current > 0) {
                return;
            }
            TargetOfDamage.CurrentHealthBars--;
            DamageKilledTheTarget = true;
            if(TargetOfDamage.ProducesHumanSounds) {
                Utils.PlaySoundEffect(TargetOfDamage.AudioSource, TargetOfDamage.IsMale ? "Grunts/Male Death " + UnityEngine.Random.Range(1, 6) : "Grunts/Female Death " + UnityEngine.Random.Range(1, 4));
            }
            if (TargetOfDamage is Player) {
                if(GameController.Instance.InterruptMusicOnDeath) {
                    Utils.SetDefaultMusic("Death");
                }
                UIManager.Instance.ShowGameOverScreen();
            }
            else {
                if (SaveFile.Instance.GameType == Constants.GameType.Story)
                {
                    GiveResourcesAfterDefeat();
                }
                DeactivateUnit(TargetOfDamage);
                bool active_enemy = false;
                foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Unit"))
                {
                    Unit unit = enemy.GetComponent<Unit>();
                    if(unit != null && unit.Faction == Constants.Faction.Enemy && unit.InCombat && unit.KnockedOut == false)
                    {
                        active_enemy = true;
                    }
                }
                if(!active_enemy)
                {
                    Player.Instance.InCombat = false;
                }
            }
            EventManager.UnitKnockedOut.Invoke(this);
        }
    }

    public void GiveResourcesAfterDefeat() {
        SaveFile.Instance.ExperiencePoints += (int)Utils.GetScaledExperienceGain(TargetOfDamage.Level, TargetOfDamage.ExperienceGainOnDefeat == 0 ? 100 : TargetOfDamage.ExperienceGainOnDefeat);
        Player.Instance.Ammo += (TargetOfDamage.IsBoss ? 3 : 1);
        if(TargetOfDamage.IsBoss) {
            SaveFile.Instance.HealChargesRemaining = SaveFile.Instance.MaxHealCharges;
        }
        else {
            SaveFile.Instance.HealChargesRemaining++;
        }
    }

    public static void DeactivateUnit(Unit defeated_unit) {
        if(defeated_unit == null) {
            return;
        }
        List<Transform> loot = new();
        foreach(Transform child in defeated_unit.transform) {
            if(defeated_unit.gameObject != null && !defeated_unit.gameObject.IsDestroyed() && child.gameObject.name.Contains("Loot")) {
                loot.Add(child);
            }
        }
        foreach(Transform item in loot) {
            item.gameObject.SetActive(true);
            item.SetParent(defeated_unit.transform.parent);
            item.gameObject.name = item.gameObject.name + " (" + defeated_unit.gameObject.name + ")";
            item.position = new Vector3(defeated_unit.transform.position.x + UnityEngine.Random.Range(-0.75f, 0.75f), defeated_unit.transform.position.y + UnityEngine.Random.Range(-0.75f, 0.75f));
        }
        defeated_unit.PutAllWeaponsBehind();
        if(defeated_unit.OnScreenBars != null)
        {
            MonoBehaviour.Destroy(defeated_unit.OnScreenBars);
        }
        if (defeated_unit.CompareTag("Enemy")) {
            defeated_unit.tag = "Knocked Out";
        }
        if (!(defeated_unit is Player) && Player.Instance.CurrentTarget == defeated_unit) {
            Player.Instance.CurrentTarget = null;
        }
        defeated_unit.enabled = false;
        NavMeshAgent agent = defeated_unit.GetComponent<NavMeshAgent>();
        if (agent != null) {
            agent.enabled = false;
        }
        UnitAI unitAI = defeated_unit.GetComponent<UnitAI>();
        if (unitAI != null) {
            unitAI.enabled = false;
        }
        defeated_unit.SpriteRenderers["Lower Body"].Bone.gameObject.tag = "Untagged";
        foreach (Transform child in defeated_unit.transform) {
            if (child.name.Contains("World Space Canvas") == false && child.name != "Lower Body" && !child.name.Contains("IK")) {
                child.gameObject.SetActive(false);
            }
        }
        foreach (DestroyOnUnitKnockedOut item in defeated_unit.GetComponentsInChildren<DestroyOnUnitKnockedOut>()) {
            MonoBehaviour.Destroy(item.gameObject);
        }
        foreach (Transform child in defeated_unit.transform.Find("World Space Canvas").transform)
        {
            if (!child.name.Contains("Indicator")) {
                child.gameObject.SetActive(false);
            }
        }
        foreach(Effect e in defeated_unit.CurrentEffects.ToList()) {
            e.EndThisEffect();
        }
        foreach(ParticleSystem ps in defeated_unit.GetComponentsInChildren<ParticleSystem>(true)) {
            if(ps.gameObject.name.Contains("PersistsOnDeath") == false) {
                ps.Stop();
            }
        }
        if(defeated_unit.Health.HUDSlider != null) {
            defeated_unit.Health.HUDSlider.gameObject.SetActive(false);
        }
        if(defeated_unit.StaggerBar.HUDSlider != null) {
            defeated_unit.StaggerBar.HUDSlider.gameObject.SetActive(false);
        }
        defeated_unit.Actions.enabled = false;
        defeated_unit.Animator.SetInteger("Current Action", 5);
        defeated_unit.KnockedOut = true;
        GameController.Instance.WaitAndRunMethod(2, TurnOffRigidbody, defeated_unit);
    }

    public static void TurnOffRigidbody(Unit defeated_unit) {
        Rigidbody2D rigidbody = defeated_unit.GetComponent<Rigidbody2D>();
        if (rigidbody != null) {
            rigidbody.simulated = false;
        }
    }

    private void PlaySound() {
        if (SourceOfDamage.PlaySoundOnlyOnce && SourceOfDamage.PlayedSoundAtLeastOnce) {
            return;
        }
        if (PlaySoundOnEnemyHit && CustomHitSound == null) {
            if(String.IsNullOrWhiteSpace(SourceOfDamage.CustomHitSound) == false) {
                Utils.PlaySoundEffect(TargetOfDamage.AudioSource, SourceOfDamage.CustomHitSound, SoundVolume);
            } 
            else {
                string hit_type = SourceOfDamage?.HitSoundType.ToString() ?? Utils.DetermineHitTypeBasedOnAbilityWeaponClass(SourceOfDamage.User.CurrentWeaponClass);
                Utils.PlaySoundEffect(TargetOfDamage.AudioSource, "Hit/" + hit_type + (IsCriticalStagger || IsCriticalInjury ? "_CriticalHit" + UnityEngine.Random.Range(1, 4) : "_Hit" + UnityEngine.Random.Range(1, 7)), (IsCriticalStagger || IsCriticalInjury ? 1.4f : 1) * SoundVolume);
            }
            SourceOfDamage.PlayedSoundAtLeastOnce = true;
        }
        else if (PlaySoundOnEnemyHit && CustomHitSound != null) {
            Utils.PlaySoundEffect(TargetOfDamage.AudioSource, CustomHitSound, SoundVolume);
            SourceOfDamage.PlayedSoundAtLeastOnce = true;
        }
    }

    private void PlayGroan() {
        if(!TargetOfDamage.ProducesHumanSounds) {
            return;
        }
        if(Injury > TargetOfDamage.Health.Maximum * 0.4f || Stagger > TargetOfDamage.StaggerBar.Maximum * 0.4f) {
            Utils.PlaySoundEffect(TargetOfDamage.AudioSource, TargetOfDamage is Player ? "Grunts/Player Big Groan 1" : !TargetOfDamage.IsMale ? "Grunts/Female Big Groan 1" : "Grunts/Male Big Groan " + UnityEngine.Random.Range(1, 5), 0.7f);
        }
        else if(Injury > TargetOfDamage.Health.Maximum * 0.1f || Stagger > TargetOfDamage.StaggerBar.Maximum * 0.1f) {
            Utils.PlaySoundEffect(TargetOfDamage.AudioSource, TargetOfDamage is Player ? "Grunts/Player Small Groan 1" : !TargetOfDamage.IsMale ? "Grunts/Female Small Groan 1" : "Grunts/Male Small Groan " + UnityEngine.Random.Range(1, 11), 0.4f);
        }
    }

    public bool CheckIfDamageWorksWithDefensiveAbilities() {
        return IsDamageOverTime == false;
    }

    private void DisplayDamageAmount() {
        if(GameController.Instance.GameplayMode != Constants.GameplayMode.Regular) {
            return;
        }
        if (TargetOfDamage is Player || SourceOfDamage.User is Player) {
            if((InjuryDealt + OverkillInjury) == 0 && StaggerDealt == 0) {
                GameObject injuryIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_InjuryIndicator")) as GameObject;
                injuryIndicator.transform.SetParent(TargetOfDamage.WorldSpaceCanvas.transform);
                injuryIndicator.GetComponent<TextMeshProUGUI>().text = (TargetOfDamage is Player && (Player.Instance.CheckIfUnderEffect(typeof(Effect_Backstep)) || Player.Instance.CheckIfUnderEffect(typeof(Effect_RollForward)) || Player.Instance.CheckIfUnderEffect(typeof(Effect_RollSideways)))) ? Label.Get("DamageDisplay_Dodged") : "0";
                injuryIndicator.transform.localScale = new Vector2(0.0075f * Settings.Instance.DamageNumbersSize, 0.0075f * Settings.Instance.DamageNumbersSize);
                injuryIndicator.transform.position = new Vector2(TargetOfDamage.transform.position.x - 0.3f + UnityEngine.Random.Range(-0.2f, 0.2f), TargetOfDamage.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f));
                    injuryIndicator.GetComponent<TextMeshProUGUI>().color = Color.grey;
                return;
            }
            if ((InjuryDealt + OverkillInjury) > 0) {
                GameObject injuryIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_InjuryIndicator")) as GameObject;
                injuryIndicator.transform.SetParent(TargetOfDamage.WorldSpaceCanvas.transform);
                injuryIndicator.GetComponent<TextMeshProUGUI>().text = IsExecute ? Label.Get("ExecuteDisplay") : ((int)(InjuryDealt + OverkillInjury)).ToString();
                float scale = IsExecute ? 0.012f * Settings.Instance.DamageNumbersSize : Utils.GetValueBasedOnMinAndMax(InjuryDealt + OverkillInjury, 0, 1000 * Utils.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.01f, 0.0375f) + (IsCriticalInjury ? 0.01f : 0);
                scale *= Settings.Instance.DamageNumbersSize;
                injuryIndicator.transform.localScale = new Vector2(scale * (IsCriticalInjury ? 1f : 0.75f), scale * (IsCriticalInjury ? 1f : 0.75f));
                injuryIndicator.transform.position = new Vector2(TargetOfDamage.transform.position.x - 0.3f + UnityEngine.Random.Range(-0.2f, 0.2f), TargetOfDamage.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f));
                if(IsCriticalInjury) {
                    injuryIndicator.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#FF3900");
                }
                if(Properties.Contains(DamageProperty.Supercharge)) {
                    injuryIndicator.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#FFBD00");
                }
            }
            if (StaggerDealt > 0) {
                GameObject staggerIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_StaggerIndicator")) as GameObject;
                staggerIndicator.transform.SetParent(TargetOfDamage.WorldSpaceCanvas.transform);
                staggerIndicator.GetComponent<TextMeshProUGUI>().text = ((int)StaggerDealt).ToString();
                float scale = Utils.GetValueBasedOnMinAndMax(StaggerDealt, 0, 1000 * Utils.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.0075f, 0.015f)  + (IsCriticalStagger ? 0.01f : 0);
                scale *= Settings.Instance.DamageNumbersSize;
                staggerIndicator.transform.localScale = new Vector2(scale * (IsCriticalStagger ? 1f : 0.75f), scale * (IsCriticalStagger ? 1f : 0.75f));
                staggerIndicator.transform.position = new Vector2(TargetOfDamage.transform.position.x + 0.3f + UnityEngine.Random.Range(-0.2f, 0.2f), TargetOfDamage.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f));
                if(IsCriticalStagger) {
                    staggerIndicator.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#9064FF");
                }
            }
        }
    }

    public void CheckIfShouldDestroyProjectile() {
        if(DamagingObject is Projectile && ((Projectile)DamagingObject).OnlyDestroyOnTargetHit) {
            if(TargetOfDamage == ((Projectile)DamagingObject).Target) {
                DamagingObject.MakeObjectDisappear(0);
            }
            else {
                return;
            }
        }
        if (DecreaseProjectileDurability && DestroyProjectileAfterDamageCalcuation && DamagingObject != null && DamagingObject.gameObject.IsDestroyed() == false) {
            DamagingObject.MakeObjectDisappear(0);
        }
    }

    private void ProjectileSpecificActions()
    {
        if(DamagingObject != null && (DamagingObject is Projectile || DamagingObject.GetType().IsSubclassOf(typeof(Projectile))))
        {
            Projectile proj = (Projectile)DamagingObject;
            if(proj.DisappearsAfterNHits > 0 && DecreaseProjectileDurability)
            {
                proj.DisappearsAfterNHits--;
            }
            Transform on_hit_vfx = proj.transform.Find("OnHit");
            if (on_hit_vfx != null && proj.DisappearsAfterNHits > 0 && DamageWasRiposted == false)
            {
                GameObject cloned_on_hit_vfx = MonoBehaviour.Instantiate(on_hit_vfx.gameObject);
                cloned_on_hit_vfx.gameObject.SetActive(true);
                cloned_on_hit_vfx.transform.position = proj.transform.position;
                cloned_on_hit_vfx.transform.localScale = on_hit_vfx.localScale;
            }
            else if (on_hit_vfx != null && DestroyProjectileAfterDamageCalcuation && DamageWasRiposted == false)
            {
                on_hit_vfx.gameObject.SetActive(true);
                on_hit_vfx.SetParent(on_hit_vfx.parent.parent);
                on_hit_vfx.transform.position = proj.transform.position;
            }
        }
    }

    private void UpdateInCombatStatus()
    {
        TargetOfDamage.InCombat = true;
        SourceOfDamage.User.InCombat = true;
        if(TargetOfDamage is Player || SourceOfDamage.User is Player)
        {
            Player.Instance.InCombatTimer = Constants.DEFAULT_FIXED_FRAMES_UNTIL_EXITING_COMBAT;
        }
        if(TargetOfDamage is not Player && TargetOfDamage.UnitAI.NavMeshAgent.enabled == false && !TargetOfDamage.CheckIfUnderEffect(typeof(Effect_RootedInPlace))) {
            TargetOfDamage.UnitAI.NavMeshAgent.enabled = true;
        }
    }
}