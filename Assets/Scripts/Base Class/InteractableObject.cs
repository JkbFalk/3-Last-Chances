using System.Security.Cryptography;
using System.Linq.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class InteractableObject : MonoBehaviour
{
    public int InteractedCount = 0;
    public bool AutoInteractOnEnter = false;
    public int MaximumInteractionCount = 1;
    public bool AvailableOncePerCycle = false;
    public string InteractableId;
    public bool DestroyOnReachMaxInteractionCount = true;
    public AnimationClip AnimationOnInteract;
    public AudioClip SoundOnInteract;
    public string SpecialInteractionLabel;
    public string DialogueId;
    public string DialogueSpeakerName = "Player";
    public string DialogueSpeakerPortrait = "Player";
    public int ExperienceGainOnFirstInteract = 0;
    public string ItemTypeReceived;
    public Item.ItemGrade ItemGrade;
    public int ItemAmount = 1;
    public string ClassWithMethodOrDialogue;
    public string MethodName;
    public string DialogueName;
    public string FlagToBeAddedOnInteract;
    public bool HasInteractionPriority = false;
    [HideInInspector]
    public Unit ParentUnit;
    public bool CannotInteractWithDuringCombat = true;
    public bool CannotInteractWithWhenParentUnitKnockedOut = true;
    [HideInInspector]
    public bool WasActiveBeforeCombat = true;
    
    public bool CanBeInteractedWith {
        get {
            bool canBeInteractedWith =
            (CannotInteractWithDuringCombat == false || Player.Instance.InCombat == false) && (CannotInteractWithWhenParentUnitKnockedOut == false || ParentUnit == null || ParentUnit.KnockedOut == false) &&
            (MaximumInteractionCount == 0 || InteractedCount < MaximumInteractionCount);
            return canBeInteractedWith;
        }
    }

    public void UpdateIndicatorVisiblity() {
        foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>(true)) {
            if(sr.sprite.name == "Info Bubble") {
                foreach(SpriteRenderer sr2 in sr.GetComponentsInChildren<SpriteRenderer>(true)) {
                    sr2.enabled = CanBeInteractedWith;
                }
            }
        }
    }

    public void Start() {
        ParentUnit = GetComponentInParent<Unit>();
        gameObject.tag = "Interactable";
        if(AvailableOncePerCycle && !String.IsNullOrEmpty(InteractableId) && SaveFile.Instance.HasFlag(InteractableId + "_Destroyed_[Cycle]")) {
            MonoBehaviour.Destroy(gameObject);
        }
    }


    public void ActivateInteractable() {
        Utils.CreateAuditLog("Interacting with object: " + gameObject.name + ", ClassWithMethodOrDialogue: " + ClassWithMethodOrDialogue + ", MethodName: "+ MethodName + ", DialogueName:" + DialogueName);
        if(!string.IsNullOrWhiteSpace(ClassWithMethodOrDialogue) && (!string.IsNullOrWhiteSpace(MethodName) || !string.IsNullOrWhiteSpace(DialogueName))) {
            Type type = Type.GetType(ClassWithMethodOrDialogue);
            if(type == null) {
                Debug.LogError("Could not find class for interactable: " + ClassWithMethodOrDialogue);
            }
            else if(!string.IsNullOrWhiteSpace(MethodName)){
                MethodInfo method = Type.GetType(ClassWithMethodOrDialogue).GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);
                if(method == null) {
                    Debug.LogError("Could not find method (" +  MethodName + ") for class (" + type + ") for interactable.");
                }
                else {
                    var shouldInteract = method.Invoke(null, method.GetParameters().Length == 0 ? null : new object[] {this});
                    if(shouldInteract != null && shouldInteract.GetType() == typeof(bool) && (bool)shouldInteract == false) {
                        return;
                    }
                }
            }
            else if(!string.IsNullOrWhiteSpace(DialogueName)){
                MethodInfo dialogue = (ClassWithMethodOrDialogue.Contains("Quest_") || ClassWithMethodOrDialogue.Contains("Mission_")) ? Type.GetType(ClassWithMethodOrDialogue).GetMethod(DialogueName) : Type.GetType(ClassWithMethodOrDialogue).GetMethod(DialogueName, BindingFlags.Public | BindingFlags.Static);
                if(dialogue == null) {
                    Debug.LogError("Could not find dialogue (" +  DialogueName + ") for class (" + type + ") for interactable.");
                }
                else {
                    Dialogue d = (Dialogue)dialogue.Invoke(null, null);
                    UIManager.Instance.StartDialogue(d);
                    if(ClassWithMethodOrDialogue == "Book") {
                        Player.Instance.PlayAnimation("ReadingBook");
                        Transform readPosition = transform.Find("Read Position");
                        if(readPosition != null) {
                            Debug.Log("Player.Instance.transform.position: " + Player.Instance.transform.position);
                            Player.Instance.transform.position = readPosition.transform.position;
                        }
                    }
                    UIManager.Instance.DialogueInteractIndicator = transform.Find("Interact Indicator");
                    if(UIManager.Instance.DialogueInteractIndicator != null) {
                        UIManager.Instance.DialogueInteractIndicator.gameObject.SetActive(false);
                    }
                }
            }
        }
        if(ExperienceGainOnFirstInteract > 0 && SaveFile.Instance.ReceivedExperienceFromInteractables.Contains("Cycle_" + SaveFile.Instance.Cycle + "_" + SceneManager.GetActiveScene().name + "_" + Utils.GetGameObjectPath(gameObject)) == false) {
            SaveFile.Instance.ExperiencePoints += ExperienceGainOnFirstInteract;
            SaveFile.Instance.ReceivedExperienceFromInteractables.Add("Cycle_" + SaveFile.Instance.Cycle + "_" + SceneManager.GetActiveScene().name + "_" + Utils.GetGameObjectPath(gameObject));
        }
        InteractedCount++;
        if(string.IsNullOrWhiteSpace(DialogueId) == false) {
            Unit speaker = GetComponentInParent<Unit>();
            NotificationController.ShowCustomizedDialogueNotification(new() {Id=DialogueId, SpeakerUnit=speaker, SpeakerName =DialogueSpeakerName, SpeakerPortrait =DialogueSpeakerPortrait});
        }
        if(!string.IsNullOrWhiteSpace(ItemTypeReceived)) {
            SaveFile.Instance.AcquireItem(ItemTypeReceived, ItemAmount, ItemGrade);
            transform.Find("Interact Indicator")?.gameObject.SetActive(false);
        }
        if(AnimationOnInteract != null || !string.IsNullOrWhiteSpace(ItemTypeReceived)) {
            Player.Instance.PlayAnimation(AnimationOnInteract == null ? "PickUpItemFromTheGround" : AnimationOnInteract.name.Replace("Dialogue_", ""));
        }
        if(SoundOnInteract != null) {
            Utils.PlaySoundEffect(Player.Instance.AudioSource, SoundOnInteract, 0.6f);
        }
        string flagToAdd = Utils.GetFormattedFlag(FlagToBeAddedOnInteract);
        if(!String.IsNullOrEmpty(flagToAdd) && !SaveFile.Instance.Flags.Contains(flagToAdd)) {
            SaveFile.Instance.AddFlag(flagToAdd);
        }
        if(DestroyOnReachMaxInteractionCount && InteractedCount >= MaximumInteractionCount) {
            if(AvailableOncePerCycle && !String.IsNullOrEmpty(InteractableId)) {
                SaveFile.Instance.AddFlag(Utils.GetFormattedFlag(InteractableId + "_Destroyed_[Cycle]"));
            }
            MonoBehaviour.Destroy(gameObject);
        }
        EventManager.ObjectInteractedWith.Invoke(this);
        Player.Instance.SetInteractPromptToClosestInteractable();
    }

    
    private void OnTriggerEnter2D(Collider2D other) {
        if(GameController.Instance.LoadingNewArea == false && GameController.Instance.GameplayMode == Constants.GameplayMode.Regular && other.CompareTag("Hitbox") == true && other is CapsuleCollider2D && other.GetComponentInParent<Player>() != null) {
            if(AutoInteractOnEnter) {
                ActivateInteractable();
            }
            else {
                Player.Instance.AddInteractable(this);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Hitbox") == true && other is CapsuleCollider2D && other.GetComponentInParent<Player>() != null) {
            Player.Instance.RemoveInteractable(this);
        }
    }

    void OnDisable()
    {
        if(Player.HasInstance()) {
            Player.Instance.RemoveInteractable(this);
        }
    }
}
