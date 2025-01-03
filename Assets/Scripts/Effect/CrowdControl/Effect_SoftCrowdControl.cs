using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_SoftCrowdControl : Effect
{
    public Effect_SoftCrowdControl(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
    }
}
