using System.Collections.Generic;

public class Bow_RiposteCounter : Counter
{

    public Bow_RiposteCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Ranged, "BowBasicAttack"));
        AddCustomSound("Riposte", "TwinBlades/TwinBlades_Riposte3", 0.7f);
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
}