using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Steamworks;
using Unity.VisualScripting;

public class Cooldown {
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
            if (CooldownIndicator != null && !(Type.IsSubclassOf(typeof(Ability)) && Player.Instance.PreparingForUltimate))
            {
                CooldownIndicator.fillAmount = _remainingDuration / TotalDuration;
            }
        }
    }
    public string Id = "";
    public Image CooldownIndicator;
    public GameObject TileInUI;

    public Cooldown(Type type, float total_duration, Unit target, string id = "")
    {
        Type = type;
        CooldownTarget = target;
        Id = id;
        TotalDuration = total_duration;
    }

    public void OnStart()
    {
        float cdrAmount =
            Type.IsSubclassOf(typeof(Effect)) ? CooldownTarget.CooldownReduction.GetEffectCooldownReduction(Type) :
            Type.IsSubclassOf(typeof(Item)) ? CooldownTarget.CooldownReduction.GetToolCooldownReduction(Type) :
            Type.IsSubclassOf(typeof(Ability)) ? CooldownTarget.CooldownReduction.GetTechniqueCooldownReduction(Type) : 0;
        TotalDuration = TotalDuration / (1 + (cdrAmount / 100));
        RemainingDuration = TotalDuration;
        if(Id != "" && (Type == typeof(Effect) || Type.IsSubclassOf(typeof(Effect)))) {
            Effect hiddenEffect = Player.Instance.CurrentEffects.FirstOrDefault(e => e.HideInUIWhileCooldownWithIdExists == Id);
            if(hiddenEffect != null && hiddenEffect.TileInUI != null) {
                MonoBehaviour.Destroy(hiddenEffect.TileInUI);
            }
        }
    }

    public void EndThisCooldown()
    {
        if (Type.IsSubclassOf(typeof(Technique)))
        {
            CooldownTarget.TechniqueCooldowns.Remove(this);
        }
        else if (Type.IsSubclassOf(typeof(Item)))
        {
            CooldownTarget.ToolCooldown = null;
        }
        else if (Type.IsSubclassOf(typeof(Effect)))
        {
            CooldownTarget.EffectCooldowns.Remove(this);
        }
        OnEnd();
    }

    public void OnEnd()
    {
        if (ShowsInUI && TileInUI != null)
        {
            MonoBehaviour.Destroy(TileInUI);
        }
        if (Id != "" && (Type == typeof(Effect) || Type.IsSubclassOf(typeof(Effect))))
        {
            Effect hiddenEffect = Player.Instance.CurrentEffects.FirstOrDefault(e => e.HideInUIWhileCooldownWithIdExists == Id);
            if (hiddenEffect != null && hiddenEffect.UICooldownDisplay == null)
            {
                hiddenEffect.ShowInUI();
            }
        }
        EventManager.CooldownEnded.Invoke(this);
    }
}