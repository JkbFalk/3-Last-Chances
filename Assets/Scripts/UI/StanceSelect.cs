using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class StanceSelect : MonoBehaviour, IPointerDownHandler, ISelectHandler
{
    public bool IsEquippedStance = false;
    public string Stance;
    public Type StanceType;
    public string Family;

    public void Start() {
        StanceType = Type.GetType(Stance);
        if(StanceType != null) {
            Family = StanceType.GetField("Family",  BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();
        }
    }

    public void UpdateUnlockedStatus() {
        if(string.IsNullOrWhiteSpace(Stance)) {
            return;
        }
        if(StanceType == null) {
            StanceType = Type.GetType(Stance);
            Family = StanceType.GetField("Family",  BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();
        }
        GetComponent<Button>().interactable = SaveFile.Instance.UnlockedStances.Contains(StanceType) || StanceType == typeof(Stance_None);
        MenuManager.Instance.StanceOverview.FirstOrDefault(sel => sel.StanceType == StanceType).transform.Find("Disabled").gameObject.SetActive(!SaveFile.Instance.UnlockedStances.Contains(StanceType));
        if(!IsEquippedStance && StanceType != typeof(Stance_None)) {
            transform.Find("Graphic1").GetComponent<Image>().color = SaveFile.Instance.StanceUpgrades.Contains(Stance + "1") ? Color.white : Color.black;
            transform.Find("Graphic2").GetComponent<Image>().color = SaveFile.Instance.StanceUpgrades.Contains(Stance + "2") ? Color.white : Color.black;
            transform.Find("Graphic3").GetComponent<Image>().color = SaveFile.Instance.StanceUpgrades.Contains(Stance + "3") ? Color.white : Color.black;
        }
        if(IsEquippedStance || StanceType != typeof(Stance_None)) {
            transform.Find(IsEquippedStance ? "UI_UpgradesLoadout/1" : "UI_UpgradesOverview/1").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.StanceUpgrades.Contains(Stance + "1") ? "UpgradeUnlocked" : "UpgradeNotUnlocked"), typeof(Sprite)) as Sprite;
            transform.Find(IsEquippedStance ? "UI_UpgradesLoadout/2" : "UI_UpgradesOverview/2").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.StanceUpgrades.Contains(Stance + "2") ? "UpgradeUnlocked" : "UpgradeNotUnlocked"), typeof(Sprite)) as Sprite;
            transform.Find(IsEquippedStance ? "UI_UpgradesLoadout/3" : "UI_UpgradesOverview/3").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.StanceUpgrades.Contains(Stance + "3") ? "UpgradeUnlocked" : "UpgradeNotUnlocked"), typeof(Sprite)) as Sprite;
        }
    }

    public void EquipStance()
    {
        if(StanceType != null && StanceType == typeof(Stance_SingularPursuit) && (SaveFile.Instance.Stances[0].StanceEffect is Stance_SingularPursuit || SaveFile.Instance.Stances[1].StanceEffect is Stance_SingularPursuit || SaveFile.Instance.Stances[2].StanceEffect is Stance_SingularPursuit)) {
            NotificationController.ShowTextNotification(Label.Get("SingularPursuitOnlyOneStanceNotification"));
        }
        else if(StanceType != null) {
            MenuManager.Instance.ShowAbilityDetails(StanceType);
            MenuManager.Instance.EquipStance(StanceType);
        }
    }

    public void OpenStanceSelection(string stance_being_changed) {
        MenuManager.Instance.ShowAbilityDetails(StanceType);
        MenuManager.Instance.transform.Find("Overview Window/Stance Select").gameObject.SetActive(true);
        MenuManager.Instance.HideEnergySelection();
        MenuManager.Instance.HideAbilitySelection();
        MenuManager.Instance.transform.Find("Overview Window/Effects").gameObject.SetActive(false);
        MenuManager.Instance.StanceBeingChanged = stance_being_changed;
        EventManager.CancelButtonPressed.AddListener(MenuManager.Instance.HideStanceSelection);
        EventManager.ExitMenu.AddListener(MenuManager.Instance.HideStanceSelection);
        if(Settings.Instance.ControlScheme == "Gamepad") {
            MenuManager.Instance.transform.Find("Overview Window/Stance Select/Stances/1-1").GetComponent<Button>().Select();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowAbilityDetails(StanceType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MenuManager.Instance.ShowAbilityDetails(StanceType);
    }
}
