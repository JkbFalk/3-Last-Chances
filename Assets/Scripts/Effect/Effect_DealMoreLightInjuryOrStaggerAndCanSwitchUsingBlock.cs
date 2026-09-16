using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_DealMoreLightInjuryOrStaggerAndCanSwitchUsingBlock : Effect { 

    public Effect_ChangeStat InjuryBuff;
    public Effect_ChangeStat StaggerBuff;

    public Effect_DealMoreLightInjuryOrStaggerAndCanSwitchUsingBlock(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AbilityUsed);
    }


    public override void OnInvokeAbilityUsed(Ability ability)
    {
        if(ability is Ability_Block) {
            ApplyBuff();
        }
    }

    public void ApplyBuff() {
        if((InjuryBuff == null || InjuryBuff.EffectEnded) && (StaggerBuff == null || StaggerBuff.EffectEnded)) {
            if(Player.Instance.LightInjury.Current > Player.Instance.LightStagger.Current) {
                InjuryBuff = new Effect_ChangeStat(Player.Instance.LightInjury, SourceOfEffect) {PercentageAmount = PercentageAmount};
                Player.Instance.AddEffect(InjuryBuff);
            }
            else {
                StaggerBuff = new Effect_ChangeStat(Player.Instance.LightStagger, SourceOfEffect) {PercentageAmount = PercentageAmount};
                Player.Instance.AddEffect(StaggerBuff);
            }
        }
        else if(InjuryBuff != null && !InjuryBuff.EffectEnded) {
            InjuryBuff.EndThisEffect();
            StaggerBuff = new Effect_ChangeStat(Player.Instance.LightStagger, SourceOfEffect) {PercentageAmount = PercentageAmount};
            Player.Instance.AddEffect(StaggerBuff);
        }
        else {
            StaggerBuff.EndThisEffect();
            InjuryBuff = new Effect_ChangeStat(Player.Instance.LightInjury, SourceOfEffect) {PercentageAmount = PercentageAmount};
            Player.Instance.AddEffect(InjuryBuff);
        }

        if(InjuryBuff != null && !InjuryBuff.EffectEnded) {
            Player.Instance.SpriteRenderers["Light Right"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#9c0606ff");
            Player.Instance.SpriteRenderers["Light Left"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#9c0606ff");
        }
        else {
            Player.Instance.SpriteRenderers["Light Right"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#4c0365ff");
            Player.Instance.SpriteRenderers["Light Left"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#4c0365ff");
        }
        Player.Instance.SpriteRenderers["Light Right"].SpriteRenderer.GetComponent<ColorChange>().UpdateMaterialProperties();
        Player.Instance.SpriteRenderers["Light Left"].SpriteRenderer.GetComponent<ColorChange>().UpdateMaterialProperties();
    }

    public override void OnStart() {
        base.OnStart();
        ApplyBuff();
    }
}
