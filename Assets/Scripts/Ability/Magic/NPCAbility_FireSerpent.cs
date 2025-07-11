using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FireSerpent : Ability {
    public static float Cooldown = 25;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public Projectile Bullet;
    public NPCAbility_FireSerpent(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        AddCustomSound("Use", "Fire/Fire7", 0.65f);
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        Properties.Add(Property.ImmuneToFlinch);
        DamageSources.Add(new DamageSource(200, 200, Constants.DamageType.Magic));
    }

    public override void CallAbilityEvent1()
    {
        Bullet = Utils.CreateProjectile(new(this), "FireSerpent");
        Bullet.CleanUpAfter(10);
        GameController.Instance.WaitAndRunMethod(1, RemoveChangeTransform);
        Bullet.transform.SetParent(User.SpriteRenderers["Right Hand"].Bone.transform);
        Bullet.transform.localPosition = new Vector2(0, 0.2f);
    }

    public void RemoveChangeTransform() {
        if(Bullet != null) {
            MonoBehaviour.Destroy(Bullet.GetComponent<ChangeTransformOverTime>());
        }
    }

    public override void CallAbilityEvent2()
    {
        Bullet.DealingDamage = true;
        Bullet.HomingOntoUnit = User.CurrentTarget;
        Bullet.IsFlying = true;
        Bullet.transform.SetParent(Area.Instance.transform);
        ParticleSystem.EmissionModule emission2 = Bullet.GetComponent<ParticleSystem>().emission;
        emission2.rateOverTime = 50;
        AddCollider();
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(Bullet == null || Bullet.gameObject == null || Bullet.gameObject.IsDestroyed()) {
            return;
        }
        else if(Bullet != null && Bullet.IsDestroyed() == false) {
            base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
            Bullet.GetComponent<ParticleSystem>().Stop();
            Bullet.GetComponent<TemporaryObject>().MakeObjectDisappear(1);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(60 * User.MagicStagger.Current / 100, new(this)));
    }

    public void AddCollider() {
        if(Bullet != null && Bullet.gameObject != null && !Bullet.gameObject.IsDestroyed()) {
            AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "FlameTrailCollider");
            aoe.transform.position = Bullet.transform.position;
            GameController.Instance.WaitAndRunMethod(0.1f, AddCollider);
        }
    }
}