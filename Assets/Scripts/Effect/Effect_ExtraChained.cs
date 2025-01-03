using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ExtraChained : Effect
{
    public float ExtraChainedAmount = 0;

    public Effect_ExtraChained(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
    }

    
    public override void OnEffectValueChanged()
    {
        ExtraChainedAmount = LinearEffectValue * 0.25f;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(ExtraChainedAmount, 1) };
    }
}
