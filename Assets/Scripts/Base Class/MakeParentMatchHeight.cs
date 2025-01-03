using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeParentMatchHeight : MonoBehaviour
{
    private RectTransform _parentTransform;
    private RectTransform _rectTransform;
    private float _height;
    void Start()
    {
        _parentTransform = transform.parent.GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();
        _height = _rectTransform.sizeDelta.y;
    }

    void Update()
    {
        if(_rectTransform.sizeDelta.y != _height)
        {
            float diff = _rectTransform.sizeDelta.y - _height;
            _parentTransform.sizeDelta = new Vector2(_parentTransform.sizeDelta.x, _parentTransform.sizeDelta.y + diff);
            _height = _rectTransform.sizeDelta.y;

        }
    }
}
