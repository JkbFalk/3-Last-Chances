using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Bleed : Effect
{
    private int counter = 0;
    public Effect_Bleed(float Bleed_per_second, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        ShowsInUI = true;
        _initialDecayingAmount = Bleed_per_second;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackDecayingAmount;
    }

    public override void ExtraBehaviourOnDecayingAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(DecayingAmount, 0);
        StackingEffectIntensityLevel = DecayingAmount < TargetOfEffect.Health.Maximum * 0.1f ? 1 : DecayingAmount < TargetOfEffect.Health.Maximum * 0.25f ? 2 : 3;
    }

    public override void OnStart() {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
    }

    public override void OnFixedUpdate()
    {
        if(EffectEnded) {
            return;
        }
        base.OnFixedUpdate();
        counter++;
        if(counter >= 50 && TargetOfEffect != null) {
            counter = 0;
            new DamageInstance(TargetOfEffect, SourceOfEffect.SourceAbility, null) {
                Properties = new List<DamageInstance.DamageProperty> { DamageInstance.DamageProperty.Bleed, DamageInstance.DamageProperty.CannotKill, DamageInstance.DamageProperty.DamageOverTime }, 
                Injury = DecayingAmount,
                PlaySoundOnEnemyHit = false
            }.CalculateAndApplyDamage();
        }
    }
}
