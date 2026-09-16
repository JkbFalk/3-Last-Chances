using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_PlundererAnima : Ability {
    public static AbilityFamily Family = AbilityFamily.Anima;
    private AreaOfEffect _aoe;
    private int _counter = 0;
    public NPCAbility_PlundererAnima(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.SmallSharp;
        DamageSources.Add(new DamageSource(300, 100, Constants.DamageType.Heavy) {KnockbackInMeters = 3f});
        WaitTimeBeforeNextAction = 0.4f;
        AddCustomSound("Use", "Wind/WindSlash", 0.9f);
        Properties.AddRange(new List<Property> {Property.ImmuneToFlinch, Property.CountersBlock, Property.CountersRiposte});
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_RootedInPlace(new(this)) };
        TransitionIntoAnimationDuration = 0;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "PlundererAnima");
        Utils.Apply2DFlip(_aoe.transform.parent.gameObject, User.Actions.IsFlipped);
        _aoe.transform.localEulerAngles = new Vector3(0, 0, 0);
        PlayCustomSound("Use");
        AdjustSize();
    }

    public void AdjustSize() {
        if(_counter < 50) {
            _counter++;
            ParticleSystem.ShapeModule shape = _aoe.transform.parent.GetComponent<ParticleSystem>().shape;
            shape.radius = 2 + 0.2f * _counter;
            _aoe.transform.localScale = new Vector2(0.4f + 0.04f * _counter, 0.4f + 0.04f * _counter);
            _aoe.transform.localPosition = new Vector2( 1.25f + 0.115f * _counter, 0);
            GameController.Instance.WaitAndRunMethod(0.02f, AdjustSize);
        }
        else if(_aoe != null && _aoe.gameObject.IsDestroyed() == false){
            _aoe.DealingDamage = false;
            _aoe.transform.parent.GetComponent<TemporaryObject>().MakeObjectDisappear(0.1f);
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        damage.TargetOfDamage.AddEffect(new Effect_Bleed(20 * User.HeavyInjury.Current / 100, new(this)));
    }
}