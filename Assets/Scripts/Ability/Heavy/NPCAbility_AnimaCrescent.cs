using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_AnimaCrescent : Ability {
    public static float Cooldown = 6;

    public NPCAbility_AnimaCrescent(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(400, 100, Constants.DamageType.Heavy));
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(200, 45);
    }
}