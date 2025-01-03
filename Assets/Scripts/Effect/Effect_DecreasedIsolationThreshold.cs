using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_DecreasedIsolationThreshold : Effect
{
    public float ThresholdMetersDecrease = 0;
    
    public Effect_DecreasedIsolationThreshold(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
    }
}
