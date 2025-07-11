using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_JumpAndWindSlash : Ability {
    public static float Cooldown = 6;

    public NPCAbility_JumpAndWindSlash(Unit ability_user) : base(ability_user) {
        AddCustomSound("Hit", "Explosion/Explosion1", 0.9f);
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Heavy) {KnockbackInMeters = 4.5f});
        DamageSources.Add(new DamageSource(150, 100, Constants.DamageType.Heavy, "WindSlash") {KnockbackInMeters = 1.5f});
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void CallAbilityEvent1()
    {
        User.ApplyForce(new Vector2(0.2f * (User.Actions.IsFlipped ? -1 : 1), 1).normalized * 3, this);
    }

    public override void CallAbilityEvent2()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(3, 90);
    }

    public override void CallAbilityEvent3()
    {
        Projectile proj = Utils.CreateProjectile(new(this), "WindSlash");
        GameObject vfx = Utils.CreateVisualEffect(new(this), "EarthHit");
        vfx.transform.position += new Vector3(User.Actions.IsFlipped ? -1.7f : 1.7f, 0, 0);
    }
}