using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_SwordThrust : Ability {
    public static float Cooldown = 3;
    public static AbilityFamily Family = AbilityFamily.Anima;
    public NPCAbility_SwordThrust(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.5f;
        DamageSources.Add(new DamageSource(300, 50, Constants.DamageType.Heavy));
        if (User.CheckIfUnderEffect(typeof(Effect_IgnisCaptainBuff)))
        {
            Properties.Add(AbilityProperty.CounteredByRoll);
        }
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(75, 45);
    }
}