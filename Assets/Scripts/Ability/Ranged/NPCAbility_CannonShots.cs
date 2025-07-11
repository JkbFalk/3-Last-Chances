using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_CannonShots : Ability {
    public static float Cooldown = 8;
    public NPCAbility_CannonShots(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(50, 150, Constants.DamageType.Ranged) {KnockbackInMeters = 3.5f});
        AddCustomSound("Shoot", "Explosion/CannonBallShot", 0.65f);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }
}
