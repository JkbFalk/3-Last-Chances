using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Analysis : Effect
{
    public GameObject Vfx;
    public int AnalysisLevel = 0;
    public Effect_Analysis(float damage_increase, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        _initialDecayingAmount = damage_increase;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackDecayingAmount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void ExtraBehaviourOnDecayingAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(DecayingAmount, 0);
        StackingEffectIntensityLevel = DecayingAmount < 20 ? 1 : DecayingAmount < 50 ? 2 : 3;
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
        RemoveBurnVFXs();
    }

    public void AddVisualEffect() {
        /*int prevLevel = BurnLevel;
        BurnLevel = 
        DecayingAmount  < Utils.GetExpectedPlayerOffensePower(Player.Instance.Level) * 2.5f ? 1 :
        DecayingAmount  < Utils.GetExpectedPlayerOffensePower(Player.Instance.Level) * 5 ? 2 :
        DecayingAmount  < Utils.GetExpectedPlayerOffensePower(Player.Instance.Level) * 7.5f ? 3 :
        DecayingAmount  < Utils.GetExpectedPlayerOffensePower(Player.Instance.Level) * 10 ? 4 : 5;
        if(prevLevel == BurnLevel) {
            return;
        }
        RemoveBurnVFXs();
        Vfx = Utils.CreateVisualEffect(SourceOfEffect, "Burn" + BurnLevel);
        Vfx.gameObject.name = "VisualEffect_Burn" + BurnLevel;
        Vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
        Vfx.transform.localPosition = Vector2.zero;
        Utils.PlaySoundEffect(Vfx.GetComponent<AudioSource>(), "Effect/Effect_Burn", 0.05f + 0.05f * BurnLevel);*/
    }

    public void RemoveBurnVFXs() {
        /*for(int i = 1; i < 6; i ++) {
            if(TargetOfEffect.SpriteRenderers["Upper Body"]?.Bone?.transform.Find("VisualEffect_Burn" + i)?.gameObject != null) {
                MonoBehaviour.Destroy(TargetOfEffect.SpriteRenderers["Upper Body"].Bone.transform.Find("VisualEffect_Burn" + i).gameObject);
            }
        }*/
    }

    public override void OnInvokeHitDealt(DamageInstance damage) {
        if(damage?.SourceOfDamage?.User == TargetOfEffect && ((damage?.SourceOfDamage?.User is Player && damage.SourceOfDamage.Is(Ability.Property.Technique)) || (damage?.SourceOfDamage?.User is not Player && damage.DamageType == Constants.DamageType.Magic))) {
            damage.DamageDealtPercentageModifier += DecayingAmount;
            base.OnInvokeHitDealt(damage);
        }
    }
}
