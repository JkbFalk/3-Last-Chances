using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Effect_Plunderer : Effect
{
    public float Amount;
    public Effect_Plunderer(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AbilityUsed);
    }

    public override void OnInvokeAbilityUsed(Ability ability) {
        
    }
}
