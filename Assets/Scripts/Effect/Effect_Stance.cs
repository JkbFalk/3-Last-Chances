using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Stance : Effect
{
    public bool IsActive {
        get {
            return Player.Instance.CurrentStance != null && Player.Instance.CurrentStance.StanceEffect == this;
        }
    }
    public bool UnlockedUpgrade1 
    {
        get {
            return SaveFile.Instance.ActiveUpgrades.Contains(GetType().ToString() + "1");
        }
    }
    public bool UnlockedUpgrade2 
    {
        get {
            return SaveFile.Instance.ActiveUpgrades.Contains(GetType().ToString() + "2");
        }
    }
    public bool UnlockedUpgrade3 
    {
        get {
            return SaveFile.Instance.ActiveUpgrades.Contains(GetType().ToString() + "3");
        }
    }
    public Stance AssignedStance;
    public Effect_Stance(SourceOfEffect source_of_effect) : base(source_of_effect) {
        IsRemovable = false;
    }

    public virtual void OnStanceActivated() {

    }

    public virtual void OnStanceDeactivated() {

    }

    public virtual void OnStanceEquipped() {

    }

    public virtual void OnStanceUnequipped() {

    }

    public void RefreshStance() {
        if(Player.Instance.CurrentStance?.StanceEffect == null) {
            return;
        }
        if(Player.Instance.CurrentStance.StanceEffect == this) {
            OnStanceDeactivated();
        }
        if(Player.Instance.CurrentStance.StanceEffect == this) {
            OnStanceActivated();
        }
    }

    public virtual void CreateStanceDisplay() {
        GameObject gauge = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Stance/UI_" + Player.Instance.CurrentStance.StanceEffect.GetType().ToString().Replace("Stance_", ""))) as GameObject;
        Player.Instance.CurrentStanceGauge = gauge;
        gauge.transform.SetParent(UIManager.Objects.StanceGaugeContainer.transform);
        gauge.transform.localPosition = Vector2.zero;
    }
}
