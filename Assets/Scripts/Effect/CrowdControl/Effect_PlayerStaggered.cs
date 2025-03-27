using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_PlayerStaggered : Effect_SoftCrowdControl
{
    private float _staggeredRegen = 0;

    public Effect_PlayerStaggered(SourceOfEffect source_of_effect) : base(source_of_effect) {
        AutoPlayEffectAnimation = false;
        CanBeNegatedByImmunityToCrowdControl = true;
        PlaySoundEffect = true;
        SoundEffectName = "Effect_HardStaggered";
        SoundEffectVolume = 0.5f;
    }
    public Effect_ChangeCompositeStat DamageDebuff;
    public Effect_ChangeCompositeStat AttackSpeedDebuff;
    public override void OnStart() {
        base.OnStart();
        TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggeredColor;
        BaseDuration = Constants.DEFAULT_PLAYER_STAGGERED_DURATION;
        _staggeredRegen = TargetOfEffect.StaggerBar.Maximum / BaseDuration;
        TargetOfEffect.StaggeredRegen = _staggeredRegen;
        TargetOfEffect.StaggerBar.Current = TargetOfEffect.StaggerBar.Maximum;
        TargetOfEffect.MovementSpeed.AddPercentageModifier(this, -30);
        DamageDebuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, SourceOfEffect) {PercentageAmount = -50};
        AttackSpeedDebuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect) {PercentageAmount = -30};
        TargetOfEffect.AddEffect(new Effect_SoftStaggered(SourceOfEffect), Constants.DEFAULT_SOFT_STAGGERED_DURATION);
        TargetOfEffect.AddEffect(DamageDebuff);
        TargetOfEffect.AddEffect(AttackSpeedDebuff);
        TargetOfEffect.Control.AddPercentageModifier(this, -50);
        TargetOfEffect.Tenacity.AddPercentageModifier(this, -50);
        TargetOfEffect.IsStaggered = true;
    }

    public override void OnEnd() {
        base.OnEnd();
        TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggerColor;
        TargetOfEffect.StaggerBar.Current = 0;
        TargetOfEffect.StaggeredRegen = 0;
        TargetOfEffect.MovementSpeed.RemovePercentageModifier(this);
        TargetOfEffect.Control.RemovePercentageModifier(this);
        TargetOfEffect.Tenacity.RemovePercentageModifier(this);
        TargetOfEffect.EndEffect(DamageDebuff);
        TargetOfEffect.EndEffect(AttackSpeedDebuff);
        TargetOfEffect.IsStaggered = false;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if(TargetOfEffect.StaggerBar.Current <= 0) {
            EndThisEffect();
        }
    }
}
