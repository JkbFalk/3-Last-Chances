using UnityEngine;

public class BA_MagicMelee_S : BasicAttack {
    public BA_MagicMelee_S(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.StrongBasicAttack);
        DamageSources.Add(new DamageSource(50, 800, Constants.DamageType.Magic, "AoE") {KnockbackInMeters = 4.5f});
        TransitionIntoAnimationDuration = 0.05f;
        AddCustomSound("Swing", "Magic/Magic_Blast1", 1f);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}