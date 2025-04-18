using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Burn : Effect
{
    private int counter = 0;
    public GameObject Vfx;
    public override int StackingEffectIntensityLevel {
        get { 
            return 
            DecayingAmount  < Utils.GetExpectedPowerForLevel(Player.Instance.Level) * 2.5f ? 1 :
            DecayingAmount  < Utils.GetExpectedPowerForLevel(Player.Instance.Level) * 5 ? 2 : 3;
        }
    }
    public Effect_Burn(float decaying_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialDecayingAmount = decaying_amount;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        Listeners.Add(EventManager.EffectStarted);
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        AddVisualEffect();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        RemoveVFXs();
    }

    public void AddVisualEffect() {
        int prevLevel = StackingEffectIntensityLevel;
        if(prevLevel == StackingEffectIntensityLevel) {
            return;
        }
        RemoveVFXs();
        Vfx = Utils.CreateVisualEffect(SourceOfEffect, "Burn" + StackingEffectIntensityLevel);
        Vfx.gameObject.name = "VisualEffect_Burn" + StackingEffectIntensityLevel;
        Vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
        Vfx.transform.localPosition = Vector2.zero;
        Utils.PlaySoundEffect(Vfx.GetComponent<AudioSource>(), "Effect/Effect_Burn", 0.05f + 0.03f * StackingEffectIntensityLevel);
    }

    public void RemoveVFXs() {
        for(int i = 1; i < 6; i ++) {
            if(TargetOfEffect.SpriteRenderers["Upper Body"]?.Bone?.transform.Find("VisualEffect_Burn" + i)?.gameObject != null) {
                MonoBehaviour.Destroy(TargetOfEffect.SpriteRenderers["Upper Body"].Bone.transform.Find("VisualEffect_Burn" + i).gameObject);
            }
        }
    }

    public override void OnInvokeEffectStarted(Effect effect) {
        if(effect.TargetOfEffect == TargetOfEffect && effect.GetType().IsSubclassOf(typeof(Effect_Staggered))) {
            Damage damage = new Damage(TargetOfEffect, SourceOfEffect.SourceAbility, null) { AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.None), Properties = new List<Damage.DamageProperty> { Damage.DamageProperty.Burn, Damage.DamageProperty.BurnExplosion }, Injury = DecayingAmount * 15}.DisableSoundOnEnemyHit().CalculateDamage();
            GameObject vfx = Utils.CreateVisualEffect(SourceOfEffect, "BurnExplosion");
            vfx.transform.SetParent(effect.TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
            vfx.transform.localPosition = Vector2.zero;
            float size = 0.7f + StackingEffectIntensityLevel * 0.25f;
            vfx.transform.localScale = new Vector2(size, size);
            base.OnInvokeEffectStarted(effect);
            EndThisEffect();
        }
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
            EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
            Damage damage = new Damage(TargetOfEffect, SourceOfEffect.SourceAbility, null) { AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.None), IsDamageOverTime = true, Properties = new List<Damage.DamageProperty> { Damage.DamageProperty.Burn }, Stagger = DecayingAmount}.DisableSoundOnEnemyHit().CalculateDamage();
            AddVisualEffect();
        }
    }
}
