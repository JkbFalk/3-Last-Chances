using System.Collections.Generic;
using UnityEngine;

public class Daggers_RollCounter : Counter
{

    public Daggers_RollCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER / 2, 0, Constants.DamageType.Light) {Knockback = 25});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }
}