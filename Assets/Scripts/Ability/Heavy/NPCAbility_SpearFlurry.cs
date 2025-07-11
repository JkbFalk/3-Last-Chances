using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_SpearFlurry : Ability {
    public static float Cooldown = 6;

    public NPCAbility_SpearFlurry(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(25, 150, Constants.DamageType.Heavy) {KnockbackInMeters = 1.8f});
        DamageSources.Add(new DamageSource(30, 10, Constants.DamageType.Heavy, "2+"));
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Heavy, "Final") {KnockbackInMeters = 3.5f});
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(1, 15);
    }

    public override void CallAbilityEvent2()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(0.5f, 15);
    }

    public override void CallAbilityEvent3()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(2, 15);
    }
}