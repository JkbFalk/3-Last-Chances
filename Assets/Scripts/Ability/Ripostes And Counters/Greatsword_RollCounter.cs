using System.Collections.Generic;
using UnityEngine;

public class Greatsword_RollCounter : Counter
{

    public Greatsword_RollCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Heavy) {Knockback = 150});
    }
}