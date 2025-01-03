using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Dialogue
{
    public string NameOfDialogue;
    public string NameOfParentClass;
    public List<DialogueLine> Lines;

    public Vector2 PlayerStartingPosition;
    public Vector2 PlayerEndingPosition;
    public bool? PlayerStartingFlipped;
    public bool? PlayerEndingFlipped;
    public Unit DialogueSpeaker;
    public Vector2 SpeakerStartingPosition;
    public Vector2 SpeakerEndingPosition;
    public bool? SpeakerStartingFlipped;
    public bool? SpeakerEndingFlipped;

    public bool ReturnUnitsToOriginalPositions = true;
    public Vector2 SpeakerStartedPosition;
    public Vector2 PlayerStartedPosition;
    public bool PlayerStartedFlipped;
    public bool SpeakerStartedFlipped;
    public AnimationClip SpeakerStartedAnimation;
    public bool AutoSaveOnDialogueEnd = true;
    public Dialogue(string name_of_parent_class, string name_of_dialogue, List<DialogueLine> lines) {
        NameOfParentClass = name_of_parent_class;
        NameOfDialogue = name_of_dialogue;
        Lines = lines;
        foreach(DialogueLine line in lines.ToList()) {
            line.NameOfParentClass = name_of_parent_class;
            line.NameOfDialogue = name_of_dialogue;
            if(line.Choices.Count == 0 && string.IsNullOrWhiteSpace(line.IdOfNextDialogueLine)) {
                line.IdOfNextDialogueLine = DialogueLine.GetDefaultNextDialogueLine(line, Lines);
            }
        }
    }
}
