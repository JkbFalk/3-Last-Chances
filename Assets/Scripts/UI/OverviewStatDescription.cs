using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverviewStatDescription : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.ShowStatDetails(gameObject.name);
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowStatDetails(gameObject.name);
    }
}
