using UnityEngine;

public class NPCAbility_DodgeAndShootArrow : Ability {
    public static float Cooldown = 6;

    public NPCAbility_DodgeAndShootArrow(Unit ability_user) : base(ability_user) {
        AddCustomSound("Draw", "Bow/Bow_Draw2", 0.6f);
        DamageSources.Add(new DamageSource(150, 150, Constants.DamageType.Ranged));
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        Utils.PlaySoundEffect(User.AudioSource, "Bow/Bow_Release" + Utils.GetRandomSoundNumber("Bow_Release"), 0.6f);
    }
}