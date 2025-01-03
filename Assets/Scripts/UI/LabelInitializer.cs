using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelInitializer : MonoBehaviour {

    public string OriginalValue;
    [HideInInspector]
    public List<string> string_params;
    public bool DisableDetailedDescription = false;
    public void Start() {
        if (string.IsNullOrEmpty(OriginalValue))
        {
            OriginalValue = GetComponent<TextMeshProUGUI>().text;
        }
        LoadLabel();
    }

    public void LoadLabel()
    {
        if (string.IsNullOrEmpty(OriginalValue))
        {
            OriginalValue = GetComponent<TextMeshProUGUI>().text;
        }
        if (string_params != null && string_params.Count > 0)
        {
            string format = Utils.InsertLabelsIntoText(OriginalValue, gameObject);
            string formatted_string = string.Format(format, string_params.ToArray());
            GetComponent<TextMeshProUGUI>().text = formatted_string;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = Utils.InsertLabelsIntoText(OriginalValue, gameObject);
        }
    }

    public void SetLabel(string label)
    {
        OriginalValue = label;
        LoadLabel();
    }

}