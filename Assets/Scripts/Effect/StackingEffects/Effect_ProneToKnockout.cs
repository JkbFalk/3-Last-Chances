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
        _initialDecayingAmount = amount;
        ShowsInUI = true;
        DefaultDecaySpeed = 0.1f;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackDecayingAmount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void ExtraBehaviourOnDecayingAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(DecayingAmount, 0);
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = 15;
    }
}
