using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Constants;

public class Effect_DisableEffectDecay : Effect
{
    public Type AffectedEffectType;
    public Effect_DisableEffectDecay(Type effect_type, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        AffectedEffectType = effect_type;
        Type = EffectType.Buff;
    }
}
