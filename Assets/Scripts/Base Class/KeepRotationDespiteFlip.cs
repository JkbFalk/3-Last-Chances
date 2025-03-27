using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class KeepRotationDespiteFlip : MonoBehaviour
{
    private Actions FlipSource;
    public bool AlwaysFlipped = false;
    public void Start() {
        FlipSource = GetComponentInParent<Actions>();
        EnsureRotation(FlipSource?.Unit);
        EventManager.UnitChangedDirection.AddListener(EnsureRotation);
    }
    public void OnEnable() {
        EnsureRotation(FlipSource?.Unit);
    }
    public void EnsureRotation(Unit unit) {
        if(FlipSource == null || unit == null || unit.Actions != FlipSource) {
            return;
        }
        transform.localEulerAngles = new Vector3(0, FlipSource.IsFlipped ? (AlwaysFlipped ? 0 : 180) : (AlwaysFlipped ? 180 : 0), 0);
    }
}
