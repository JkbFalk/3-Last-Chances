public class BA_TwinBlades_FS : BasicAttack {

    public BA_TwinBlades_FS(Unit ability_user) : base(ability_user) {
        Properties.Add(AbilityProperty.StrongBasicAttack);
        DamageSources.Add(new DamageSource(100 / 2, 750 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }
}