using UnityEngine;

public class Effect_Unkillable : Effect {

    public Effect_Unkillable(SourceOfEffect source_of_effect, bool display_alpha_effect = false, bool generate_energy_on_dodge = false) : base(source_of_effect) {
        ShowsInUI = true;
        Type = EffectType.Buff;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.ExtendDuration;
    }
}