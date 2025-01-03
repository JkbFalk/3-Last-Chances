using UnityEngine;

public class BA_Polearm_Sweep : BasicAttack {
    public BA_Polearm_Sweep(Unit ability_user, int combo_counter) : base(ability_user) {
        IsStrongBasicAttack = true;
        DamageSources.Add(new DamageSource(50, 200 + combo_counter * 70, Constants.DamageType.Heavy) {Knockback = 800});
        TransitionIntoAnimationDuration = 0.05f;
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}