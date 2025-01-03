using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Constants;

public class Effect_RefundCooldownForDamageCategory : Effect
{
    public float RefundAmount = 0;
    public Constants.DamageType DamageCategory;
    public Effect_RefundCooldownForDamageCategory(Constants.DamageType category, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        DamageCategory = category;
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

    public override void OnEffectValueChanged()
    {
        RefundAmount = 0.5f * LinearEffectValue;
        DescriptionParameters = new List<string> { "{WeaponCategory_" + DamageCategory.ToString() + "}", ((int)RefundAmount).ToString()};
    }

    public void Activate(Cooldown cooldown)
    {
        if(!cooldown.Type.IsSubclassOf(typeof(Technique))) {
            return;
        }
        FieldInfo fInfo = cooldown.Type.GetField("TechniqueDamageCategory", BindingFlags.Public | BindingFlags.Static);
        if(fInfo != null && ((Constants.DamageType)fInfo.GetValue(null) == DamageCategory)) {
            cooldown.RemainingDuration *= 1 - RefundAmount / 100;
        }
        else {
            PropertyInfo pInfo = cooldown.Type.GetProperty("TechniqueDamageCategory", BindingFlags.Public | BindingFlags.Static);
            if(pInfo != null && ((Constants.DamageType)fInfo.GetValue(null) == DamageCategory)) {
                cooldown.RemainingDuration *= 1 - RefundAmount / 100;
            }
        }
        
    }
}
