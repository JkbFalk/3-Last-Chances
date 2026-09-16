using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEffect : MonoBehaviour
{
    public string EffectTypeName;
    void Start()
    {
        Unit unit = GetComponent<Unit>();
        Type effectType = AbilityTypeRegistry.GetRequiredByName("Effect_" + EffectTypeName.Replace("Effect_", ""));
        if (effectType == null)
        {
            return;
        }
        Effect effect = (Effect)Activator.CreateInstance(effectType, new object[] {new SourceOfEffect(GetComponent<Unit>())});
        unit.AddEffect(effect);
        if(effect.AutoPlayEffectAnimation) {
            unit.EffectAnimationBeingPlayed = effect;
            unit.PlayAnimation(effect.NameOfAnimationToAutoPlay == null ? effect.GetType().ToString() : effect.NameOfAnimationToAutoPlay, effect.InstantlyTransitionIntoAnimation ? 0 : 0.1f);
        }
    }
}
