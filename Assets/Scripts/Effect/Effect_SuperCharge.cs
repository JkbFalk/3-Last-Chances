using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Supercharge : Effect
{
    public GameObject Vfx;
    public Effect_Supercharge(float damage_increase, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialDecayingAmount = damage_increase;
        DisplayEffectIndicator = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        Listeners.Add(EventManager.DamageDealt);
        DescriptionLabel = "Effect_Supercharge_Explanation";
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

    public override void OnInvokeDamageDealt(Damage damage) {
        if(damage.SourceOfDamage?.User == TargetOfEffect && damage.IsDamageOverTime == false && damage.IsExtraDamage == false) {
            Damage d = new Damage(damage.TargetOfDamage, new Ability_SourcelessDamage(TargetOfEffect), damage.DamagingObject) {AbilityDamageSource = new(0, 0, Constants.DamageType.None), Injury = DecayingAmount, IsExtraDamage = true, Properites = new() {Damage.DamageProperty.Supercharge}};
            d.CalculateDamage();
            base.OnInvokeDamageDealt(damage);
        }
    }
}
