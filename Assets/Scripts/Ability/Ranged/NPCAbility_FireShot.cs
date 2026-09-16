using UnityEngine;

public class NPCAbility_FireShot : Ability {
    public static float Cooldown = 6;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public NPCAbility_FireShot(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 250, Constants.DamageType.Ranged) {KnockbackInMeters = 5.5f});
        AddCustomSound("Shoot", "Explosion/CannonBallShot", 0.65f);
        WaitTimeBeforeNextAction = 0.3f;
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        GameObject vfx = Utils.CreateVisualEffect(new(this), "CannonBallExplosion", User.ProjectileSpawnLocation.transform.position.x, User.ProjectileSpawnLocation.transform.position.y);
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(50 * User.MagicStagger.Current / 100, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        GameObject vfx = Utils.CreateVisualEffect(new(this), "FireShotExplosion", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
    }
}