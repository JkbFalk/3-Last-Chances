using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformDebugger : MonoBehaviour
{
    public bool position = false;
    private Vector3 previousPosition = Vector3.zero;
    public bool rotation = false;
    private Vector3 previousRotation = Vector3.zero;
    public bool localPosition = false;
    private Vector3 previousLocalPosition = Vector3.zero;
    public bool localRotation = false;
    private Vector3 previousLocalRotation = Vector3.zero;
    void Start()
    {
        /*if(position) {
            transform.SetPositionExecuting += (sender, args) => {
                if(previousPosition != transform.position) {
                    Debug.Log($"Position of object {gameObject.name} changed from {previousPosition} to {transform.position}"); 
                    previousPosition = transform.position;
                }
            };
        }
        if(rotation) {
            transform.SetEulerAnglesExecuting += (sender, args) => {
                if(previousRotation != transform.eulerAngles) {
                    Debug.Log($"Rotation of object {gameObject.name} changed from {previousRotation} to {transform.eulerAngles}"); 
                    previousRotation = transform.eulerAngles;
                }
            };
        }
        if(localPosition) {
            transform.SetLocalPositionExecuting += (sender, args) => {
                if(previousLocalPosition != transform.localPosition) {
                    Debug.Log($"Local Position of object {gameObject.name} changed from {previousLocalPosition} to {transform.localPosition}"); 
                    previousLocalPosition = transform.localPosition;
                }
            };
        }
        if(localRotation) {
            transform.SetLocalEulerAnglesExecuting += (sender, args) => {
                if(previousLocalRotation != transform.localEulerAngles) {
                    Debug.Log($"Local Rotation of object {gameObject.name} changed from {previousLocalRotation} to {transform.localEulerAngles}"); 
                    previousLocalRotation = transform.localEulerAngles;
                }
            };
        }*/
    }
}
