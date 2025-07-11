using System.Collections.Generic;
using UnityEngine;

public class Longblade_Riposte : Riposte
{

    public Longblade_Riposte(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_RIPOSTE, 0, Constants.DamageType.Light) {KnockbackInMeters = 0.5f});
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.STAGGER_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.DamageType.Light, "Projectile Redirect"));
    }
}