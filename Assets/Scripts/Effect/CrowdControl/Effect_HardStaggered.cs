using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Effect_HardStaggered : Effect_Staggered {
    private float _staggeredRegen = 0;
    private float _regenMultiplier = 1;

    public Effect_HardStaggered(SourceOfEffect source_of_effect) : base(source_of_effect) {
        CanBeNegatedByImmunityToCrowdControl = false;
        PriorityLevel = 2;
        PlaySoundEffect = true;
        SoundEffectVolume = 0.5f;
    }

    public override void OnStart() {
        base.OnStart();
        _staggeredRegen = TargetOfEffect.StaggerBar.Maximum / BaseDuration;
        if(TargetOfEffect.StaggerBar.HUDFill != null && TargetOfEffect.StaggerBar.HUDFill.IsDestroyed() == false) {
            TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggeredColor;
        }
        TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggeredColor;
        TargetOfEffect.StaggeredRegen = _staggeredRegen;
        TargetOfEffect.StaggerBar.Current = TargetOfEffect.StaggerBar.Maximum;
        foreach(Effect e in TargetOfEffect.CurrentEffects.ToList()) {
            if(e.Type == EffectType.Buff && e.IsRemovable) {
                e.EndThisEffect();
            }
        }
        EventManager.DamageDealt.AddListener(CheckIfTookHealthDamage);
    }

    public void CheckIfTookHealthDamage(Damage damage) {
        if(damage.TargetOfDamage == TargetOfEffect && damage.InjuryDealt > 0) {
            _regenMultiplier += damage.InjuryDealt * 10 / damage.TargetOfDamage.Health.Maximum;
            damage.TargetOfDamage.StaggeredRegen = TargetOfEffect.StaggerBar.Maximum / BaseDuration * _regenMultiplier;
        }
    }

    public override void OnEnd() {
        base.OnEnd();
        EventManager.DamageDealt.RemoveListener(CheckIfTookHealthDamage);
        if(TargetOfEffect.StaggerBar.HUDFill != null && TargetOfEffect.StaggerBar.HUDFill.IsDestroyed() == false) {
            TargetOfEffect.StaggerBar.HUDFill.color = Colors.StaggerColor;
        }
        TargetOfEffect.StaggerBar.Maximum = TargetOfEffect.StaggerBars[0]* (TargetOfEffect.IsHostile ? Damage.GlobalEnemySurvivabilityModifier : 1);
        TargetOfEffect.StaggerBar.Current = 0;
        TargetOfEffect.StaggeredRegen = 0;
        TargetOfEffect.IsStaggered = false;
        TargetOfEffect.CurrentStaggerBars = TargetOfEffect.StaggerBars.Count;
    }
}