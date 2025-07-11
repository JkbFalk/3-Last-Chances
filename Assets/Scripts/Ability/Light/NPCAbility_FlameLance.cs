using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlameLance : Ability {

    private int _counter = 0;
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;
    private AreaOfEffect _aoe2;

    public NPCAbility_FlameLance(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/FlamethrowerLoop1", 0.4f);
        DamageSources.Add(new DamageSource(150, 100, Constants.DamageType.Heavy) {KnockbackInMeters = 6f});
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        Properties.Add(Property.ImmuneToFlinch);
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        _aoe = Utils.CreateAreaOfEffect(new(this), "FlameLance");
        _aoe2 = Utils.CreateAreaOfEffect(new(this), "FlameLance", User.ProjectileSpawnLocation.transform.position.x, User.ProjectileSpawnLocation.transform.position.y + 1.5f);
        ExtendLance();
        PlayCustomSound("Use");
        GameController.Instance.WaitAndRunMethod(0.01f, AdjustRotation);
    }

    public void AdjustRotation() {
        _aoe.transform.up = Utils.GetDirectionVector(_aoe.transform.position, Target.transform.position, User.Actions.IsFlipped, 30);
        _aoe2.transform.up = Utils.GetDirectionVector(_aoe2.transform.position, Target.transform.position, User.Actions.IsFlipped, 30);
    }

    public void ExtendLance() {
        if(_counter < 100) {
            if(_counter % 40 == 0) {
                PlayCustomSound("Use");
            }
            _counter++;
            foreach(AreaOfEffect aoe in new List<AreaOfEffect> {_aoe, _aoe2}) {
                if(aoe.IsDestroyed() == false) {
                    ParticleSystem.ShapeModule shape = aoe.GetComponent<ParticleSystem>().shape;
                    shape.scale = new Vector3(1 + 0.25f * _counter, 0.2f);
                    shape.position = new Vector3(0, 0.075f * _counter);
                    aoe.GetComponent<BoxCollider2D>().size = new Vector3(0.3f, 1 + 0.25f * _counter);
                    aoe.GetComponent<BoxCollider2D>().offset = new Vector3(0, 0.075f * _counter);
                }
            }
            GameController.Instance.WaitAndRunMethod(0.01f / User.HeavyAttackSpeed.Current, ExtendLance);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_aoe != null) {
            _aoe.DealingDamage = false;
            _aoe.MakeObjectDisappear(1);
        }
        if(_aoe2 != null) {
            _aoe2.DealingDamage = false;
            _aoe2.MakeObjectDisappear(1);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(40 * User.MagicStagger.Current / 100, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}