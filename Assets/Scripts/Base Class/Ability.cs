using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ES3Types;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ability {

    public static float GetEnergyCost(Type ability_type) {
        if(Player.Instance.PreparingForUltimate) {
            return Constants.ENERGY_REQUIRED_TO_USE_ULTIMATE;
        }
        float cost = 0;
        MethodInfo getEnergyMethod = ability_type.GetMethod("GetEnergyCost", BindingFlags.Public | BindingFlags.Static);
        if(getEnergyMethod != null) {
            cost = (float)getEnergyMethod.Invoke(null, new object[] {});
        }
        else {
            FieldInfo field = ability_type.GetField("EnergyCost", BindingFlags.Public | BindingFlags.Static);
            if(field == null) {
                return 0;
            }
            cost = (float)field.GetValue(null);
        }
        if(Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_PowerWithoutLimit) && Player.Instance.CurrentStance.StanceEffect.UnlockedUpgrade2 && ability_type.GetField("IsVariableEnergyTechnique", BindingFlags.Public | BindingFlags.Static) != null) {
            cost *= 2;
        }
        return cost;
    }

    public static float GetCooldown(Type ability_type) {
        FieldInfo field = ability_type.GetField("Cooldown", BindingFlags.Public | BindingFlags.Static);
        if(field == null) {
            return 0;
        }
        return (float)field.GetValue(null);
    }

    public static AbilityFamily GetFamily(Type ability_type) {
        FieldInfo field = ability_type.GetField("Family", BindingFlags.Public | BindingFlags.Static);
        if(field == null) {
            return AbilityFamily.None;
        }
        return (AbilityFamily)field.GetValue(null);
    }

    public enum AbilityInterruptType { Damage, Dodge, BasicAttack, Block, EnergyAbility, StanceSwitch}
    public enum Property { BasicAttack, StrongBasicAttack, Technique, Riposte, Counter, Backstab, Ultimate, UpgradeA, UpgradeB, Unstoppable, Charged, AlreadyGeneratedEnergy, ImmuneToFlinch, CounteredByBackstep, CounteredByBlock, CounteredByRiposte, CounteredByRoll, CountersBackstep, CountersBlock, CountersRiposte, CountersRoll, IgnoresImmunityToHits};
    public List<Property> Properties = new List<Property>();    
    public bool AbilityEnded = false;
    public List<Effect> TriggeredEffects = new List<Effect>();
    public bool TriggeredEffectWithId(string effect_id)
    {
        return TriggeredEffects.FirstOrDefault(effect => effect.Id == effect_id) != null;
    }
    private bool _canInterruptCurrentAbility = false;
    public bool CanInterruptCurrentAbility {
        get {
            return _canInterruptCurrentAbility;
        }
        set {
            _canInterruptCurrentAbility = value;
            if (User.Actions.QueuedInputs.Count > 0 && Utils.CheckIfUnitCanPerformActions(User)) {
                User.Actions.PerformQueuedActions();
            }
        }
    }
    public List<TemporaryObject> ObjectsToDestroyOnceAbilityEnds = new List<TemporaryObject>();
    public List<AbilityInterruptType> CanAlwaysBeInterruptedBy = new List<AbilityInterruptType>();
    public bool TurningOnCollisionClearsAffectedEnemyList = true;
    public bool PlayedSoundAtLeastOnce = false;
    public bool PlaySoundOnlyOnce = false;
    public bool PlaySoundOnEnemyHit = true;
    public bool HoldingTechniqueButton = false;
    public Ability OriginalRipostedAbility;
    public int RipostedCount = 0;
    public float PowerBudget = 0;
    protected List<Effect> EffectsAffectingUserDuringAbility;
    public string CustomHitSound;
    public Constants.HitSoundTypeEnum HitSoundType;
    public string WeaponCollisionName = "Default";
    public float HitSoundVolume = 0.5f;
    public bool Disabled = false;
    public Dictionary<string, (string, float)> CustomSounds = new Dictionary<string, (string, float)>();
    public bool HitsTriggerDamagedState = true;
    public bool AutoPlayAbilityAnimation = true;
    public bool CanMoveWhileUsing = false;
    public TemporaryObject MostRecentTemporaryObjectThatHitEnemy;
    public static bool DoesNotRequireTarget = false;
    public string Name { get; set; }
    public float EnergyGainedOnHit { get; set; } = 0f;
    public bool ShowWeaponTrails { get; protected set; } = false;

    public enum AbilityFamily { Ignis, Glacies, Anima, Molis, Salutis, Tonitrui, Proprius, None }
    
    public static List<Ability.AbilityFamily> GetAllAbilityFamilies()
    {
        return new List<Ability.AbilityFamily> { Ability.AbilityFamily.Anima, Ability.AbilityFamily.Ignis, Ability.AbilityFamily.Glacies, Ability.AbilityFamily.Molis, Ability.AbilityFamily.Salutis, Ability.AbilityFamily.Tonitrui, Ability.AbilityFamily.Proprius };
    }

    public bool Is(Property property)
    {
        return Properties.Contains(property);
    }

    public bool IsNot(Property property) {
        return !Properties.Contains(property);
    }

    public bool IsCounterable
    {
        get
        {
            return Is(Property.CounteredByBackstep) || Is(Property.CounteredByRoll) || Is(Property.CounteredByRiposte) || Is(Property.CounteredByBlock) ;
        }
    }

    public List<DamageSource> DamageSources = new List<DamageSource>();

    public enum DamageTriggerLimitType { OncePerUnit, OncePerUnitFromEachSource, OncePerUnitExceptTwinWeapon }

    public DamageTriggerLimitType DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;

    public float WaitTimeBeforeNextAction { get; protected set; } = 0;
    public string NameOfAnimationToAutoPlay;

    public static explicit operator Ability(Type v) {
        throw new NotImplementedException();
    }

    public Constants.DamageType ScalesWith {
        get {
            return DamageSources.Count == 0 ? Constants.DamageType.None : DamageSources[0].DamageType;
        }
    }

    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public string FlavorText { get; private set; }
    public Dictionary<Unit, List<DamagingObject>> AffectedEnemies = new Dictionary<Unit, List<DamagingObject>>();
    public Dictionary<DestructibleEnvironment, List<DamagingObject>> AffectedDestructibles = new Dictionary<DestructibleEnvironment, List<DamagingObject>>();
    public Vector2 SavedShotTarget;
    public Item ItemBeingUsed;
    public int CurrentStage = 0;
    public float TransitionIntoAnimationDuration = Constants.DEFAULT_CROSSFADE_DURATION;
    public float TransitionOutOfAnimationDuration = Constants.DEFAULT_CROSSFADE_DURATION;

    public Unit User;
    public Unit Target;

    public virtual void ActionsToPerformDuringAnotherAbility() {

    }

    public void ResetPotentialTargets()
    {
        AffectedEnemies.Clear();
        AffectedDestructibles.Clear();
    }

    public void EndThisAbility()
    {
        User.Actions.EndCurrentAbility();
    }

    public static int GetAmmoRequiredToUseAbility(Type ability_type)
    {
        FieldInfo field = ability_type.GetField("AmmoRequiredToUseAbility", BindingFlags.Public | BindingFlags.Static);
        return field == null ? 0 : (int)field.GetValue(null);
    }

    public Boolean CheckIfUnitWasDamagedByObject(Unit unit_to_check, DamagingObject damaging_object)
    {
        return AffectedEnemies.ContainsKey(unit_to_check) && AffectedEnemies[unit_to_check].Contains(damaging_object);
    }

    public virtual void AdditionalActionsOnAdvancingAbilityStage() { }

    public void UpdateAffectedEnemyList(Unit unit_getting_attacked, DamagingObject source_of_hit)
    {
        if (AffectedEnemies.ContainsKey(unit_getting_attacked))
        {
            AffectedEnemies[unit_getting_attacked].Add(source_of_hit);
        }
        else
        {
            AffectedEnemies.Add(unit_getting_attacked, new List<DamagingObject> { source_of_hit });
        }
    }

    public void UpdateAffectedDestructibleList(DestructibleEnvironment destructible_getting_attacked, DamagingObject source_of_hit)
    {
        if (AffectedDestructibles.ContainsKey(destructible_getting_attacked))
        {
            AffectedDestructibles[destructible_getting_attacked].Add(source_of_hit);
        }
        else
        {
            AffectedDestructibles.Add(destructible_getting_attacked, new List<DamagingObject> { source_of_hit });
        }
    }

    public virtual void CallAbilityEvent1() {}
    public virtual void CallAbilityEvent2() {}
    public virtual void CallAbilityEvent3() {}
    public virtual void CallAbilityEvent4() {}
    public virtual void CallAbilityEvent5() {}
    public virtual void CallAbilityEvent6() {}

    public bool ScaleMaxTimeWithAttackSpeed = false;
    public bool ScaleMaxTimeWithCombatSpeed = false;
    public float TimePassed { get; private set; } = 0;
    private float _maxTimePassed;
    public bool CountingTime { get; private set; }
    public void StartCountingTime(float max_time_passed_assuming_normal_speed)
    {
        _maxTimePassed = max_time_passed_assuming_normal_speed;
        if (ScaleMaxTimeWithAttackSpeed && ScaleMaxTimeWithCombatSpeed)
        {
            _maxTimePassed /= AttackSpeed.ScaledWithCombatSpeed;
        }
        if (ScaleMaxTimeWithAttackSpeed && !ScaleMaxTimeWithCombatSpeed)
        {
            _maxTimePassed /= AttackSpeed.Current;
        }
        CountingTime = true;
        TimePassed = 0;
    }

    public float PercentageOfMaxTimePassed
    {
        get
        {
            float charge = TimePassed / (_maxTimePassed != 0 ? _maxTimePassed : 1) * 100;
            return charge > 99 ? 100 : charge;
        }
    }

    public bool ShowingChargeBar = false;

    public void ShowChargeBar()
    {
        ShowingChargeBar = true;
        UIManager.Objects.ChargeBarSlider.gameObject.SetActive(true);
        UIManager.Objects.ChargeBarSlider.value = 0;
        UIManager.Objects.ChargeBarAbilityText.GetComponent<TextMeshProUGUI>().text = Label.Get(GetType().ToString());
    }

    private AttackSpeed _attackSpeed;
    public AttackSpeed AttackSpeed
    {
        get
        {
            return DamageSources.Count == 0 ? User.MagicAttackSpeed : DamageSources[0].DamageType == Constants.DamageType.Heavy ? User.HeavyAttackSpeed : DamageSources[0].DamageType == Constants.DamageType.Light ? User.LightAttackSpeed : DamageSources[0].DamageType == Constants.DamageType.Ranged ? User.RangedAttackSpeed : User.MagicAttackSpeed;
        }
    }

    public float GetValueBasedOnPercentageOfTimePassed(float value_at_0, float value_at_100)
    {
        float value = value_at_0 + PercentageOfMaxTimePassed / 100 * (value_at_100 - value_at_0);
        if (float.IsNaN(value))
        {
            return value_at_0;
        }
        return value;
    }

    public void StopCountingTime()
    {
        CountingTime = false;
        if (ShowingChargeBar)
        {
            ShowingChargeBar = false;
            UIManager.Objects.ChargeBarSlider.gameObject.SetActive(false);
        }
    }

    public virtual void AdditionalAbilitySpecificActionsOnTryingToMove() {
    }

    public virtual void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile) {
        if (GetAmmoRequiredToUseAbility(GetType()) > 0) {
            Player.Instance.Ammo--;
            if(Player.Instance.Ammo < 1) {
                projectile.IsFinalAmmo = true;
            }
        }
    }

    public virtual void OnAbilityStart() {
        if(GetType().ToString().StartsWith("AI_") == false) {
            Utils.CreateAuditLog("Unit (" + User.GetType() + ") used ability: " + GetType());
        }
        if (User is Player == false)
        {
            AddOrUpdateCooldown();
        }
        User.ProjectileSpawnLocation.transform.localPosition = Vector3.zero;
        EventManager.AbilityUsed.Invoke(this);
        if (EffectsAffectingUserDuringAbility != null && EffectsAffectingUserDuringAbility.Count > 0) {
            foreach (Effect e in EffectsAffectingUserDuringAbility) {
                User.AddEffect(e);
            }
        }
        if(User != null && User is Player && (GetType().ToString().Contains("Gauntlets") || (!String.IsNullOrWhiteSpace(NameOfAnimationToAutoPlay) && NameOfAnimationToAutoPlay.Contains("Gauntlets")))) {
            User.LeftArmInFrontOfWeapon = false;
            User.RightArmInFrontOfWeapon = false;
        }
        User.SetDefaultSortingOrder();
    }

    public void AddOrUpdateCooldown() {
        float cd = GetCooldown(GetType());
        if ((ItemBeingUsed == null || Item.GetCooldown(ItemBeingUsed.GetType()) == 0) && (cd == 0))
        {
            return;
        }
        if (ItemBeingUsed != null && User.ToolCooldown == null)
        {
            User.AddCooldown(ItemBeingUsed);
            if (ItemBeingUsed.Type == Constants.ItemType.Tool)
            {
                ItemBeingUsed.Amount--;
            }
        }
        else
        {
            bool isStacksBased = GetType().GetField("IsStacksBasedTechnique") != null;
            if (isStacksBased)
            {
                Player.Instance.UpdateTechniqueStacksAmount(GetType(), Player.Instance.CurrentTechniqueStacks[GetType()] - 1, Player.Instance.PreparingForUltimate);
            }
            else
            {
                User.AddCooldown(this, cd);
            }
        }
    }

    public virtual void OnAbilityEnd() {
        if (WaitTimeBeforeNextAction != 0 && User.UnitAI != null) {
            User.PlayAnimation(User.InCombat ? "IdleInCombat" : "Idle");
            User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Waiting;
            float wait_time = WaitTimeBeforeNextAction * 50 / User.UnitAI.Aggressiveness;
            if (wait_time > WaitTimeBeforeNextAction * 50 * 3)
            {
                wait_time = WaitTimeBeforeNextAction * 50 * 3;
            }
            User.UnitAI.WaitTimeBeforeNextAction = (int)(wait_time * UnityEngine.Random.Range(0f, 2f));
        }
        if (EffectsAffectingUserDuringAbility != null && EffectsAffectingUserDuringAbility.Count > 0) {
            foreach (Effect e in EffectsAffectingUserDuringAbility) {
                User.EndEffect(e);
            }
        }
        if (ItemBeingUsed != null)
        {
            ItemBeingUsed.OnEndUse();
        }
        foreach (TemporaryObject item in ObjectsToDestroyOnceAbilityEnds)
        {
            if(item != null && item.gameObject.IsDestroyed() == false) {
                item.MakeObjectDisappear();
            }
        }
        if (User.SpriteRenderers.ContainsKey("Heavy") && User.SpriteRenderers["Heavy"].Weapon != null)
        {
            User.SpriteRenderers["Heavy"].Weapon.DealingDamage = false;
        }
        if (User.SpriteRenderers.ContainsKey("Light Right") && User.SpriteRenderers["Light Right"].Weapon != null)
        {
            User.SpriteRenderers["Light Right"].Weapon.DealingDamage = false;
        }
        if (User.SpriteRenderers.ContainsKey("Light Left") && User.SpriteRenderers["Light Left"].Weapon != null)
        {
            User.SpriteRenderers["Light Left"].Weapon.DealingDamage = false;
        }
        if (User.SpriteRenderers.ContainsKey("Ranged") && User.SpriteRenderers["Ranged"].Weapon != null)
        {
            User.SpriteRenderers["Ranged"].Weapon.DealingDamage = false;
        }
        if (ShowingChargeBar)
        {
            ShowingChargeBar = false;
            UIManager.Objects.ChargeBarSlider.gameObject.SetActive(false);
        }
        User.LeftArmInFrontOfWeapon = true;
        User.RightArmInFrontOfWeapon = true;
        User.Actions.SetFaceVariant("Regular");
        User.RecalculateSortingOrder();
        AbilityEnded = true;
        EventManager.AbilityEnded.Invoke(this);
    }

    public virtual void OnAbilityButtonPress() {
        HoldingTechniqueButton = true;
    }

    public virtual void OnAbilityButtonRelease() {
        HoldingTechniqueButton = false;
    }

    public virtual void OnBasicAttackButtonPress() {
        if (CanAlwaysBeInterruptedBy.Contains(AbilityInterruptType.BasicAttack)) {
            User.Actions.PerformBasicAttack();
        }
    }

    public virtual void OnBasicAttackButtonRelease() {
    }

    public virtual bool CheckIfShouldShowDangerSign() {
        return IsCounterable;
    }

    public virtual void OnBlockButtonPress() {
        Player.Instance.Actions.UseAbility(typeof(Ability_Block));
    }

    public virtual void OnBlockButtonRelease() {
    }

    public virtual void OnDodgeButtonPress() {
        if (!Player.Instance.Actions.CurrentAbilityBeingPerformed.GetType().IsSubclassOf(typeof(Ability_Dodge)) && (CanInterruptCurrentAbility || CanAlwaysBeInterruptedBy.Contains(AbilityInterruptType.Dodge))) {
            Player.Instance.Actions.CurrentAbilityBeingPerformed = Player.Instance.Actions.GetDodgeTypeThatShouldBeUsed();
        }
        else {
            Ability_Dodge dodge =  Player.Instance.Actions.GetDodgeTypeThatShouldBeUsed();
            User.Actions.QueuedInputs.Add(new Actions.QueuedInput("PerformQueuedDodge") {AbilityToPerform = dodge.GetType(), DodgeDirection = dodge.Direction});
        }
    }

    public virtual void OnDodgeButtonRelease() {
    }

    public virtual void ExtraBehaviourOnHit(DamageInstance damage) {
    }

    public virtual void ExtraBehaviourOnDamage(DamageInstance damage) {
    }

    public Ability(Unit ability_user) {
        if (ability_user == null)
        {
            return;
        }
        User = ability_user;
        Target = User.CurrentTarget;
    }

    public static bool CheckIfCanPerformAbility(Unit user, Type ability_type, Item item = null) {
        if (user == null || user.Actions == null || ability_type == null || (ability_type.IsSubclassOf(typeof(Technique)) && SaveFile.Instance.UnlockedAbilities.Contains(ability_type) == false))
        {
            return false;
        }
        bool canPerformTheAbility = 
        Utils.CheckIfUnitCanPerformActions(user) 
        || (user.Actions.CurrentAbilityBeingPerformed != null && ability_type.IsSubclassOf(typeof(BasicAttack)) && user.Actions.CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Contains(Ability.AbilityInterruptType.BasicAttack))
        || (user.Actions.CurrentAbilityBeingPerformed != null && ability_type.IsSubclassOf(typeof(Technique)) && user.Actions.CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Contains(AbilityInterruptType.EnergyAbility)) 
        || (user.Actions.CurrentAbilityBeingPerformed != null && ability_type.IsSubclassOf(typeof(Ability_StanceSwitch)) && user.Actions.CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Contains(AbilityInterruptType.StanceSwitch))
        || (user.Actions.CurrentAbilityBeingPerformed != null && ability_type.IsSubclassOf(typeof(Ability_Block)) && user.Actions.CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Contains(AbilityInterruptType.Block))
        || (user.Actions.CurrentAbilityBeingPerformed != null && ability_type.IsSubclassOf(typeof(Ability_Dodge)) && user.Actions.CurrentAbilityBeingPerformed.CanAlwaysBeInterruptedBy.Contains(AbilityInterruptType.Dodge));

        bool enoughResource = CheckIfEnoughResourceToUseAbility(user, ability_type);
        if (canPerformTheAbility && !enoughResource && item == null && user is Player)
        {
            UIManager.Instance.DisplayNotEnoughEnergyWarningForGivenAbilityType(ability_type);
        }
        if (canPerformTheAbility && GetAmmoRequiredToUseAbility(ability_type) > 0 && Player.Instance.Ammo < GetAmmoRequiredToUseAbility(ability_type) && user is Player)
        {
            UIManager.Instance.DisplayNotEnoughAmmoWarning();
            enoughResource = false;
        }
        MethodInfo method = ability_type.GetMethod("CheckIfSpecialConditionsAreFulfilled");
        if (canPerformTheAbility && method != null)
        {
            bool can_perform_ability = (bool)method.Invoke(null, new object[] { user });
            if (can_perform_ability == false)
            {
                return false;
            }
        }
        if (user is Player && Player.Instance.PreparingForUltimate && ability_type.IsSubclassOf(typeof(Technique)))
        {
            Ability.AbilityFamily family = GetFamily(ability_type);
            if (!SaveFile.Instance.IsUltimateFamilyUnlocked(family))
            {
                return false;
            }
        }
        if(item != null) {
            MethodInfo method2 = item.GetType().GetMethod("CheckIfSpecialConditionsAreFulfilled");
            if (canPerformTheAbility && method != null)
            {
                bool can_perform_ability = (bool)method.Invoke(null, new object[] { user });
                if (can_perform_ability == false)
                {
                    return false;
                }
            }
        }
        MethodInfo method3 = ability_type.GetMethod("CheckIfAbilityUsableDependingOnCombat");
        if (canPerformTheAbility && method3 != null)
        {
            bool can_perform_ability = (bool)method3.Invoke(null, new object[] { user.InCombat });
            if (can_perform_ability == false)
            {
                return false;
            }
        }
        bool onCooldown;
        if (item != null)
        {
            onCooldown = user.ToolCooldown != null;
        }
        else
        {
            onCooldown = user.TechniqueCooldowns.FirstOrDefault(cooldown => cooldown.Type == ability_type) != null;
            bool isStacksBased = ability_type.GetField("IsStacksBasedTechnique") != null;
            if(isStacksBased && ((Player.Instance.PreparingForUltimate == false && Player.Instance.CurrentTechniqueStacks[ability_type] > 0) || (Player.Instance.PreparingForUltimate && Player.Instance.CurrentUltimateTechniqueStacks[ability_type] > 0))) {
                onCooldown = false;
            }
        }
        return canPerformTheAbility && enoughResource && !onCooldown;
    }

    public void OnUpdate()
    {
        if (CountingTime)
        {
            TimePassed += Time.deltaTime;
            if (ShowingChargeBar)
            {
                UIManager.Objects.ChargeBarSlider.value = PercentageOfMaxTimePassed / 100;
            }
        }
        AdditionalActionsOnUpdate();
    }

    public virtual void AdditionalActionsOnUpdate() { }

    public static bool CheckIfEnoughResourceToUseAbility(Unit user, Type ability_type, Item item = null) {
        if (item != null && item.Type == Constants.ItemType.Tool && item.Amount > 0) {
            return true;
        }
        return user.Energy.Current >= GetEnergyCost(ability_type);
    }

    public virtual void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit) {
        if (unit_getting_attacked != null && CheckIfDamageTriggerIsValid(unit_getting_attacked, object_hitting)) {
            MostRecentTemporaryObjectThatHitEnemy = object_hitting;
            UpdateAffectedEnemyList(unit_getting_attacked, object_hitting);
            DamageSource source = GetDamageSource(object_hitting.EffectiveColliderName, object_hitting);
            if(source == null) {
                return;
            }
            DamageInstance damage_dealt = new DamageInstance(unit_getting_attacked, this, object_hitting) {
                KnockbackInMeters = source.KnockbackInMeters,
                SoundVolume = HitSoundVolume,
                AbilityDamageSource = source,
                SourceOfCollision = collider_being_hit,
                PlaySoundOnEnemyHit =  PlaySoundOnEnemyHit && (!PlaySoundOnlyOnce || !PlayedSoundAtLeastOnce)
            };
            if (PlaySoundOnEnemyHit) {
                PlayedSoundAtLeastOnce = true;
            }
            if (source.CustomHitSound != null) {
                damage_dealt.CustomHitSound = source.CustomHitSound;
            }
            ExtraBehaviourOnHit(damage_dealt);
            damage_dealt.CalculateAndApplyDamage();
            bool successfulHit = damage_dealt.DamageWasBlocked == false && (damage_dealt.InjuryDealt > 0 || damage_dealt.StaggerDealt > 0);
            if (successfulHit) {
                ExtraBehaviourOnDamage(damage_dealt);
            }
            damage_dealt.CheckIfShouldDestroyProjectile();
        }
    }

    public virtual bool CheckIfDamageTriggerIsValid(Unit unit_getting_attacked, DamagingObject source_of_hit)
    {
        if (DamageTriggerLimit == DamageTriggerLimitType.OncePerUnit && AffectedEnemies.ContainsKey(unit_getting_attacked))
        {
            return false;
        }
        else if (DamageTriggerLimit == DamageTriggerLimitType.OncePerUnitExceptTwinWeapon && AffectedEnemies.ContainsKey(unit_getting_attacked) && 
        !(source_of_hit == User.SpriteRenderers["Light Left"].Weapon && AffectedEnemies[unit_getting_attacked].Count == 1 && AffectedEnemies[unit_getting_attacked].Contains(User.SpriteRenderers["Light Right"].Weapon)) && 
        !(source_of_hit == User.SpriteRenderers["Light Right"].Weapon &&  AffectedEnemies[unit_getting_attacked].Count == 1 && AffectedEnemies[unit_getting_attacked].Contains(User.SpriteRenderers["Light Left"].Weapon))) {
            return false;
        }
        else if (DamageTriggerLimit == DamageTriggerLimitType.OncePerUnitFromEachSource && AffectedEnemies.ContainsKey(unit_getting_attacked) && AffectedEnemies[unit_getting_attacked].Contains(source_of_hit))
        {
            return false;
        }
        return true;
    }

    public void AddCustomSound(string sound_name, string sound_filename, float volume = 1) {
        CustomSounds.Add(sound_name, (sound_filename, volume));
        if(!Utils.LoadedAudioClips.ContainsKey(sound_filename)) {
            GameController.Instance.StartCoroutine(PreloadSound(sound_filename));
        }
    }

    public IEnumerator PreloadSound(string sound_filepath) {
        ResourceRequest request = Resources.LoadAsync("Sounds/Sound Effects/" + sound_filepath, typeof(AudioClip));
        yield return request;
        if(request.asset == null) {
            Debug.LogError("Could not find custom sound effect: " + sound_filepath);
        }
        else if(Utils.LoadedAudioClips.ContainsKey(sound_filepath) == false) {
            Utils.LoadedAudioClips.Add(sound_filepath, request.asset as AudioClip);
        }
    }

    public void PlayCustomSound(string sound_name, float volume = 1, AudioSource source = null) {

        if (CustomSounds.ContainsKey(sound_name)) {
            (string, float) custom_sound = CustomSounds[sound_name];
            if(custom_sound.Item1 == null)
            {
                Debug.LogError("Null audio clip for ability " + GetType() + " (" + User.name + ")");
                return;
            }
            Utils.PlaySoundEffect(source == null ? User.AudioSource : source, custom_sound.Item1, custom_sound.Item2);
        }
        else {
            Utils.PlaySoundEffect(source == null ? User.AudioSource : source, sound_name, volume);
        }
    }

    // FILE: Assets/Scripts/Base Class/Ability.cs

    public void ChaseCurrentTargetAtGivenDegreeAngle(float max_dash_distance_in_meters, float max_angle, Unit target = null) {
        Unit finalTarget = target != null ? target : (User is Player ? Player.Instance.CurrentTarget : User.CurrentTarget);
        if (User is Player && finalTarget == null)
        {
            Vector2 aimVector = User.Actions.GetCurrentAimVector();
            Vector2 dir = CombatMath.GetDirectionVector(Vector2.zero, aimVector, User.Actions.IsFlipped, max_angle);
            float dashDistance = max_dash_distance_in_meters;

            if (Settings.Instance.ControlScheme == "Keyboard")
            {
                float distanceToPointer = Vector2.Distance(GameController.Instance.PlayerControls.CurrentWorldspacePointerPosition, Player.Instance.transform.position);
                dashDistance = Mathf.Min(distanceToPointer, max_dash_distance_in_meters);
            }

            User.Rigidbody2D.linearVelocity = Vector2.zero;
            User.PushInTargetDirection(dir * dashDistance, this);
            return;
        }
        if (finalTarget == null)
        {
            Vector2 dir = CombatMath.GetDirectionVector(User.transform.position, User.transform.position + new Vector3(User.Actions.IsFlipped ? -2f : 2f, 0), User.Actions.IsFlipped, max_angle);
            User.Rigidbody2D.linearVelocity = Vector2.zero;
            User.PushInTargetDirection(dir * max_dash_distance_in_meters, this);
        }
        else
        {
            float stopOffset = 1.0f;
            float rawDistance = Vector2.Distance(User.transform.position, finalTarget.transform.position);
            float distanceToTarget = Mathf.Max(0f, rawDistance - stopOffset);
            float dashDistance = Mathf.Min(distanceToTarget, max_dash_distance_in_meters);

            Vector2 dir = CombatMath.GetDirectionVector(User.transform.position, finalTarget.transform.position, User.Actions.IsFlipped, max_angle);
            User.Rigidbody2D.linearVelocity = Vector2.zero;
            User.PushInTargetDirection(dir * dashDistance, this);
        }
    }

    public DamageSource GetDamageSource(string name, DamagingObject sourceObject = null)
    {
        if (DamageSources == null || DamageSources.Count == 0)
        {
            Debug.LogError($"[Combat] No damage sources defined on ability {GetType().Name}.");
            return null;
        }

        // 1. Single-source abilities always map directly
        if (DamageSources.Count == 1)
        {
            return DamageSources[0];
        }

        // 2. Normalize weapon bone names to WeaponCollisionName ("Default")
        if (name == "Heavy Bone" || name == "Light Left Bone" || name == "Light Right Bone" || name == "Ranged Bone")
        {
            name = WeaponCollisionName;
        }

        // Clean up unity naming artifacts: "AoE (Clone)" or "AoE_1" -> "AoE"
        string cleanName = name.Replace("(Clone)", "").Trim();

        // 3. Exact match
        DamageSource found = DamageSources.FirstOrDefault(s => s.ColliderName.Equals(cleanName, StringComparison.OrdinalIgnoreCase));
        if (found != null) return found;

        // 4. Semantic Fallback: If hitting with an AreaOfEffect, find any source containing "AoE"
        if (sourceObject is AreaOfEffect || cleanName.IndexOf("AoE", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            found = DamageSources.FirstOrDefault(s => s.ColliderName.IndexOf("AoE", StringComparison.OrdinalIgnoreCase) >= 0);
            if (found != null) return found;
        }

        // 5. Default Fallback: If not found, look for "Default"
        found = DamageSources.FirstOrDefault(s => s.ColliderName.Equals("Default", StringComparison.OrdinalIgnoreCase));
        if (found != null) return found;

        // 6. Graceful Degradation: Never return null and break combat—log a warning and use the primary source
        Debug.LogWarning($"[Combat] Ability '{GetType().Name}' could not find DamageSource matching '{name}'. Defaulting to primary DamageSource.");
        return DamageSources[0];
    }
    
    public void AddDamageSource(float _health, float _stagger, Constants.DamageType _damage_type, string _colliderName = "Default")
    {
        if(_colliderName == "Default" && DamageSources.FirstOrDefault(item => item.ColliderName == _colliderName) != null)
        {
            Debug.LogError("Ability " + GetType() + " already contains damage source with collider name " + _colliderName);
        }
        DamageSources.Add(new DamageSource(_health, _stagger, _damage_type, _colliderName));
    }

    public Stat GetInjuryStatForDamageSource(DamageSource source)
    {
        if(source == null || User == null) {
            return new Injury(Constants.DamageType.None, null, 100);
        }
        return source.DamageType == Constants.DamageType.Heavy ? User.HeavyInjury : source.DamageType == Constants.DamageType.Light ? User.LightInjury : source.DamageType == Constants.DamageType.Ranged ? User.RangedInjury : source.DamageType == Constants.DamageType.Magic ? User.MagicInjury : new Injury(Constants.DamageType.None, null, 100);
    }

    public Stat GetStaggerStatForDamageSource(DamageSource source)
    {
        if(source == null || User == null) {
            return new Stagger(Constants.DamageType.None, null, 100);
        }
        return source.DamageType == Constants.DamageType.Heavy ? User.HeavyStagger : source.DamageType == Constants.DamageType.Light ? User.LightStagger : source.DamageType == Constants.DamageType.Ranged ? User.RangedStagger : source.DamageType == Constants.DamageType.Magic ? User.MagicStagger : new Stagger(Constants.DamageType.None, null, 100);
    }

    internal object GetField(string v, BindingFlags bindingFlags)
    {
        throw new NotImplementedException();
    }

    public class DamageSource{
        public string ColliderName;
        public float InjuryScaling = 0;
        public float FlatInjury = 0;
        public float StaggerScaling = 0;
        public float FlatStagger = 0;
        public Constants.DamageType DamageType;
        public float KnockbackInMeters = 0;
        public string CustomHitSound;
        public float KnockbackIntoRange = 0;
        public Dictionary<Constants.DamageType, float> HybridInjurySource;
        public Dictionary<Constants.DamageType, float> HybridStaggerSource;
        public DamageSource(float _health_scaling, float _stagger_scaling, Constants.DamageType _damage_type, string _collider_name = "Default")
        {
            InjuryScaling = _health_scaling;
            StaggerScaling = _stagger_scaling;
            ColliderName = _collider_name;
            DamageType = _damage_type;
        }

        public DamageSource(Dictionary<Constants.DamageType, float> hybridInjurySource, Dictionary<Constants.DamageType, float> hybridStaggerSource, Constants.DamageType _damage_type, string _collider_name = "Default")
        {
            HybridInjurySource = hybridInjurySource;
            HybridStaggerSource = hybridStaggerSource;
            ColliderName = _collider_name;
            DamageType = _damage_type;
        }
    }
}