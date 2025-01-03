using System.Collections.Generic;
using UnityEngine;

public class Daggers_Riposte : Riposte
{

    public Daggers_Riposte(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_RIPOSTE / 2, 0, Constants.DamageType.Light) {Knockback = 10});
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.STAGGER_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.DamageType.Light, "Projectile Redirect"));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }
}