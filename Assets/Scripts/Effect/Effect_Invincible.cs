using UnityEngine;

public class Effect_Invincible : Effect {

    public Effect_Invincible(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDuration;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage == TargetOfEffect) {
            damage.Injury = 0;
            damage.Stagger = 0;
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }
}