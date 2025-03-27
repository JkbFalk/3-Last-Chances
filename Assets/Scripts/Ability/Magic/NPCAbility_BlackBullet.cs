using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_BlackBullet : Ability {
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Salutis;
    public Projectile Bullet;
    public NPCAbility_BlackBullet(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.25f;
        HitSoundVolume = 0.3f;
        DamageSources.Add(new DamageSource(200, 400, Constants.DamageType.Magic));
        AddCustomSound("Use", "Ice/Ice_Use4", 0.3f);
        HitSoundType = Constants.HitSoundTypeEnum.Magic;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        Properties.AddRange(new List<Ability.AbilityProperty> {AbilityProperty.IgnoresImmunityToHits});
    }

    public override void CallAbilityEvent1()
    {
        GameObject vfx1 = Utils.CreateVisualEffect(new(this), "BlackFlames");
        vfx1.GetComponent<AttachObjectToBodyPart>().Unit = User;
        GameObject vfx2 = Utils.CreateVisualEffect(new(this), "BlackFlames");
        vfx1.GetComponent<AttachObjectToBodyPart>().BodyPartName = "Left Hand";
        vfx1.GetComponent<AttachObjectToBodyPart>().Unit = User;
    }

    public override void CallAbilityEvent2()
    {
        Bullet = Utils.CreateProjectile(new(this), "BlackBullet");
        Bullet.CleanUpAfter(20 / User.MagicAttackSpeed.Current);
        GameController.Instance.WaitAndRunMethod(1, RemoveChangeTransform);
    }

    public void RemoveChangeTransform() {
        if(Bullet != null) {
            MonoBehaviour.Destroy(Bullet.GetComponent<ChangeTransformOverTime>());
        }
    }

    public override void CallAbilityEvent3()
    {
        Bullet.HomingOntoUnit = User.CurrentTarget;
        Bullet.IsFlying = true;
        Bullet.DealingDamage = true;
    }

    public static void AdditionalActionsOnSettingsAbilityAsPotentialAction(Unit user) {
        if(!user.gameObject.name.Contains("Clarise1")) {
            user.AddEffect(new Effect_RipostingSpecificAbility(new(user)) {AbilitiesToRiposte = new List<Type> {typeof(NPCAbility_BlackBullet)}});
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 5);
    }
}