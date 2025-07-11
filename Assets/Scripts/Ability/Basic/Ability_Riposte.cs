using System.Collections.Generic;
using UnityEngine;

public class Riposte : Ability {
    public Riposte(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.Riposte);
        TransitionIntoAnimationDuration = 0.05f;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_Invincible(new(this)) };
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        projectile.transform.up = Target.transform.position - User.ProjectileSpawnLocation.transform.position;
    }
}