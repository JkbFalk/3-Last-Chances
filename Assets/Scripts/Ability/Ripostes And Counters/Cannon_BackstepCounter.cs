using System.Collections.Generic;
using UnityEngine;

public class Cannon_BackstepCounter : Counter
{

    public Cannon_BackstepCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Light) {KnockbackInMeters = 1f});
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Rigidbody2D.velocity = Vector2.zero;
    }
}