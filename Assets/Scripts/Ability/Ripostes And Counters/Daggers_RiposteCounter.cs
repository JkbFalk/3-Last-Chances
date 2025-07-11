using System.Collections.Generic;

public class Daggers_RiposteCounter : Counter {

    public Daggers_RiposteCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER / 2, 0, Constants.DamageType.Light) {KnockbackInMeters = 0.25f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        AddCustomSound("Riposte", "Daggers/Daggers_Swing7", 0.7f);
    }
}