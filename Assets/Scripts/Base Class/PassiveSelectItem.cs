using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveSelectItem : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ICancelHandler
{
    public int ChoiceSetNumber = 1;

    public void Start()
    {
        transform.Find("Icon").gameObject.SetActive(!gameObject.name.Contains("Stance_"));
        transform.Find("Stance").gameObject.SetActive(gameObject.name.Contains("Stance_"));
        if(gameObject.name.Contains("_UpgradeA"))
        {
            string ability_name = gameObject.name.Replace("_UpgradeA", "");
            transform.Find("Title/Text").GetComponent<TextMeshProUGUI>().text = Label.Get("AbilityMasteryTitle") + ": " + Label.Get(ability_name);
            Type ability_type = Type.GetType(ability_name);
            MethodInfo desc = ability_type.GetMethod("GetMasteryADescriptionValues", BindingFlags.Public | BindingFlags.Static);
            if (desc != null)
            {
                transform.Find("Description").GetComponent<LabelInitializer>().string_params = (List<string>)desc.Invoke(null, null);
                transform.Find("Description").GetComponent<LabelInitializer>().SetLabel("{" + ability_name + "_MasteryA_Description}");
            }
            else
            {
                transform.Find("Description").GetComponent<TextMeshProUGUI>().text = Label.Get(ability_name + "_MasteryA_Description");
            }
            transform.Find("Icon/Image").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(ability_name);
        }
        else if (gameObject.name.Contains("_UpgradeB"))
        {
            string ability_name = gameObject.name.Replace("_UpgradeB", "");
            transform.Find("Title/Text").GetComponent<TextMeshProUGUI>().text = Label.Get("AbilityMasteryTitle") + ": " + Label.Get(ability_name);
            Type ability_type = Type.GetType(ability_name);
            MethodInfo desc = ability_type.GetMethod("GetMasteryBDescriptionValues", BindingFlags.Public | BindingFlags.Static);
            if (desc != null)
            {
                transform.Find("Description").GetComponent<LabelInitializer>().string_params = (List<string>)desc.Invoke(null, null);
                transform.Find("Description").GetComponent<LabelInitializer>().SetLabel("{" + ability_name + "_MasteryB_Description}");
            }
            else
            {
                transform.Find("Description").GetComponent<TextMeshProUGUI>().text = Label.Get(ability_name + "_MasteryB_Description");
            }
            transform.Find("Icon/Image").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(ability_name);
        }
        else if (gameObject.name.Contains("Stance_"))
        {
            string ability_name = gameObject.name.Replace("_Unlock", "").Replace("_Upgrade1", "").Replace("_Upgrade2", "").Replace("_Upgrade3", "");
            Type ability_type = Type.GetType(ability_name);
            string upgrade_number = gameObject.name.Contains("_Upgrade") ? gameObject.name.Substring(gameObject.name.Length - 1, 1) : "0";
            FieldInfo family = ability_type.GetField("Family", BindingFlags.Public | BindingFlags.Static);
            transform.Find("Stance/Stance Border").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + ability_name, typeof(Sprite)) as Sprite;
            transform.Find("Stance/Border Upper").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
            transform.Find("Stance/Border Lower").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
            transform.Find("Stance").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
            transform.Find("Title/Text").GetComponent<TextMeshProUGUI>().text = Label.Get(upgrade_number == "0" ? "StanceUnlockTitle" : "UpgradeUnlockTitle") + (upgrade_number == "0" ? "" : " " + upgrade_number) +": " + Label.Get(ability_name);
            transform.Find("Description").GetComponent<LabelInitializer>().SetLabel("{" + gameObject.name.Replace("_Unlock", "") + "_Description}");
        }
        else if (gameObject.name.Contains("_Unlock"))
        {
            string ability_name = gameObject.name.Replace("_Unlock", "");
            transform.Find("Title/Text").GetComponent<TextMeshProUGUI>().text = Label.Get("AbilityUnlockTitle") + ": " + Label.Get(ability_name);
            transform.Find("Description").GetComponent<LabelInitializer>().SetLabel("{" + gameObject.name.Replace("_Unlock", "") + "_Description}");
            transform.Find("Icon/Image").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(ability_name);
            Type abilityType = Type.GetType( ability_name);
            MethodInfo desc = abilityType.GetMethod("GetDescriptionValues", BindingFlags.Public | BindingFlags.Static);
            if (desc != null)
            {
                transform.Find("Description").GetComponent<LabelInitializer>().string_params = Utils.RoundAllNumbers((List<string>)desc.Invoke(null, null));
            }
        }
        else
        {
            string[] stat_name_split = gameObject.name.Split('~');
            transform.Find("Title/Text").GetComponent<TextMeshProUGUI>().text = Label.Get("Effect_" + stat_name_split[0]);
            transform.Find("Description").GetComponent<TextMeshProUGUI>().text = Label.Get("Effect_"  + stat_name_split[0] + "_Description");
            transform.Find("Icon/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + Utils.GetImageNameForStat(stat_name_split[0]), typeof(Sprite)) as Sprite;
            if (stat_name_split.Length > 1)
            {
                transform.Find("Description").GetComponent<LabelInitializer>().string_params = new List<string> {stat_name_split[1]};
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PassiveSelect passive_select = GetComponentInParent<PassiveSelect>();
        if (passive_select.Selection[ChoiceSetNumber - 1] == gameObject.name)
        {
            SetSelectionActive(false);
        }
        else
        {
            if(string.IsNullOrEmpty(passive_select.Selection[ChoiceSetNumber - 1]) == false)
            {
                passive_select.transform.Find(passive_select.Selection[ChoiceSetNumber - 1]).GetComponent<PassiveSelectItem>().SetSelectionActive(false);
            }
            SetSelectionActive(true);
        }
        passive_select.CheckIfEnableConfirmButton();
    }

    public void SetSelectionActive(bool active)
    {
        if(active)
        {
            GetComponentInParent<PassiveSelect>().Selection[ChoiceSetNumber - 1] = gameObject.name;
            transform.Find("Stroke").GetComponent<Image>().color = Colors.SelectedColor;
            GetComponent<Button>().Select();
        }
        else
        {
            GetComponentInParent<PassiveSelect>().Selection[ChoiceSetNumber - 1] = null;
            transform.Find("Stroke").GetComponent<Image>().color = Color.white;
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PassiveSelect passive_select = GetComponentInParent<PassiveSelect>();
        if (passive_select.Selection[ChoiceSetNumber - 1] == gameObject.name)
        {
            SetSelectionActive(false);
        }
        else
        {
            if (string.IsNullOrEmpty(passive_select.Selection[ChoiceSetNumber - 1]) == false)
            {
                passive_select.transform.Find(passive_select.Selection[ChoiceSetNumber - 1]).GetComponent<PassiveSelectItem>().SetSelectionActive(false);
            }
            SetSelectionActive(true);
        }
        passive_select.CheckIfEnableConfirmButton();
    }

    public void OnCancel(BaseEventData eventData)
    {
        SetSelectionActive(false);
        PassiveSelect passive_select = GetComponentInParent<PassiveSelect>();
        passive_select.CheckIfEnableConfirmButton();
        foreach(Transform child in transform.parent)
        {
            child.GetComponent<PassiveSelectItem>().SetSelectionActive(false);
        }
    }
}
