using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Steamworks;
using Unity.VisualScripting;

public class Cooldown {
    public float ExtraCooldownReduction;
    public bool ShowsInUI = false;
    public string PathToCooldownGraphic = "";
    public Sprite CooldownGraphic;
    public Type Type;
    public Unit CooldownTarget;
    public float NewCooldownIndicatorExtraScaleTimer = Constants.NEW_COOLDOWN_OR_EFFECT_HIGHER_SCALE_TIMER;
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
    public string Identifier = "";
    public Image CooldownDisplay;
    public GameObject TileInUI;

    public Cooldown(Type type, float total_duration, Unit target, string identifier = "") {
        Type = type;
        CooldownTarget = target;
        Identifier = identifier;
        TotalDuration = total_duration / (1 + (CooldownTarget.CooldownReduction.Current + ExtraCooldownReduction) / 100);
        RemainingDuration = TotalDuration;
        if(identifier != "" && (Type == typeof(Effect) || Type.IsSubclassOf(typeof(Effect)))) {
            Effect hiddenEffect = Player.Instance.CurrentEffects.FirstOrDefault(e => e.HideInUIWhileCooldownWithIdExists == identifier);
            if(hiddenEffect != null && hiddenEffect.TileInUI != null) {
                MonoBehaviour.Destroy(hiddenEffect.TileInUI);
            }
        }
    }

    public void OnEnd() {
        if(ShowsInUI && TileInUI != null) {
            MonoBehaviour.Destroy(TileInUI);
        }
        if(Identifier != "" && (Type == typeof(Effect) || Type.IsSubclassOf(typeof(Effect)))) {
            Effect hiddenEffect = Player.Instance.CurrentEffects.FirstOrDefault(e => e.HideInUIWhileCooldownWithIdExists == Identifier);
            if(hiddenEffect != null && hiddenEffect.UICooldownDisplay == null) {
                hiddenEffect.ShowInUI();
            }
        }
    }

    public string GetCooldownTechniqueFamily() {
        if(Type == null || Type.IsSubclassOf(typeof(Ability))) {
            return "";
        }
        return Ability.GetFamily(Type).ToString();
    }
}