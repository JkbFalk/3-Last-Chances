using System.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Area_FirstMission
{

    public static void OnStart()
    {
        EventManager.EnemyDefeated.AddListener(CheckIfActivateMapPickUp);
        if(SaveFile.Instance.HasFlag("FirstMission_FoundMap")) {
            Area.Instance.transform.Find("Leo Group").gameObject.SetActive(true);
            Area.Instance.transform.Find("Interactables/Leo Dialogue Trigger").gameObject.SetActive(true);
        }
    }

    public static void CheckIfActivateMapPickUp(Damage dmg) {
        if(dmg.TargetOfDamage.gameObject.name.Contains("Unit_Leader")) {
            Utils.GetUnit("Leader").transform.Find("Map").gameObject.SetActive(true);
            Utils.GetUnit("Leader").transform.Find("Map").transform.SetParent(Area.Instance.transform);
        }
    }
}
