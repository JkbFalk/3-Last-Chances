using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubMenuItem : MonoBehaviour, IPointerDownHandler
{
    public string MenuType;
    public int Index;
    public void OnPointerDown(PointerEventData eventData)
    {
        if(MenuType == "Main Menu Selection") {
            MenuManager.Instance.SelectedSubMenu = Index;
        }
        else if(MenuType == "Options Menu Selection") {
            MenuManager.Instance.SelectedOptionsSubMenu = Index;
        }
    }
}
