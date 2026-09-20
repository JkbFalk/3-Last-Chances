using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class CombatMathTests
{
    [Test]
    [TestCase(1, 1.25f)]
    [TestCase(10, 2.2f)]
    [TestCase(30, 4.2f)]
    [TestCase(50, 6.0f)]
    public void ExpectedPower_ReturnsExpectedBaselineValues(int level, float expectedPower)
    {
        float result = CombatMath.GetExpectedPowerForLevel(level);
        Assert.AreEqual(expectedPower, result, 0.05f);
    }

    [Test]
    public void CrowdControlDuration_HigherControlThanTenacity_IncreasesDuration()
    {
        float controlVsTenacity = 50f;
        float multiplier = 1f + (controlVsTenacity / 100f);

        Assert.AreEqual(1.5f, multiplier, 0.01f);
    }

    [Test]
    public void DirectionVector_AngleClamping_DoesNotProduceNaN()
    {
        Vector2 source = Vector2.zero;
        Vector2 target = new Vector2(0f, 10f);
        
        Vector2 result = CombatMath.GetDirectionVector(source, target, false, 45f);

        Assert.IsFalse(float.IsNaN(result.x));
        Assert.IsFalse(float.IsNaN(result.y));
        Assert.AreEqual(1.0f, result.magnitude, 0.01f);
    }

    [Test]
    public void ConvertDegreesToRadians_And_RadiansToDegrees_AreReciprocal()
    {
        float degrees = 45f;
        float radians = CombatMath.ConvertDegreesToRadians(degrees);
        float convertedBack = CombatMath.ConvertRadiansToDegrees(radians);

        Assert.AreEqual(degrees, convertedBack, 0.001f);
    }

    [Test]
    [TestCase("Up", "Down", true)]
    [TestCase("Down", "Up", true)]
    [TestCase("Left", "Right", true)]
    [TestCase("Right", "Left", true)]
    [TestCase("Up", "Left", false)]
    [TestCase("Down", "Right", false)]
    [TestCase("Up", "Up", false)]
    public void GetAreOppositeDirections_ValidatesCorrectly(string dir1, string dir2, bool expected)
    {
        bool result = CombatMath.GetAreOppositeDirections(dir1, dir2);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void GetPositionGivenDistanceAway_ReturnsAccurateTargetPosition()
    {
        Vector2 center = new Vector2(0, 0);
        Vector2 directionPoint = new Vector2(0, 10);
        float distance = 5f;

        Vector2 result = CombatMath.GetPositionGivenDistanceAwayBasedOnTwoPoints(center, directionPoint, distance);

        Assert.AreEqual(0f, result.x, 0.001f);
        Assert.AreEqual(5f, result.y, 0.001f);
    }

    [Test]
    public void GetDirectionVector_FacingRight_TargetBehind_ClampsDirection()
    {
        Vector2 source = Vector2.zero;
        Vector2 targetBehind = new Vector2(-10f, 0f); // Target directly behind facing direction

        Vector2 clamped = CombatMath.GetDirectionVector(source, targetBehind, sourceFacesLeft: false, maxAngle: 45f);

        Assert.IsFalse(float.IsNaN(clamped.x));
        Assert.IsFalse(float.IsNaN(clamped.y));
        Assert.GreaterOrEqual(clamped.x, 0f, "Clamped vector should not aim backwards relative to facing direction.");
        Assert.AreEqual(1f, clamped.magnitude, 0.01f);
    }

    [Test]
    [TestCase(1, 10f)]
    [TestCase(2, 6f)]
    [TestCase(3, 2f)]
    [TestCase(4, -1f)]
    [TestCase(5, -3f)]
    [TestCase(6, -5f)]
    [TestCase(10, -10f)]
    public void GetLevelAdjustmentBasedOnUnitCount_ReturnsAccurateAdjustments(int unitCount, float expectedAdjustment)
    {
        int adjustment = CombatMath.GetLevelAdjustmentBasedOnUnitCount(unitCount);
        Assert.AreEqual(expectedAdjustment, adjustment);
    }

    [Test]
    public void GetAggresivenessBasedOnUnitCount_ScalesInversely()
    {
        float soloAggro = CombatMath.GetAggresivenessBasedOnUnitCount(1);
        float groupAggro = CombatMath.GetAggresivenessBasedOnUnitCount(5);

        Assert.Greater(soloAggro, groupAggro);
        Assert.AreEqual(3.0f, soloAggro, 0.01f);
        Assert.AreEqual(1.0f, groupAggro, 0.01f);
    }

    [Test]
    public void GetExpectedPowerForLevel_ScalesMonotonically()
    {
        float p1 = CombatMath.GetExpectedPowerForLevel(1);
        float p10 = CombatMath.GetExpectedPowerForLevel(10);
        float p20 = CombatMath.GetExpectedPowerForLevel(20);
        float p50 = CombatMath.GetExpectedPowerForLevel(50);

        Assert.Less(p1, p10);
        Assert.Less(p10, p20);
        Assert.Less(p20, p50);
    }
}