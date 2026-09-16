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
}