using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_ReverseScissors: Ability {
    public static float Cooldown = 6;

    public NPCAbility_ReverseScissors(Unit ability_user) : base(ability_user) {
        AddCustomSound("Swing1", "TwinBlades/TwinBlades_Swing1", 0.9f);
        AddCustomSound("Swing2", "TwinBlades/TwinBlades_Swing7", 0.9f);
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Light) {Knockback = 50});
        DamageSources.Add(new DamageSource(200, 50, Constants.DamageType.Light, "2") {Knockback = 50});
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(200, 30);
    }

    public override void CallAbilityEvent2()
    {
    }
}