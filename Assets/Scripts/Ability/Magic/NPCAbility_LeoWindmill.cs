using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;

public class NPCAbility_LeoWindmill : Ability {

    public static new bool DoesNotRequireTarget = true;
    public static float Cooldown = 35;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;

    public NPCAbility_LeoWindmill(Unit ability_user) : base(ability_user) {
        AddCustomSound("Appear", "Fire/Fire2", 0.8f);
        AddCustomSound("Waving", "Fire/Fire8", 0.5f);
        DamageSources.Add(new DamageSource(100, 100, Constants.DamageType.Magic) {KnockbackInMeters = 2f});
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 1f;
        Properties.Add(Property.ImmuneToFlinch);
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "LeoWindmill");
        _aoe.GetComponentInParent<IKManager2D>().transform.SetParent(User.transform);
        _aoe.GetComponentInParent<IKManager2D>().transform.localPosition = Vector2.zero;
        _aoe.GetComponentInParent<IKManager2D>().gameObject.name = "AreaOfEffect_LeoWindmill";
        User.Animator.Rebind();
        GameController.Instance.WaitAndRunMethod(0.01f, RestartAnimation);
    }

    public void RestartAnimation() {
        User.PlayAnimation("LeoWindmill", 0, 0.071f);
    }

    public override void CallAbilityEvent2()
    {
        ResetPotentialTargets();
    }

    public override void CallAbilityEvent3()
    {
        for(int i = 0; i < 10; i++) {
            GameController.Instance.WaitAndRunMethod(0.05f * i, FadeOutWindmill, 90 - 10 * i);
        }
        foreach(AreaOfEffect aoe in _aoe.GetComponentInParent<IKManager2D>().GetComponentsInChildren<AreaOfEffect>()) {
            aoe.DealingDamage = false;
        }
    }

    public void FadeOutWindmill(int alpha) {
        if(alpha == 0 && _aoe != null && _aoe!= null) {
            _aoe.GetComponentInParent<IKManager2D>().GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
        else {
            foreach(ParticleSystemRenderer ps in _aoe.GetComponentInParent<IKManager2D>().GetComponentsInChildren<ParticleSystemRenderer>()) {
                ps.material.SetFloat("_Alpha", alpha / 100f);
            }
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_aoe != null && _aoe.gameObject!= null) {
            _aoe.MakeObjectDisappear();
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        damage.TargetOfDamage.AddEffect(new Effect_Burn(20 * User.MagicStagger.Current / 100, new(this)));
    }
}