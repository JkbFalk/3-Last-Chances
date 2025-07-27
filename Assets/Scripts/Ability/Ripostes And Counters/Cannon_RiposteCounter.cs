using System.Collections.Generic;

public class Cannon_RiposteCounter : Counter {

    public Cannon_RiposteCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Ranged) {KnockbackInMeters = 1f});
        AddCustomSound("Riposte", "TwinBlades/TwinBlades_Riposte3", 0.7f);
    }
}