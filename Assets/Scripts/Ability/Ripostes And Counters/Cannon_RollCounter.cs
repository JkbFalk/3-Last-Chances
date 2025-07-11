using System.Collections.Generic;
using UnityEngine;

public class Cannon_RollCounter : Counter
{

    public Cannon_RollCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Light) {KnockbackInMeters = 1f});
    }
}