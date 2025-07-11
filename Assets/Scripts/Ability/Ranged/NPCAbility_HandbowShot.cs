using UnityEngine;

public class NPCAbility_HandbowShot : Ability {
    public static float Cooldown = 6;

    public NPCAbility_HandbowShot(Unit ability_user) : base(ability_user) {
        AddCustomSound("Shoot", "Bow/Bow_Release8", 0.6f);
        DamageSources.Add(new DamageSource(50, 150, Constants.DamageType.Ranged) {KnockbackInMeters = 4.5f});
    }
}