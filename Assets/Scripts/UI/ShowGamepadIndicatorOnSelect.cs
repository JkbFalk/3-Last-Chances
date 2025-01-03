using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowGamepadIndicatorOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public float XAdjustment = -50;
    public float YAdjustment = 0;

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.SetGamepadIndicator(gameObject, XAdjustment, YAdjustment);
    }

    public void OnDeselect(BaseEventData eventData) {
        CanvasElements.MenuCanvas.GamepadIndicator.SetActive(false);
    }
}
