using UnityEngine;

public class Effect_ImmunityToStealthAttack : Effect {

    public Effect_ImmunityToStealthAttack(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        DisplayEffectIndicator = true;
        DescriptionLabel = "Effect_ImmunityToStealthAttack_Explanation";
    }
}