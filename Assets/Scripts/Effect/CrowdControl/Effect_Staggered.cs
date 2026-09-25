// FILE: Assets\Scripts\Effect\CrowdControl\Effect_Staggered.cs
using System.Linq;
using UnityEngine;

public class Effect_Staggered : Effect_HardCrowdControl
{
    public Effect_Staggered(SourceOfEffect source_of_effect) : base(source_of_effect) 
    {
        CanBeNegatedByImmunityToCrowdControl = false;
        PriorityLevel = 9;
        PlaySoundEffect = true;
        SoundEffectName = "Effect_HardStaggered";
        SoundEffectVolume = 0.5f;
    }

    public override void OnStart() 
    {
        // Cancel all counter/riposte animations since they are now fully broken
        foreach (Effect e in TargetOfEffect.CurrentEffects.ToList()) {
            if (e is Effect_Riposted || e is Effect_RiposteCountered || e is Effect_RollCountered || e is Effect_BackstepCountered || e is Effect_Flinching) {
                e.EndThisEffect();
            }
        }

        base.OnStart();

        // Hard set the duration to 10 seconds
        BaseDuration = 10f;
        RemainingDuration = 10f;
        
        if (TargetOfEffect.StaggerBar.HUDFill != null && !TargetOfEffect.StaggerBar.HUDFill== null) {
            TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggeredColor;
        }

        // Lock the UI bar at Maximum
        TargetOfEffect.StaggerBar.Current = TargetOfEffect.StaggerBar.Maximum;
        TargetOfEffect.StaggeredRegen = TargetOfEffect.StaggerBar.Maximum / BaseDuration;

        // Strip removable buffs as a penalty for getting Staggered
        foreach (Effect e in TargetOfEffect.CurrentEffects.ToList()) {
            if (e.Type == EffectType.Buff && e.IsRemovable) {
                e.EndThisEffect();
            }
        }
    }

    public override void OnEnd() 
    {
        base.OnEnd();
        if (TargetOfEffect.StaggerBar.HUDFill != null && !TargetOfEffect.StaggerBar.HUDFill== null) {
            TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggerColor;
        }

        // Reset the Stagger Bar to 0 so the cycle can start over
        TargetOfEffect.StaggerBar.Current = 0;
        TargetOfEffect.StaggeredRegen = 0;
        TargetOfEffect.IsStaggered = false;
    }
}