using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_FireWave : Ability {

    public static float Cooldown = 13;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public Projectile Bullet;
    public NPCAbility_FireWave(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        AddCustomSound("Use", "Fire/Fire6", 0.65f);
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        DamageSources.Add(new DamageSource(200, 150, Constants.DamageType.Magic));
    }

    public override void CallAbilityEvent1()
    {
        Bullet = Utils.CreateProjectile(new(this), "FireWave");
        Bullet.CleanUpAfter(10 / User.MagicAttackSpeed.Current);
        Bullet.transform.up = (Target.transform.position - Bullet.transform.position).normalized;
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(40 * User.MagicStagger.Current / 100, new(this)));
    }
}