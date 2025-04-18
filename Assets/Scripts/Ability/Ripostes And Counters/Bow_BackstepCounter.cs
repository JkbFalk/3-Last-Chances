using System.Collections.Generic;
using UnityEngine;

public class Bow_BackstepCounter : Counter
{

    public Bow_BackstepCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Ranged, "BowBasicAttack"));
    }

    public override void CallAbilityEvent1()
    {
        Utils.PlaySoundEffect(User.AudioSource, "Bow/Bow_Draw" + Utils.GetRandomSoundNumber("Bow_Draw"), 0.6f);
    }


    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        projectile.OnlyDestroyOnTargetHit = true;
        projectile.Target = Target;
        Utils.PlaySoundEffect(User.AudioSource, "Bow/Bow_Release" + Utils.GetRandomSoundNumber("Bow_Release"), 0.6f);
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Rigidbody2D.velocity = Vector2.zero;
        Utils.CopyItemAppearanceForPlayer(Constants.ItemType.Tool, "Projectile_BowBasicAttack");
    }
}