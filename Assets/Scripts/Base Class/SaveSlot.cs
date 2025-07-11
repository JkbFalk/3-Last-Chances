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
        /*Navigation nav = transform.Find("Background").GetComponent<Button>().navigation;
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = gameObject.name == "-3" ? transform.parent.Find(Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + "/Background").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) - 1).ToString() + "/Background").GetComponent<Button>();
        nav.selectOnLeft = transform.parent.parent.parent.Find("Back Button").GetComponent<Button>();
        nav.selectOnRight = transform.Find("Delete Button").GetComponent<Button>();
        nav.selectOnDown = gameObject.name == Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES.ToString() ? transform.parent.Find("-3/Background").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) + 1).ToString() + "/Background").GetComponent<Button>();
        transform.Find("Background").GetComponent<Button>().navigation = nav;
        transform.Find("Background").gameObject.AddComponent<CenterScrollRectOnItemWhenSelected>();

        Navigation nav2 = transform.Find("Delete Button").GetComponent<Button>().navigation;
        nav2.mode = Navigation.Mode.Explicit;
        nav2.selectOnUp = gameObject.name == "-3" ? transform.parent.Find(Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES + "/Delete Button").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) - 1).ToString() + "/Delete Button").GetComponent<Button>();
        nav2.selectOnLeft = transform.Find("Background").GetComponent<Button>();
        nav2.selectOnRight = transform.parent.parent.parent.Find("Back Button").GetComponent<Button>();
        nav2.selectOnDown = gameObject.name == Constants.MAXIMUM_AMOUNT_OF_SAVE_FILES.ToString() ? transform.parent.Find("-3/Delete Button").GetComponent<Button>() : transform.parent.Find((Int32.Parse(gameObject.name) + 1).ToString() + "/Delete Button").GetComponent<Button>();
        transform.Find("Delete Button").GetComponent<Button>().navigation = nav2;*/
    }

    public void ShowDeleteSaveModal() {
        GameController.Instance.SaveSlotNumberCurrentlySelected = Int32.Parse(gameObject.name);
        GameController.Instance.ActionToExecuteOnPromptConfirm = GameController.Instance.DeleteSave;
        GameController.Instance.ShowDeleteSaveModal(Int32.Parse(gameObject.name));
    }

    public void ShowSaveOrLoadModal() {
        if(GameController.Instance.GameplayMode != Constants.GameplayMode.OnStartScreen && (!GameController.Instance.Saving || ES3.FileExists("SaveFile" + Int32.Parse(gameObject.name) + ".es3"))) {
            GameController.Instance.SaveSlotNumberCurrentlySelected = Int32.Parse(gameObject.name);
            GameController.Instance.ActionToExecuteOnPromptConfirm = GameController.Instance.SaveOrLoadGame;
            GameController.Instance.ShowSaveOrLoadModal(Int32.Parse(gameObject.name));
        }
        else {
            GameController.Instance.SaveOrLoadGame(Int32.Parse(gameObject.name));
        }
    }
}
