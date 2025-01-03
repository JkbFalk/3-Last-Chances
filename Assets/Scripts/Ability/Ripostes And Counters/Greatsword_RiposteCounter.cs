using System.Collections.Generic;

public class Greatsword_RiposteCounter : Counter{

    public Greatsword_RiposteCounter(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(Constants.INJURY_PERCENTAGE_FROM_COUNTER, 0, Constants.DamageType.Heavy) {Knockback = 150});
        AddCustomSound("Riposte", "TwinBlades/TwinBlades_Riposte3", 0.7f);
    }
}