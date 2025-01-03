using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_ProtectFromFlinchingOnce : Effect
{
    public float Cooldown = 15;

    public Effect_ProtectFromFlinchingOnce(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
    }

    public override void OnEffectValueChanged()
    {
        Cooldown = LinearEffectValue == 10 ? 20 : LinearEffectValue == 20 ? 10 : LinearEffectValue == 40 ? 5 : 0;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(Cooldown) };
    }
}
