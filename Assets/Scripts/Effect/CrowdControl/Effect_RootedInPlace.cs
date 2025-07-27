using UnityEngine;
using UnityEngine.AI;

public class Effect_RootedInPlace : Effect {

    public Effect_RootedInPlace(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Neutral;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDuration;
    }

    public override void OnStart()
    {
        base.OnStart();
        TargetOfEffect.Rigidbody2D.mass = 1000;
        if (TargetOfEffect?.UnitAI?.NavMeshAgent != null && TargetOfEffect is not Player)
        {
            TargetOfEffect.UnitAI.NavMeshAgent.enabled = false;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        TargetOfEffect.Rigidbody2D.mass = 1;
        if (TargetOfEffect?.UnitAI?.NavMeshAgent != null && TargetOfEffect is not Player)
        {
            TargetOfEffect.UnitAI.NavMeshAgent.enabled = true;
        }
    }
}