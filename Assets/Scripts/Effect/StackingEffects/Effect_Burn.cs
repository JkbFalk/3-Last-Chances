using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_Burn : Effect
{
    public GameObject Vfx;
    public Effect_Burn(float decaying_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialAmount = decaying_amount;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.StackAmount;
        Listeners.Add(EventManager.EffectStarted);
    }

    public override void ExtraBehaviourOnAmountChange(float amount_decayed = 0, float amount_changed = 0)
    {
        UIText = Utils.GetFormattedFloat(Amount, 0);
        StackingEffectIntensityLevel = Amount < TargetOfEffect.StaggerBar.Maximum * 0.1f ? 1 : Amount < TargetOfEffect.StaggerBar.Maximum * 0.25f ? 2 : 3;
        AddVisualEffect();
    }

    public override void OnStart()
    {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        AddVisualEffect();
        EventManager.OneTenthSecondElapsedInGame.AddListener(ApplyBurn);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        RemoveVFXs();
        EventManager.OneTenthSecondElapsedInGame.RemoveListener(ApplyBurn);
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
            new DamageInstance(TargetOfEffect, SourceOfEffect.SourceAbility, null) { 
                AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.None), 
                Properties = new List<DamageInstance.DamageProperty> { DamageInstance.DamageProperty.Burn, DamageInstance.DamageProperty.BurnExplosion }, 
                Injury = Amount * 15,
                PlaySoundOnEnemyHit = false
            }.CalculateAndApplyDamage();
            GameObject vfx = Utils.CreateVisualEffect(SourceOfEffect, "BurnExplosion");
            vfx.transform.SetParent(effect.TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
            vfx.transform.localPosition = Vector2.zero;
            float size = 0.7f + StackingEffectIntensityLevel * 0.25f;
            vfx.transform.localScale = new Vector2(size, size);
            base.OnInvokeEffectStarted(effect);
            if (Player.Instance?.CurrentStance?.StanceEffect is Stance_HeatOfBattle && SaveFile.Instance.ActiveUpgrades.Contains("Stance_HeatOfBattle3"))
            {
                Amount = Amount * Stance_HeatOfBattle.Upgrade3LeaveBehindBurnStacksPercentage / 100;
            }
            else
            {
                EndThisEffect();
            }
        }
    }

    public void ApplyBurn()
    {
        if (EffectEnded || TargetOfEffect == null)
        {
            return;
        }
        new DamageInstance(TargetOfEffect, SourceOfEffect.SourceAbility, null)
        {
            AbilityDamageSource = new Ability.DamageSource(0, 0, Constants.DamageType.None),
            Properties = new List<DamageInstance.DamageProperty> { DamageInstance.DamageProperty.Burn, DamageInstance.DamageProperty.DamageOverTime },
            Stagger = Amount / 10,
            PlaySoundOnEnemyHit = false
        }.CalculateAndApplyDamage();
    }
}
