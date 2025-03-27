public class BA_Greatsword_S : BasicAttack {

    public BA_Greatsword_S(Unit ability_user) : base(ability_user) {
        Properties.Add(AbilityProperty.StrongBasicAttack);
        DamageSources.Add(new DamageSource(50, 400, Constants.DamageType.Heavy));
    }
}