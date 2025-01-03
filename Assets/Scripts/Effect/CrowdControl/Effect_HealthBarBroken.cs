using System.Linq;
using UnityEngine;

public class Effect_HealthBarBroken : Effect_HardCrowdControl
{

    public Effect_HealthBarBroken(SourceOfEffect source_of_effect) : base(source_of_effect) {
        CanBeNegatedByImmunityToCrowdControl = false;
        PriorityLevel = 3;
        PlaySoundEffect = true;
    }

    public override void OnStart()
    {
        foreach(Effect e in TargetOfEffect.CurrentEffects.ToList())
        {
            if(e != this && e.Type == EffectType.Debuff && e.IsRemovable)
            {
                TargetOfEffect.EndEffect(e);
            }
        }
        if(TargetOfEffect.Actions.CurrentAbilityBeingPerformed != null)
        {
            TargetOfEffect.Actions.CurrentAbilityBeingPerformed.EndThisAbility();
            TargetOfEffect.PlayAnimation("HealthBarBroken");
        }
    }
}