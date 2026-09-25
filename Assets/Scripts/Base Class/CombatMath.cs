// FILE: Assets/Scripts/Base Class/CombatMath.cs
using System;
using UnityEngine;

public static class CombatMath
{
    public static float ConvertDegreesToRadians(float angleInDegrees)
    {
        return (float)(Math.PI / 180f * angleInDegrees);
    }

    public static float ConvertRadiansToDegrees(float angleInRadians)
    {
        return (float)(angleInRadians * (180f / Math.PI));
    }

    public static Vector2 GetDirectionVector(Vector2 source, Vector2 target, bool sourceFacesLeft, float maxAngle = 90f)
    {
        Vector2 directionVector = (target - source).normalized;
        Vector2 absoluteVector = new Vector2(Math.Abs(directionVector.x), Math.Abs(directionVector.y));
        float angleOfDirectionVector = ConvertRadiansToDegrees((float)Math.Atan(absoluteVector.y / absoluteVector.x));

        if ((sourceFacesLeft && directionVector.x > 0) || (!sourceFacesLeft && directionVector.x < 0))
        {
            Vector2 vectorReduced = new Vector2((float)(Math.Sin(ConvertDegreesToRadians(90f - maxAngle)) / Math.Sin(ConvertDegreesToRadians(maxAngle))), 1f).normalized;
            return new Vector2(directionVector.x > 0 ? -vectorReduced.x : vectorReduced.x, directionVector.y > 0 ? vectorReduced.y : -vectorReduced.y).normalized;
        }

        if (angleOfDirectionVector > maxAngle)
        {
            Vector2 vectorReduced = new Vector2((float)(Math.Sin(ConvertDegreesToRadians(90f - maxAngle)) / Math.Sin(ConvertDegreesToRadians(maxAngle))), 1f).normalized;
            return new Vector2(directionVector.x > 0 ? vectorReduced.x : -vectorReduced.x, directionVector.y > 0 ? vectorReduced.y : -vectorReduced.y).normalized;
        }

        return directionVector;
    }

    public static bool GetAreOppositeDirections(string direction1, string direction2)
    {
        if ((direction1 == "Up" && direction2 == "Down") || (direction1 == "Down" && direction2 == "Up")) return true;
        if ((direction1 == "Left" && direction2 == "Right") || (direction1 == "Right" && direction2 == "Left")) return true;
        return false;
    }

    public static Vector2 GetPositionGivenDistanceAwayBasedOnTwoPoints(Vector2 centerPoint, Vector2 directionPoint, float distance)
    {
        Vector2 repositionVector = (directionPoint - centerPoint).normalized * distance;
        return centerPoint + repositionVector;
    }

    public static void KnockbackEnemyBasedOnMeleeWeaponDistance(DamageInstance damage, float optimalDistance)
    {
        float distance = Vector2.Distance(damage.TargetOfDamage.transform.position, damage.SourceOfDamage.User.transform.position);
        if (distance >= optimalDistance) return;

        bool sourceToTheLeft = damage.SourceOfDamage.User.transform.position.x <= damage.TargetOfDamage.transform.position.x;
        Vector2 direction = (damage.TargetOfDamage.transform.position - (damage.SourceOfDamage.User.transform.position + (sourceToTheLeft ? Vector3.left : Vector3.right))).normalized;
        damage.TargetOfDamage.PushInTargetDirection(direction * (optimalDistance - distance) * 5f, damage.SourceOfDamage);
    }

    public static bool CheckIfCurrenTargetIsInFrontOfUnit(Unit unit, float maxDistance = 2f)
    {
        if (unit.CurrentTarget == null) return false;
        if (unit.Actions.IsFlipped && unit.CurrentTarget.transform.position.x > unit.transform.position.x) return false;
        if (!unit.Actions.IsFlipped && unit.CurrentTarget.transform.position.x < unit.transform.position.x) return false;
        return Vector2.Distance(unit.transform.position, unit.CurrentTarget.transform.position) < maxDistance;
    }

    public static bool CheckIfGivenUnitIsInFrontOfUnit(Unit unit, Unit unitToCheck)
    {
        return (unit.Actions.IsFlipped && unitToCheck.transform.position.x < unit.transform.position.x) ||
               (!unit.Actions.IsFlipped && unitToCheck.transform.position.x > unit.transform.position.x);
    }

