using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_BerserkerRage : Effect
{
    private Color _eyeColor;
    public Effect_BerserkerRage(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        AdditionalEffectsAffectingTargetDuringEffect.Add(new Effect_Unkillable(SourceOfEffect));
    }

    public override void OnStart()
    {
        base.OnStart();
        EventManager.EffectStarted.AddListener(Activate);
        _eyeColor = TargetOfEffect.UnitColorChange.Eye;
        TargetOfEffect.UnitColorChange.Eye = Color.red;
        TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        TargetOfEffect.SpriteRenderers["Upper Body"].Bone.transform.Find("VisualEffect_PowerfulEnemy").gameObject.SetActive(true);
        TargetOfEffect.StaggerBar.Maximum = TargetOfEffect.StaggerBar.Maximum / 2;
        if(TargetOfEffect.StaggerBar.Current >= TargetOfEffect.StaggerBar.Maximum) {
            TargetOfEffect.StaggerBar.Current = TargetOfEffect.StaggerBar.Maximum - 1;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        EventManager.EffectStarted.RemoveListener(Activate);
        TargetOfEffect.UnitColorChange.Eye = _eyeColor;
        TargetOfEffect.UnitColorChange.UpdateMaterialProperties();
        TargetOfEffect.SpriteRenderers["Upper Body"].Bone.transform.Find("VisualEffect_PowerfulEnemy").gameObject.SetActive(false);
    }

    public void Activate(Effect e)
    {
        if(e.TargetOfEffect == TargetOfEffect && e is Effect_HardStaggered) {
            EndThisEffect();
        }
    }
}
