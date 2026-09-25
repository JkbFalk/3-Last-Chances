using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Prone : Effect
{
    public GameObject Vfx;
    public int ProneLevel = 0;
    public Effect_Prone(float damage_increase, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialAmount = damage_increase;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(Amount, 0);
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
        if (damage?.TargetOfDamage == TargetOfEffect && damage.SourceOfDamage.User == Player.Instance && Player.Instance.CurrentStance.StanceEffect is Stance_CombatBlacksmith && SaveFile.Instance.ActiveUpgrades.Contains("Stance_CombatBlacksmith1"))
        {
            damage.DamageDealtPercentageModifier += damage.Is(DamageInstance.DamageProperty.Tool) ? (Amount * Stance_CombatBlacksmith.Upgrade1ProneEffectivnessForTools / 100) : 0;
        }
        else if (damage?.TargetOfDamage == TargetOfEffect)
        {
            damage.DamageDealtPercentageModifier += Amount;
            base.OnInvokeHitDealt(damage);
        }
    }
}
