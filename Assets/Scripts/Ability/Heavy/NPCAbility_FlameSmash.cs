using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlameSmash : Ability {

    private int _counter = 0;
    public static float Cooldown = 5;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;

    public NPCAbility_FlameSmash(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/FlamethrowerLoop1", 0.4f);
        DamageSources.Add(new DamageSource(200, 400, Constants.DamageType.Heavy) {KnockbackInMeters = 2f});
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.5f;
        Properties.Add(Property.ImmuneToFlinch);
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        _aoe = Utils.CreateAreaOfEffect(new(this), "FlameSmash");
        ExtendWave();
        PlayCustomSound("Use");
    }

    public void ExtendWave() {
        if(_counter < 100) {
            if(_counter % 40 == 0) {
                PlayCustomSound("Use");
            }
            _counter++;
            ParticleSystem.ShapeModule shape = _aoe.GetComponent<ParticleSystem>().shape;
            shape.scale = new Vector3( 0.15f * _counter,1.5f);
            ParticleSystem.ShapeModule shape2 = _aoe.transform.GetChild(0).GetComponent<ParticleSystem>().shape;
            shape2.scale = new Vector3( 0.15f * _counter,1.5f);
            _aoe.GetComponent<BoxCollider2D>().size = new Vector3( 0.15f * _counter,1.5f);
            _aoe.transform.position += (User.Actions.IsFlipped ? Vector3.left : Vector3.right) * 0.1f;
            GameController.Instance.WaitAndRunMethod(0.02f / User.HeavyAttackSpeed.Current, ExtendWave);
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(40 * User.MagicStagger.Current / 100, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}