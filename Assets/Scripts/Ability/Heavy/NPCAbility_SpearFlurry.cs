using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_SpearFlurry : Ability {
    public static float Cooldown = 6;

    public NPCAbility_SpearFlurry(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(25, 150, Constants.DamageType.Heavy) {Knockback=180});
        DamageSources.Add(new DamageSource(30, 10, Constants.DamageType.Heavy, "2+"));
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Heavy, "Final") {Knockback=350});
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(90, 15, 15);
    }

    public override void CallAbilityEvent2()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(40, 15, 15);
    }

    public override void CallAbilityEvent3()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(150, 15, 15);
    }
}