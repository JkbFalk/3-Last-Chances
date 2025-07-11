using System.Collections.Generic;
using UnityEngine;

public class Magic_Riposte : Riposte {

    public Magic_Riposte(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_RIPOSTE, 0, Constants.DamageType.Magic, "AoE") {KnockbackInMeters = 1f});
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.STAGGER_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.DamageType.Heavy, "Projectile Redirect"));
    }
}