// FILE: Assets/Scripts/Ability/Technique/Ability_WindRush.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Ability_WindRush : Technique
{
    public static float EnergyCost = 1f;
    public static float Cooldown = 0f;
    public static AbilityFamily Family = AbilityFamily.Anima;

    public static float InjuryScaling = 300f;
    public static float StaggerScaling = 300f;
    public static float BaseSharpGain = 25f;

    public static float MasteryAExtraStaggerScaling = 350f;
    public static float MasteryBExtraInjuryScaling = 250f;

    public static float UltimateInjuryScaling = 800f;
    public static float UltimateStaggerScaling = 800f;
    public static float UltimateStaggerAoEScalingPerSecond = 100f;
    public static float UltimateWallDurationInSeconds = 20f;

    public static string REACTION_WINDOW_EFFECT_ID = "WindRush_ReactionWindow";
    public const float REACTION_WINDOW_DURATION = 3.0f;

    private const float UPGRADE_B_OVERSHOOT_DISTANCE = 4.5f;
    private const float MAX_DASH_DISTANCE = 20.0f;

    private Unit _intendedTarget;
    private bool _intendedTargetWasHit = false;
    private bool _usedReactionBonus = false;

    private Unit _upgradeBTarget;
    private bool _hasHitUpgradeBTarget = false;
    private bool _counterTriggered = false;

    private AreaOfEffect _ultimateAoe;
    private bool _dealingAoEDamage = false;
    private Unit _targetOfDamage = null;

    public Ability_WindRush(Unit ability_user) : base(ability_user)
    {
        NameOfAnimationToAutoPlay = "WindRush_" + User.CurrentWeaponClass;
        AddCustomSound("Start", "Ability/Ability_WindBlast_Use", 0.4f);
        AddCustomSound("WindBlast", "Ability/Ability_WindBlast_Dash", 0.6f);

        TurningOnCollisionClearsAffectedEnemyList = false;

        _usedReactionBonus = Is(Property.UpgradeA) && User.CheckIfUnderEffectWithGivenId(REACTION_WINDOW_EFFECT_ID);

        float finalInjury = Is(Property.Ultimate) ? UltimateInjuryScaling : InjuryScaling;
        float finalStagger = Is(Property.Ultimate) ? UltimateStaggerScaling : StaggerScaling;

        if (Is(Property.UpgradeB) && IsNot(Property.Ultimate))
        {
            finalInjury += MasteryBExtraInjuryScaling;
        }

        if (_usedReactionBonus && IsNot(Property.Ultimate))
        {
            finalStagger += MasteryAExtraStaggerScaling;
        }

        if (User.CurrentWeaponDamageType == Constants.DamageType.Light)
        {
            DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
            DamageSources.Add(new DamageSource(finalInjury / 2f, finalStagger / 2f, User.CurrentWeaponDamageType, "Default")
            {
                KnockbackInMeters = 0f,
                KnockbackIntoRange = 0.01f
            });
        }
        else
        {
            DamageSources.Add(new DamageSource(finalInjury, finalStagger, User.CurrentWeaponDamageType, "Default")
            {
                KnockbackInMeters = 0f,
                KnockbackIntoRange = 0.01f
            });
        }

        DamageSources.Add(new DamageSource(0, UltimateStaggerAoEScalingPerSecond / 2f, User.CurrentWeaponDamageType, "AoE")
        {
            KnockbackInMeters = 0f,
            KnockbackIntoRange = 0.01f
        });

        if (Is(Property.UpgradeB))
        {
            EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_RollForward(new(User)) };
        }

        _intendedTarget = User.CurrentTarget;
    }

    public static float GetEnergyCost()
    {
        if (SaveFile.Instance.ActiveUpgrades.Contains("Ability_WindRush_UpgradeA") && Player.Instance.CheckIfUnderEffectWithGivenId(REACTION_WINDOW_EFFECT_ID))
        {
            return 0f;
        }
        return EnergyCost;
    }

    public static void OnEquip()
    {
        EventManager.DamageWasDodged.AddListener(OnReactionTriggered);
        EventManager.AbilityWasRipostedOrCountered.AddListener(OnParryOrCounterTriggered);
    }

    public static void OnUnequip()
    {
        EventManager.DamageWasDodged.RemoveListener(OnReactionTriggered);
        EventManager.AbilityWasRipostedOrCountered.RemoveListener(OnParryOrCounterTriggered);
        Effect_WindRushReactionWindow.ApplySlotVisuals(false);
    }

    private static void OnReactionTriggered(DamageInstance damage, Ability dodge)
    {
        if (damage.TargetOfDamage == Player.Instance && SaveFile.Instance.ActiveUpgrades.Contains("Ability_WindRush_UpgradeA"))
        {
            ActivateReactionWindow();
        }
    }

    private static void OnParryOrCounterTriggered(Ability ability, bool wasCountered)
    {
        if (SaveFile.Instance.ActiveUpgrades.Contains("Ability_WindRush_UpgradeA"))
        {
            ActivateReactionWindow();
        }
    }

    private static void ActivateReactionWindow()
    {
        Effect existingWindow = Player.Instance.GetEffectWithGivenId(REACTION_WINDOW_EFFECT_ID);
        if (existingWindow != null)
        {
            existingWindow.RemainingDuration = REACTION_WINDOW_DURATION;
            return;
        }

        Player.Instance.AddEffect(new Effect_WindRushReactionWindow(new(Player.Instance)), REACTION_WINDOW_DURATION);
    }

    public override void CallAbilityEvent1()
    {
        ConsumeEnergyAndCooldownForTheAbility();
        Effect reactionBuff = User.GetEffectWithGivenId(REACTION_WINDOW_EFFECT_ID);
        reactionBuff?.EndThisEffect();

        if (_intendedTarget == null)
        {
            _intendedTarget = User.CurrentTarget != null ? User.CurrentTarget : User.GetClosestValidTarget(true);
        }

        if (Is(Property.UpgradeB))
        {
            _upgradeBTarget = _intendedTarget;
            _hasHitUpgradeBTarget = false;
            _counterTriggered = false;
        }

        GameObject vfx = Utils.CreateVisualEffect(new(this), "WindRush");
        if (vfx != null)
        {
            vfx.GetComponent<AttachObjectToBodyPart>().Initialize(User);
            vfx.transform.eulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? -90 : 90);
        }

        User.Rigidbody2D.linearVelocity = Vector2.zero;

        if (Is(Property.UpgradeB))
        {
            User.SetIgnoreUnitCollisions(true);
            PerformUpgradeBDash();
        }
        else
        {
            ChaseCurrentTargetAtGivenDegreeAngle(15f, 60f, _intendedTarget);
        }
    }

    public override void CallAbilityEvent2()
    {
        base.CallAbilityEvent2(); 

        if (Is(Property.UpgradeB) && !_counterTriggered)
        {
            Unit targetToDamage = _upgradeBTarget != null ? _upgradeBTarget : _intendedTarget;

            if (targetToDamage != null && !targetToDamage.KnockedOut)
            {
                // If the target is currently using a roll-counterable action, execute RollCounter
                if (IsCounterableByRoll(targetToDamage, out Ability enemyAbility))
                {
                    TriggerRollCounter(targetToDamage, enemyAbility);
                    return;
                }

                DamageSource defaultSource = GetDamageSource("Default");
                CombatMath.SimulateWeaponHit(this, targetToDamage, defaultSource);
            }
        }
    }

    public override void AdditionalActionsOnUpdate()
    {
        base.AdditionalActionsOnUpdate();

        // Proactively scan for counterable enemies while dashing through them
        if (Is(Property.UpgradeB) && !_counterTriggered && !AbilityEnded)
        {
            CheckForRollCounterOpportunity();
        }
    }

    private void CheckForRollCounterOpportunity()
    {
        // 1. Check intended primary target first
        Unit primary = _upgradeBTarget != null ? _upgradeBTarget : _intendedTarget;
        if (primary != null && !primary.KnockedOut && Vector2.Distance(User.transform.position, primary.transform.position) <= 2.5f)
        {
            if (IsCounterableByRoll(primary, out Ability enemyAbility))
            {
                TriggerRollCounter(primary, enemyAbility);
                return;
            }
        }

        // 2. Check any other nearby enemy passed during the dash
        List<Unit> nearbyEnemies = Utils.GetSpecifiedUnits(u =>
            u.IsHostile &&
            !u.KnockedOut &&
            u != User &&
            Vector2.Distance(User.transform.position, u.transform.position) <= 2.5f);

        foreach (Unit enemy in nearbyEnemies)
        {
            if (IsCounterableByRoll(enemy, out Ability enemyAbility))
            {
                TriggerRollCounter(enemy, enemyAbility);
                return;
            }
        }
    }

    private bool IsCounterableByRoll(Unit unit, out Ability activeAbility)
    {
        activeAbility = null;
        if (unit == null || unit.KnockedOut || unit.Actions == null) return false;

        activeAbility = unit.Actions.CurrentAbilityBeingPerformed;
        if (activeAbility == null) return false;

        if (activeAbility.Is(Property.CounteredByRoll)) return true;

        if (SaveFile.Instance.DifficultyLevel == 0 && activeAbility.Is(Property.Counter)) return true;

        return false;
    }

    private void TriggerRollCounter(Unit enemy, Ability enemyAbility)
    {
        _counterTriggered = true;

        Type rollCounterType = AbilityTypeRegistry.GetRollCounter(User.CurrentWeaponClass);
        if (rollCounterType == null) return;

        int variant = UnityEngine.Random.Range(1, 4);
        Counter rollCounter = (Counter)Activator.CreateInstance(rollCounterType, new object[] { User });
        rollCounter.NameOfAnimationToAutoPlay = User.CurrentWeaponClass.ToString() + "_RollCounter" + variant;
        rollCounter.Target = enemy;
        rollCounter.OriginalRipostedAbility = enemyAbility;

        User.Actions.FaceUnit(enemy);
        User.Actions.CurrentAbilityBeingPerformed = rollCounter;

        if (User.Actions.CurrentAbilityBeingPerformed.IsNot(Property.AlreadyGeneratedEnergy))
        {
            User.Energy.GenerateEnergy(Constants.EnergyGainSource.Counter, enemy.IsBoss);
            User.Actions.CurrentAbilityBeingPerformed.Properties.Add(Property.AlreadyGeneratedEnergy);
        }

        EventManager.AbilityWasRipostedOrCountered.Invoke(enemyAbility, true);

        enemy.AddEffect(new Effect_RollCountered(new SourceOfEffect(User)) { NameOfAnimationToAutoPlay = "RollCountered" + variant }, 4f);
        enemy.Animator.SetFloat("Special Animation Speed", Player.Instance.CurrentWeaponAttackSpeed.Current);
        GameController.Instance.WaitAndRunMethod(1f, Utils.AdjustRemainingCounteredAnimation, enemy);

        new DamageInstance(enemy, rollCounter, null)
            .SetDamageSource(0, Constants.STAGGER_PERCENTAGE_FROM_COUNTER, User.CurrentWeaponDamageType)
            .CalculateAndApplyDamage();

        if (User is Player || enemy is Player)
        {
            CameraController.Instance.ShakeScreen(0.2f, 0.1f);
        }

        if (Player.Instance.IsStaggered)
        {
            Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Maximum * 0.3f;
        }
    }

    private void PerformUpgradeBDash()
    {
        Vector2 dashDirection;
        float dashDistance;

        if (_intendedTarget != null)
        {
            Vector2 toTarget = (Vector2)_intendedTarget.transform.position - (Vector2)User.transform.position;
            float rawDistance = toTarget.magnitude;
            dashDirection = CombatMath.GetDirectionVector(User.transform.position, _intendedTarget.transform.position, User.Actions.IsFlipped, 60f);
            dashDistance = Mathf.Clamp(rawDistance + UPGRADE_B_OVERSHOOT_DISTANCE, 8f, MAX_DASH_DISTANCE);
        }
        else
        {
            dashDirection = CombatMath.GetDirectionVector(Vector2.zero, User.Actions.GetCurrentAimVector(), User.Actions.IsFlipped, 60f);

            if (Settings.Instance.ControlScheme == "Keyboard")
            {
                float pointerDistance = Vector2.Distance(GameController.Instance.PlayerControls.CurrentWorldspacePointerPosition, User.transform.position);
                dashDistance = Mathf.Clamp(pointerDistance + UPGRADE_B_OVERSHOOT_DISTANCE, 8f, MAX_DASH_DISTANCE);
            }
            else
            {
                dashDistance = MAX_DASH_DISTANCE;
            }
        }

        User.PushInTargetDirection(dashDirection * dashDistance, this);
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        Unit hitUnit = damage.TargetOfDamage;

        if (Is(Property.UpgradeB) && !_hasHitUpgradeBTarget && hitUnit != null)
        {
            _hasHitUpgradeBTarget = true;
            _upgradeBTarget = hitUnit;
        }

        if ((_intendedTarget == null || hitUnit == _intendedTarget) && !_intendedTargetWasHit)
        {
            _intendedTargetWasHit = true;
            _intendedTarget = hitUnit;

            PlayCustomSound("WindBlast");

            if (hitUnit.Rigidbody2D != null)
            {
                hitUnit.Rigidbody2D.linearVelocity = Vector2.zero;
            }

            if (!Is(Property.UpgradeB))
            {
                User.Rigidbody2D.linearVelocity = Vector2.zero;
            }

            if (hitUnit?.Actions?.CurrentAbilityBeingPerformed != null && !hitUnit.Actions.CurrentAbilityBeingPerformed.Is(Property.Unstoppable))
            {
                hitUnit.Actions.EndCurrentAbility();
                hitUnit.AddEffect(new Effect_Flinching(new(this)), Constants.DEFAULT_FLINCHING_DURATION);   
            }

            bool wasAttacking = hitUnit.Actions.CurrentActionBeingPerformed == Constants.ActionType.UsingAbility &&
                                hitUnit.Actions.CurrentAbilityBeingPerformed != null &&
                                !hitUnit.Actions.CurrentAbilityBeingPerformed.GetType().IsSubclassOf(typeof(AI));

            float sharpAwarded = wasAttacking ? (BaseSharpGain * 3f) : BaseSharpGain;
            User.AddEffect(new Effect_Sharp(sharpAwarded, new(this)));

            BlowAwayOtherEnemies(hitUnit);

            if (Is(Property.Ultimate))
            {
                _ultimateAoe = Utils.CreateAreaOfEffect(new(this), "WindRush_Ultimate");
                _ultimateAoe.gameObject.transform.parent.gameObject.SetActive(false);
                GameController.Instance.WaitAndRunMethod(0.2f, ActivateUltimateWall);
                _targetOfDamage = hitUnit;
            }
        }
    }

    private void BlowAwayOtherEnemies(Unit primaryTarget)
    {
        List<Unit> peripheralEnemies = Utils.GetSpecifiedUnits(u =>
            u.IsHostile &&
            u != primaryTarget &&
            u != User &&
            Vector2.Distance(u.transform.position, primaryTarget.transform.position) < 5.0f);

        foreach (Unit enemy in peripheralEnemies)
        {
            Vector2 pushDir = (enemy.transform.position - primaryTarget.transform.position).normalized;
            if (pushDir == Vector2.zero)
            {
                pushDir = User.Actions.IsFlipped ? Vector2.left : Vector2.right;
            }
            enemy.PushInTargetDirection(pushDir * 15f, this);
            enemy.AddEffect(new Effect_Flinching(new(this)), Constants.DEFAULT_FLINCHING_DURATION);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();

        if (Is(Property.UpgradeB))
        {
            User.SetIgnoreUnitCollisions(false);
        }
    }

    public void ActivateUltimateWall()
    {
        if (_ultimateAoe == null || _targetOfDamage == null || _targetOfDamage.KnockedOut) return;

        _ultimateAoe.gameObject.transform.parent.gameObject.SetActive(true);
        _ultimateAoe.transform.parent.position = _targetOfDamage.transform.position;
        _dealingAoEDamage = true;

        EventManager.UnitKnockedOut.AddListener(CheckIfDestroyWall);
        EventManager.OneSecondElapsedInGame.AddListener(PullTargetIntoCenter);

        GameController.Instance.WaitAndRunMethod(UltimateWallDurationInSeconds, TurnOffUltimateWall);
    }

    public void PullTargetIntoCenter()
    {
        if (_targetOfDamage == null || _targetOfDamage.KnockedOut || _ultimateAoe == null) return;

        ResetPotentialTargets();
        _targetOfDamage.PushIntoPosition(_ultimateAoe.transform.parent.position, this, 1.1f);
    }

    public void CheckIfDestroyWall(DamageInstance damage)
    {
        if (damage.TargetOfDamage == _targetOfDamage)
        {
            TurnOffUltimateWall();
        }
    }

    public void TurnOffUltimateWall()
    {
        _dealingAoEDamage = false;
        EventManager.OneSecondElapsedInGame.RemoveListener(PullTargetIntoCenter);
        EventManager.UnitKnockedOut.RemoveListener(CheckIfDestroyWall);

        if (_ultimateAoe != null && !_ultimateAoe.IsDestroyed && _ultimateAoe.gameObject != null)
        {
            MonoBehaviour.Destroy(_ultimateAoe.transform.parent.gameObject);
        }
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if (object_hitting is AreaOfEffect && !_dealingAoEDamage)
        {
            return;
        }

        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }

    public override bool CheckIfDamageTriggerIsValid(Unit unit_getting_attacked, DamagingObject source_of_hit)
    {
        if (!_dealingAoEDamage)
        {
            return base.CheckIfDamageTriggerIsValid(unit_getting_attacked, source_of_hit);
        }
        return !AffectedEnemies.ContainsKey(unit_getting_attacked);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string>
        {
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponInjury.Current * InjuryScaling / 100f),
            Utils.GetFormattedFloat(InjuryScaling),
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * StaggerScaling / 100f),
            Utils.GetFormattedFloat(StaggerScaling),
            Utils.GetFormattedFloat(BaseSharpGain),
            Utils.GetFormattedFloat(BaseSharpGain * 3f)
        };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string>
        {
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * MasteryAExtraStaggerScaling / 100f),
            Utils.GetFormattedFloat(MasteryAExtraStaggerScaling)
        };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string>
        {
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponInjury.Current * MasteryBExtraInjuryScaling / 100f),
            Utils.GetFormattedFloat(MasteryBExtraInjuryScaling)
        };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string>
        {
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponInjury.Current * UltimateInjuryScaling / 100f),
            Utils.GetFormattedFloat(UltimateInjuryScaling),
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * UltimateStaggerScaling / 100f),
            Utils.GetFormattedFloat(UltimateStaggerScaling),
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * UltimateStaggerAoEScalingPerSecond / 100f),
            Utils.GetFormattedFloat(UltimateStaggerAoEScalingPerSecond),
            Utils.GetFormattedFloat(UltimateWallDurationInSeconds)
        };
    }
}