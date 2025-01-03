using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_HardCrowdControl : Effect
{
    public Effect_HardCrowdControl(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        AutoPlayEffectAnimation = true;
    }
}
