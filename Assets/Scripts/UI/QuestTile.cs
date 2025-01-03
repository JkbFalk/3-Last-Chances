using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestTile : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Quest Quest;
    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowQuestDetails(Quest);
    }

    public void OnDeselect(BaseEventData eventData) {
    }

    public void OnSubmit(BaseEventData eventData)
    {
        MenuManager.Instance.SelectedQuestTile = this;
        if(MenuManager.Instance.transform.Find("Journal Window/Description Window/Viewport/Quest Details Scrollbar").gameObject.activeSelf) {
            MenuManager.Instance.transform.Find("Journal Window/Description Window/Viewport/Quest Details Scrollbar").GetComponent<Scrollbar>().Select();
        }
    }
}
