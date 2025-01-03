using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Bleed : Effect
{
    private int counter = 0;
    public Effect_Bleed(float bleed_per_second, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        DisplayEffectIndicator = true;
        _initialDecayingAmount = bleed_per_second;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        DescriptionLabel = "Effect_Bleed_Explanation";
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
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
            Damage damage = new Damage(TargetOfEffect, SourceOfEffect.SourceAbility, null) { IsDamageOverTime = true, Properites = new List<Damage.DamageProperty> { Damage.DamageProperty.Bleed, Damage.DamageProperty.CannotKill }, Injury = DecayingAmount}.DisableSoundOnEnemyHit().CalculateDamage();
        }
    }
}
