using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelectController : MonoBehaviour
{
    public void MissionDetailsConfirm() {
        MenuManager.Instance.StartMission();
    }

    public void MissionDetailsCancel() {
        MenuManager.Instance.HideMissionDetails();
    }

    public void ChooseSurvivalType(int option) {
        GameController.Instance.ChooseSurvivalType(option);
    }


    public void CancelSurvival() {
        transform.Find("Survival Type Selection").gameObject.SetActive(false);
    }

    public void SaveGame() {
        GameController.Instance.ToggleSavePanel(true);
    }

    public void LoadGame() {
        GameController.Instance.ToggleLoadPanel(true);
    }

    public void BackToTitle() {
        GameController.Instance.ReturnToTitle();
    }

    public void QuitGame() {
        GameController.Instance.QuitGame();
    }
}
