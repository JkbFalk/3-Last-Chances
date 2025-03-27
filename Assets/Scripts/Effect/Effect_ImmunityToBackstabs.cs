using UnityEngine;

public class Effect_ImmunityToBackstabs : Effect {

    public Effect_ImmunityToBackstabs(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        ShowsInUI = true;
    }
}