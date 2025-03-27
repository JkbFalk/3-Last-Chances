using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Steamworks;

public class Cooldown {
    public bool ShowCooldownInEffectUI = false;
    public string PathToCooldownGraphic = "";
    public Sprite CooldownGraphic;
    public Type Type;
    public float TotalDuration;
    private float _remainingDuration;
    public float RemainingDuration {
        get => _remainingDuration;
        set {
            _remainingDuration = value;
            if(CooldownDisplay != null && !(Type.IsSubclassOf(typeof(Ability)) && Player.Instance.PreparingForUltimate)) {
                CooldownDisplay.fillAmount = _remainingDuration / TotalDuration;
            }
        }
    }
    public string ExtraInfo = "";
    public Image CooldownDisplay;

    public Cooldown(Type type, float total_duration, Unit target, float remaining_duration = 0) {
        Utils.CreateAuditLog("Adding cooldown (" + type + ") for unit " + target.gameObject.name + ": " + total_duration + (remaining_duration != 0 ? "/" + remaining_duration : ""));
        Type = type;
        float extraCDR = 0;
        if(target is Player && type.IsSubclassOf(typeof(Ability)) && Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "TechniqueCooldownReduction")) != null) {
            extraCDR += ((Effect_Description)Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "TechniqueCooldownReduction"))).PercentageAmount;
        }
        if(target is Player && type.IsSubclassOf(typeof(Ability)) && Ability.GetFamily(type) == Ability.AbilityFamily.Salutis && Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "SalutisTechniqueCooldownReduction")) != null) {
            extraCDR += ((Effect_Description)Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "SalutisTechniqueCooldownReduction"))).PercentageAmount;
        }
        else if(target is Player && type.IsSubclassOf(typeof(Ability)) && Ability.GetFamily(type) == Ability.AbilityFamily.Proprius && Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "PropriusTechniqueCooldownReduction")) != null) {
            extraCDR += ((Effect_Description)Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "PropriusTechniqueCooldownReduction"))).PercentageAmount;
        }
        else if(target is Player && type.IsSubclassOf(typeof(Item)) && Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "ToolCooldownReduction")) != null) {
            extraCDR += ((Effect_Description)Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "ToolCooldownReduction"))).PercentageAmount;
        }
        else if(target is Player && type.IsSubclassOf(typeof(Effect)) && Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "EffectCooldownReduction")) != null) {
            extraCDR += ((Effect_Description)Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.Identifier == "EffectCooldownReduction"))).PercentageAmount;
        }
        TotalDuration = total_duration * (100 / (100 + (target.CooldownReduction.Current + (extraCDR / 100) - 1) * 100));
        RemainingDuration = remaining_duration != 0 ?  remaining_duration * (100 / (100 + (target.CooldownReduction.Current + (extraCDR / 100) - 1) * 100)) : TotalDuration;
    }
}