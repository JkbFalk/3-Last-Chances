using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_HeavySlam : Ability {
    public static float Cooldown = 3;

    public NPCAbility_HeavySlam(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(75, 450, Constants.DamageType.Heavy));
    }

        public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(100, 45);
    }
}