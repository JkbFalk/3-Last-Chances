using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueLineItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI TextMeshPro;
    public DialogueLine DialogueLine;
    public DialogueChoice DialogueChoice;
    public string Id;
    public bool IsSelected;
    public bool CurrentlyReadingSpeech = false;
    private bool _canClick = false;

    public void Initialize(DialogueLine _dialogueLine) {
        DialogueLine = _dialogueLine;
        Id = _dialogueLine.Id;
        transform.GetComponent<LabelInitializer>().string_params = DialogueLine.StringParams;
        transform.GetComponent<LabelInitializer>().SetLabel("{" + Id + "}");
        TextMeshPro = transform.GetComponent<TextMeshProUGUI>();
        if(UIManager.Instance.SkippingDialogue == false && Settings.Instance.DialogueTextSpeed < Constants.MAX_DIALOGUE_SPEED) {
            TextMeshPro.maxVisibleCharacters = 0;
            RevealExtraDialogueLineCharacter();
        }
    }

    public void Initialize(DialogueChoice _dialogueChoice) {
        DialogueChoice = _dialogueChoice;
        Id = _dialogueChoice.Id;
        TextMeshPro = transform.GetComponent<TextMeshProUGUI>();
        if(DialogueChoice != null) {
            Debug.Log("ENABLED1? " + Id + " , " + DialogueChoice + " , " + GetComponent<Button>().enabled);
            GameController.Instance.WaitAndRunMethodRealtime(Constants.SECONDS_UNTIL_DIALOGUE_CHOICES_BECOME_CLICKABLE, UnlockChoiceClick);
            if(Type.GetType(UIManager.Instance.CurrentDialogue.NameOfParentClass)?.GetMethod("CheckIfEnabled_" + DialogueChoice.Id) != null) {
                DialogueChoice.Disabled = !(bool)Type.GetType(UIManager.Instance.CurrentDialogue.NameOfParentClass).GetMethod("CheckIfEnabled_" + DialogueChoice.Id).Invoke(null, null);
            }
            DialogueChoice.DialogueLineItem = this;
            TextMeshPro.color = DialogueChoice.Disabled ? Colors.GetColorFromCode("#707070") : SaveFile.Instance.ReadDialogueLines.Contains(Id) ? Colors.GetColorFromCode("#A0A0A0")  : Color.white;
        }
        transform.GetComponent<LabelInitializer>().string_params = DialogueChoice.StringParams;
        transform.GetComponent<LabelInitializer>().SetLabel((DialogueChoice.HideChoiceTextIfDisabled && DialogueChoice.Disabled ? "???" : "{" + Id + "}") + (DialogueChoice != null && Label.ContainsKey(Id.Substring(0, Id.Length-1) + "Requirement") ? " *" + Label.Get(Id.Substring(0, Id.Length-1) + "Requirement") + "*" : ""));
    }

    public void UnlockChoiceClick() {
        Debug.Log("TEST1 " + Id + " , " + DialogueChoice + " , " + GetComponent<Button>().enabled);
        if(gameObject.IsDestroyed() == false) {
            Debug.Log("ENABLED2? " + Id + " , " + DialogueChoice + " , " + GetComponent<Button>().enabled);
            GetComponent<Button>().enabled = true;
            _canClick = true;
            Debug.Log("ENABLED3? " + Id + " , " + DialogueChoice + " , " + GetComponent<Button>().enabled);
        }
    }

    public void RevealExtraDialogueLineCharacter() {
        if(UIManager.Instance.SkippingDialogue || TextMeshPro.maxVisibleCharacters >= TextMeshPro.text.Length) {
            TextMeshPro.maxVisibleCharacters = TextMeshPro.text.Length;
            return;
        }
        TextMeshPro.maxVisibleCharacters++;
        if(TextMeshPro.text[TextMeshPro.maxVisibleCharacters - 1] == '"') {
            CurrentlyReadingSpeech = !CurrentlyReadingSpeech;
        }
        string currentChar = Utils.ConvertCharacterFromForeignLanguages(TextMeshPro.text[TextMeshPro.maxVisibleCharacters - 1].ToString().ToUpper());
        if(CurrentlyReadingSpeech && Constants.VOWELS.Contains(currentChar)) {
            CanvasElements.AudioListener.pitch = DialogueLine.SpeakerUnit == null ? UnityEngine.Random.Range(0.9f, 1.1f) : DialogueLine.SpeakerUnit.VoicePitch + UnityEngine.Random.Range(-0.1f, 0.1f);
            CanvasElements.AudioListener.PlayOneShot(GameController.Instance.SpeechBeepClips[((DialogueLine.SpeakerUnit != null && DialogueLine.SpeakerUnit is Player) ? "Player" : (DialogueLine.SpeakerUnit != null && DialogueLine.SpeakerUnit.IsMale) ? "Male" : (DialogueLine.SpeakerUnit == null && DialogueLine.SpeakerIsMale) ? "Male" : "Female") + currentChar]);
        }
        GameController.Instance.WaitAndRunMethodRealtime(0.03f / Settings.Instance.DialogueTextSpeed * 10, RevealExtraDialogueLineCharacter);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if((DialogueChoice == null || DialogueChoice.Disabled == false) && eventData.button == PointerEventData.InputButton.Left) {
            ProceedToNext();
        }
    }

    public void ProceedToNext() {
        if(DialogueChoice != null && DialogueChoice.Disabled == false) {
            SelectChoice(DialogueChoice);
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ChoiceSelect");
        }
        else if(DialogueLine.IdOfNextDialogueLine == "END") {
            UIManager.Instance.EndDialogue();
        }
        else {
            UIManager.Instance.ProgressToNextDialogueLine();
        }
    }

    public void SelectChoice(DialogueChoice choice) {
        if(DialogueChoice.Disabled || _canClick == false) {
            return;
        }
        SaveFile.Instance.ReadDialogueLines.Add(choice.Id);
        if(Type.GetType(UIManager.Instance.CurrentDialogueLine?.NameOfParentClass)?.GetMethod(choice.Id) != null) {
            Type.GetType(UIManager.Instance.CurrentDialogueLine.NameOfParentClass).GetMethod(choice.Id).Invoke(null, null);
        }
        MenuManager.Instance.AddHistoryEntry(new NotificationController.InGameDialogue() { Id=choice.Id, SpeakerName=Label.Get("Name_Player"), SpeakerPortrait= "Player"}, true, choice.StringParams);
        DialogueLine next_line = UIManager.Instance.CurrentDialogue?.Lines?.FirstOrDefault(line => line.Id == choice.IdOfNextDialogueLine);
        if(choice == null) {
            Debug.LogError("Could not find choice for " + DialogueLine + " : " + Id);
        }
        if(choice?.IdOfNextDialogueLine == "END") {
            UIManager.Instance.EndDialogue();
        }
        else {
            UIManager.Instance.ProgressToNextDialogueLine(next_line);
        }
        Utils.CreateAuditLog("Selected choice: " + choice.Id + " (-> " + choice.IdOfNextDialogueLine +")");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LineSelected();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LineDeselected();
    }

    public void OnSelect(BaseEventData eventData)
    {
        LineSelected();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        LineDeselected();
    }

    public void LineSelected() {
        IsSelected = true;
        if(DialogueChoice != null) {
            GetComponent<TextMeshProUGUI>().color = Colors.SelectedColor;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ChoiceHover");
        }
    }

    public void LineDeselected() {
        IsSelected = false;
        if(DialogueChoice != null) {
            GetComponent<TextMeshProUGUI>().color = DialogueChoice == null ? Color.white : DialogueChoice.Disabled ? Colors.GetColorFromCode("#707070") : SaveFile.Instance.ReadDialogueLines.Contains(Id) ? Colors.GetColorFromCode("#A0A0A0")  : Color.white;
        }
    }
}
