using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubMenuItem : MonoBehaviour, IPointerDownHandler
{
    public int Index;
    public void OnPointerDown(PointerEventData eventData)
    {
        MenuManager.Instance.SelectedSubMenu = Index;
    }
}
