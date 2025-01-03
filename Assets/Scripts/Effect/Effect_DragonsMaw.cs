using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_DragonsMaw : Effect { 

    public Effect_ChangeCompositeStat InjuryBuff;
    public Effect_ChangeCompositeStat StaggerBuff;
    public float BuffAmount = 0;

    public Effect_DragonsMaw(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        DisplayEffectIndicator = true;
        EffectGraphic = Utils.LoadSpriteFromMultiple("Heavy Icons", "Heavy Icons_9");
    }

    public override void OnEffectValueChanged()
    {
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(BuffAmount) };
    }
}
