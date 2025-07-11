using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MasteryUnlockTile : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    public string Ability;
    public Type AbilityType;
    public string Row;
    public string Tree;

    public string Mastery;

    public void Start() {
        Row = transform.parent.parent.gameObject.name;
        Tree = transform.parent.parent.parent.parent.parent.parent.gameObject.name;
        AbilityType = Type.GetType(Ability);
        Mastery = gameObject.name == "1" || gameObject.name == "3" ? "A" : "B";
    }

    public void OnClick() {
        if((Mastery == "A" && SaveFile.Instance.AbilitiesMasteryA.Contains(AbilityType)) || (Mastery == "B" && SaveFile.Instance.AbilitiesMasteryB.Contains(AbilityType))) {
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
        if(Mastery == "A") {
            if(SaveFile.Instance.AbilitiesMasteryB.Contains(AbilityType)) {
                transform.parent.Find(gameObject.name == "1" ? "2" : "4").GetComponent<MasteryUnlockTile>().RefundTile();
            }
            SaveFile.Instance.UnlockAbilityMasteryA(AbilityType);
        }
        else {
            if(SaveFile.Instance.AbilitiesMasteryA.Contains(AbilityType)) {
                transform.parent.Find(gameObject.name == "2" ? "1" : "3").GetComponent<MasteryUnlockTile>().RefundTile();
            }
            SaveFile.Instance.UnlockAbilityMasteryB(AbilityType);
        }
    }

    public void RefundTile() {
        if(Mastery == "A" && SaveFile.Instance.AbilitiesMasteryA.Contains(AbilityType)) {
            SaveFile.Instance.UnlearnAbilityMasteryA(AbilityType);
        }
        else if(Mastery == "B" && SaveFile.Instance.AbilitiesMasteryB.Contains(AbilityType)) {
            SaveFile.Instance.UnlearnAbilityMasteryB(AbilityType);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) {
            MenuManager.Instance.ShowSkillTreeMasteryDetails(this);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowSkillTreeMasteryDetails(this);
    }

    public void UpdateUnlockedStatus() {
        if(AbilityType == null) {
            Start();
        }
        bool unlocked = (Mastery == "A" && SaveFile.Instance.AbilitiesMasteryA.Contains(AbilityType)) || (Mastery == "B" && SaveFile.Instance.AbilitiesMasteryB.Contains(AbilityType));
        transform.Find("Mask").GetComponent<Image>().color = unlocked ? Colors.GetFamilyColor(Tree) : Color.black;
        transform.parent.parent.Find("Connector" + gameObject.name).GetComponent<Image>().color = unlocked ? Colors.UnlockedTile : Color.black;
    }
}
