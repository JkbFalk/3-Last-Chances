using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_WindupUpperCut : Ability {
    public static float Cooldown = 6;

    public NPCAbility_WindupUpperCut(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        if(User.IsBoss) {
            Properties.Add(AbilityProperty.CounteredByBackstep);
            DamageSources.Add(new DamageSource(300, 500, Constants.DamageType.Heavy));
        }
        else {
            DamageSources.Add(new DamageSource(150, 300, Constants.DamageType.Heavy));
        }
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(200, 45);
    }
}