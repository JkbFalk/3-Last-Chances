using UnityEngine;

public class Effect_ImmunityToFreeze : Effect {

    public Effect_ImmunityToFreeze(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        DisplayEffectIndicator = true;
        DescriptionLabel = "Effect_ImmunityToFreeze_Explanation";
    }
}