using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_SpearThrow: Ability {
    public static float Cooldown = 6;

    public NPCAbility_SpearThrow(Unit ability_user) : base(ability_user) {
        AddCustomSound("Hit", "Explosion/Explosion1", 0.6f);
        DamageSources.Add(new DamageSource(100, 500, Constants.DamageType.Ranged) {KnockbackInMeters=10f});
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        base.ExtraBehaviourOnHit(damage);
        Utils.CreateVisualEffect(new(this), "PowerfulHit", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
        PlayCustomSound("Hit");
    }
}