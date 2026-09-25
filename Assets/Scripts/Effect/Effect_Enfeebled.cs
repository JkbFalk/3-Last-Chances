using System.Linq;
using UnityEngine;

public class Effect_Enfeebled : Effect {

    public float DecreasedDamageDonePercentage = 0;

    public Effect_Enfeebled(float increase_amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Debuff;
        DecreasedDamageDonePercentage = increase_amount;
        ShowsInUI = true;
        Listeners.Add(EventManager.HitDealt);
        UIText = "-" + increase_amount.ToString() + "%";
        BaseDuration = 30;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
    }

    public override void OnInvokeHitDealt(DamageInstance damage) {
        if(damage.SourceOfDamage.User == TargetOfEffect) {
            damage.DamageDealtMultiplier -= DecreasedDamageDonePercentage / 100;
            base.OnInvokeHitDealt(damage);
        }
    }
}