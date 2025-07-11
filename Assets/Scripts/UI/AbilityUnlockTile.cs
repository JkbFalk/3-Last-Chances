using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Reflection;
using UnityEditor.PackageManager;

public class AbilityUnlockTile : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    public string Ability;
    public Type AbilityType;
    public string Row;
    public bool IsUltimateUnlock = false;

    public void Start() {
        Row = transform.parent.parent.gameObject.name;
        AbilityType = Type.GetType(Ability);
    }

    public void OnClick() {
        CheckIfCanUnlockTile();
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) {
            MenuManager.Instance.ShowSkillTreeAbilityDetails(this, false, IsUltimateUnlock);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowSkillTreeAbilityDetails(this);
    }
    
    public void UpdateUnlockedStatus()
    {
        if (AbilityType == null)
        {
            Start();
        }
        Debug.Log(AbilityType + " , " + Utils.GetGameObjectPath(gameObject));
        FieldInfo family = AbilityType.GetField("Family", BindingFlags.Public | BindingFlags.Static);
        if (IsUltimateUnlock)
        {
            int row = int.Parse(Row);
            transform.Find("Mask").GetComponent<Image>().color = (row == 4 && SaveFile.Instance.Level >= 30) || (row != 4 && SaveFile.Instance.Level >= 25) ? Colors.GetFamilyColor(family.GetValue(null).ToString()) : Color.black;
            transform.parent.parent.Find("ConnectorUltimate/Active").gameObject.SetActive((row == 4 && SaveFile.Instance.Level >= 30) || (row != 4 && SaveFile.Instance.Level >= 25));
        }
        else
        {
            transform.Find("Mask").GetComponent<Image>().color = SaveFile.Instance.UnlockedAbilities.Contains(AbilityType) ? Colors.GetFamilyColor(family.GetValue(null).ToString()) : Color.black;
        }
    }
}
