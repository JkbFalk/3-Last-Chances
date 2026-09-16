using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_BackstepCounter : Counter
{

    public TwinBlades_BackstepCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER / 2, 0, Constants.DamageType.Light) {KnockbackInMeters = 1f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Rigidbody2D.linearVelocity = Vector2.zero;
    }
}