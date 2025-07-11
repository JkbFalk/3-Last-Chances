public class BA_Greatsword_FS : BasicAttack {

    public BA_Greatsword_FS(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.StrongBasicAttack);
        DamageSources.Add(new DamageSource(75, 600, Constants.DamageType.Heavy));
        AddCustomSound("Swing1", "Greatsword/Greatsword_Swing2", 1f);
        AddCustomSound("Swing2", "Greatsword/Greatsword_Swing6", 1f);
        AddCustomSound("Explosion", "Explosion/Ground Explosion", 0.3f);
    }
}