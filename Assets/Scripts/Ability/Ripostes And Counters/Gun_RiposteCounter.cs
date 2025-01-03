using System.Collections.Generic;

public class Gun_RiposteCounter : Counter
{

    public Gun_RiposteCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Ranged, "GunBasicAttack"));
        AddCustomSound("Riposte", "TwinBlades/TwinBlades_Riposte3", 0.7f);
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        projectile.OnlyDestroyOnTargetHit = true;
        projectile.Target = Target;
        Utils.PlaySoundEffect(User.AudioSource, "Gun/Gun_BasicAttack" + Utils.GetRandomSoundNumber("Gun_BasicAttack"), 0.6f);
    }
}