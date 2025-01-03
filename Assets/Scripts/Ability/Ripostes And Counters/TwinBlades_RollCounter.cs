using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_RollCounter : Counter
{

    public TwinBlades_RollCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER / 2, 0, Constants.DamageType.Light) {Knockback = 100});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }
}