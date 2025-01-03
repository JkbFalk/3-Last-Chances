using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_HealFromMovement : Effect {
    public float HealAmount = 0;
    public Vector2 PreviousPosition;

    public Effect_HealFromMovement(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if(PreviousPosition == null) {
            PreviousPosition = TargetOfEffect.transform.position;
            return;
        }
        float distance = Vector2.Distance(PreviousPosition, TargetOfEffect.transform.position);
        if(distance == 0) {
            return;
        }
        TargetOfEffect.Health.Current += TargetOfEffect.Health.Maximum / 100 * Vector2.Distance(PreviousPosition, TargetOfEffect.transform.position) * HealAmount / 10;
        PreviousPosition = TargetOfEffect.transform.position;
    }
}
