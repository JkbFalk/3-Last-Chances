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
        _initialAmount = Bleed_per_second;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(Amount, 0);
        StackingEffectIntensityLevel = Amount < TargetOfEffect.Health.Maximum * 0.1f ? 1 : Amount < TargetOfEffect.Health.Maximum * 0.25f ? 2 : 3;
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
                Injury = Amount,
                PlaySoundOnEnemyHit = false
            }.CalculateAndApplyDamage();
        }
    }
}
