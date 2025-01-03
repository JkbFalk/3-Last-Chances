using UnityEngine;

public class BA_MagicMelee_S : BasicAttack {
    public BA_MagicMelee_S(Unit ability_user) : base(ability_user) {
        IsStrongBasicAttack = true;
        DamageSources.Add(new DamageSource(50, 800, Constants.DamageType.Magic, "AoE") {Knockback = 450});
        TransitionIntoAnimationDuration = 0.05f;
        AddCustomSound("Swing", "Magic/Magic_Blast1", 1f);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}