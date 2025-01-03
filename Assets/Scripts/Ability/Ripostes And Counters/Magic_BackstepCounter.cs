using System.Collections.Generic;
using UnityEngine;

public class Magic_BackstepCounter : Counter
{

    public Magic_BackstepCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Magic, "AoE") {Knockback = 150});
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Rigidbody2D.velocity = Vector2.zero;
    }
}