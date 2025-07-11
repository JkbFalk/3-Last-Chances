using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Incision : Effect
{
    private int counter = 0;
    public Effect_Incision(float incision_per_second, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        ShowsInUI = true;
        _initialDecayingAmount = incision_per_second;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        UIText = Utils.GetFormattedFloat(DecayingAmount, 0);
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
            new Damage(TargetOfEffect, SourceOfEffect.SourceAbility, null) {
                Properties = new List<Damage.DamageProperty> { Damage.DamageProperty.Incision, Damage.DamageProperty.CannotKill, Damage.DamageProperty.DamageOverTime }, 
                Injury = DecayingAmount,
                PlaySoundOnEnemyHit = false
            }.CalculateAndApplyDamage();
        }
    }
}
