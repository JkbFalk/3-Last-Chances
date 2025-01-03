using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestDetailsScrollbar : MonoBehaviour, ICancelHandler
{
    public void OnCancel(BaseEventData eventData)
    {
        MenuManager.Instance.SelectedQuestTile.GetComponent<Button>().Select();
    }
}
