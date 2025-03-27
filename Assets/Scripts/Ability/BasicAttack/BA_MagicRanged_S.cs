public class BA_MagicRanged_S : BasicAttack {
    public BA_MagicRanged_S(Unit ability_user) : base(ability_user) {
        Properties.Add(AbilityProperty.StrongBasicAttack);
        DamageSources.Add(new DamageSource(25, 400, Constants.DamageType.Magic) {Knockback = 200});
        TransitionIntoAnimationDuration = 0.05f;
        AddCustomSound("Swing", "Magic/Magic_Blast4", 1f);
    }
}