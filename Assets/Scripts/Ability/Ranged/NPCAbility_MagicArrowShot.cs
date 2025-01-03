using UnityEngine;

public class NPCAbility_MagicArrowShot : Ability {
    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Anima;
    public NPCAbility_MagicArrowShot(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 300, Constants.DamageType.Ranged) {Knockback = 150});
        AddCustomSound("Release", "Bow/Bow_Release1", 0.8f);
        AddCustomSound("Draw", "Bow/Bow_Draw1", 0.8f);
        WaitTimeBeforeNextAction = 0.3f;
    }
}