using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueUpdater : MonoBehaviour
{
    private LabelInitializer _labelInit;
    private Slider _slider;

    public void Start()
    {
        _slider = GetComponent<Slider>();
        transform.Find("Left Display").GetComponent<TextMeshProUGUI>().text = _slider.minValue.ToString();
        transform.Find("Right Display").GetComponent<TextMeshProUGUI>().text = _slider.maxValue.ToString();
        _labelInit = transform.Find("Label").GetComponent<LabelInitializer>();
        _labelInit.Start();
        UpdateValuesDisplayed();
    }
    public void UpdateValuesDisplayed()
    {
        if(_labelInit != null)
        {
            _labelInit.string_params = new List<string> { _slider.value.ToString() };
            _labelInit.LoadLabel();
        }
    }
}
