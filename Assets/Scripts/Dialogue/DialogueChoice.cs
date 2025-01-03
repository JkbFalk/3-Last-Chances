using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChoice
{
    private string _IdOfNextDialogueLine;
    public string IdOfNextDialogueLine {
        get => _IdOfNextDialogueLine;
        set {
            _IdOfNextDialogueLine = value;
        }
    }
    public List<string> StringParams;
    public bool Disabled = false;
    public string Id;
    public DialogueLineItem DialogueLineItem;
    public string ChoiceDisabledReason;
    public List<string> ChoiceDisabledReasonParams;
    public bool HideChoiceTextIfDisabled = true;
    public DialogueChoice(string id) {
        Id = id;
        IdOfNextDialogueLine = id.Replace("_0", "_10");
    }
}
