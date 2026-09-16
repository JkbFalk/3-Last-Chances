using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Sleep : Effect_HardCrowdControl
{
    public Effect_Sleep(SourceOfEffect source_of_effect) : base(source_of_effect) {
        ShowsInUI = true;
        Listeners.Add(EventManager.DamageDealt);
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.ExtendDuration;
    }

    public override void OnStart()
    {
        base.OnStart();
        TargetOfEffect.Health.AddPercentageRegeneration(this, 1);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        TargetOfEffect.Health.RemovePercentageRegeneration(this, 1);
        TargetOfEffect.PlayAnimation("WobblyGetUp");
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        base.OnInvokeDamageDealt(damage);
        if(damage.TargetOfDamage == TargetOfEffect && (damage.InjuryDealt > 0)) {
            EndThisEffect();
        }
    }
}
