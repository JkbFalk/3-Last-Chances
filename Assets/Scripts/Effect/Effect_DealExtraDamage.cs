using System;
using UnityEngine;

public class Effect_DealExtraDamage : Effect {
    //Value = 10 dmg or stagger / 100p
    public float FlatDamageAmount = 0;
    public float FlatStaggerAmount = 0;
    public int ChargesRemaining = 0;
    public Type RequiredClassOrSubclass;

    public Effect_DealExtraDamage(float flat_damage_amount, float flat_stagger_amount, int charges, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        FlatDamageAmount = flat_damage_amount;
        FlatStaggerAmount = flat_stagger_amount;
        ChargesRemaining = charges;
        TriggersOncePerAbility = true;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect || (RequiredClassOrSubclass != null && damage.SourceOfDamage.GetType() != RequiredClassOrSubclass && damage.SourceOfDamage.GetType().IsSubclassOf(RequiredClassOrSubclass) == false))
        {
            return;
        }
        damage.Injury += FlatDamageAmount;
        damage.Stagger += FlatStaggerAmount;
        if(ChargesRemaining > 0)
        {
            ChargesRemaining--;
            if(ChargesRemaining == 0)
            {
                EndThisEffect();
            }
        }
        base.OnInvokeHitDealt(damage);
    }
}