using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Stance : Effect
{
    public static float StancePB
    {
        get
        {
            return PB.STANCE_DEFAULT_PB * (1 + Player.Instance.StancePower.Current / 100);
        }
    }
    public static float StanceUpgrade1PB
    {
        get
        {
            return PB.STANCE_UPGRADE_1_DEFAULT_PB * (1 + Player.Instance.StancePower.Current / 100);
        }
    }
    public static float StanceUpgrade2PB
    {
        get
        {
            return PB.STANCE_UPGRADE_2_DEFAULT_PB * (1 + Player.Instance.StancePower.Current / 100);
        }
    }
    public static float StanceUpgrade3PB
    {
        get
        {
            return PB.STANCE_UPGRADE_3_DEFAULT_PB * (1 + Player.Instance.StancePower.Current / 100);
        }
    }
    public bool IsActive
    {
        get
        {
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

    public Item WeaponTheStanceIsAttachedTo
    {
        get
        {
            if (SaveFile.Instance.Stances[0].StanceEffect == this)
            {
                return SaveFile.Instance.EquippedHeavyWeapon;
            }
            else if (SaveFile.Instance.Stances[1].StanceEffect == this)
            {
                return SaveFile.Instance.EquippedLightWeapon;
            }
            else if (SaveFile.Instance.Stances[2].StanceEffect == this)
            {
                return SaveFile.Instance.EquippedRangedWeapon;
            }
            else
            {
                return null;
            }
        }
    }
    public Stance AssignedStance;
    public Effect_Stance(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
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
