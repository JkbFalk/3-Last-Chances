using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownLabelInitializer : MonoBehaviour, IPointerClickHandler
{
    public string PathToLabel = "Template/Viewport/Content/Item/Item Label";
    public string PathToContent = "Dropdown List/Viewport/Content";
    public void Start()
    { 
        transform.Find(PathToLabel).gameObject.AddComponent<LabelInitializer>();
        if(transform.Find("Label") != null)
        {
            transform.Find("Label").gameObject.AddComponent<LabelInitializer>();
        }
        GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate {
            DropdownValueChanged(GetComponent<TMP_Dropdown>());
        });
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        if(transform.Find("Label") != null)
        {
            transform.Find("Label").GetComponent<LabelInitializer>().SetLabel(change.captionText.text);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.Find("Dropdown List") != null) {
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            Transform content = transform.Find(PathToContent);
            int option_count = 0;
            for (int i = 0; i < content.childCount; i++)
            {
                if(content.GetChild(i).gameObject.activeSelf && content.GetChild(i).gameObject.name != "Image" && content.GetChild(i).gameObject.activeSelf)
                {
                    content.GetChild(i).Find("Item Label").GetComponent<LabelInitializer>().OriginalValue = dropdown.options[option_count].text;
                    content.GetChild(i).Find("Item Label").GetComponent<LabelInitializer>().Start();
                    option_count++;
                }
            }
        }
    }
}
