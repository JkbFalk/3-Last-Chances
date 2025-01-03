public class BA_TwinBlades_FFS : BasicAttack {

    public BA_TwinBlades_FFS(Unit ability_user) : base(ability_user) {
        IsStrongBasicAttack = true;
        DamageSources.Add(new DamageSource(150 / 2, 1000 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        AddCustomSound("Swing1", "TwinBlades/TwinBlades_Swing15", 0.7f);
        AddCustomSound("Swing2", "TwinBlades/TwinBlades_Swing3", 0.7f);
    }
}