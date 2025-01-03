using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Freeze : Effect
{
    public GameObject Vfx;
    public float FreezePercentage = 0;
    public int FreezeLevel = 0;

    public Effect_ChangeStat MSSlowEffect;
    public Effect_ChangeCompositeStat ASSlowEffect;
    public Effect_Freeze(float freeze, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Debuff;
        _initialDecayingAmount = freeze;
        DisplayEffectIndicator = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        DescriptionLabel = "Effect_Freeze_Explanation";
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        if(MSSlowEffect == null || ASSlowEffect == null) {
            return;
        }
        FreezePercentage = DecayingAmount / TargetOfEffect.StaggerBar.Maximum * 100 / TargetOfEffect.Tenacity.Current * SourceOfEffect.User.Control.Current;
        if(FreezePercentage > 100) {
            FreezePercentage = 100;
        }
        EffectIndicatorText = Utils.GetFormattedFloat(FreezePercentage) + "%";
        AddVisualEffect();
        if(TargetOfEffect is not Player) {
            SpecialSkinColorDuringHardCrowdControl = new Color(0, Utils.GetValueBasedOnMinAndMax(FreezePercentage, 0, 100, 0, 0.5f), Utils.GetValueBasedOnMinAndMax(FreezePercentage, 0, 100, 0, 1));
        }
        MSSlowEffect.PercentageAmount = -FreezePercentage / 2;
        ASSlowEffect.PercentageAmount = -FreezePercentage / 2;
        if(FreezePercentage >= 100) {
            bool immune = TargetOfEffect.CheckIfUnderEffect(typeof(Effect_ImmunityToFreeze));
            if(!immune) {
                TargetOfEffect.AddEffect(new Effect_FreezeInPlace(SourceOfEffect), 5);
                TargetOfEffect.AddEffect(new Effect_ImmunityToFreeze(SourceOfEffect), 15 / (SourceOfEffect?.User?.Control?.Current != null ? SourceOfEffect.User.Control.Current : 1) * TargetOfEffect.Tenacity.Current);
                AddDecayingAmount(-TargetOfEffect.StaggerBar.Maximum * 0.25f, false);
            }
        }
    }

    public override void OnUpdate() {
        if(TargetOfEffect is not Player) {
            TargetOfEffect.UnitColorChange.SpecialSkinColor = SpecialSkinColorDuringHardCrowdControl;
            TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        ASSlowEffect = new Effect_ChangeCompositeStat(TargetOfEffect, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect);
        MSSlowEffect = new Effect_ChangeStat(TargetOfEffect.MovementSpeed, SourceOfEffect);
        TargetOfEffect.AddEffect(ASSlowEffect);
        TargetOfEffect.AddEffect(MSSlowEffect);
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        AddDecayingAmount(DecayingAmount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ASSlowEffect.EndThisEffect();
        MSSlowEffect.EndThisEffect();
        TargetOfEffect.UnitColorChange.SpecialSkinColor = Color.white;
        TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        MonoBehaviour.Destroy(Vfx.gameObject);
    }

    public void AddVisualEffect() {
        int prevLevel = FreezeLevel;
        FreezeLevel = 
        FreezePercentage < 20 ? 1 : FreezePercentage < 40 ? 2 : FreezePercentage < 60 ? 3 : FreezePercentage < 80 ? 4 : 5;
        if(prevLevel == FreezeLevel) {
            return;
        }
        if(Vfx != null && Vfx.gameObject.IsDestroyed() == false) {
            MonoBehaviour.Destroy(Vfx.gameObject);
        }
        Vfx = Utils.CreateVisualEffect(SourceOfEffect, "Freeze" + FreezeLevel);
        Vfx.gameObject.name = "VisualEffect_Freeze" + FreezeLevel;
        Vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
        Vfx.transform.localPosition = Vector2.zero;
        Utils.PlaySoundEffect(Vfx.GetComponent<AudioSource>(), "Effect/Effect_Freeze", 0.05f + 0.02f * FreezeLevel);
    }
}
