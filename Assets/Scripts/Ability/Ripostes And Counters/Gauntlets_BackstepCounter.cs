using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_BackstepCounter : Counter
{

    public Gauntlets_BackstepCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Light) {KnockbackInMeters = 1f});
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Rigidbody2D.linearVelocity = Vector2.zero;
    }
}