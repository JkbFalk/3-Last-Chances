using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightOnGamepadSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        if(Settings.Instance.ControlScheme == "Gamepad") {
            GetComponent<Image>().color = Colors.GetColorFromCode("#3A505E");
        }
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Image>().color = Color.black;
    }
}