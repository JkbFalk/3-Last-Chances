using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Riposte : Riposte {

    public Greatsword_Riposte(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_RIPOSTE, 0, Constants.DamageType.Heavy) {Knockback = 100});
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_RIPOSTE, 0, Constants.DamageType.Heavy, "SmallCircleAoE") {Knockback = 100});
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.STAGGER_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.DamageType.Heavy, "Projectile Redirect"));
    }
}