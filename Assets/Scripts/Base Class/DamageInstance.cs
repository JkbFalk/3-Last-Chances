using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class DamageInstance {
    public static float GlobalEnemyDamageModifier {
        get {
            switch(SaveFile.Instance.DifficultyLevel) {
                case 0: return 0.6f;
                case 1: return 1f;
                case 2: return 1.2f;
                case 3: return 1.3f;
                default: return 1;
            }
        }
    }
    public static float GlobalEnemySurvivabilityModifier {
        get {
            switch(SaveFile.Instance.DifficultyLevel) {
                case 0: return 0.7f;
                case 1: return 1f;
                case 2: return 1.5f;
                case 3: return 2f;
                default: return 1;
            }
        }
    }
    public bool DamageKilledTheTarget = false;
    public float SoundVolume = 1;
    public bool PlaySoundOnEnemyHit = true;
    public bool DamageWasBlocked = false;
    public bool DamageWasRiposted = false;
    public string CustomHitSound;
    public bool DestroyProjectileAfterDamageCalcuation = false;
    public bool DecreaseProjectileDurability = true;
    public DamagingObject DamagingObject { get; set; }
    public Unit TargetOfDamage { get; set; }
    public Ability SourceOfDamage { get; set; }
    public enum DamageProperty { Burn, Freeze, Bleed, CannotKill, CannotStagger, Supercharge, BurnExplosion, FinalAmmo, DamageOverTime, Execute, CriticalStagger, CriticalInjury, ExtraDamage, Tool };
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
    public float KnockbackInMeters { get; set; } = 0;
    public bool InjuryWasHigherThan0 = false;
    public bool StaggerWasHigherThan0  = false;
    public float ArmorModifier = 0;
    public float ArmorPenetrationModifier = 0;
    public float RetainedArmorPercentageWhileStaggered = 0;

    public float OverkillInjury {get; set;} = 0;
    public bool WillBeFatalBlow = false;
    public float Injury {get; set;} = 0;
    public float Stagger { get; set; } = 0;
    public float Damage { get { return Injury + Stagger; } }
    public float InjuryDealt { get; set; } = 0;
    public float StaggerDealt { get; set; } = 0;
    public float DamageDealt { get { return InjuryDealt + StaggerDealt; } }
    public float PreMitigationInjury { get; set; } = 0;
    public float PreMitigationStagger { get; set; } = 0;
    public float PreMitigationDamage { get { return PreMitigationInjury + PreMitigationStagger; } }
    public bool CanCauseFlinching = true;
    public float HealthLost { get; set; } = 0;
    public float DamageDealtMultiplier = 1;
    public float DamageDealtPercentageModifier = 0;
    public float InjuryDealtPercentageModifier = 0;
    public float StaggerDealtPercentageModifier = 0;
    public float DamageDealtFlatModifier = 0;
    public float InjuryDealtFlatModifier = 0;
    public float StaggerDealtFlatModifier = 0;
    public string Id;
    public bool TriggersOnHitEffects
    {
        get {
            return IsNot(DamageProperty.DamageOverTime) && IsNot(DamageProperty.BurnExplosion) && IsNot(DamageProperty.ExtraDamage);
        }
    }

    public DamageInstance(Unit damage_target, Ability damaging_ability, DamagingObject source_of_collision)
    {
        Id = Guid.NewGuid().ToString();
        TargetOfDamage = damage_target;
        SourceOfDamage = damaging_ability;
        DamagingObject = source_of_collision;
        if (source_of_collision != null && (source_of_collision is Projectile || source_of_collision.GetType().IsSubclassOf(typeof(Projectile))))
        {
            Projectile proj = (Projectile)source_of_collision;
            DestroyProjectileAfterDamageCalcuation = DecreaseProjectileDurability && proj.DisappearsAfterNHits == 1;
        }
    }

    public bool IsWeaponDamage {
        get {
            return AbilityDamageSource != null && (DamageType == Constants.DamageType.Heavy || DamageType == Constants.DamageType.Light || DamageType == Constants.DamageType.Ranged || DamageType == Constants.DamageType.CurrentWeapon);
        }
    }

    public DamageInstance SetDamageSource(float health_scaling, float stagger_scaling, Constants.DamageType damage_type = Constants.DamageType.None) {
        AbilityDamageSource = new Ability.DamageSource(health_scaling, stagger_scaling, damage_type);
        return this;
    }

    public DamageInstance CalculateAndApplyDamage() {
        if(TargetOfDamage.KnockedOut) {
            return this;
        }
        ActivateUnitBehaviourOnHit();
        if(DamagingObject is Projectile && ((Projectile)DamagingObject).IsFinalAmmo) {
            Properties.Add(DamageProperty.FinalAmmo);
        }
        DamageType = SourceOfDamage.ScalesWith == Constants.DamageType.CurrentWeapon ? Player.Instance.CurrentWeaponDamageType : SourceOfDamage.ScalesWith;
        EventManager.HitDealt.Invoke(this);
        CalculateDamageValues();
        EventManager.AfterHitDamageCalculation.Invoke(this);
        DecreaseTargetStaggerBar();
        DecreaseTargetHealth();
        if (DamageDealt > 0) {
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
            globalDamageModifier = GlobalEnemyDamageModifier;
        }
        float initialInjury = Injury;
        float initialStagger = Stagger;

        InjuryWasHigherThan0 = Injury > 0;
        StaggerWasHigherThan0 = Stagger > 0;

        float effectiveArmor = (TargetOfDamage.Armor.Current + ArmorModifier) * (1 - ArmorPenetrationModifier / 100);
        if(TargetOfDamage.IsStaggered && effectiveArmor > 0) {
            effectiveArmor *= RetainedArmorPercentageWhileStaggered / 100;
        }
        float armorDamageReduction = 1 / (1 + effectiveArmor / 100);

        Injury += GetCalculatedEffectiveWeaponDamage();

        PreMitigationInjury = (Injury + InjuryDealtFlatModifier + DamageDealtFlatModifier) * DamageDealtMultiplier * globalDamageModifier;

        Injury = PreMitigationInjury * armorDamageReduction;

        Stagger += GetCalculatedEffectiveWeaponDamage(false);

        PreMitigationStagger = (Stagger + StaggerDealtFlatModifier + DamageDealtFlatModifier) * DamageDealtMultiplier * globalDamageModifier;

        Stagger = PreMitigationStagger * armorDamageReduction;

        string injuryAuditLogString = ((InjuryDealtFlatModifier + DamageDealtFlatModifier) != 0 ? $"({PreMitigationInjury} + {Utils.GetFormattedFloat(InjuryDealtFlatModifier + DamageDealtFlatModifier)}" : $"{PreMitigationInjury}") + ((InjuryDealtPercentageModifier + DamageDealtPercentageModifier) != 0 ? $" * {Utils.GetFormattedFloat(InjuryDealtPercentageModifier + DamageDealtPercentageModifier)}" : "") + (DamageDealtMultiplier != 1 ? $" x{Utils.GetFormattedFloat(DamageDealtMultiplier, 2)}" : "");

        string staggerAuditLogString = ((StaggerDealtFlatModifier + DamageDealtFlatModifier) != 0 ? $"({PreMitigationStagger} + {Utils.GetFormattedFloat(StaggerDealtFlatModifier + DamageDealtFlatModifier)}" : $"{PreMitigationStagger}") + ((StaggerDealtPercentageModifier + DamageDealtPercentageModifier) != 0 ? $" * {Utils.GetFormattedFloat(StaggerDealtPercentageModifier + DamageDealtPercentageModifier)}" : "") + (DamageDealtMultiplier != 1 ? $" x{Utils.GetFormattedFloat(DamageDealtMultiplier, 2)}" : "");

        /*Utils.CreateAuditLog(
            $"{Utils.GetFormattedFloat(Injury + Stagger)}[D] dealt to {TargetOfDamage} by {SourceOfDamage.User} using {SourceOfDamage.GetType()} ({AbilityDamageSource.DamageType})" +

            $"\n{injuryAuditLogString}[I], {staggerAuditLogString}[S] vs {effectiveArmor}[A]" + (ArmorPenetrationModifier > 0 ? $" vs {ArmorPenetrationModifier}%[PEN]" : "") + $" = {PreMitigationDamage}[D] -> {Utils.GetFormattedFloat(Damage)}[D] ({Utils.GetFormattedFloat((1 - armorDamageReduction) * 100)}% Mitigation)" +

            $"\n({AbilityDamageSource.InjuryScaling}% * {Utils.GetFormattedFloat(SourceOfDamage.User.GetInjuryStatForGivenDamageType(AbilityDamageSource.DamageType).Current)}(Potency * Stat) + {initialInjury + InjuryDealtFlatModifier + DamageDealtFlatModifier}(Flat)) * {InjuryDealtPercentageModifier + DamageDealtPercentageModifier}(Percentage) x{DamageDealtMultiplier}(Multiplier) = {Utils.GetFormattedFloat(Injury)}[I]" +

            $"\n({AbilityDamageSource.StaggerScaling}% * {Utils.GetFormattedFloat(SourceOfDamage.User.GetStaggerStatForGivenDamageType(AbilityDamageSource.DamageType).Current)}(Potency * Stat) + {initialStagger + StaggerDealtFlatModifier + DamageDealtFlatModifier}(Flat)) * {StaggerDealtPercentageModifier + DamageDealtPercentageModifier}(Percentage) x{DamageDealtMultiplier}(Multiplier) = {Utils.GetFormattedFloat(Stagger)}[S]");
        */
        InjuryWasHigherThan0 = InjuryWasHigherThan0 || Injury > 0;
        StaggerWasHigherThan0 = StaggerWasHigherThan0 || Stagger > 0;
    }

    public float GetCalculatedEffectiveWeaponDamage(bool is_injury = true) {
        float effectiveWeaponInjury = 0, effectiveWeaponStagger = 0, total = 0;
        if (is_injury && AbilityDamageSource.HybridInjurySource != null) {
            foreach(Constants.DamageType damage_type in AbilityDamageSource.HybridInjurySource.Keys) {
                effectiveWeaponInjury = SourceOfDamage.User.GetInjuryStatForGivenDamageType(damage_type).Current + SourceOfDamage.User.GetInjuryStatForGivenDamageType(damage_type).Current * (InjuryDealtPercentageModifier + DamageDealtPercentageModifier) / 100;
                total += AbilityDamageSource.HybridInjurySource[damage_type] / 100 * effectiveWeaponInjury + AbilityDamageSource.FlatInjury;
            }
        }
        else if (is_injury){
            effectiveWeaponInjury = SourceOfDamage.GetInjuryStatForDamageSource(AbilityDamageSource).Current + SourceOfDamage.GetInjuryStatForDamageSource(AbilityDamageSource).Current * (InjuryDealtPercentageModifier + DamageDealtPercentageModifier) / 100;
            total += AbilityDamageSource.InjuryScaling / 100 * effectiveWeaponInjury + AbilityDamageSource.FlatInjury;
        }
        else if (AbilityDamageSource.HybridStaggerSource != null) {
            foreach(Constants.DamageType damage_type in AbilityDamageSource.HybridStaggerSource.Keys) {
                effectiveWeaponStagger = SourceOfDamage.User.GetStaggerStatForGivenDamageType(damage_type).Current + SourceOfDamage.User.GetStaggerStatForGivenDamageType(damage_type).Current * (StaggerDealtPercentageModifier + DamageDealtPercentageModifier) / 100;
                total += AbilityDamageSource.HybridStaggerSource[damage_type] / 100 * effectiveWeaponStagger + AbilityDamageSource.FlatStagger;
            }
        }
        else {
            effectiveWeaponStagger = SourceOfDamage.GetStaggerStatForDamageSource(AbilityDamageSource).Current + SourceOfDamage.GetStaggerStatForDamageSource(AbilityDamageSource).Current * (StaggerDealtPercentageModifier + DamageDealtPercentageModifier) / 100;
            total += AbilityDamageSource.StaggerScaling / 100 * effectiveWeaponStagger + AbilityDamageSource.FlatStagger;
        }
        return total;
    }

    private void DecreaseTargetHealth() {
        float enemyHealthBarMaximum = TargetOfDamage.Health.Maximum;
        float currentHealth = TargetOfDamage.Health.Current;
        InjuryWasHigherThan0 = InjuryWasHigherThan0 || Injury > 0;
        if (Injury > 0) {
            if(Injury > 100000) {
                Properties.Add(DamageProperty.Execute);
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
            Properties.Add(DamageProperty.CriticalInjury);
        }
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
            Properties.Add(DamageProperty.CriticalStagger);
        }
    }

    private void ActivateUnitBehaviourOnHit() {
        if (TargetOfDamage.CurrentTarget == null && !(TargetOfDamage is Player)) {
            TargetOfDamage.CurrentTarget = SourceOfDamage.User;
        }
    }

    private void CalculateKnockback() {
        if (KnockbackInMeters != 0) {
            TargetOfDamage.ApplyKnockback(KnockbackInMeters, DamagingObject != null ? DamagingObject.transform.position : SourceOfDamage.User.transform.position, SourceOfDamage);
        }
    }

    private void EnsureDistanceFromTarget() {
        if(SourceOfDamage.User is Player && TargetOfDamage is not Player && IsNot(DamageProperty.DamageOverTime)) {
            CombatMath.KnockbackEnemyBasedOnMeleeWeaponDistance(this, AbilityDamageSource.KnockbackIntoRange != 0 ? AbilityDamageSource.KnockbackIntoRange : GetMinimumDistanceFromTarget(Utils.GetPlayerWeaponClassForDamageType(AbilityDamageSource.DamageType)));
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
            Constants.WeaponClass.Cannon => 1.2f,
            Constants.WeaponClass.Magic => Player.Instance.CurrentStance.StanceEffect is Stance_MindOverMatter ? 1.3f : 2.0f,
            _ => 0,
        };
    }
    
    private void CalculateEnergyGeneration() {
        if (SourceOfDamage.Is(Ability.Property.BasicAttack) && SourceOfDamage.IsNot(Ability.Property.AlreadyGeneratedEnergy)) {
            SourceOfDamage.User.Energy.GenerateEnergy(Constants.EnergyGainSource.BasicAttack, TargetOfDamage.IsBoss);
            SourceOfDamage.Properties.Add(Ability.Property.AlreadyGeneratedEnergy);
        }
        else if (SourceOfDamage.EnergyGainedOnHit > 0) {
            SourceOfDamage.User.Energy.GenerateEnergy(SourceOfDamage.EnergyGainedOnHit);
            SourceOfDamage.Properties.Add(Ability.Property.AlreadyGeneratedEnergy);
        }
        if (HealthLost > 0 && TargetOfDamage is Player) {
            TargetOfDamage.Energy.GenerateEnergy(Constants.EnergyGainSource.HealthLost, TargetOfDamage.IsBoss, HealthLost);
        }
    }

    private void CheckIfTargetShouldFlinch()
    {
        if (CanCauseFlinching && !TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Block)) && TargetOfDamage.Actions.CurrentActionBeingPerformed != Constants.ActionType.UnderHardCrowdControl && 
            ((TargetOfDamage is Player && StaggerDealt > TargetOfDamage.StaggerBar.Maximum * Constants.PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_PLAYER_FLINCH / 100) || (TargetOfDamage is not Player && !TargetOfDamage.IsBoss && StaggerDealt >= TargetOfDamage.StaggerBar.Maximum * Constants.PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_REGULAR_FLINCH / 100) || (TargetOfDamage is not Player && TargetOfDamage.IsBoss &&  StaggerDealt >= TargetOfDamage.StaggerBar.Maximum * Constants.PERCENTAGE_OF_MAX_STAGGER_BAR_NEEDED_FOR_BOSS_FLINCH / 100)) &&
            (TargetOfDamage.Actions.CurrentAbilityBeingPerformed == null ||
            (TargetOfDamage.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.Property.ImmuneToFlinch) && !TargetOfDamage.Actions.CurrentAbilityBeingPerformed.Is(Ability.Property.Counter) && !TargetOfDamage.Actions.CurrentAbilityBeingPerformed.Is(Ability.Property.Unstoppable))))
        {
            Effect protection = TargetOfDamage.GetEffect(new Func<Effect, bool>(effect => effect.Id.Contains("ProtectFromFlinchingOnce")));
            if (protection != null && TargetOfDamage.CheckIfEffectWithGivenIdIsOnCooldown(protection.Id)) {
                TargetOfDamage.AddCooldown(typeof(Effect), protection.FlatAmount, protection.Id);
            }
            else
            {
                TargetOfDamage.AddEffect(new Effect_Flinching(new(SourceOfDamage)), Constants.DEFAULT_FLINCHING_DURATION);
            }
        }
    }

    private void ActivatePlayerBehaviourOnDamage() {
        if (TargetOfDamage is Player && IsNot(DamageProperty.DamageOverTime)) {
            CameraController.Instance.ShakeScreen();
        }
        if(DamagingObject is UnitWeapon) {
            if(SourceOfDamage.User is Player && SourceOfDamage.Is(Ability.Property.Technique)) {
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
        if (TargetOfDamage.Health.Current <= 0 && GameController.Instance.EnemiesCanBeKilled && TargetOfDamage.CurrentHealthBars > 1)
        {
            TargetOfDamage.CurrentHealthBars--;
            TargetOfDamage.Health.Maximum = TargetOfDamage.HealthBars[TargetOfDamage.HealthBars.Count - TargetOfDamage.CurrentHealthBars] * (TargetOfDamage.IsHostile ? DamageInstance.GlobalEnemySurvivabilityModifier : 1);
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
            TargetOfDamage.AddEffect(new Effect_ChangeStat(Player.Instance.Armor, new(SourceOfDamage)) { PercentageAmount = 200 });
            EventManager.Takedown.Invoke(this);
        }
        else if (TargetOfDamage.Health.Current <= 0 && GameController.Instance.EnemiesCanBeKilled)
        {
            EventManager.UnitWouldBeDefeated.Invoke(this);
            if (TargetOfDamage.Health.Current > 0)
            {
                return;
            }
            TargetOfDamage.CurrentHealthBars--;
            DamageKilledTheTarget = true;
            if (TargetOfDamage.ProducesHumanSounds)
            {
                Utils.PlaySoundEffect(TargetOfDamage.AudioSource, TargetOfDamage.IsMale ? "Grunts/Male Death " + UnityEngine.Random.Range(1, 6) : "Grunts/Female Death " + UnityEngine.Random.Range(1, 4));
            }
            if (TargetOfDamage is Player)
            {
                if (GameController.Instance.InterruptMusicOnDeath)
                {
                    Utils.SetDefaultMusic("Death");
                }
                UIManager.Instance.ShowGameOverScreen();
            }
            else
            {
                if (SaveFile.Instance.GameType == Constants.GameType.Story)
                {
                    GiveResourcesAfterDefeat();
                }
                DeactivateUnit(TargetOfDamage);
                bool active_enemy = false;
                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Unit"))
                {
                    Unit unit = enemy.GetComponent<Unit>();
                    if (unit != null && unit.Faction == Constants.Faction.Enemy && unit.InCombat && unit.KnockedOut == false)
                    {
                        active_enemy = true;
                    }
                }
                if (!active_enemy)
                {
                    Player.Instance.InCombat = false;
                }
            }
            EventManager.UnitKnockedOut.Invoke(this);
            EventManager.Takedown.Invoke(this);
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
                Utils.PlaySoundEffect(
                    TargetOfDamage.AudioSource, 
                    "Hit/" + hit_type + ((Is(DamageProperty.CriticalInjury) || Is(DamageProperty.CriticalStagger)) ? "_CriticalHit" + UnityEngine.Random.Range(1, 4) : "_Hit" + UnityEngine.Random.Range(1, 7)), 
                    ((Is(DamageProperty.CriticalInjury) || Is(DamageProperty.CriticalStagger)) ? 1.4f : 1) * SoundVolume
                );
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
            Utils.PlaySoundEffect(TargetOfDamage.AudioSource, TargetOfDamage is Player ? "Grunts/Player Large Groan 1" : !TargetOfDamage.IsMale ? "Grunts/Female Big Groan 1" : "Grunts/Male Big Groan " + UnityEngine.Random.Range(1, 5), 0.7f);
        }
        else if(Injury > TargetOfDamage.Health.Maximum * 0.1f || Stagger > TargetOfDamage.StaggerBar.Maximum * 0.1f) {
            Utils.PlaySoundEffect(TargetOfDamage.AudioSource, TargetOfDamage is Player ? "Grunts/Player Small Groan 1" : !TargetOfDamage.IsMale ? "Grunts/Female Small Groan 1" : "Grunts/Male Small Groan " + UnityEngine.Random.Range(1, 11), 0.4f);
        }
    }

    public bool CheckIfInteractsWithCounters() {
        return IsNot(DamageProperty.DamageOverTime);
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
            if ((InjuryDealt + OverkillInjury) > 0)
            {
                GameObject injuryIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_InjuryIndicator")) as GameObject;
                injuryIndicator.transform.SetParent(TargetOfDamage.WorldSpaceCanvas.transform);
                injuryIndicator.GetComponent<TextMeshProUGUI>().text = Is(DamageProperty.Execute) ? Label.Get("ExecuteDisplay") : ((int)(InjuryDealt + OverkillInjury)).ToString();
                float scale = Is(DamageProperty.Execute) ? 0.012f * Settings.Instance.DamageNumbersSize : Utils.GetValueBasedOnMinAndMax(InjuryDealt + OverkillInjury, 0, 100 * CombatMath.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.01f, 0.0375f) + (Is(DamageProperty.CriticalInjury) ? 0.01f : 0);
                scale *= Settings.Instance.DamageNumbersSize;
                injuryIndicator.transform.localScale = new Vector2(scale * (Is(DamageProperty.CriticalInjury) ? 1f : 0.75f), scale * (Is(DamageProperty.CriticalInjury) ? 1f : 0.75f));
                injuryIndicator.transform.position = new Vector2(TargetOfDamage.transform.position.x - 0.3f + UnityEngine.Random.Range(-0.2f, 0.2f), TargetOfDamage.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f));
                if (Is(DamageProperty.CriticalInjury))
                {
                    injuryIndicator.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#FF3900");
                }
                if (Properties.Contains(DamageProperty.Supercharge))
                {
                    injuryIndicator.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#FFBD00");
                }
                injuryIndicator.GetComponent<UIFloatController>().DisappearAfter = Utils.GetValueBasedOnMinAndMax(InjuryDealt + OverkillInjury, 0, 100 * CombatMath.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.2f, 2f);
                injuryIndicator.GetComponent<UIFloatController>().DisappearTime = Utils.GetValueBasedOnMinAndMax(InjuryDealt + OverkillInjury, 0, 100 * CombatMath.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.1f, 1f);
            }
            if (StaggerDealt > 0)
            {
                GameObject staggerIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_StaggerIndicator")) as GameObject;
                staggerIndicator.transform.SetParent(TargetOfDamage.WorldSpaceCanvas.transform);
                staggerIndicator.GetComponent<TextMeshProUGUI>().text = ((int)StaggerDealt).ToString();
                float scale = Utils.GetValueBasedOnMinAndMax(StaggerDealt, 0, 100 * CombatMath.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.0075f, 0.015f) + (Is(DamageProperty.CriticalStagger) ? 0.01f : 0);
                scale *= Settings.Instance.DamageNumbersSize;
                staggerIndicator.transform.localScale = new Vector2(scale * (Is(DamageProperty.CriticalStagger) ? 1f : 0.75f), scale * (Is(DamageProperty.CriticalStagger) ? 1f : 0.75f));
                staggerIndicator.transform.position = new Vector2(TargetOfDamage.transform.position.x + 0.3f + UnityEngine.Random.Range(-0.2f, 0.2f), TargetOfDamage.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f));
                if (Is(DamageProperty.CriticalStagger))
                {
                    staggerIndicator.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#9064FF");
                }
                staggerIndicator.GetComponent<UIFloatController>().DisappearAfter = Utils.GetValueBasedOnMinAndMax(StaggerDealt, 0, 100 * CombatMath.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.2f, 2f);
                staggerIndicator.GetComponent<UIFloatController>().DisappearTime = Utils.GetValueBasedOnMinAndMax(StaggerDealt, 0, 100 * CombatMath.GetExpectedPowerForLevel(SaveFile.Instance.Level), 0.1f, 1f);
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