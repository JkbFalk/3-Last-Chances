public class NPCAbility_QuickDoubleSlash : Ability {
    public static float Cooldown = 8;
    public NPCAbility_QuickDoubleSlash(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(0, 340, Constants.DamageType.Heavy));
        DamageSources.Add(new DamageSource(180, 0, Constants.DamageType.Heavy, "2"));
        WaitTimeBeforeNextAction = 0.1f;
        AddCustomSound("Swing1", "Greatsword/Greatsword_Swing11", 0.4f);
        AddCustomSound("Swing2", "Greatsword/Greatsword_Swing10", 0.4f);
    }
}