using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Blackfire : Effect
{
    public float TotalHealthScaling = 0;
    public float ExtraHealthScaling = 0;
    public float TotalStaggerScaling = 0;
    public float ExtraStaggerScaling = 0;
    public Effect_Blackfire(float total_health_scaling,float total_stagger_scaling, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        TotalHealthScaling = total_health_scaling;
        TotalStaggerScaling = total_stagger_scaling;
        DisplayEffectIndicator = true;
        DescriptionLabel = "Effect_Blackfire_Explanation";
    }
}
