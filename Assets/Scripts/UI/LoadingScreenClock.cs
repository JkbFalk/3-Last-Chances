using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenClock : MonoBehaviour
{
    private Image clock;
    void Start()
    {
        clock = GetComponent<Image>();
    }

    void Update()
    {
        if(clock.fillClockwise) {
            clock.fillAmount += Time.fixedDeltaTime / 2;
            if(clock.fillAmount >= 1) {
                clock.fillClockwise = false;
            }
        }
        else {
            clock.fillAmount -= Time.fixedDeltaTime / 2;
            if(clock.fillAmount <= 0) {
                clock.fillClockwise = true;
            }
        }
    }
}
