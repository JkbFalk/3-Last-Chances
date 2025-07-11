using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_TripleJumpStab : Ability {

    public static float Cooldown = 6;
    public NPCAbility_TripleJumpStab(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(0, 250, Constants.DamageType.Light) {KnockbackInMeters = 0.7f});
        DamageSources.Add(new DamageSource(200, 0, Constants.DamageType.Light, "2") {KnockbackInMeters = 2f});
        WaitTimeBeforeNextAction = 0.5f;
        AddCustomSound("Swing1", "Blade/Blade_Swing1", 0.6f);
        AddCustomSound("Swing2", "Blade/Blade_Swing2", 0.8f);
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(4, 60);
    }

    public override void CallAbilityEvent2()
    {
        ResetPotentialTargets();
    }

    public override void CallAbilityEvent3()
    {
        Properties.Add(Property.CounteredByRoll);
    }
}