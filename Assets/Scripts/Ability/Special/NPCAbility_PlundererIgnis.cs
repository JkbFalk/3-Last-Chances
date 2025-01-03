using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_PlundererIgnis : Ability {
    private int _counter = 0;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;

    public NPCAbility_PlundererIgnis(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/FlamethrowerLoop1", 1.25f);
        DamageSources.Add(new DamageSource(200, 100, Constants.DamageType.Heavy) {Knockback = 600});
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        CanBeInterruptedByFlinching = false;
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
        TransitionIntoAnimationDuration = 0;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        _aoe = Utils.CreateAreaOfEffect(new(this), "PlundererIgnis");
        ExtendWave();
        PlayCustomSound("Use");
        GameController.Instance.WaitAndRunMethod(0.01f, AdjustRotation);
    }

    public void AdjustRotation() {
        _aoe.transform.up = Utils.GetDirectionVector(_aoe.transform.position, Target.transform.position, User.Actions.IsFlipped, 30);
    }

    public void ExtendWave() {
        if(_counter < 20) {
            _counter++;
            ParticleSystem.ShapeModule shape = _aoe.GetComponent<ParticleSystem>().shape;
            shape.scale = new Vector3(1 + 1.25f * _counter, 1f);
            shape.position = new Vector3(0, 0.65f * _counter);
            _aoe.GetComponent<BoxCollider2D>().size = new Vector3(1.5f, 1 + 1.25f * _counter);
            _aoe.GetComponent<BoxCollider2D>().offset = new Vector3(0, 0.65f * _counter);
            GameController.Instance.WaitAndRunMethod(0.1f / User.HeavyAttackSpeed.Current, ExtendWave);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_aoe != null && _aoe.gameObject.IsDestroyed() == false) {
            _aoe.DealingDamage = false;
            _aoe.MakeObjectDisappear(1);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(40 * User.MagicStagger.Current / 100, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}