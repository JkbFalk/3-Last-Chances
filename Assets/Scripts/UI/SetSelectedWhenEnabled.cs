using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSelectedWhenEnabled : MonoBehaviour
{
        void OnEnable()
    {
        if(Settings.Instance.ControlScheme == "Gamepad")
        {
            GetComponent<Selectable>().Select();
        }
    }

}
