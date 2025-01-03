using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_SwordSweep : Ability {
    public static float Cooldown = 3;
    public NPCAbility_SwordSweep(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 80, Constants.DamageType.Heavy));
        WaitTimeBeforeNextAction = 0.1f;
        if(User.CheckIfUnderEffect(typeof(Effect_Ignis_Paladin_Buff))) {
            AbilityModifiers.Add(Constants.AbilityModifier.CounteredByBackstep);
        }
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(200, 75);
    }
}