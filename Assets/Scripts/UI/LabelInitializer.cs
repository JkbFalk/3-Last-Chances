using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelInitializer : MonoBehaviour {

    /*private string _originalValue;
    public string OriginalValue {
        get => _originalValue;
        set {
            //Debug.Log("CHANGING OriginalValue of " + gameObject.name + " from " + _originalValue + " to " +  value);
            _originalValue = value;
        }
    }*/
    public string OriginalValue;
    [HideInInspector]
    public List<string> string_params;
    public bool DisableDetailedDescription = false;
    public void Start() {
        PlayerControls.AllLabelInitializers.Add(this);
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

    public void RefreshLabel(bool detailed) {
        OriginalValue = OriginalValue.Replace((detailed && !OriginalValue.Contains("DescriptionDetailed")) ? "Description" : "DescriptionDetailed", detailed ? "DescriptionDetailed" : "Description");
        LoadLabel();
    }

    public void SetLabelWithIncrementedToken(string label, string[] tile_params)
    {
        OriginalValue = label;
        String text = "";
        int index = 0;
        List<String> extractedIds = ExtractLabelIds(label);
        for(int i = 0; i < extractedIds.Count; i++) {
            text += GetLabelWithIncrementedTokens(extractedIds[i], ref index) + "\n\n" + (SaveFile.Instance.UnlockedPowerUps.Contains(extractedIds[i].Replace("Effect_", "").Replace("DescriptionDetailed", "").Replace("Description", "")) ? ""  : Utils.GetCalculatedStatIncrease(extractedIds[i], tile_params[i]));
        }
        LoadLabel();
    }

    public List<string> ExtractLabelIds(string text) {
        List<String> extractedIds = new List<string>();
        String tempId = "";
        bool extractingId = false;
        for(int i = 0; i < text.Length; i++) {
            if(text[i] == '{') {
                extractingId = true;
            }
            else if(text[i] == '}') {
                extractingId = false;
                extractedIds.Add(tempId);
                Debug.Log("EXTRACTED ID! " + tempId);
                tempId = "";
            }
            else if(extractingId){
                tempId += text[i];
            }
        }
        return extractedIds;
    }

    public string GetLabelWithIncrementedTokens(string label, ref int start_index) {
        if(!Label.ContainsKey(label)) {
            Debug.LogError("Could not find label: " + label);
            return label;
        }
        string text = Label.Get(label);
        int highestParamIndex;
        for(highestParamIndex = 0; text.Contains("{" + highestParamIndex + "}"); highestParamIndex++);
        start_index += highestParamIndex - 1;
        for(int i = 0; i < text.Length; i++) {
            if(text[i] == '{' && text.Length > i+2 && text[i+2] == '}') {
                text = text.Substring(0, i+1) + (Int32.Parse(text[i+1].ToString()) + start_index).ToString() + text.Substring(i+2, text.Length - i - 2);
            }
        }
        if(highestParamIndex == 1) {
            start_index++;
        }
        return text;
    }

    public void OnDestroy() {
        PlayerControls.AllLabelInitializers.Remove(this);
    }
}