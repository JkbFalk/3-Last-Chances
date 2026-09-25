using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_Fireball : Ability {

    public static float Cooldown = 12;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private Projectile _fireBall;
    private Vector3 _intendedDestination;
    private float _distance;
    private int _counter = 0;
    private bool _dealtAoEDmg = false;

    public NPCAbility_Fireball(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Ability/Ability_Flamethrower", 0.2f);
        AddCustomSound("Explosion", "Fire/Fire4", 0.6f);
        DamageSources.Add(new DamageSource(50, 50, Constants.DamageType.Magic, "FireballNPC"));
        DamageSources.Add(new DamageSource(200, 0, Constants.DamageType.Magic, "AoE"));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        NameOfAnimationToAutoPlay = "FireballNPC";
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        _fireBall = Utils.CreateProjectile(new(this), "FireballNPC");
        _fireBall.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        _fireBall.CleanUpAfter(5 / User.MagicAttackSpeed.Current);
    }

    public override void CallAbilityEvent2()
    {
        _intendedDestination = Target.transform.position;
        _distance = Vector2.Distance(_fireBall.transform.position, _intendedDestination) / 2;
        _fireBall.transform.SetParent(User.transform.parent);
        _fireBall.IsFlying = true;
        _fireBall.transform.eulerAngles = Vector3.zero;
        _fireBall.transform.up = (_intendedDestination + new Vector3(0, _distance) - _fireBall.transform.position).normalized;
        _fireBall.DealingDamage = true;
        GameController.Instance.WaitAndRunMethod(1, Explode);
        AdjustFireballAngle();
    }

    public void Explode() {
        if(_fireBall != null && _fireBall.gameObject != null && _fireBall.gameObject!= null) {
            AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "FireballExplosion");
            PlayCustomSound("Explosion", 0.6f, aoe.transform.parent.GetComponent<AudioSource>());
            aoe.transform.parent.position = _fireBall.transform.position;
            MonoBehaviour.Destroy(_fireBall.gameObject);
        }
    } 

    public void AdjustFireballAngle() {
        if(_counter < 100 && _fireBall != null && _fireBall.gameObject != null && _fireBall.gameObject!= null) {
            _counter++;
            _fireBall.transform.up = (_intendedDestination + new Vector3(0, _distance - _distance * _counter / 50) - _fireBall.transform.position).normalized;
            GameController.Instance.WaitAndRunMethod(0.01f, AdjustFireballAngle);
            _fireBall.DealingDamage = true;
        }
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(object_hitting.gameObject.name == "AoE" && _dealtAoEDmg) {
            return;
        }
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {    
        if(damage.DamagingObject.gameObject.name != "AoE") {
            AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "FireballExplosion");
            PlayCustomSound("Explosion", 0.6f, aoe.transform.parent.GetComponent<AudioSource>());
            aoe.transform.parent.position = damage.TargetOfDamage.transform.position;
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        if(damage.DamagingObject.gameObject.name == "AoE") {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(60 * User.MagicStagger.Current / 100, new(this)));
            _dealtAoEDmg = true;
        }
    }
}