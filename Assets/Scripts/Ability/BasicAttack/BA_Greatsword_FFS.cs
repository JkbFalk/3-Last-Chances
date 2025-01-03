public class BA_Greatsword_FFS : BasicAttack {

    public BA_Greatsword_FFS(Unit ability_user) : base(ability_user) {
        IsStrongBasicAttack = true;
        DamageSources.Add(new DamageSource(100, 800, Constants.DamageType.Heavy));
        AddCustomSound("Swing1", "Greatsword/Greatsword_Swing23", 1f);
        AddCustomSound("Swing2", "Greatsword/Greatsword_Swing22", 1f);
        AddCustomSound("Explosion", "Explosion/Ground Explosion", 0.3f);
    }
}