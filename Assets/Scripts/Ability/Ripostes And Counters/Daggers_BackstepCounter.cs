using System.Collections.Generic;
using UnityEngine;

public class Daggers_BackstepCounter : Counter
{

    public Daggers_BackstepCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER / 2, 0, Constants.DamageType.Light) {Knockback = 25});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Rigidbody2D.velocity = Vector2.zero;
    }
}