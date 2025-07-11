using System.Collections.Generic;

public class Polearm_RiposteCounter : Counter{

    public Polearm_RiposteCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Heavy) {KnockbackInMeters = 1.5f});
        AddCustomSound("Riposte", "TwinBlades/TwinBlades_Riposte3", 0.7f);
    }
}