public class Effect_Unstoppable : Effect {

    public Effect_Unstoppable(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        DisplayEffectIndicator = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDuration;
        DescriptionLabel = "Effect_Unstoppable_Explanation";
    }
}