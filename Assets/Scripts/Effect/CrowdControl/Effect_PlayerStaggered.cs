using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        TargetOfEffect.Energy.Current = 0;
        TargetOfEffect.MovementSpeed.AddPercentageModifier(this, -45);
        foreach(Effect e in TargetOfEffect.CurrentEffects.ToList()) {
            if(e.Type == EffectType.Buff && e.IsRemovable) {
                e.EndThisEffect();
            }
        }
        TargetOfEffect.IsStaggered = true;

    }

    public override void OnEnd() {
        base.OnEnd();
        TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggerColor;
        TargetOfEffect.StaggerBar.Current = 0;
        TargetOfEffect.StaggeredRegen = 0;
        TargetOfEffect.MovementSpeed.RemovePercentageModifier(this);
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
