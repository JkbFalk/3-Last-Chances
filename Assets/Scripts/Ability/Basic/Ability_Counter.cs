using System.Collections.Generic;
using UnityEngine;

public class Counter : Ability {

    public Counter(Unit ability_user) : base(ability_user) {
        IsCounter = true;
        TransitionIntoAnimationDuration = 0.05f;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_Invincible(new(this)) };
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        projectile.transform.up = Target.SpriteRenderers["Upper Body"].Bone.transform.position - User.ProjectileSpawnLocation.transform.position;
    }

}