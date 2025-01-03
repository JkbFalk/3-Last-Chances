using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeSelectOnInput : MonoBehaviour, ISubmitHandler, ICancelHandler
{
    public Selectable SelectOnSubmit;
    public Selectable SelectOnCancel;

    public void OnSubmit(BaseEventData eventData)
    {
        if(SelectOnSubmit != null)
        {
            SelectOnSubmit.Select();
        }
    }
    public void OnCancel(BaseEventData eventData)
    {
        if (SelectOnCancel != null)
        {
            SelectOnCancel.Select();
        }
    }
}
