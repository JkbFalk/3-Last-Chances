using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_BladeThrust : Ability
{
    public static float Cooldown = 6;

    public NPCAbility_BladeThrust(Unit ability_user) : base(ability_user)
    {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(200, 0, Constants.DamageType.Heavy));
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(4, 25);
    }
}