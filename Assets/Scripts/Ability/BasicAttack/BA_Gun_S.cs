public class BA_Gun_S : BasicAttack {
    public static int AmmoRequiredToUseAbility = 3;
    public BA_Gun_S(Unit ability_user) : base(ability_user) {
        IsStrongBasicAttack = true;
        DamageSources.Add(new DamageSource(300, 600, Constants.DamageType.Ranged));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        Utils.PlaySoundEffect(User.AudioSource, "Gun/Gun_BasicAttack" + Utils.GetRandomSoundNumber("Gun_BasicAttack"), 0.6f);
    }
}