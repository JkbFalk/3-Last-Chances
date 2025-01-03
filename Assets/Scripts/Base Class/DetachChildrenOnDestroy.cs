using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachChildrenOnDestroy : MonoBehaviour {

    public void OnDestroy() {
        foreach (Transform child in transform) {
            child.SetParent(transform.parent);
        }
    }
}