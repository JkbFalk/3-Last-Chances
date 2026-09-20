using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class AbilityDamageResolutionTests
{
    private class DummyTestAbility : Ability
    {
        public DummyTestAbility() : base(null) { }
    }

    private class MockDamagingObject : DamagingObject { }

    private GameObject _dummyHolder;

    [SetUp]
    public void Setup()
    {
        _dummyHolder = new GameObject("TestDamagingObjects");
    }

    [TearDown]
    public void Teardown()
    {
        if (_dummyHolder != null)
        {
            Object.DestroyImmediate(_dummyHolder);
        }
    }

    [Test]
    public void GetDamageSource_SingleSourceConfigured_AlwaysReturnsOnlySource()
    {
        var ability = new DummyTestAbility();
        ability.DamageSources.Add(new Ability.DamageSource(100f, 50f, Constants.DamageType.Heavy, "UniqueName"));

        var result = ability.GetDamageSource("AnyArbitraryName", null);

        Assert.IsNotNull(result);
        Assert.AreEqual("UniqueName", result.ColliderName);
    }

    [Test]
    public void GetDamageSource_NormalizesBoneNamesToDefault()
    {
        var ability = new DummyTestAbility();
        ability.WeaponCollisionName = "Default";
        ability.DamageSources.Add(new Ability.DamageSource(100f, 50f, Constants.DamageType.Heavy, "Default"));
        ability.DamageSources.Add(new Ability.DamageSource(50f, 25f, Constants.DamageType.Magic, "AoE"));

        var heavyBoneSource = ability.GetDamageSource("Heavy Bone", null);
        var lightLeftBoneSource = ability.GetDamageSource("Light Left Bone", null);
        var rangedBoneSource = ability.GetDamageSource("Ranged Bone", null);

        Assert.AreEqual("Default", heavyBoneSource.ColliderName);
        Assert.AreEqual("Default", lightLeftBoneSource.ColliderName);
        Assert.AreEqual("Default", rangedBoneSource.ColliderName);
    }

    [Test]
    public void GetDamageSource_AoECollider_FallsBackToAoESource()
    {
        var ability = new DummyTestAbility();
        ability.DamageSources.Add(new Ability.DamageSource(100f, 50f, Constants.DamageType.Heavy, "Default"));
        ability.DamageSources.Add(new Ability.DamageSource(200f, 100f, Constants.DamageType.Magic, "Main AoE"));

        GameObject aoeGo = new GameObject("AreaOfEffect_Explosion");
        aoeGo.transform.SetParent(_dummyHolder.transform);
        var aoe = aoeGo.AddComponent<AreaOfEffect>();

        var source = ability.GetDamageSource("UnrecognizedName", aoe);

        Assert.IsNotNull(source);
        Assert.AreEqual("Main AoE", source.ColliderName);
    }

    [Test]
    public void CheckIfDamageTriggerIsValid_OncePerUnit_PreventsMultiHit()
    {
        var ability = new DummyTestAbility();
        ability.DamageTriggerLimit = Ability.DamageTriggerLimitType.OncePerUnit;

        GameObject enemyGo = new GameObject("Enemy");
        enemyGo.transform.SetParent(_dummyHolder.transform);
        Unit dummyEnemy = enemyGo.AddComponent<Unit>();

        GameObject weaponGo = new GameObject("Weapon");
        weaponGo.transform.SetParent(_dummyHolder.transform);
        DamagingObject weapon = weaponGo.AddComponent<MockDamagingObject>();

        // First hit should be valid
        bool firstCheck = ability.CheckIfDamageTriggerIsValid(dummyEnemy, weapon);
        Assert.IsTrue(firstCheck);

        // Record hit
        ability.UpdateAffectedEnemyList(dummyEnemy, weapon);

        // Subsequent hit should be blocked
        bool secondCheck = ability.CheckIfDamageTriggerIsValid(dummyEnemy, weapon);
        Assert.IsFalse(secondCheck);
    }

    [Test]
    public void CheckIfDamageTriggerIsValid_OncePerUnitFromEachSource_AllowsDistinctSources()
    {
        var ability = new DummyTestAbility();
        ability.DamageTriggerLimit = Ability.DamageTriggerLimitType.OncePerUnitFromEachSource;

        GameObject enemyGo = new GameObject("Enemy");
        enemyGo.transform.SetParent(_dummyHolder.transform);
        Unit dummyEnemy = enemyGo.AddComponent<Unit>();

        GameObject sourceGo1 = new GameObject("Source1");
        sourceGo1.transform.SetParent(_dummyHolder.transform);
        DamagingObject source1 = sourceGo1.AddComponent<MockDamagingObject>();

        GameObject sourceGo2 = new GameObject("Source2");
        sourceGo2.transform.SetParent(_dummyHolder.transform);
        DamagingObject source2 = sourceGo2.AddComponent<MockDamagingObject>();

        // Hit by source 1
        Assert.IsTrue(ability.CheckIfDamageTriggerIsValid(dummyEnemy, source1));
        ability.UpdateAffectedEnemyList(dummyEnemy, source1);

        // Source 1 again -> blocked
        Assert.IsFalse(ability.CheckIfDamageTriggerIsValid(dummyEnemy, source1));

        // Source 2 -> valid
        Assert.IsTrue(ability.CheckIfDamageTriggerIsValid(dummyEnemy, source2));
    }
}