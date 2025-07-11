using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour, ISubmitHandler, ISelectHandler, IPointerEnterHandler, IPointerClickHandler
{
    public string PathToButtonSubmitSoundFile;
    public string PathToButtonSelectSoundFile;
    private Button _button;
    private Toggle _toggle;
    private TMP_Dropdown _dropdown;
    private Slider _slider;
    public void Start() {
        _button = GetComponent<Button>();
        _toggle = GetComponent<Toggle>();
        _dropdown = GetComponent<TMP_Dropdown>();
        _slider = GetComponent<Slider>();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if(HasInteractableButton() && String.IsNullOrWhiteSpace(PathToButtonSubmitSoundFile) == false) {
            Utils.PlaySoundEffect(GameController.Instance.AudioSource, PathToButtonSubmitSoundFile, 0.8f);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(HasInteractableButton() && String.IsNullOrWhiteSpace(PathToButtonSubmitSoundFile) == false) {
            Utils.PlaySoundEffect(GameController.Instance.AudioSource, PathToButtonSubmitSoundFile, 0.8f);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(HasInteractableButton() && String.IsNullOrWhiteSpace(PathToButtonSelectSoundFile) == false) {
            Utils.PlaySoundEffect(GameController.Instance.AudioSource, PathToButtonSelectSoundFile, 0.5f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(HasInteractableButton() && String.IsNullOrWhiteSpace(PathToButtonSelectSoundFile) == false) {
            Utils.PlaySoundEffect(GameController.Instance.AudioSource, PathToButtonSelectSoundFile, 0.5f);
        }
    }

    public bool HasInteractableButton() {
        return (_button != null && _button.interactable) || (_toggle != null && _toggle.interactable) || (_dropdown != null && _dropdown.interactable) || (_slider != null && _slider.interactable);
    }
}
