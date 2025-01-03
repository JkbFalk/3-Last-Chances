using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_DrawSlash : Ability {

    public static float Cooldown = 8;
    public NPCAbility_DrawSlash(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.5f;
        DamageSources.Add(new DamageSource(120, 0, Constants.DamageType.Heavy));
        DamageSources.Add(new DamageSource(120, 0, Constants.DamageType.Heavy, "2"));
        AddCustomSound("Swing1", "TwinBlades/TwinBlades_Swing6", 1f);
        AddCustomSound("Swing2", "TwinBlades/TwinBlades_Swing5", 1f);
    }

    public override void CallAbilityEvent1() {
        User.Actions.PushUnitForward(100);
    }
}