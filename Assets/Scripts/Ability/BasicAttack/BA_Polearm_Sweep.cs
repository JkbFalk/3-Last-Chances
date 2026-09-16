using UnityEngine;

public class BA_Polearm_Sweep : BasicAttack {
    public BA_Polearm_Sweep(Unit ability_user, int combo_counter) : base(ability_user) {
        Properties.Add(Property.StrongBasicAttack);
        DamageSources.Add(new DamageSource(50, 200 + combo_counter * 70, Constants.DamageType.Heavy) {KnockbackInMeters = 8f});
        TransitionIntoAnimationDuration = 0.05f;
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}