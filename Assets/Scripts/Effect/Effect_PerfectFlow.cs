using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public class Effect_PerfectFlow : Effect {

    public Effect_PerfectFlow(SourceOfEffect source_of_effect) : base(source_of_effect) {
        DisplayEffectIndicator = true;
        Type = EffectType.Buff;
        Listeners.Add(EventManager.HitDealt);
    }

}