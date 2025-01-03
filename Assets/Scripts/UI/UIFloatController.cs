
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

    private void Start() {
        if(DisappearAfter > 0) {
            GameController.Instance.WaitAndRunMethod(DisappearAfter, Destroy);
        }
        _text = GetComponent<TextMeshProUGUI>();
        originalPosition = transform.localPosition;
    }

    private void Update() {
        if (Time.timeScale > 0) {
            float distance = Vector2.Distance(originalPosition, transform.localPosition);
            if(StartGoingInOppositeDirectionAfterDistance > 0 && bouncingBack == false && distance > StartGoingInOppositeDirectionAfterDistance) {
                FloatBehavior = FloatBehavior == Behavior.FloatUpToDown ? Behavior.FloatDownToUp : FloatBehavior == Behavior.FloatDownToUp ? Behavior.FloatUpToDown : FloatBehavior == Behavior.FloatRightToLeft ? Behavior.FloatLeftToRight : Behavior.FloatRightToLeft;
                bouncingBack = true;
            }
            if(bouncingBack && distance < StartGoingInOppositeDirectionAfterDistance * 0.1f) {
                bouncingBack = false;
            }
            if (IsDisappearing)
            {
                _text.color =  new Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a - (1 / DisappearTime) * Time.deltaTime);
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

    public void Destroy() {
        if (DisappearTime == 0) {
            MonoBehaviour.Destroy(gameObject);
        }
        else
        {
            IsDisappearing = true;
        }
    }
}