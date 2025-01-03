using UnityEngine;

public class DisableTransformChange : MonoBehaviour {
    public bool OnlyDisableWhenHasParent = true;
    public bool DisablePositionChange = false;
    public bool DisableRotationChange = false;
    public bool DisableLocalPositionChange = false;
    public bool DisableLocalRotationChange = false;
    public bool DisableLocalScaleChange = false;
    private Vector3 _position;
    private Vector3 _rotation;
    private Vector3 _localPosition;
    private Vector3 _localRotation;
    private Vector3 _localScale;

    private void Start()
    {
        _position = transform.position;
        _rotation = transform.eulerAngles;
        _localPosition = transform.localPosition;
        _localRotation = transform.localEulerAngles;
        _localScale = transform.localScale;
    }
    private void Update() {
        if (OnlyDisableWhenHasParent && transform.parent == null) {
            return;
        }
        if(DisablePositionChange && transform.position != _position)
        {
            transform.position = _position;
        }
        if (DisableRotationChange && transform.eulerAngles != _rotation)
        {
            transform.eulerAngles = _rotation;
        }
        if (DisableLocalPositionChange && transform.localPosition != _localPosition)
        {
            transform.localPosition = _localPosition;
        }
        if (DisableLocalRotationChange && transform.localEulerAngles != _localRotation)
        {
            transform.localEulerAngles = _localRotation;
        }
        if (DisableLocalScaleChange && transform.localScale != _localScale)
        {
            transform.localScale = _localScale;
        }
    }
}