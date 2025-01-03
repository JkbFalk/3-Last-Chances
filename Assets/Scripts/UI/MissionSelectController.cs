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
}
