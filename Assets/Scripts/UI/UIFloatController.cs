using System.Linq.Expressions;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIFloatController : MonoBehaviour {

    public enum Behavior { FloatUpToDown, FloatDownToUp, FloatLeftToRight, FloatRightToLeft };

    public Behavior FloatBehavior = Behavior.FloatDownToUp;
    public float DisappearAfter = 0.3f;
    public float FloatSpeed = 0.4f;
    public float DisappearTime = 0;
    public bool IsDisappearing = false;
    public bool bouncingBack = false;
    private Vector2 originalPosition;
    public float StartGoingInOppositeDirectionAfterDistance = 0;
    private TextMeshProUGUI _text;
    private Color _defaultColor;
    private bool _initialized;

    private void OnEnable() {
        if (_text == null) {
            _text = GetComponent<TextMeshProUGUI>();
        }
        if (!_initialized && _text != null) {
            _defaultColor = _text.color;
            _initialized = true;
        }
        else if (_text != null) {
            _text.color = _defaultColor;
        }
        originalPosition = transform.localPosition;
        IsDisappearing = false;
        bouncingBack = false;
    }

    private void Update() {
        if (Time.timeScale > 0) {
            DisappearAfter -= Time.deltaTime;
            float distance = Vector2.Distance(originalPosition, transform.localPosition);
            if(StartGoingInOppositeDirectionAfterDistance > 0 && bouncingBack == false && distance > StartGoingInOppositeDirectionAfterDistance) {
                FloatBehavior = FloatBehavior == Behavior.FloatUpToDown ? Behavior.FloatDownToUp : FloatBehavior == Behavior.FloatDownToUp ? Behavior.FloatUpToDown : FloatBehavior == Behavior.FloatRightToLeft ? Behavior.FloatLeftToRight : Behavior.FloatRightToLeft;
                bouncingBack = true;
            }
            if(bouncingBack && distance < StartGoingInOppositeDirectionAfterDistance * 0.1f) {
                bouncingBack = false;
            }
            if (DisappearAfter < 0)
            {
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a - (1 / DisappearTime) * Time.deltaTime);
                DisappearTime -= Time.deltaTime;
                if (DisappearTime <= 0)
                {
                    if (GetComponent<PooledObject>() != null) {
                        ObjectPool.Release(gameObject);
                    }
                    else {
                        MonoBehaviour.Destroy(gameObject);
                    }
                }
            }
            if (FloatBehavior == Behavior.FloatUpToDown) {
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - FloatSpeed * Time.deltaTime);
            }
            else if (FloatBehavior == Behavior.FloatDownToUp) {
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + FloatSpeed * Time.deltaTime);
            }
            else if (FloatBehavior == Behavior.FloatLeftToRight) {
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x + FloatSpeed * Time.deltaTime, transform.localPosition.y); ;
            }
            else if (FloatBehavior == Behavior.FloatRightToLeft) {
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x - FloatSpeed * Time.deltaTime, transform.localPosition.y);
            }
        }
    }
}