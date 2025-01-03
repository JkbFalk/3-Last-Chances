using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CenterScrollRectOnItemWhenSelected : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        CenterOnItem();
    }

    public float AnimTime = 0.15f;
    public bool Log = true;
    [HideInInspector]
    public RectTransform MaskTransform;

    private ScrollRect _sr;
    Transform _scrollT;
    private RectTransform _content;

    private void Start()
    {
        _sr = GetComponentInParent<ScrollRect>();
        if(_sr == null) {
            Debug.LogWarning("No RectTransform found: " + Utils.GetGameObjectPath(gameObject));
            return;
        }
        _scrollT = _sr.transform;
        _content = _sr.content;
        MaskTransform = _sr.GetComponent<RectTransform>();
    }

    public void CenterOnItem()
    {
        RectTransform target = GetComponent<RectTransform>();
        if(Settings.Instance.ControlScheme != "Gamepad")
        {
            return;
        }
        if(_scrollT == null)
        {
            Start();
        }
        //the item is here
        var itemAnchorPositionInScroll = _scrollT.InverseTransformPoint(target.position);
        //but must be here
        var targetAnchorPositionInScroll = _scrollT.InverseTransformPoint(MaskTransform.TransformPoint(Vector2.zero));
        //so it has to move this distance
        var difference = targetAnchorPositionInScroll - itemAnchorPositionInScroll;
        difference.z = 0f;
        difference.x = 0f;
        //we move the content to align the item at the correct position
        var newAnchoredPosition = _content.parent.InverseTransformPoint(_content.position + difference);
        _content.anchoredPosition = newAnchoredPosition;
    }
}
