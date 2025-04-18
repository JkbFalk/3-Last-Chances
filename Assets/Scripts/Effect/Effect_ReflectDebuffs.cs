using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Effect_ReflectDebuffs : Effect
{
    public float ReflectionEffectivness;

    public Effect_ReflectDebuffs(float reflect_effectivness, SourceOfEffect source_of_effect) : base(source_of_effect) {
        ReflectionEffectivness = reflect_effectivness;
        Type = EffectType.Buff;
        Listeners.Add(EventManager.EffectStarted);
        TriggersOncePerAbility = true;
    }

    /*public override void OnEffectValueChanged()
    {
        DamageReductionGainedPerDebuff *= PowerBudget;
        DamageGainedPerDebuff *= PowerBudget;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(DamageGainedPerDebuff), Utils.GetFormattedFloat(DamageReductionGainedPerDebuff), MaxDebuffs.ToString()};
    }


    public override void OnInvokeEffectStarted(Effect effect)
    {
        if(effect.TargetOfEffect != Player.Instance || effect.Type != EffectType.Debuff) {
            return;
        }
        base.OnInvokeEffectStarted(effect);
        Activate();
    }*/
}
