using System.Collections.Generic;
using UnityEngine;

public class Effect_Ignis_Paladin_Buff : Effect {
    private Effect_Prone _proneEffect;

    public Effect_Ignis_Paladin_Buff(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        AdditionalEffectsAffectingTargetDuringEffect = new List<Effect> { new Effect_Prone(BaseDuration, SourceOfEffect) };
    }

    public override void OnStart() {
        base.OnStart();
        TargetOfEffect.UnitAI.BaseAggressiveness *= 3;
        TargetOfEffect.HeavyInjury.AddPercentageModifier(this, 30);
        TargetOfEffect.MagicInjury.AddPercentageModifier(this, 30);
        TargetOfEffect.HeavyStagger.AddPercentageModifier(this, 30);
        TargetOfEffect.MagicStagger.AddPercentageModifier(this, 30);
        TargetOfEffect.MovementSpeed.AddPercentageModifier(this, 20);
        TargetOfEffect.HeavyAttackSpeed.AddPercentageModifier(this, 20);
        TargetOfEffect.SpriteRenderers["Heavy"].SpriteRenderer.transform.Find("Heavy Bone/VisualEffect_Weapon_FlameEdge_World").gameObject.SetActive(true);
        ParticleSystem local = TargetOfEffect.SpriteRenderers["Heavy"].SpriteRenderer.transform.Find("Heavy Bone/VisualEffect_Weapon_FlameEdge_Local").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule local_main = local.main;
        local_main.simulationSpeed = 4;
        ParticleSystem.EmissionModule local_emission = local.emission;
        local_emission.rateOverTime = 40;
    }

    public override void OnEnd() {
        base.OnEnd();
        TargetOfEffect.UnitAI.BaseAggressiveness /= 3;
        TargetOfEffect.HeavyInjury.RemovePercentageModifier(this);
        TargetOfEffect.MagicInjury.RemovePercentageModifier(this);
        TargetOfEffect.HeavyStagger.RemovePercentageModifier(this);
        TargetOfEffect.MagicStagger.RemovePercentageModifier(this);
        TargetOfEffect.MovementSpeed.RemovePercentageModifier(this);
        TargetOfEffect.HeavyAttackSpeed.RemovePercentageModifier(this);
        TargetOfEffect.SpriteRenderers["Heavy"].SpriteRenderer.transform.Find("Heavy Bone/VisualEffect_Weapon_FlameEdge_World").gameObject.SetActive(false);
        ParticleSystem local = TargetOfEffect.SpriteRenderers["Heavy"].SpriteRenderer.transform.Find("Heavy Bone/VisualEffect_Weapon_FlameEdge_Local").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule local_main = local.main;
        local_main.simulationSpeed = 1;
        ParticleSystem.EmissionModule local_emission = local.emission;
        local_emission.rateOverTime = 20;
    }
}