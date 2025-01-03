public class NPCAbility_MultipleStabs : Ability {

    public static float Cooldown = 4;
    public NPCAbility_MultipleStabs(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Light));
        DamageSources.Add(new DamageSource(50, 0, Constants.DamageType.Light, "2+"));
        WaitTimeBeforeNextAction = 0.1f;
    }
}