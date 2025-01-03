using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Chained : Effect
{
    private int counter = 0;
    public Effect_Chained(float chained_per_second, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Effect_ExtraChained extraChained = ((Effect_ExtraChained)Player.Instance.GetEffect(typeof(Effect_ExtraChained)));
        Type = EffectType.Debuff;
        _initialDecayingAmount = extraChained != null ? chained_per_second + extraChained.ExtraChainedAmount : chained_per_second;
        DisplayEffectIndicator = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        Listeners.Add(EventManager.HitDealt);
        DescriptionLabel = "Effect_Chained_Explanation";
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
    }

    public override void OnStart() {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
    }

    public override void OnInvokeHitDealt(Damage damage) {
        if(damage.SourceOfDamage.User == TargetOfEffect) {
            damage.Injury -= DecayingAmount;
            damage.Stagger -= DecayingAmount;
            base.OnInvokeHitDealt(damage);
        }
    }
}
