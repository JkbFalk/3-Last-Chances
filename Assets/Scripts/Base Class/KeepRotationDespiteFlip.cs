using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotationDespiteFlip : MonoBehaviour
{
    private Actions FlipSource;
    public bool AlwaysFlipped = false;
    public void Start() {
        FlipSource = GetComponentInParent<Actions>();
        EnsureRotation();
    }
    public void OnEnable() {
        EnsureRotation();
    }
    public void EnsureRotation() {
        if(FlipSource == null) {
            return;
        }
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.Rotate(0, FlipSource.IsFlipped ? (AlwaysFlipped ? 0 : 180) : (AlwaysFlipped ? 180 : 0), 0);
    }
}
