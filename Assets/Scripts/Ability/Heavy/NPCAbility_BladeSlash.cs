using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_BladeSlash : Ability {
    public static float Cooldown = 6;
    public NPCAbility_BladeSlash(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(100, 200, Constants.DamageType.Heavy));
    }
}