    public static bool CheckIfGameObjectIsBehindUnit(GameObject gameObject, Unit unit)
    {
        if (unit.Actions.IsFlipped && gameObject.transform.position.x > unit.transform.position.x) return true;
        if (!unit.Actions.IsFlipped && gameObject.transform.position.x < unit.transform.position.x) return true;
        return false;
    }

    public static bool CheckIfPlayerIsFacingUnit(Unit unit)
    {
        return (Player.Instance.transform.position.x > unit.transform.position.x && Player.Instance.Actions.IsFlipped) ||
               (Player.Instance.transform.position.x <= unit.transform.position.x && !Player.Instance.Actions.IsFlipped);
    }

    public static float GetEffectiveCrowdControlDuration(Unit sourceOfCC, Unit targetOfCC)
    {
        float effectiveTenacity = targetOfCC.Tenacity.Current;
        if (sourceOfCC == Player.Instance && effectiveTenacity > 0)
        {
            float flatReduction = 0;
            foreach (Effect e in Player.Instance.GetEffects(effect => effect.Id.Contains("TenacityPenetration")))
            {
                effectiveTenacity *= 1f - e.PercentageAmount;
                flatReduction += e.FlatAmount;
            }
            effectiveTenacity -= flatReduction;
        }
        if (targetOfCC == Player.Instance && Player.Instance.CurrentStance.StanceEffect is Stance_Brawler &&
            SaveFile.Instance.ActiveUpgrades.Contains("Stance_Brawler3") &&
            !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_Brawler_TenacityWhileAttacking") &&
            Player.Instance.Actions.CurrentAbilityBeingPerformed != null)
        {
            effectiveTenacity += Stance_Brawler.Upgrade3TenacityWhileAttacking;
        }

        float controlVsTenacity = sourceOfCC.Control.Current - effectiveTenacity;
        return controlVsTenacity >= 0 ? 1f + (controlVsTenacity / 100f) : 1f / (1f + Math.Abs(controlVsTenacity / 100f));
    }

    public static float GetExpectedPlayerPowerForLevel(int level)
    {
        if (level <= 1) return 1.25f;
        if (level <= 10) return Utils.GetValueBasedOnMinAndMax(level, 1, 10, 1.25f, 2.2f);
        if (level <= 20) return Utils.GetValueBasedOnMinAndMax(level, 10, 20, 2.2f, 3.1f);
        if (level <= 30) return Utils.GetValueBasedOnMinAndMax(level, 20, 30, 3.1f, 4.2f);
        if (level <= 40) return Utils.GetValueBasedOnMinAndMax(level, 30, 40, 4.2f, 5.2f);
        if (level <= 50) return Utils.GetValueBasedOnMinAndMax(level, 40, 50, 5.2f, 6.5f);
        return Utils.GetValueBasedOnMinAndMax(level, 50, 100, 6.5f, 10f);
    }

    public static float GetEnemyHealthScalingForLevel(int level)
    {
        if (level <= 1) return 1.2f;
        if (level <= 10) return Utils.GetValueBasedOnMinAndMax(level, 1, 10, 1.2f, 2.4f);
        if (level <= 20) return Utils.GetValueBasedOnMinAndMax(level, 10, 20, 2.4f, 4.2f);
        if (level <= 30) return Utils.GetValueBasedOnMinAndMax(level, 20, 30, 4.2f, 7.0f);
        if (level <= 40) return Utils.GetValueBasedOnMinAndMax(level, 30, 40, 7.0f, 10.5f);
        if (level <= 50) return Utils.GetValueBasedOnMinAndMax(level, 40, 50, 10.5f, 15f);
        return Utils.GetValueBasedOnMinAndMax(level, 50, 100, 15f, 25.0f);
    }

    public static float GetExpectedPowerForLevel(int level)
    {
        if (level < 1) return Utils.GetValueBasedOnMinAndMax(level, -25, 1, 0.3f, Constants.EXPECTED_POWER_AT_LEVEL_1);
        if (level <= 10) return Utils.GetValueBasedOnMinAndMax(level, 1, 10, Constants.EXPECTED_POWER_AT_LEVEL_1, Constants.EXPECTED_POWER_AT_LEVEL_10);
        if (level <= 20) return Utils.GetValueBasedOnMinAndMax(level, 10, 20, Constants.EXPECTED_POWER_AT_LEVEL_10, Constants.EXPECTED_POWER_AT_LEVEL_20);
        if (level <= 30) return Utils.GetValueBasedOnMinAndMax(level, 20, 30, Constants.EXPECTED_POWER_AT_LEVEL_20, Constants.EXPECTED_POWER_AT_LEVEL_30);
        if (level <= 40) return Utils.GetValueBasedOnMinAndMax(level, 30, 40, Constants.EXPECTED_POWER_AT_LEVEL_30, Constants.EXPECTED_POWER_AT_LEVEL_40);
        if (level <= 50) return Utils.GetValueBasedOnMinAndMax(level, 40, 50, Constants.EXPECTED_POWER_AT_LEVEL_40, Constants.EXPECTED_POWER_AT_LEVEL_50);
        return Utils.GetValueBasedOnMinAndMax(level, 50, 100, Constants.EXPECTED_POWER_AT_LEVEL_50, Constants.EXPECTED_POWER_AT_LEVEL_100);
    }

