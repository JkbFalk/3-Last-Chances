using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LabelInitializer : MonoBehaviour, IPointerMoveHandler {

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
    private TextMeshProUGUI _tmp;
    public TextMeshProUGUI TextMeshPro {
        get {
            if(_tmp == null) {
                _tmp = GetComponent<TextMeshProUGUI>();
            }
            return _tmp;
        }
    }

    public void Start() {
        PlayerControls.AllLabelInitializers.Add(this);
        if (string.IsNullOrEmpty(OriginalValue))
        {
            OriginalValue = TextMeshPro.text;
        }
        LoadLabel();
    }

    public void LoadLabel()
    {
        if (string.IsNullOrEmpty(OriginalValue))
        {
            OriginalValue = TextMeshPro.text;
        }
        if (string_params != null && string_params.Count > 0)
        {
            string format = Utils.InsertLabelsIntoText(OriginalValue, gameObject);
            string formatted_string = string.Format(format, string_params.ToArray());
            TextMeshPro.text = formatted_string;
        }
        else
        {
            TextMeshPro.text = Utils.InsertLabelsIntoText(OriginalValue, gameObject);
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

    public void SetLabelWithIncrementedToken(string label, List<string> function_params)
    {
        OriginalValue = label;
        int index = 0;
        List<String> extractedIds = ExtractLabelIds(label);
        for(int i = 0; i < extractedIds.Count; i++) {
            label = label.Replace("{" + extractedIds[i] + "}", GetLabelWithIncrementedTokens(extractedIds[i], ref index));
        }
        if (function_params != null && function_params.Count > 0)
        {
            string format = Utils.InsertLabelsIntoText(label, gameObject);
            string formatted_string = string.Format(format, function_params.ToArray());
            TextMeshPro.text = formatted_string;
        }
        else
        {
            TextMeshPro.text = Utils.InsertLabelsIntoText(label, gameObject);
        }
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

    public void OnPointerMove(PointerEventData eventData)
    {
		int linkIndex = TMP_TextUtilities.FindIntersectingLink(TextMeshPro, Input.mousePosition, null);
        if(linkIndex == -1) {
            if(MenuManager.Instance.TooltipDisplay.ChangeInProgress == false && MenuManager.Instance.TooltipDisplay.Visiblity > 0) {
                MenuManager.Instance.TooltipDisplay.HideOverTimeFromFull(0.12f);
                MenuManager.Instance.TooltipTransformChange.SetScaleChangeOverTime(0.2f, MenuManager.Instance.TooltipTransformChange.transform.localScale.x, 0.05f);
            }
            return;
        }
		string linkId = TextMeshPro.textInfo.linkInfo[linkIndex].GetLinkID();
        if(Input.mousePosition.x < Screen.width * 0.18f && Input.mousePosition.y > Screen.height * 0.7f) {
            MenuManager.Instance.Tooltip.transform.parent.gameObject.GetComponent<RectTransform>().pivot = new Vector2(-0.03f, 1.03f);
        }
        else if(Input.mousePosition.x < Screen.width * 0.18f && Input.mousePosition.y <= Screen.height * 0.7f) {
            MenuManager.Instance.Tooltip.transform.parent.gameObject.GetComponent<RectTransform>().pivot = new Vector2(-0.03f, -0.03f);
        }
        else if(Input.mousePosition.x >= Screen.width * 0.18f && Input.mousePosition.y > Screen.height * 0.7f) {
            MenuManager.Instance.Tooltip.transform.parent.gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.03f, 1.03f);
        }
        else{
            MenuManager.Instance.Tooltip.transform.parent.gameObject.GetComponent<RectTransform>().pivot = new Vector2(1.03f, -0.03f);
        }
        MenuManager.Instance.Tooltip.transform.parent.gameObject.transform.position = Input.mousePosition;
        MenuManager.Instance.Tooltip.transform.parent.gameObject.SetActive(true);
        if(MenuManager.Instance.TooltipDisplay.ChangeInProgress == false && MenuManager.Instance.TooltipDisplay.Visiblity < 1) {
            MenuManager.Instance.TooltipDisplay.ShowOverTimeFromZero(0.12f);
            MenuManager.Instance.TooltipTransformChange.SetScaleChangeOverTime(0.2f, MenuManager.Instance.TooltipTransformChange.transform.localScale.x, 1);
            MenuManager.Instance.TooltipDisplay.CheckIfShouldHideOnceFinishedChanging = true;
            MenuManager.Instance.TooltipDisplay.LabelInitializer = this;
        }
        if(linkId == "FirstItemEffects") {
            List<Effect> ef = EffectList.GetEffect(MenuManager.Instance.CurrentDetailedItemDescription.FirstItemEffects[0].EffectName, MenuManager.Instance.CurrentDetailedItemDescription.FirstItemEffects[0].PortionOfPowerBudget * MenuManager.Instance.CurrentDetailedItemDescription.GetItemFirstEffectPB());
            MenuManager.Instance.Tooltip.string_params = ef[0].DescriptionParameters;
            MenuManager.Instance.Tooltip.SetLabel("{Effect_" + MenuManager.Instance.CurrentDetailedItemDescription.FirstItemEffects[0].EffectName + "_DescriptionDetailed}");
        }
        else if(linkId == "SecondItemEffects") {
            List<Effect> ef = EffectList.GetEffect(MenuManager.Instance.CurrentDetailedItemDescription.SecondItemEffects[0].EffectName, MenuManager.Instance.CurrentDetailedItemDescription.SecondItemEffects[0].PortionOfPowerBudget * MenuManager.Instance.CurrentDetailedItemDescription.GetItemSecondEffectPB());
            MenuManager.Instance.Tooltip.string_params = ef[0].DescriptionParameters;
            MenuManager.Instance.Tooltip.SetLabel("{Effect_" + MenuManager.Instance.CurrentDetailedItemDescription.SecondItemEffects[0].EffectName + "_DescriptionDetailed}");
        }
        else {
            MenuManager.Instance.Tooltip.SetLabel("{" + linkId + "}");
        }
    }

    public void CheckIfShouldHide() {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(TextMeshPro, Input.mousePosition, null);
        if(linkIndex == -1) {
            if(MenuManager.Instance.TooltipDisplay.Visiblity > 0) {
                MenuManager.Instance.TooltipDisplay.HideOverTimeFromFull(0.15f);
                MenuManager.Instance.TooltipTransformChange.SetScaleChangeOverTime(0.3f, 1, 0.05f);
            }
            return;
        }
    }
}