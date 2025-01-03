using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_JumpSlashDown : Ability {
    public static float Cooldown = 6;

    public NPCAbility_JumpSlashDown(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(220, 70, Constants.DamageType.Heavy));
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(200, 45);
    }
}