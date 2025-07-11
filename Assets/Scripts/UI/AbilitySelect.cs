using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitySelect : MonoBehaviour, IPointerDownHandler, ISelectHandler
{
    public bool IsStanceEquippedAbility = false;
    public string Ability;
    public Type AbilityType;

    public void Start() {
        AbilityType = Type.GetType(Ability);
        if(IsStanceEquippedAbility == false) {
            transform.GetComponent<Button>().interactable = !String.IsNullOrWhiteSpace(Ability) && SaveFile.Instance.UnlockedAbilities.Contains(AbilityType);
        }
    }

    public void UpdateUnlockedStatus() {
        if(AbilityType == null) {
            AbilityType = Type.GetType(Ability);
            if(AbilityType == null && IsStanceEquippedAbility) {
                transform.Find("UI_UpgradesLoadout/1").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/UpgradeNotUnlocked", typeof(Sprite)) as Sprite;
                transform.Find("UI_UpgradesLoadout/2").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/UpgradeNotUnlocked", typeof(Sprite)) as Sprite;
                return;
            }
            else if(AbilityType == null) {
                return;
            }
        }
        GetComponent<Button>().interactable = SaveFile.Instance.UnlockedAbilities.Contains(AbilityType);
        FieldInfo family = AbilityType.GetField("Family", BindingFlags.Public | BindingFlags.Static);
        transform.Find("Mask").GetComponent<Image>().color = SaveFile.Instance.UnlockedAbilities.Contains(AbilityType) ? Color.black : Color.grey;
        transform.Find(IsStanceEquippedAbility ? "UI_UpgradesLoadout/1" : "UI_UpgradesOverview/1").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.AbilitiesMasteryA.Contains(AbilityType) ? "UpgradeUnlocked" : "UpgradeNotUnlocked"), typeof(Sprite)) as Sprite;
        transform.Find(IsStanceEquippedAbility ? "UI_UpgradesLoadout/2" : "UI_UpgradesOverview/2").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.AbilitiesMasteryB.Contains(AbilityType) ? "UpgradeUnlocked" : "UpgradeNotUnlocked"), typeof(Sprite)) as Sprite;
    }

    public void EquipAbility()
    {
        if(AbilityType != null) {
            MenuManager.Instance.ShowAbilityDetails(AbilityType);
            MenuManager.Instance.EquipAbility(AbilityType);
        }
    }

    public void OpenAbilitySelection(string ability_being_changed) {
        MenuManager.Instance.ShowAbilityDetails(AbilityType);
        MenuManager.Instance.transform.Find("Character Window/Ability Select").gameObject.SetActive(true);
        MenuManager.Instance.HideEnergySelection();
        MenuManager.Instance.HideStanceSelection();
        MenuManager.Instance.transform.Find("Character Window/Effects").gameObject.SetActive(false);
        MenuManager.Instance.AbilityBeingChanged = ability_being_changed;
        EventManager.CancelButtonPressed.AddListener(MenuManager.Instance.HideAbilitySelection);
        EventManager.ExitMenu.AddListener(MenuManager.Instance.HideAbilitySelection);
        if(Settings.Instance.ControlScheme == "Gamepad") {
            MenuManager.Instance.transform.Find("Character Window/Ability Select/Abilities/Anima/1").GetComponent<Button>().Select();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowAbilityDetails(AbilityType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MenuManager.Instance.ShowAbilityDetails(AbilityType);
    }
}
