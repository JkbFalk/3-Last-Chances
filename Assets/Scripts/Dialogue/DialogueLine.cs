using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DialogueLine
{
    public string NameOfDialogue;
    public string NameOfParentClass;
    public string Id;
    public string AudioClip;
    public float AudioClipVolume = 1;
    public List<DialogueChoice> Choices = new List<DialogueChoice>();
    public enum DialogueSpeaker { Left, Right, None};
    public bool ShowSpeakerBox = true;
    public string Speaker;
    public Unit SpeakerUnit;
    public bool SpeakerIsMale = true;
    public string SpeakerName;
    public string SpeakerPortrait;
    public string Animation;
    public List<string> StringParams;
    public bool TurnSpeakersToFaceEachOther = true;
    public float WaitTimeBeforeAllowingToProceed = 0;

    private string _IdOfNextDialogueLine;
    public string IdOfNextDialogueLine {
        get => _IdOfNextDialogueLine;
        set {
            _IdOfNextDialogueLine = value;
        }
    }
    public DialogueLine(string id)
    {
        Id = id;
    }

    public List<string> GetChoiceIds() {
        List<string> ids = new List<string>();
        foreach(DialogueChoice choice in Choices) {
            ids.Add(Id);
        }
        return ids;
    }

    public static string GetDefaultNextDialogueLine(DialogueLine line, List<DialogueLine> lines) {
        int index = lines.IndexOf(line) + 1;
        if(index < lines.Count) {
            return lines[index].Id;
        }
        return "END";
    }

    public void OnStart() {
        if(AudioClip != null) {
            Utils.PlaySoundEffect(Player.Instance.AudioSource, AudioClip, AudioClipVolume);
        }
        if(Speaker == "Player") {
            SpeakerUnit = Player.Instance;
        }
        else if(Speaker != null){
            SpeakerUnit = Utils.GetUnit(Speaker);
        }
        if(SpeakerUnit != null) {
            CameraController.Instance.CenteredOnObject = SpeakerUnit.gameObject;
        }
        if(TurnSpeakersToFaceEachOther && SpeakerUnit != null && SpeakerUnit.transform.position.x > Player.Instance.transform.position.x && SpeakerUnit.Actions != null && !SpeakerUnit.Actions.IsFlipped) {
            SpeakerUnit.Actions.IsFlipped = true;
        }
        else if(TurnSpeakersToFaceEachOther && SpeakerUnit != null && SpeakerUnit.transform.position.x < Player.Instance.transform.position.x && SpeakerUnit.Actions != null && SpeakerUnit.Actions.IsFlipped) {
            SpeakerUnit.Actions.IsFlipped = false;
        }
        if(TurnSpeakersToFaceEachOther && SpeakerUnit != null && SpeakerUnit.transform.position.x > Player.Instance.transform.position.x && Player.Instance.Actions.IsFlipped) {
            Player.Instance.Actions.IsFlipped = false;
        }
        else if(TurnSpeakersToFaceEachOther && SpeakerUnit != null && SpeakerUnit.transform.position.x < Player.Instance.transform.position.x && !Player.Instance.Actions.IsFlipped) {
            Player.Instance.Actions.IsFlipped = true;
        }
        if(!string.IsNullOrWhiteSpace(NameOfParentClass) && Type.GetType(NameOfParentClass).GetMethod(Id) != null) {
            Type.GetType(NameOfParentClass).GetMethod(Id).Invoke(null, null);
        }
        if(Animation != null && SpeakerUnit != null){
            SpeakerUnit.SetDefaultSortingOrder();
            SpeakerUnit.Actions.SetFaceVariant("Default");
            SpeakerUnit.PlayAnimation(Animation.Replace("Dialogue_", ""));
        }
        if(WaitTimeBeforeAllowingToProceed > 0) {
            UIManager.Instance.CanGoToNextDialogueLine = false;
            GameController.Instance.WaitAndRunMethodRealtime(WaitTimeBeforeAllowingToProceed, UnlockProceed);
        }
    }

    public void UnlockProceed() {
        UIManager.Instance.CanGoToNextDialogueLine = true;
    }

    public void OnEnd() {
        if(!string.IsNullOrWhiteSpace(NameOfParentClass) && Type.GetType(NameOfParentClass).GetMethod("OnEnd_" + Id) != null) {
            Type.GetType(NameOfParentClass).GetMethod("OnEnd_" +Id).Invoke(null, null);
        }
    }
}
