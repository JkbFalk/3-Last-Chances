public class BA_TwinBlades_S : BasicAttack {

    public BA_TwinBlades_S(Unit ability_user) : base(ability_user) {
        Properties.Add(AbilityProperty.StrongBasicAttack);
        DamageSources.Add(new DamageSource(100 / 2, 500 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        AddCustomSound("Fly", "Blade/Blade_Fly3", 0.7f);
    }
}