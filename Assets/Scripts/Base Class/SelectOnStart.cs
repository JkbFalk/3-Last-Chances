using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnStart : MonoBehaviour
{
    void Start()
    {
        if(gameObject.name == "0")  {
            GetComponent<Selectable>().Select();
        }
    }

}
