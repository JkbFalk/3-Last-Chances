using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTransform : MonoBehaviour {
    public bool FreezePosition = false;
    public bool FreezeGlobalRotation = false;
    public bool FreezeLocalRotation = false;
    public bool FreezeScale = false;

    public void Update() {
        if (FreezePosition && transform.localPosition != Vector3.zero) {
            transform.localPosition = Vector3.zero;
        }
        if (FreezeGlobalRotation && transform.localRotation.eulerAngles != Vector3.zero) {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        if (FreezeLocalRotation && transform.localRotation.eulerAngles != Vector3.zero)
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        if (FreezeScale && transform.localScale != Vector3.zero) {
            transform.localScale = Vector3.zero;
        }
    }
}