using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Reflection;

public class StanceUnlockTile : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    public string Ability;
    public Type AbilityType;
    public string Row;
    public string Tree;

    public bool IsUpgrade;

    public void Start() {
        Row = transform.parent.parent.gameObject.name;
        AbilityType = Type.GetType(Ability.Replace("1", "").Replace("2", "").Replace("3", ""));
        IsUpgrade = Ability.EndsWith("1") || Ability.EndsWith("2") || Ability.EndsWith("3");
        if(AbilityType != null) {
            FieldInfo family = AbilityType.GetField("Family", BindingFlags.Public | BindingFlags.Static);
            Tree = family.GetValue(null).ToString();
        }
    }

    public void OnClick() {
        if((IsUpgrade == false && SaveFile.Instance.UnlockedStances.Contains(AbilityType)) || (IsUpgrade && SaveFile.Instance.StanceUpgrades.Contains(Ability))) {
            RefundTile();
        }
        else if(CheckIfCanUnlockTile()) {
            UnlockTile();
        }
    }

    public bool CheckIfCanUnlockTile() {
        int row = int.Parse(Row);
        if(row == 4 && SaveFile.Instance.Level < 30) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {30}));
        }
        else if(row >= 3 && SaveFile.Instance.Level < 20) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {20}));
        }
        else if(row >= 2 && SaveFile.Instance.Level < 10) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {10}));
        }
        else {
            return true;
        }
        return false;
    }

    public void UnlockTile() {
        if(IsUpgrade && !SaveFile.Instance.UnlockedStances.Contains(AbilityType)) {
            NotificationController.ShowTextNotification(Label.Get("CannotLearnUpgradeWithoutStance"));
        }
        else {
            SaveFile.Instance.UnlockStance(AbilityType, IsUpgrade ? Ability.Substring(Ability.Length - 1) : "0");
        }
    }

    public void RefundTile() {
        if(!IsUpgrade && (SaveFile.Instance.StanceUpgrades.Contains(AbilityType + "1") || SaveFile.Instance.StanceUpgrades.Contains(AbilityType + "2") || SaveFile.Instance.StanceUpgrades.Contains(AbilityType + "3"))) {
            NotificationController.ShowTextNotification(Label.Get("CannotUnlearnStanceWithUpgradeWarning"));
        }
        else if(SaveFile.Instance.UnlockedStances.Contains(AbilityType) || SaveFile.Instance.StanceUpgrades.Contains(Ability)) {
            SaveFile.Instance.UnlearnStance(AbilityType, IsUpgrade ? Ability.Substring(Ability.Length - 1) : "0");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) {
            MenuManager.Instance.ShowSkillTreeStanceDetails(this);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowSkillTreeStanceDetails(this);
    }

    public void UpdateUnlockedStatus() {
        if(AbilityType == null) {
            Start();
        }
        transform.parent.parent.Find("NotUnlocked").gameObject.SetActive((transform.parent.parent.gameObject.name == "2" && SaveFile.Instance.Level < 10) || (transform.parent.parent.gameObject.name == "3" && SaveFile.Instance.Level < 20) || (transform.parent.parent.gameObject.name == "4" && SaveFile.Instance.Level < 30));
        transform.Find("Disabled").gameObject.SetActive((IsUpgrade && !SaveFile.Instance.StanceUpgrades.Contains(Ability)) || (!IsUpgrade && !SaveFile.Instance.UnlockedStances.Contains(AbilityType)));
    }
}
