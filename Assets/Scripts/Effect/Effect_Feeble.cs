using System.Linq;
using UnityEngine;

public class Effect_Feeble : Effect {

    public float DecreasedDamageDonePercentage = 0;

    public Effect_Feeble(float increase_amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Debuff;
        DecreasedDamageDonePercentage = increase_amount;
        DisplayEffectIndicator = true;
        Listeners.Add(EventManager.HitDealt);
        EffectIndicatorText = "-" + increase_amount.ToString() + "%";
        BaseDuration = 30;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        DescriptionLabel = "Effect_Feeble_Explanation";
    }

    public override void OnInvokeHitDealt(Damage damage) {
        if(damage.SourceOfDamage.User == TargetOfEffect) {
            damage.DamageDealtMultiplier -= DecreasedDamageDonePercentage / 100;
            base.OnInvokeHitDealt(damage);
        }
    }
}