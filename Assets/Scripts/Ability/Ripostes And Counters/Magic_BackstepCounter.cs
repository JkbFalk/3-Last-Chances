using System.Collections.Generic;
using UnityEngine;

public class Magic_BackstepCounter : Counter
{

    public Magic_BackstepCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Magic, "AoE") {KnockbackInMeters = 1.5f});
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Rigidbody2D.linearVelocity = Vector2.zero;
    }
}