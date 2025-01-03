using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_ThrowTwinBlades: Ability {
    public static float Cooldown = 6;

    public NPCAbility_ThrowTwinBlades(Unit ability_user) : base(ability_user) {
        AddCustomSound("Fly", "Blade/Blade_Fly3", 0.7f);
        DamageSources.Add(new DamageSource(25, 150, Constants.DamageType.Light) {Knockback = 150});
        DamageSources.Add(new DamageSource(450, 0, Constants.DamageType.Light, "2") {Knockback = 250});
    }

    public override void CallAbilityEvent4()
    {
        if(Utils.CheckIfCurrenTargetIsInFrontOfUnit(User) == false) {
            EndThisAbility();
        }
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
    }
}