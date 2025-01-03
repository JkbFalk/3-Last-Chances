using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ExtraVoid : Effect
{
    public float ExtraVoidAmount = 0;

    public Effect_ExtraVoid(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
    }

    
    public override void OnEffectValueChanged()
    {
        ExtraVoidAmount = LinearEffectValue * 0.25f;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(ExtraVoidAmount, 1) };
    }
}
