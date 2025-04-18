using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class EventManager {
    public static UnityEvent OneFifthSecondElapsedInGame = new UnityEvent();
    public static UnityEvent OneFifthSecondElapsedRealtime = new UnityEvent();
    public static UnityEvent<Damage> HitDealt = new UnityEvent<Damage>();
    public static UnityEvent<Damage> AfterHitDamageCalculation = new UnityEvent<Damage>();
    public static UnityEvent<Damage> AboutToHandleFatalBlow = new UnityEvent<Damage>();
    public static UnityEvent<Damage> DamageDealt = new UnityEvent<Damage>();
    public static UnityEvent<Damage> DamageWasDodged = new UnityEvent<Damage>();
    public static UnityEvent<Ability> AbilityUsed = new UnityEvent<Ability>();
    public static UnityEvent<Ability, float> AbilityEnergyConsumed = new UnityEvent<Ability, float>();
    public static UnityEvent<Ability> AbilityEnded = new UnityEvent<Ability>();
    public static UnityEvent<Damage> UnitKnockedOut = new UnityEvent<Damage>();
    public static UnityEvent AmmoAmountChanged = new UnityEvent();
    public static UnityEvent<Damage> UnitWouldBeDefeated = new UnityEvent<Damage>();
    public static UnityEvent<Damage> HealthBarBroken = new UnityEvent<Damage>();
    public static UnityEvent<Unit> UnitHealthChanged = new UnityEvent<Unit>();
    public static UnityEvent<Projectile> ProjectileCreated = new UnityEvent<Projectile>();
    public static UnityEvent<Ability, bool> AbilityWasRipostedOrCountered = new UnityEvent<Ability, bool>();
    public static UnityEvent<Quest, QuestObjective> QuestObjectiveUpdated = new UnityEvent<Quest, QuestObjective>();
    public static UnityEvent<Stat, float> UnitStatCurrentAmountChanged = new UnityEvent<Stat, float>();
    /// <summary>
    /// 1st Param: base gain, 2nd Param: actual Energy gained, 3rd Param: source of gain
    /// </summary>
    public static UnityEvent<float, float, Constants.EnergyGainSource> GeneratedEnergy = new UnityEvent<float, float, Constants.EnergyGainSource>();
    public static UnityEvent PlayerTargetChanged = new UnityEvent();
    public static UnityEvent PlayerObjectReinitialized = new UnityEvent();
    public static UnityEvent SubmitButtonPressed = new UnityEvent();
    public static UnityEvent CancelButtonPressed = new UnityEvent();
    public static UnityEvent<InteractableObject> ObjectInteractedWith = new UnityEvent<InteractableObject>();
    public static UnityEvent<DestructibleEnvironment> DestructibleDestroyed = new UnityEvent<DestructibleEnvironment>();
    public static UnityEvent<Effect> EffectStarted = new UnityEvent<Effect>();
    /// <summary>
    /// First param: already existing effect, Second param: newly created effect
    /// </summary>
    public static UnityEvent<Effect, Effect> EffectEmpowered = new UnityEvent<Effect, Effect>();
    public static UnityEvent<Effect> EffectDecayingAmountChanged = new UnityEvent<Effect>();
    public static UnityEvent<Effect> EffectActivated = new UnityEvent<Effect>();
    public static UnityEvent<Effect> EffectEnded = new UnityEvent<Effect>();
    public static UnityEvent<Cooldown> AboutToAddCooldown = new UnityEvent<Cooldown>();
    public static UnityEvent<Cooldown> CooldownAdded = new UnityEvent<Cooldown>();
    public static UnityEvent<Unit> UnitChangedDirection = new UnityEvent<Unit>();
    public static UnityEvent<Unit> EnterCombat = new UnityEvent<Unit>();
    public static UnityEvent<Unit> ExitCombat = new UnityEvent<Unit>();
    /// <summary>
    /// First param: previously equipped item, Second param: newly equipped item
    /// </summary>
    public static UnityEvent<Item, Item> ItemEquipped = new UnityEvent<Item, Item>();
    public static UnityEvent FinishedLoadingArea = new UnityEvent();
    public static UnityEvent FinishedTakingScreenshot = new UnityEvent();
    public static UnityEvent StanceSwitched = new UnityEvent();
    public static UnityEvent ExitMenu = new UnityEvent();
}