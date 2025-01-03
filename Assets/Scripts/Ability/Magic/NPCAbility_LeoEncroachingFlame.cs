using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_LeoEncroachingFlame : Ability {

    public static new bool DoesNotRequireTarget = true;
    public static float Cooldown = 35;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public AreaOfEffect aoe;
    private int _counter = 0;

    public NPCAbility_LeoEncroachingFlame(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 200, Constants.DamageType.Magic) {Knockback = -1000});
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 1f;
        CanBeInterruptedByFlinching = false;
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        aoe = Utils.CreateAreaOfEffect(new(this), "LeoEncroachingFlame");
        aoe.transform.parent.position = Vector2.zero;
    }

    public override void CallAbilityEvent1()
    {
        DecreaseRadius();
        aoe.DealingDamage = true;
    }

    public void DecreaseRadius() {
        if(_counter < 100) {
            if(_counter % 20 == 0) {
                Utils.PlaySoundEffect(User.AudioSource, "Fire/Fire6", 0.05f);
            }
            _counter++;
            ParticleSystem.ShapeModule shape = aoe.GetComponentInParent<ParticleSystem>().shape;
            shape.radius = 4.5f - 0.045f * _counter;
            aoe.transform.localScale = new Vector3(1 - 0.01f * _counter, 1 - 0.01f * _counter);
            GameController.Instance.WaitAndRunMethod(0.02f / User.MagicAttackSpeed.Current, DecreaseRadius);
        }
        else if(aoe != null && aoe.gameObject.IsDestroyed() == false) {
            aoe.transform.parent.GetComponent<TemporaryObject>().MakeObjectDisappear(0.1f);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_Burn(120 * User.MagicStagger.Current / 100, new(this)));
    }
}