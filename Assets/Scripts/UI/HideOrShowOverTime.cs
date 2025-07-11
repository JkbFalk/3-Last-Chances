using System;
using UnityEngine;

public class HideOrShowOverTime : MonoBehaviour {
    private CanvasGroup _canvasGroup;

    private CanvasGroup CanvasGroup {
        get {
            if (_canvasGroup == null) {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }
    public bool ChangeInProgress = false;
    public float Visiblity = 0;
    public bool DestroyAfterHiding = false;
    private bool _hiding = false;
    private bool _showing = false;

    private float _seconds = 0;

    private void Update() {
        if (_showing) {
            CanvasGroup.alpha = CanvasGroup.alpha +  (1 / (_seconds / (Time.unscaledDeltaTime > 0.05f ? 0.05f : Time.unscaledDeltaTime)));
            if (CanvasGroup.alpha >= 1) {
                _showing = false;
                ChangeInProgress = false;
                Visiblity = CanvasGroup.alpha;
                if(CheckIfShouldHideOnceFinishedChanging) {
                    CheckIfShouldHideOnceFinishedChanging = false;
                    LabelInitializer.CheckIfShouldHide();
                }
            }
        }
        else if (_hiding) {
            CanvasGroup.alpha = CanvasGroup.alpha -  (1 / (_seconds / (Time.unscaledDeltaTime > 0.05f ? 0.05f : Time.unscaledDeltaTime)));
            if (CanvasGroup.alpha <= 0) {
                _hiding = false;
                ChangeInProgress = false;
                Visiblity = CanvasGroup.alpha;
                if(CheckIfShouldHideOnceFinishedChanging) {
                    CheckIfShouldHideOnceFinishedChanging = false;
                    LabelInitializer.CheckIfShouldHide();
                }
                if (DestroyAfterHiding) {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void HideOverTime(float seconds) {
        _seconds = seconds;
        _hiding = true;
        _showing = false;
        ChangeInProgress = true;
    }

    public void HideOverTimeFromFull(float seconds) {
        if(seconds == 0) {
            CanvasGroup.alpha = 0;
            return;
        }
        _seconds = seconds;
        CanvasGroup.alpha = 1;
        _hiding = true;
        _showing = false;
        ChangeInProgress = true;
    }

    public void ShowOverTime(float seconds) {
        _seconds = seconds;
        _showing = true;
        _hiding = false;
        ChangeInProgress = true;
    }

    public void ShowOverTimeFromZero(float seconds) {
        if(seconds == 0) {
            CanvasGroup.alpha = 1;
            return;
        }
        _seconds = seconds;
        CanvasGroup.alpha = 0;
        _showing = true;
        _hiding = false;
        ChangeInProgress = true;
    }
    public LabelInitializer LabelInitializer;
    public bool CheckIfShouldHideOnceFinishedChanging = false;

}