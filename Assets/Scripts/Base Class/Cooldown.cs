using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
        TotalDuration = total_duration * (100 / (100 + (target.CooldownReduction.Current - 1) * 100));
        RemainingDuration = remaining_duration != 0 ?  remaining_duration * (100 / (100 + (target.CooldownReduction.Current - 1) * 100)) : TotalDuration;
    }
}