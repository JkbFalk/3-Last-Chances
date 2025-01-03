using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    void Start()
    {
        Navigation nav = transform.Find("Background").GetComponent<Button>().navigation;
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = gameObject.name == "-1" ? transform.parent.Find(Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + "/Background").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) - 1).ToString() + "/Background").GetComponent<Button>();
        nav.selectOnLeft = transform.parent.parent.parent.Find("Back Button").GetComponent<Button>();
        nav.selectOnRight = transform.Find("Delete").GetComponent<Button>();
        nav.selectOnDown = gameObject.name == Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES.ToString() ? transform.parent.Find("-1/Background").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) + 1).ToString() + "/Background").GetComponent<Button>();
        transform.Find("Background").GetComponent<Button>().navigation = nav;
        transform.Find("Background").gameObject.AddComponent<CenterScrollRectOnItemWhenSelected>();

        Navigation nav2 = transform.Find("Delete").GetComponent<Button>().navigation;
        nav2.mode = Navigation.Mode.Explicit;
        nav2.selectOnUp = gameObject.name == "-1" ? transform.parent.Find(Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + "/Delete").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) - 1).ToString() + "/Delete").GetComponent<Button>();
        nav2.selectOnLeft = transform.Find("Background").GetComponent<Button>();
        nav2.selectOnRight = transform.parent.parent.parent.Find("Back Button").GetComponent<Button>();
        nav2.selectOnDown = gameObject.name == Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES.ToString() ? transform.parent.Find("-1/Delete").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) + 1).ToString() + "/Delete").GetComponent<Button>();
        transform.Find("Delete").GetComponent<Button>().navigation = nav2;
    }

    public void ShowDeleteSaveModal() {
        GameController.Instance.ShowDeleteSaveModal(Int32.Parse(gameObject.name));
    }
}
