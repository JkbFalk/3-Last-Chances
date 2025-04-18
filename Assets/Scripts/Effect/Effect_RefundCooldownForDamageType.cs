using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Constants;

public class Effect_RefundCooldownForDamageType : Effect
{
    public float RefundAmount = 0;
    public Constants.DamageType DamageType;
    public Effect_RefundCooldownForDamageType(Constants.DamageType type, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        DamageType = type;
    }

    public override void OnStart()
    {
        base.OnStart();
        EventManager.CooldownAdded.AddListener(Activate);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        EventManager.CooldownAdded.RemoveListener(Activate);
    }


    public void Activate(Cooldown cooldown)
    {
        if(!cooldown.Type.IsSubclassOf(typeof(Technique))) {
            return;
        }
        FieldInfo fInfo = cooldown.Type.GetField("TechniqueDamageType", BindingFlags.Public | BindingFlags.Static);
        if(fInfo != null && ((Constants.DamageType)fInfo.GetValue(null) == DamageType)) {
            cooldown.RemainingDuration *= 1 - RefundAmount / 100;
        }
        else {
            PropertyInfo pInfo = cooldown.Type.GetProperty("TechniqueDamageType", BindingFlags.Public | BindingFlags.Static);
            if(pInfo != null && ((Constants.DamageType)fInfo.GetValue(null) == DamageType)) {
                cooldown.RemainingDuration *= 1 - RefundAmount / 100;
            }
        }
        
    }
}
