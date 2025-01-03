using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ExecuteFunctionOnEvent : MonoBehaviour, ISelectHandler
{
    public UnityEvent OnSelect;

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        OnSelect.Invoke();
    }
}
