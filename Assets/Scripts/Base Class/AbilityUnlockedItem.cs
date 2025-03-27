using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilityUnlockedItem : MonoBehaviour
{
    void Start()
    {
        string ability_name = gameObject.name.Replace("_Unlock", "");
        transform.Find("Title/Text").GetComponent<TextMeshProUGUI>().text = Label.Get(ability_name);
        transform.Find("Description").GetComponent<TextMeshProUGUI>().text = Label.Get(ability_name + "_Description");
        transform.Find("Icon/Image").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(ability_name);
    }

}
