using UnityEngine;

public class Effect_IncreaseRiposteDamage : Effect {

    public float Amount;

    public Effect_IncreaseRiposteDamage(float amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Amount = amount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnInvokeHitDealt(Damage damage) {
        if(damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsRiposte) {
            damage.ExtraInjuryDealtPercentage += Amount;
            damage.ExtraStaggerDealtPercentage += Amount;
            base.OnInvokeHitDealt(damage);
        }
    }
}