using UnityEngine;
using System.Linq;

public class Effect_Stun : Effect_HardCrowdControl
{

    public Effect_Stun(SourceOfEffect source_of_effect) : base(source_of_effect) {
        DisplayEffectIndicator = true;
        DescriptionLabel = "Effect_Stun_Explanation";
    }
}