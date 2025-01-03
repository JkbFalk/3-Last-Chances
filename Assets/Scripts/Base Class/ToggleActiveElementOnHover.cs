using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ToggleActiveElementOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public string PathToElement;
    private Transform _element;

    private void Start()
    {
        if (string.IsNullOrEmpty(PathToElement) == false)
        {
            if(PathToElement.Contains("../")) {
                _element = transform.parent.Find(PathToElement.Replace("../", ""));
            }
            else
            {
                _element = transform.Find(PathToElement);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _element.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _element.gameObject.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _element.gameObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _element.gameObject.SetActive(false);
    }
}
