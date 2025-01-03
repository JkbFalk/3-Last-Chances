using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Effect_SoftStaggered : Effect_Staggered {
    private float _staggeredRegen = 0;

    public Effect_SoftStaggered(SourceOfEffect source_of_effect) : base(source_of_effect) {
        CanBeNegatedByImmunityToCrowdControl = false;
        PriorityLevel = 4;
        PlaySoundEffect = true;
        SoundEffectVolume = 0.5f;
    }

    public override void OnStart() {
        base.OnStart();
        if(TargetOfEffect.StaggerBar.HUDFill != null && TargetOfEffect.StaggerBar.HUDFill.IsDestroyed() == false) {
            TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggeredColor;
        }
        _staggeredRegen = TargetOfEffect.StaggerBar.Maximum / BaseDuration;
        TargetOfEffect.StaggeredRegen = _staggeredRegen;
        TargetOfEffect.StaggerBar.Current = TargetOfEffect.StaggerBar.Maximum;
    }

    public override void OnEnd() {
        base.OnEnd();
        if(TargetOfEffect.StaggerBar.HUDFill != null && TargetOfEffect.StaggerBar.HUDFill.IsDestroyed() == false) {
            TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggerColor;
        }
        TargetOfEffect.StaggerBar.Current = 0;
        TargetOfEffect.StaggeredRegen = 0;
        TargetOfEffect.IsStaggered = false;
    }
}