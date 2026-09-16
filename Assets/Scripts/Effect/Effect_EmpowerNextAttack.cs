using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_EmpowerNextAttack : Effect
{
    public Func<DamageInstance, Effect_EmpowerNextAttack, bool> ConditionCheck;
    public Effect_EmpowerNextAttack(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        ShowsInUI = true;
        PathToUIGraphic = "UI/Damage";
        Listeners.AddRange(new List<UnityEngine.Events.UnityEventBase> { EventManager.HitDealt });
    }

    public override void OnStart()
    {
        base.OnStart();
        if (PercentageAmount > 0)
        {
            UIText = "+" + PercentageAmount + "%";
        }
        else if(FlatAmount > 0)
        {
            UIText = "+" + FlatAmount;
        }
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && (ConditionCheck == null || ConditionCheck.Invoke(damage, this)))
        {
            base.OnInvokeHitDealt(damage);
            damage.DamageDealtFlatModifier = FlatAmount;
            damage.DamageDealtPercentageModifier = PercentageAmount;
            EndThisEffect();   
        }
    }
}
