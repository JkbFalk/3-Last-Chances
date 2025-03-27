public class Effect_Unstunnable : Effect {

    public Effect_Unstunnable(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDuration;
    }
}