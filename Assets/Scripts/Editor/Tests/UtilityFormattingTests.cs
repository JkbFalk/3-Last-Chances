using NUnit.Framework;

[TestFixture]
public class UtilityFormattingTests
{
    [Test]
    [TestCase(0, "0")]
    [TestCase(999, "999")]
    [TestCase(1000, "1,000")]
    [TestCase(12345, "12,345")]
    [TestCase(1000000, "1,000,000")]
    public void GetFormattedInteger_FormatsWithCommas(int input, string expected)
    {
        string result = Utils.GetFormattedInteger(input);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void GetFormattedFloat_RoundsSmallNumbersToOneDecimal()
    {
        float val = 42.56f;
        string formatted = Utils.GetFormattedFloat(val);
        Assert.AreEqual("42.6", formatted);
    }

    [Test]
    public void GetFormattedFloat_RoundsLargeNumbersToInteger()
    {
        float val = 125.75f;
        string formatted = Utils.GetFormattedFloat(val);
        Assert.AreEqual("126", formatted);
    }

    [Test]
    public void GetFormattedFloat_ForcedDecimals_IncludesTrailingZeroes()
    {
        float whole = 10f;
        string formattedTwoDecimals = Utils.GetFormattedFloat(whole, force_show_decimals: 2);
        string formattedOneDecimal = Utils.GetFormattedFloat(whole, force_show_decimals: 1);

        Assert.AreEqual("10.00", formattedTwoDecimals);
        Assert.AreEqual("10.0", formattedOneDecimal);
    }

    [Test]
    public void GetValueBasedOnMinAndMax_InterpolatesAndClamps()
    {
        // 50% between range [0, 10], mapping to [100, 200]
        float mid = Utils.GetValueBasedOnMinAndMax(5f, 0f, 10f, 100f, 200f);
        Assert.AreEqual(150f, mid, 0.001f);

        // Below minimum clamps to valueAtMinRange
        float below = Utils.GetValueBasedOnMinAndMax(-5f, 0f, 10f, 100f, 200f);
        Assert.AreEqual(100f, below, 0.001f);

        // Above maximum clamps to valueAtMaxRange
        float above = Utils.GetValueBasedOnMinAndMax(15f, 0f, 10f, 100f, 200f);
        Assert.AreEqual(200f, above, 0.001f);
    }

    [Test]
    public void CalculatePB_AppliesScalingAndMultipliers()
    {
        float basePb = 10f;
        float scalesWith = 2f;
        var multipliers = new System.Collections.Generic.List<float> { 1.5f, 2.0f };

        // 10 * 2 * 1.5 * 2 = 60
        float result = EffectList.CalculatePB(basePb, scalesWith, multipliers);

        Assert.AreEqual(60f, result, 0.001f);
    }
}