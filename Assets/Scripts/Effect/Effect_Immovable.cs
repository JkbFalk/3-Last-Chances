using UnityEngine;

public class Effect_Immovable : Effect {

    public Effect_Immovable(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.ExtendDuration;
    }
}