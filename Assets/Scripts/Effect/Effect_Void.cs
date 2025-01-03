using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Void : Effect
{
    //public GameObject Vfx;
    public int VoidLevel = 0;

    public Effect_ChangeStat ReduceSBEffect;
    public Effect_Void(float sb_reduced, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Effect_ExtraVoid extraVoid = ((Effect_ExtraVoid)Player.Instance.GetEffect(typeof(Effect_ExtraVoid)));
        Type = EffectType.Debuff;
        _initialDecayingAmount = extraVoid != null ? sb_reduced + extraVoid.ExtraVoidAmount : sb_reduced;
        DisplayEffectIndicator = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        DefaultDecaySpeed = 0;
        DescriptionLabel = "Effect_Void_Explanation";
        PathToEffectGraphic = "Effect/Void";
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        if(ReduceSBEffect == null) {
            return;
        }
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
        ReduceSBEffect.FlatAmount = -DecayingAmount;
        AddVisualEffect();
    }

    public override void OnStart()
    {
        base.OnStart();
        ReduceSBEffect = new Effect_ChangeStat(TargetOfEffect.StaggerBar, SourceOfEffect) {FlatAmount = -DecayingAmount};
        TargetOfEffect.AddEffect(ReduceSBEffect);
        BaseDuration = 0;
        AddDecayingAmount(DecayingAmount);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ReduceSBEffect.EndThisEffect();
        //MonoBehaviour.Destroy(Vfx.gameObject);
    }

    public void AddVisualEffect() {
        /*int prevLevel = VoidLevel;
        VoidLevel = 
        VoidAmount < 20 ? 1 : VoidAmount < 40 ? 2 : VoidAmount < 60 ? 3 : VoidAmount < 80 ? 4 : 5;
        if(prevLevel == VoidLevel) {
            return;
        }
        if(Vfx != null && Vfx.gameObject.IsDestroyed() == false) {
            MonoBehaviour.Destroy(Vfx.gameObject);
        }
        Vfx = Utils.CreateVisualEffect(SourceOfEffect, "Freeze" + VoidLevel);
        Vfx.gameObject.name = "VisualEffect_Freeze" + VoidLevel;
        Vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Upper Body"].Bone);
        Vfx.transform.localPosition = Vector2.zero;
        Utils.PlaySoundEffect(Vfx.GetComponent<AudioSource>(), "Effect/Effect_Freeze", 0.05f + 0.02f * VoidLevel);*/
    }
}



