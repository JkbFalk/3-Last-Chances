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
        Type = EffectType.Buff;
        _initialDecayingAmount = damage_increase;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackDecayingAmount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void ExtraBehaviourOnDecayingAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(DecayingAmount, 0);
        StackingEffectIntensityLevel = DecayingAmount < 20 ? 1 : DecayingAmount < 50 ? 2 : 3;
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
    }


    public override void OnInvokeHitDealt(DamageInstance damage) {
        if(damage?.SourceOfDamage?.User == TargetOfEffect && ((damage?.SourceOfDamage?.User is Player && (damage.SourceOfDamage.Is(Ability.Property.Riposte) || damage.SourceOfDamage.Is(Ability.Property.Counter))) || (damage?.SourceOfDamage?.User is not Player && (damage.SourceOfDamage.Is(Ability.Property.Counter) || damage.SourceOfDamage.Is(Ability.Property.Unstoppable))))) {
            damage.InjuryDealtPercentageModifier += DecayingAmount;
            damage.StaggerDealtPercentageModifier += DecayingAmount;
            base.OnInvokeHitDealt(damage);
        }
    }
}
