using System.Collections.Generic;
using UnityEngine;

public class Effect_RykerPayback : Effect {

    public float ExplosionGauge = 0;
    private GameObject _glowVFX;

    public Effect_RykerPayback(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnStart()
    {
        base.OnStart();
        EventManager.EffectStarted.AddListener(Activate);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        EventManager.EffectStarted.RemoveListener(Activate);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        ExplosionGauge += damage.InjuryDealt / TargetOfEffect.Health.Maximum * 50;
        UpdateGlowVFX();
        base.OnInvokeDamageDealt(damage);
    }

    public void Activate(Effect e)
    {
        if(e.TargetOfEffect == TargetOfEffect && e.GetType().IsSubclassOf(typeof(Effect_Staggered))) {
            ExplosionGauge = 0;
        }
    }

    public void UpdateGlowVFX()
    {
        if (_glowVFX == null)
        {
            _glowVFX = TargetOfEffect.SpriteRenderers["Upper Body"].Bone.transform.Find("VisualEffect_Ryker_Payback_Glow").gameObject;
        }
        _glowVFX.SetActive(ExplosionGauge > 10);
        if (ExplosionGauge < 25)
        {
            TargetOfEffect.UnitAI.Actions[typeof(NPCAbility_RykerPayback)] = 3;
            _glowVFX.transform.localScale = new Vector2(0.5f, 0.5f);
            ParticleSystem.MainModule main = _glowVFX.GetComponent<ParticleSystem>().main;
            main.simulationSpeed = 0.5f;
        }
        else if (ExplosionGauge < 50)
        {
            TargetOfEffect.UnitAI.Actions[typeof(NPCAbility_RykerPayback)] = 6;
            _glowVFX.transform.localScale = new Vector2(1f, 1f);
            ParticleSystem.MainModule main = _glowVFX.GetComponent<ParticleSystem>().main;
            main.simulationSpeed = 1f;
        }
        else if (ExplosionGauge < 75)
        {
            TargetOfEffect.UnitAI.Actions[typeof(NPCAbility_RykerPayback)] = 10;
            _glowVFX.transform.localScale = new Vector2(1.5f, 1.5f);
            ParticleSystem.MainModule main = _glowVFX.GetComponent<ParticleSystem>().main;
            main.simulationSpeed = 1.5f;
        }
        else
        {
            TargetOfEffect.UnitAI.Actions[typeof(NPCAbility_RykerPayback)] = 100;
            _glowVFX.transform.localScale = new Vector2(2.5f, 2.5f);
            ParticleSystem.MainModule main = _glowVFX.GetComponent<ParticleSystem>().main;
            main.simulationSpeed = 2.5f;
        }
    }
}