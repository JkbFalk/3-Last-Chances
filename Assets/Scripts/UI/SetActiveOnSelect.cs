using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetActiveOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject[] SetGameObjectsActiveOnSelect;
    public GameObject[] SetGameObjectsNotActiveOnSelect;
    public GameObject[] SetGameObjectsActiveOnDeselect;
    public GameObject[] SetGameObjectsNotActiveOnDeselect;

    public void OnSelect(BaseEventData eventData)
    {
        if(SetGameObjectsActiveOnSelect.Length > 0)
        {
            foreach(GameObject item in SetGameObjectsActiveOnSelect)
            {
                item.SetActive(true);
            }
        }
        if (SetGameObjectsNotActiveOnSelect.Length > 0)
        {
            foreach (GameObject item in SetGameObjectsNotActiveOnSelect)
            {
                item.SetActive(false);
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (SetGameObjectsActiveOnDeselect.Length > 0)
        {
            foreach (GameObject item in SetGameObjectsActiveOnDeselect)
            {
                item.SetActive(true);
            }
        }
        if (SetGameObjectsNotActiveOnDeselect.Length > 0)
        {
            foreach (GameObject item in SetGameObjectsNotActiveOnDeselect)
            {
                item.SetActive(false);
            }
        }
    }
}
