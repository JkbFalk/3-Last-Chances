using System.Collections.Generic;
using UnityEngine;

public class Gun_Riposte : Riposte
{

    public Gun_Riposte(Unit ability_user) : base(ability_user) {
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_RIPOSTE, 0, Constants.DamageType.Ranged, "GunBasicAttack"));
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.STAGGER_PERCENTAGE_FROM_PROJECTILE_RIPOSTE, Constants.DamageType.Ranged, "Projectile Redirect"));
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        projectile.OnlyDestroyOnTargetHit = true;
        projectile.Target = Target;
        Utils.PlaySoundEffect(User.AudioSource, "Gun/Gun_BasicAttack" + Utils.GetRandomSoundNumber("Gun_BasicAttack"), 0.6f);
    }
}