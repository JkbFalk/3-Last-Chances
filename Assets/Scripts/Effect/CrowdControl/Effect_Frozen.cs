using UnityEngine;
using System.Linq;

public class Effect_Frozen : Effect_HardCrowdControl
{

    private GameObject _vfx;
    public Effect_Frozen(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        AutoPlayEffectAnimation = false;
        SpecialSkinColorDuringHardCrowdControl = new Color(0, 0.5f, 1);
        AdditionalEffectsAffectingTargetDuringEffect = new System.Collections.Generic.List<Effect> { new Effect_RootedInPlace(SourceOfEffect) };
        ShowsInUI = true;
        Listeners.Add(EventManager.HitDealt);
        PriorityLevel = 10;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.ExtendDuration;
    }

    public override void OnStart()
    {
        TargetOfEffect.Actions.EndCurrentAbility();
        base.OnStart();
        TargetOfEffect.Animator.enabled = false;
        GameObject vfx = Utils.CreateVisualEffect(SourceOfEffect, "Frozen", TargetOfEffect.transform.position.x, TargetOfEffect.transform.position.y);
        GameObject _vfx = Utils.CreateVisualEffect(SourceOfEffect, "Frozen_Continuous", TargetOfEffect.transform.position.x, TargetOfEffect.transform.position.y + 0.5f);
        _vfx.GetComponent<DestroyGameObjectAfterGivenTime>().DestroyAfterSeconds = RemainingDuration;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        TargetOfEffect.Animator.enabled = true;
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if(damage.TargetOfDamage == TargetOfEffect && BaseDuration - RemainingDuration > 0.25f) {
            damage.InjuryDealtPercentageModifier += 200;
            damage.StaggerDealtPercentageModifier += 200;
            damage.TargetOfDamage.AddEffect(new Effect_Flinching(new(damage.SourceOfDamage)), Constants.DEFAULT_FLINCHING_DURATION);
            Utils.PlaySoundEffect(damage.TargetOfDamage.AudioSource, "Effect/Effect_Frozen", 0.9f);
            Utils.CreateVisualEffect(SourceOfEffect, "FrozenDestroy", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
            EndThisEffect();
            base.OnInvokeHitDealt(damage);
        }
    }
}