using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Freeze : Effect
{
    public GameObject Vfx;
    public override int StackingEffectIntensityLevel {
        get { 
            return 
            DecayingAmount  < Utils.GetExpectedPowerForLevel(Player.Instance.Level) * 2.5f ? 1 :
            DecayingAmount  < Utils.GetExpectedPowerForLevel(Player.Instance.Level) * 5 ? 2 : 3;
        }
    }
    public Effect_Freeze(float decaying_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialDecayingAmount = decaying_amount;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        Listeners.Add(EventManager.EffectStarted);
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        UIText = Utils.GetFormattedFloat(DecayingAmount, 0);
        AddVisualEffect();
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        AddVisualEffect();
        EventManager.OneTenthSecondElapsedInGame.AddListener(ApplyFreeze);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        RemoveVFXs();
        EventManager.OneTenthSecondElapsedInGame.RemoveListener(ApplyFreeze);
    }

    public void AddVisualEffect() {
        int prevLevel = StackingEffectIntensityLevel;
        if(prevLevel == StackingEffectIntensityLevel) {
            return;
        }
        RemoveVFXs();
        Vfx = Utils.CreateVisualEffect(SourceOfEffect, "Freeze" + StackingEffectIntensityLevel);
        Vfx.gameObject.name = "VisualEffect_Freeze" + StackingEffectIntensityLevel;
        Vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
        Vfx.transform.localPosition = Vector2.zero;
        Utils.PlaySoundEffect(Vfx.GetComponent<AudioSource>(), "Effect/Effect_Freeze", 0.05f + 0.03f * StackingEffectIntensityLevel);
    }

    public void RemoveVFXs() {
        for(int i = 1; i < 6; i ++) {
            if(TargetOfEffect.SpriteRenderers["Upper Body"]?.Bone?.transform.Find("VisualEffect_Freeze" + i)?.gameObject != null) {
                MonoBehaviour.Destroy(TargetOfEffect.SpriteRenderers["Upper Body"].Bone.transform.Find("VisualEffect_Freeze" + i).gameObject);
            }
        }
    }

    public override void OnInvokeEffectStarted(Effect effect) {
        if(effect.TargetOfEffect == TargetOfEffect && effect.GetType().IsSubclassOf(typeof(Effect_Staggered))) {
            float freezeDuration = Constants.DEFAULT_HARD_STAGGERED_DURATION + DecayingAmount / Utils.GetExpectedPowerForLevel(Player.Instance.Level) / effect.TargetOfEffect.StaggerBar.Maximum * 50;
            effect.TargetOfEffect.AddEffect(new Effect_Frozen(SourceOfEffect), freezeDuration);
            GameObject vfx = Utils.CreateVisualEffect(SourceOfEffect, "FreezeInPlace");
            vfx.transform.SetParent(effect.TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
            vfx.transform.localPosition = Vector2.zero;
            float size = 0.7f + StackingEffectIntensityLevel * 0.25f;
            vfx.transform.localScale = new Vector2(size, size);
            base.OnInvokeEffectStarted(effect);
            EndThisEffect();
        }
    }

    public void ApplyFreeze()
    {
        if (EffectEnded || TargetOfEffect == null)
        {
            return;
        }
        new Damage(TargetOfEffect, SourceOfEffect.SourceAbility, null)
        {
            AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.None),
            Properties = new List<Damage.DamageProperty> { Damage.DamageProperty.Burn, Damage.DamageProperty.DamageOverTime },
            Stagger = DecayingAmount / 10,
            PlaySoundOnEnemyHit = false
        }.CalculateAndApplyDamage();
    }
}
