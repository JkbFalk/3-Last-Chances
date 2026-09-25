using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_ProneToKnockout : Effect
{
    public GameObject Vfx;
    public int ProneLevel = 0;
    public Effect_ProneToKnockout(float amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialAmount = amount;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(Amount, 0);
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = 15;
    }
}
