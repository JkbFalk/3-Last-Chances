using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_SwordSlam : Ability {
    public static float Cooldown = 3;
    public NPCAbility_SwordSlam(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(80, 250, Constants.DamageType.Heavy));
        if (User.CheckIfUnderEffect(typeof(Effect_IgnisCaptainBuff)))
        {
            Properties.Add(AbilityProperty.CounteredByRiposte);
        }
    }
}