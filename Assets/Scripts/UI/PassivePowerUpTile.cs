using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassivePowerUpTile : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    [HideInInspector]
    public string Id;
    [HideInInspector]
    public string Tree;
    [HideInInspector]
    public string Row;
    public string PowerUp;
    public float[] SplitPowerBudget;
    public bool IsSkillTreeSpeciality = true;
    [HideInInspector]
    public bool IsAvailable = false;
    public List<Effect> PowerUpEffects = new List<Effect>();

    private void Start() {
        Tree = transform.parent.parent.parent.parent.parent.name;
        Row = transform.parent.name;
        Id = Tree + "-" + Row + "-" + gameObject.name;
        IsAvailable = Row == "1";
        transform.parent.Find("Disabled").gameObject.SetActive(!IsAvailable);
    }

    public void OnClick() {
        if(SaveFile.Instance.UnlockedPowerUps.Contains(Id)) {
            RefundTile();
        }
        else if(CheckIfCanUnlockTile()) {
            UnlockTile();
        }
    }

    public bool CheckIfCanUnlockTile() {
        int row = int.Parse(Row);
        if(row >= 10 && SaveFile.Instance.Level < 30) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {30}));
        }
        else if(row >= 7 && SaveFile.Instance.Level < 20) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {20}));
        }
        else if(row >= 4 && SaveFile.Instance.Level < 10) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {10}));
        }
        else if(SaveFile.Instance.MaxPassivePowerUps == 0 && SaveFile.Instance.UnlockedPowerUps.FirstOrDefault(powerup => powerup.StartsWith(Tree + "-" + Row)) == null) {
            NotificationController.ShowTextNotification(Label.Get("PowerUpPointsRequired"));
        }
        else {
            return true;
        }
        return false;
    }

    public void UnlockTile() {
        if(IsAvailable == false) {
            NotificationController.ShowTextNotification(Label.Get("PreviousPowerUpTierRequired"));
            return;
        }
        if(Row == "12" && SaveFile.Instance.UnlockedPowerUps.FirstOrDefault(power_up => power_up.Contains("12") && !power_up.Contains(Tree)) != null) {
            NotificationController.ShowTextNotification(Label.Get("OnlyOneHealingPowerUp"));
            return;
        }
        foreach(PassivePowerUpTile power_up in transform.parent.GetComponentsInChildren<PassivePowerUpTile>()) {
            if(!power_up.RefundTile(true)) {
                return;
            }
        }
        SaveFile.Instance.UnlockedPowerUps.Add(Id);
        ApplyPowerUpOfThisTile();
        SaveFile.Instance.UsedPassivePowerUps++;
        CheckIfRowIsActive(transform.parent.Find("Active"));
        GetComponent<Image>().color = Colors.Gold;
        MenuManager.Instance.ShowPowerUpDetails(this, true);
    }

    public bool RefundTile(bool swap_power_up = false) {
        if(SaveFile.Instance.UnlockedPowerUps.Contains(Id)) {
            if(swap_power_up == false && CheckIfAlreadyUnlockedHigherTier()) {
                NotificationController.ShowTextNotification("CannotRefundPowerUpWhenHigherTierUnlocked");
                return false;
            }
            SaveFile.Instance.UnlockedPowerUps.Remove(Id);
            foreach(Effect effect in PowerUpEffects) {
                Player.Instance.EndEffect(effect);
            }
            SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] = SaveFile.Instance.UnlockedPowerUps.Where(p => p.StartsWith(Tree)).Count();
            MenuManager.Instance.transform.Find("Skill Tree Window/Category Selection/" + Tree + "/Text").GetComponent<TextMeshProUGUI>().text = Tree + " (" + SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] + ")";
            SaveFile.Instance.UsedPassivePowerUps--;
            CheckIfRowIsActive(transform.parent.Find("Active"));
            GetComponent<Image>().color = Color.black;
            MenuManager.Instance.ShowPowerUpDetails(this, true);
        }
        return true;
    }

    public bool CheckIfAlreadyUnlockedHigherTier() {
        int row = int.Parse(Row);
        for(int i = row + 1; i < 12; i++) {
            if(SaveFile.Instance.UnlockedPowerUps.FirstOrDefault(power_up => power_up.StartsWith(Tree + "-" + i)) != null) {
                return true;
            }
        }
        return false;
    }

    public void AddPassivePowerUp(string power_up_name) {
        PowerUpEffects.AddRange(EffectList.GetEffect(power_up_name, GetPowerBudgetForTile()));
        SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] = SaveFile.Instance.UnlockedPowerUps.Where(p => p.StartsWith(Tree)).Count();
        MenuManager.Instance.transform.Find("Skill Tree Window/Category Selection/" + Tree + "/Text").GetComponent<TextMeshProUGUI>().text = Tree + " (" + SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] + ")";
    }

    public void ApplyPowerUpOfThisTile() {
        string[] power_ups = PowerUp.Split("+");
        PowerUpEffects = new List<Effect>();
        for(int i = 0; i < power_ups.Length; i++) {
            AddPassivePowerUp(power_ups[i]);
        }
        foreach(Effect e in PowerUpEffects) {
            e.IsRemovable = false;
            e.ShowsInMenu = false;
            Player.Instance.AddEffect(e);
        }
    }

    public float GetPowerBudgetForTile(int index = 0) {
        float amount = 0;
        if(Row == "12") {
            amount = PB.ROW12_HEAL_POWER_UP_PB;
        }
        else {
            amount = IsSkillTreeSpeciality ? PB.SPECIALITY_SKILL_TREE_PB : PB.NON_SPECIALITY_SKILL_TREE_PB;
        }
        if(SplitPowerBudget != null && SplitPowerBudget.Length > 0) {
            if(SplitPowerBudget.Length < index) {
                Debug.LogError($"Index ({index}) too high for split power budget: " + Utils.GetGameObjectPath(gameObject));
                return 0;
            }
            amount *= SplitPowerBudget[index];
        }
        return amount;
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowPowerUpDetails(this);
    }

    public void UpdateUnlockedStatus() {
        if(string.IsNullOrWhiteSpace(Tree)) {
            Start();
        }
        GetComponent<Image>().color = SaveFile.Instance.UnlockedPowerUps.Contains(Id) ? Colors.Gold : Color.black; 
        CheckIfRowIsActive(transform.parent.Find("Active"));
        MenuManager.Instance.transform.Find("Skill Tree Window/Category Selection/" + Tree + "/Text").GetComponent<TextMeshProUGUI>().text = Tree + " (" + SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] + ")";
    }

    public void CheckIfRowIsActive(Transform row) {
        bool isActive = false;
        for(int i = 1; i < 5; i++) {
            if(transform.parent.Find(i.ToString()) != null && SaveFile.Instance.UnlockedPowerUps.Contains(transform.parent.Find(i.ToString()).GetComponent<PassivePowerUpTile>().Id)) {
                isActive = true;
            }
        }
        row.gameObject.SetActive(isActive);
        if(Row != "12" && !String.IsNullOrEmpty(Row)) {
            int nextRow = Int32.Parse(Row) + 1;
            bool highEnoughLevel = (nextRow < 4 || SaveFile.Instance.Level >= 5) && (nextRow < 7 || SaveFile.Instance.Level >= 15) && (nextRow < 10 || SaveFile.Instance.Level >= 30);
            for(int i = 1; i < 5; i++) {
                if(i == 3 && Row == "11") {
                    break;
                }
                transform.parent.parent.Find((Int32.Parse(Row) + 1) + "/" + i).GetComponent<PassivePowerUpTile>().IsAvailable = highEnoughLevel && isActive;
            }
            transform.parent.parent.Find((Int32.Parse(Row) + 1).ToString() + "/Disabled").gameObject.SetActive(!highEnoughLevel || !isActive);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) {
            MenuManager.Instance.ShowPowerUpDetails(this);
        }
    }
}
