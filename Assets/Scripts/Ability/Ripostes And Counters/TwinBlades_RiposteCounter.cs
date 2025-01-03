using System.Collections.Generic;

public class TwinBlades_RiposteCounter : Counter {

    public TwinBlades_RiposteCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER / 2, 0, Constants.DamageType.Light) {Knockback = 100});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        AddCustomSound("Riposte", "TwinBlades/TwinBlades_Riposte3", 0.7f);
    }
}