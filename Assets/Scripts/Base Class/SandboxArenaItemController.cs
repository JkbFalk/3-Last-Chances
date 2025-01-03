using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Slider = UnityEngine.UI.Slider;

public class SandboxArenaItemController : MonoBehaviour, ISubmitHandler, ICancelHandler
{
    private float _savedValue = 0;
    private Slider _slider;
    private Button _button;

    public void Start()
    {
        _slider = GetComponent<Slider>();
        _button = transform.parent.Find("Edit Button").GetComponent<Button>();
        if (Settings.Instance.ControlScheme == "Gamepad")
        {
            _button.gameObject.SetActive(true);
            _slider.gameObject.SetActive(false);
        }
    }
    public void EnableEdit()
    {
        _button.gameObject.SetActive(false);
        _slider.gameObject.SetActive(true);
        _slider.Select();
        _savedValue = _slider.value;
    }

    public void UpdateAmount()
    {
        if(_slider != null)
        {
            transform.parent.Find("Amount").GetComponent<TextMeshProUGUI>().text = _slider.value.ToString();
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        _button.gameObject.SetActive(true);
        _slider.gameObject.SetActive(false);
        _button.Select();
    }

    public void OnCancel(BaseEventData eventData)
    {
        _slider.value = _savedValue;
        _button.gameObject.SetActive(true);
        _slider.gameObject.SetActive(false);
        _button.Select();
    }
}