    public static float GetExpectedControlLevel(int level)
    {
        if (level < 1) return Utils.GetValueBasedOnMinAndMax(level, -25, 1, 0.5f, 1f);
        if (level <= 10) return Utils.GetValueBasedOnMinAndMax(level, 1, 10, 1f, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_10);
        if (level <= 20) return Utils.GetValueBasedOnMinAndMax(level, 10, 20, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_10, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_20);
        if (level <= 30) return Utils.GetValueBasedOnMinAndMax(level, 20, 30, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_20, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_30);
        if (level <= 40) return Utils.GetValueBasedOnMinAndMax(level, 30, 40, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_30, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_40);
        if (level <= 50) return Utils.GetValueBasedOnMinAndMax(level, 40, 50, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_40, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_50);
        return Utils.GetValueBasedOnMinAndMax(level, 50, 100, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_50, Constants.CONTROL_AND_TENACITY_MULTIPLIER_AT_LEVEL_100);
    }

    public static int GetLevelAdjustmentBasedOnUnitCount(int unitCount)
    {
        return unitCount switch
        {
            0 => 0,
            1 => 10,
            2 => 6,
            3 => 2,
            4 => -1,
            5 => -3,
            6 => -5,
            _ => -unitCount
        };
    }

    public static float GetAggresivenessBasedOnUnitCount(int unitCount)
    {
        return 2.5f / unitCount + 0.5f;
    }

    public static DamageInstance SimulateWeaponHit(Ability ability, Unit target,  Ability.DamageSource damageSource)
    {
        if (target == null || target.KnockedOut || ability == null || damageSource == null) 
            return null;

        Constants.DamageType damageType = (damageSource.DamageType == Constants.DamageType.CurrentWeapon || damageSource.DamageType == Constants.DamageType.None)
            ? ability.User.CurrentWeaponDamageType
            : damageSource.DamageType;

        DamagingObject weapon = null;
        if (ability.User?.SpriteRenderers != null)
        {
            string slot = damageType == Constants.DamageType.Light ? "Light Right" :
                        damageType == Constants.DamageType.Ranged ? "Ranged" : "Heavy";
            if (ability.User.SpriteRenderers.TryGetValue(slot, out var info))
            {
                weapon = info.Weapon;
            }
        }

        ability.TurningOnCollisionClearsAffectedEnemyList = false;

        ability.UpdateAffectedEnemyList(target, weapon);
        if (damageType == Constants.DamageType.Light && ability.User?.SpriteRenderers != null &&
            ability.User.SpriteRenderers.TryGetValue("Light Left", out var leftInfo) && leftInfo.Weapon != null)
        {
            ability.UpdateAffectedEnemyList(target, leftInfo.Weapon);
        }

        DamageInstance damage = new DamageInstance(target, ability, weapon)
        {
            AbilityDamageSource = damageSource,
            KnockbackInMeters = damageSource.KnockbackInMeters,
            CustomHitSound = damageSource.CustomHitSound
        };
        ability.ExtraBehaviourOnHit(damage); 
        damage.CalculateAndApplyDamage();
        return damage;
    }

    public static DamageInstance SimulateWeaponHit(Ability ability,  Unit target, float injury, float stagger = 0f, Constants.DamageType damageType = Constants.DamageType.None)
    {
        if (ability == null) return null;

        if (damageType == Constants.DamageType.None)
        {
            damageType = ability.ScalesWith != Constants.DamageType.None 
                ? ability.ScalesWith 
                : (ability.User != null ? ability.User.CurrentWeaponDamageType : Constants.DamageType.None);
        }

        return SimulateWeaponHit(ability, target, new Ability.DamageSource(injury, stagger, damageType));
    }
}