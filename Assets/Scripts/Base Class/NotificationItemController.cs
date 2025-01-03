using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationItemController : MonoBehaviour
{
    public float StartHidingAfterNSeconds = 6;
    void Start()
    {
        GetComponent<HideOrShowOverTime>().ShowOverTimeFromZero(0.2f);
        StartCoroutine(HideNotification(StartHidingAfterNSeconds));
    }

    public IEnumerator HideNotification(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        GetComponent<HideOrShowOverTime>().HideOverTimeFromFull(0.5f);
    }
}
