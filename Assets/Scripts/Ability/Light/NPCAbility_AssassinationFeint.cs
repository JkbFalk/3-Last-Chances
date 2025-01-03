using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_AssassinationFeint : Ability {

    public static float Cooldown = 5;
    public static AbilityFamily Family = AbilityFamily.Salutis;
    public NPCAbility_AssassinationFeint(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 100, Constants.DamageType.Light));
        WaitTimeBeforeNextAction = 0.1f;
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(150, 10, 5);
    }
}