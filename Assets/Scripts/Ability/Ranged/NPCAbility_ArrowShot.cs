public class NPCAbility_ArrowShot : Ability {
    public static float Cooldown = 6;
    public NPCAbility_ArrowShot(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 80, Constants.DamageType.Ranged));
        AddCustomSound("Release", "Bow/Bow_Release2", 0.8f);
        AddCustomSound("Draw", "Bow/Bow_Draw2", 0.8f);
        WaitTimeBeforeNextAction = 0.1f;
    }
}