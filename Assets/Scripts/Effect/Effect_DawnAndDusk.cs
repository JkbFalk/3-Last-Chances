using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_DawnAndDusk : Effect { 

    public Effect_ChangeStat InjuryBuff;
    public Effect_ChangeStat StaggerBuff;
    public float BuffAmount;

    public Effect_DawnAndDusk(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AbilityUsed);
    }

    public override void OnEffectValueChanged()
    {
        BuffAmount = LinearEffectValue * 1.25f;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(BuffAmount) };
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
                InjuryBuff = new Effect_ChangeStat(Player.Instance.LightInjury, SourceOfEffect) {PercentageAmount = BuffAmount};
                Player.Instance.AddEffect(InjuryBuff);
            }
            else {
                StaggerBuff = new Effect_ChangeStat(Player.Instance.LightStagger, SourceOfEffect) {PercentageAmount = BuffAmount};
                Player.Instance.AddEffect(StaggerBuff);
            }
        }
        else if(InjuryBuff != null && !InjuryBuff.EffectEnded) {
            InjuryBuff.EndThisEffect();
            StaggerBuff = new Effect_ChangeStat(Player.Instance.LightStagger, SourceOfEffect) {PercentageAmount = BuffAmount};
            Player.Instance.AddEffect(StaggerBuff);
        }
        else {
            StaggerBuff.EndThisEffect();
            InjuryBuff = new Effect_ChangeStat(Player.Instance.LightInjury, SourceOfEffect) {PercentageAmount = BuffAmount};
            Player.Instance.AddEffect(InjuryBuff);
        }

        if(InjuryBuff != null && !InjuryBuff.EffectEnded) {
            Player.Instance.SpriteRenderers["Light Right"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#FFFFFF");
            Player.Instance.SpriteRenderers["Light Left"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#FFFFFF");
        }
        else {
            Player.Instance.SpriteRenderers["Light Right"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#000000");
            Player.Instance.SpriteRenderers["Light Left"].SpriteRenderer.GetComponent<ColorChange>().Blue = Colors.GetColorFromCode("#000000");
        }
        Player.Instance.SpriteRenderers["Light Right"].SpriteRenderer.GetComponent<ColorChange>().UpdateMaterialProperties();
        Player.Instance.SpriteRenderers["Light Left"].SpriteRenderer.GetComponent<ColorChange>().UpdateMaterialProperties();
    }

    public override void OnStart() {
        base.OnStart();
        ApplyBuff();
    }
}
