using System.Collections.Generic;
using UnityEngine;

public class Effect_DealExtraDamageAfterTakingDamage : Effect {

    public float FlatDamageAmount = 0;
    public float FlatStaggerAmount = 0;
    public float Duration = 3;
    public Effect_DealExtraDamage ExtraDamageEffect;

    public Effect_DealExtraDamageAfterTakingDamage(float base_health_damage, float base_stagger_damage, float duration, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        FlatDamageAmount = base_health_damage;
        FlatStaggerAmount = base_stagger_damage;
        Duration = duration;
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnEffectValueChanged()
    {
        FlatDamageAmount *= LinearEffectValue;
        FlatStaggerAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Duration.ToString(), Utils.GetFormattedFloat(FlatDamageAmount), Utils.GetFormattedFloat(FlatStaggerAmount) };
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        if(ExtraDamageEffect == null || ExtraDamageEffect.EffectEnded)
        {
            ExtraDamageEffect = new Effect_DealExtraDamage(FlatDamageAmount, FlatStaggerAmount, 1, SourceOfEffect) {DisplayEffectIndicator = true, EffectGraphic = Utils.LoadSpriteFromMultiple("Heavy Icons", "Heavy Icons_1")};
            damage.TargetOfDamage.AddEffect(ExtraDamageEffect, Duration);
        }
        base.OnInvokeDamageDealt(damage);
    }
}