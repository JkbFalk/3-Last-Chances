using System.Collections.Generic;
using UnityEngine;

public class Polearm_RollCounter : Counter
{

    public Polearm_RollCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Heavy) {Knockback = 150});
    }
}