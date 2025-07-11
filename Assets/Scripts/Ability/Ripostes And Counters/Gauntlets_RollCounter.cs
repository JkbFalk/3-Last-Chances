using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_RollCounter : Counter
{

    public Gauntlets_RollCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Light) {KnockbackInMeters = 1f});
    }
}