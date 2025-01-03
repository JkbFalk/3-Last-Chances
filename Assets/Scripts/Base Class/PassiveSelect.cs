using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveSelect : MonoBehaviour {

    public string[] Selection;
    public List<string> PassiveList1;
    public List<string> PassiveList2;

    private void Start()
    {
        InitializeOptions();
    }

    public void InitializeOptions()
    {
        CanvasElements.LevelUpSelection.SetActive(true);
        for (int i = transform.childCount; i > 0; i--)
        {
            MonoBehaviour.Destroy(transform.GetChild(i-1).gameObject);
        }
        Selection = new string[2] { null, null };
        transform.parent.Find("1 Choice").gameObject.SetActive(PassiveList2.Count == 0);
        transform.parent.Find("2 Choices").gameObject.SetActive(PassiveList2.Count > 0);
        transform.parent.Find("Button").GetComponent<Button>().interactable = false;
        bool first_time = true;
        if (PassiveList1.Count > 0)
        {
            foreach (string passive_name in PassiveList1)
            {
                GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_PassiveSelectItem")) as GameObject;
                if(first_time && GameController.Instance.transform.Find("Unlock Information").gameObject.activeSelf == false)
                {
                    item.GetComponent<Button>().Select();
                    first_time = false;
                }
                item.name = passive_name;
                item.transform.SetParent(transform);
            }
        }
        transform.parent.Find("Options 1").gameObject.SetActive(PassiveList2.Count > 0);
        transform.parent.Find("Options 2").gameObject.SetActive(PassiveList2.Count > 0);
        if (PassiveList2.Count > 0)
        {
            foreach (string passive_name in PassiveList2)
            {
                GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_PassiveSelectItem")) as GameObject;
                item.name = passive_name;
                item.GetComponent<PassiveSelectItem>().ChoiceSetNumber = 2;
                item.transform.SetParent(transform);
                item.transform.SetAsLastSibling();
            }
        }
        else
        {
            Selection[1] = "Only one choice";
        }
    }

    public void ConfirmSelection()
    {
        SurvivalController.PowerUps.Add(Selection[0]);
        Utils.AddPowerUpToPlayer(Selection[0]);
        if(SurvivalController.SelectingAbilities)
        {
            SurvivalController.SelectingAbilities = false;
            GameController.Instance.WaitAndRunMethod(0.01f, ShowSurvivalPowerUpSelectionAfterAbilitySelect);
        }
        UIManager.Instance.DisplayAreaTransitionScreen(Label.Get("SurvivalLevelDisplay") + " " + SaveFile.Instance.SurvivalLevel.ToString());
        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities)
        {
            ability.AbilityGraphic.transform.Find("Disabled").gameObject.SetActive(!SaveFile.Instance.UnlockedAbilities.Contains(ability.Type));
        }
        CanvasElements.LevelUpSelection.SetActive(false);
        GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;
    }

    public void ShowSurvivalPowerUpSelectionAfterAbilitySelect()
    {
        if(SaveFile.Instance.SurvivalLevel == 1)
        {
            Utils.ShowLevelUpSelection(new List<string> { "Injury~30%", "Stagger~30%", "AttackSpeed~12%", "DamageReduction~15%" });
        }
        else
        {
            Utils.ShowLevelUpSelection(SurvivalController.Get4RandomPowerUps());
        }
    }

    public void CheckIfEnableConfirmButton()
    {
        if(PassiveList2.Count == 0)
        {
            transform.parent.Find("Button").GetComponent<Button>().interactable = !string.IsNullOrEmpty(Selection[0]);
        }
        else
        {
            transform.parent.Find("Button").GetComponent<Button>().interactable = !string.IsNullOrEmpty(Selection[0]) && !string.IsNullOrEmpty(Selection[1]);
        }
    }
}
