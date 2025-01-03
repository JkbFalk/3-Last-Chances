using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Sharp : Effect
{
    public GameObject Vfx;
    public int SharpLevel = 0;
    public Effect_Sharp(float damage_increase, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialDecayingAmount = damage_increase;
        DisplayEffectIndicator = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        Listeners.Add(EventManager.HitDealt);
        DescriptionLabel = "Effect_Sharp_Explanation";
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
    }


    public override void OnInvokeHitDealt(Damage damage) {
        if(damage?.SourceOfDamage?.User == TargetOfEffect && ((damage?.SourceOfDamage?.User is Player && (damage.SourceOfDamage.IsRiposte || damage.SourceOfDamage.IsCounter)) || (damage?.SourceOfDamage?.User is not Player && (damage.SourceOfDamage.IsCounterable || damage.SourceOfDamage.IsUncounterable)))) {
            damage.ExtraInjuryDealtPercentage += DecayingAmount;
            damage.ExtraStaggerDealtPercentage += DecayingAmount;
            base.OnInvokeHitDealt(damage);
        }
    }
}
