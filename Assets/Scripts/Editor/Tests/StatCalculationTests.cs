using NUnit.Framework;

[TestFixture]
public class StatCalculationTests
{
    private class TestStat : Stat
    {
        public TestStat(float baseAmount) : base(null, baseAmount)
        {
            // By setting CurrentCanBeLowerThanMaximum to true, 
            // RecalculateMaximumAmount avoids evaluating Owner.InCombat (safe for isolated unit tests)
            CurrentCanBeLowerThanMaximum = true;
        }
    }

    [Test]
    public void Stat_InitialState_MatchesBase()
    {
        var stat = new TestStat(100f);

        Assert.AreEqual(100f, stat.Base);
        Assert.AreEqual(100f, stat.Maximum);
        Assert.AreEqual(100f, stat.Current);
    }

    [Test]
    public void Stat_AddFlatModifier_IncreasesMaximumAndRecalculates()
    {
        var stat = new TestStat(100f);
        object source = new object();

        stat.AddFlatModifier(source, 25f);

        Assert.AreEqual(125f, stat.Maximum);
    }

    [Test]
    public void Stat_AddPercentageModifier_ScalesBaseAndFlatModifiers()
    {
        var stat = new TestStat(100f);
        object source1 = new object();
        object source2 = new object();

        stat.AddFlatModifier(source1, 50f);        // Base + Flat = 150
        stat.AddPercentageModifier(source2, 20f);  // 150 + 20% = 180

        Assert.AreEqual(180f, stat.Maximum, 0.001f);
    }

    [Test]
    public void Stat_RemoveModifier_RestoresPreviousValue()
    {
        var stat = new TestStat(100f);
        object flatSource = new object();
        object percentSource = new object();

        stat.AddFlatModifier(flatSource, 50f);
        stat.AddPercentageModifier(percentSource, 50f);
        Assert.AreEqual(225f, stat.Maximum, 0.001f); // (100 + 50) * 1.5

        stat.RemovePercentageModifier(percentSource, 50f);
        Assert.AreEqual(150f, stat.Maximum, 0.001f);

        stat.RemoveFlatModifier(flatSource, 50f);
        Assert.AreEqual(100f, stat.Maximum, 0.001f);
    }

    [Test]
    public void Stat_Current_ClampedBetweenZeroAndMaximum()
    {
        var stat = new TestStat(100f);

        stat.Current = 150f;
        Assert.AreEqual(100f, stat.Current);

        stat.Current = -20f;
        Assert.AreEqual(0f, stat.Current);
    }
}