using System.Collections.Generic;

public class NPCAbility_HeavySweep : Ability {
    public static float Cooldown = 3;
    public NPCAbility_HeavySweep(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        if(User.IsBoss && User.DamageType == Constants.DamageType.Heavy)
        {
            DamageSources.Add(new DamageSource(100, 450, Constants.DamageType.Heavy) {KnockbackInMeters = 6f});
            Properties.Add(Property.CounteredByBackstep);
        }
        else {
            DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Heavy) {KnockbackInMeters = 3f});
        }
    }
}