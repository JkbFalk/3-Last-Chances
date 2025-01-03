using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_DashAndDualSlash : Ability {

    public static float Cooldown = 8;
    public NPCAbility_DashAndDualSlash(Unit ability_user) : base(ability_user) {
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        DamageSources.Add(new DamageSource(0, 400 / 2, Constants.DamageType.Light));
        DamageSources.Add(new DamageSource(200 / 2, 0, Constants.DamageType.Light, "2"));
        AddCustomSound("Dash", "Footsteps/Footsteps_Earth2", 1f);
        WaitTimeBeforeNextAction = 0.1f;
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        User.Actions.SetWeaponCollisionName("2");
        ResetPotentialTargets();
    }
}