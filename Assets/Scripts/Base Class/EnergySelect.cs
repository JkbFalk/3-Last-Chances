using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnergySelect : MonoBehaviour, IPointerDownHandler, ISelectHandler, ISubmitHandler, IPointerEnterHandler
{
    public Ability.AbilityFamily Family;

    public void Start() {
        Family = 
        transform.parent.gameObject.name == "Anima" ? Ability.AbilityFamily.Anima : 
        transform.parent.gameObject.name == "Ignis" ? Ability.AbilityFamily.Ignis : 
        transform.parent.gameObject.name == "Glacies" ? Ability.AbilityFamily.Glacies : 
        transform.parent.gameObject.name == "Molis" ? Ability.AbilityFamily.Molis : 
        transform.parent.gameObject.name == "Salutis" ? Ability.AbilityFamily.Salutis : 
        transform.parent.gameObject.name == "Tonitrui" ? Ability.AbilityFamily.Tonitrui : 
        transform.parent.gameObject.name == "Proprius" ? Ability.AbilityFamily.Proprius : 
        Ability.AbilityFamily.None;
    }
    public void SelectEnergy() {
        MenuManager.Instance.HideEnergySelection();

        if(Player.Instance.EnergyEffect != null) {
            Player.Instance.EnergyEffect.EndThisEffect();
        }
        if(Family == Ability.AbilityFamily.None) {
            Player.Instance.EnergyEffect = null;
            SaveFile.Instance.EquippedEnergyFamily = Ability.AbilityFamily.None;
            MenuManager.Instance.UpdateEnergyBarVisuals();
            return;
        }
        Effect_EnergyUpgrade new_effect = (Effect_EnergyUpgrade)Activator.CreateInstance(Type.GetType("Effect_" + Family + "Energy"), new object[] {null});
        Player.Instance.EnergyEffect = new_effect;
        SaveFile.Instance.EquippedEnergyFamily = Family;
        new_effect.UpgradeLevel = SaveFile.Instance.EnergyUpgrades[Family];
        Player.Instance.AddEffect(new_effect);
        MenuManager.Instance.UpdateEnergyBarVisuals();
        MenuManager.Instance.ShowEnergyDetails(Family);
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowEnergyDetails(Family);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left) {
            SelectEnergy();
        }
        else if(eventData.button == PointerEventData.InputButton.Right) {
            MenuManager.Instance.ShowEnergyDetails(Family);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        SelectEnergy();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.ShowEnergyDetails(Family);
    }
}
