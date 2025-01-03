using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Chest : InteractableObject
{
    public bool IsOpen = false;
    public string RequiredKeyName;
    public new void Start() {
        base.Start();
        ClassWithMethodOrDialogue = "Chest";
        MethodName = "ChestSpecialInteraction";
        SpecialInteractionLabel = "Interact_OpenChest";
        MaximumInteractionCount = 1;
        if(AvailableOncePerCycle && !String.IsNullOrEmpty(InteractableId) && SaveFile.Instance.Flags.Contains(Utils.GetFormattedFlag(InteractableId + "_Destroyed_[Cycle]"))) {
            MonoBehaviour.Destroy(gameObject);
        }
    }

    public static bool ChestSpecialInteraction(InteractableObject interactable) {
        Chest chest = (Chest)interactable;
        if(string.IsNullOrWhiteSpace(chest.RequiredKeyName)) {
            chest.IsOpen = true;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Interact/Open Chest", 0.8f);
            if(chest.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite.name == "Treasure Chest (Locked)") {
                chest.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Environment/Treasure Chest (Open)", typeof(Sprite)) as Sprite;
            }
            chest.transform.Find("Interact Indicator").gameObject.SetActive(false);
            if(interactable.AvailableOncePerCycle && !String.IsNullOrEmpty(interactable.InteractableId)) {
                SaveFile.Instance.AddFlag(Utils.GetFormattedFlag(interactable.InteractableId + "_Destroyed_[Cycle]"));
            }
            return true;
        }
        else if(!string.IsNullOrWhiteSpace(chest.RequiredKeyName) && SaveFile.Instance.HasKey(chest.RequiredKeyName)) {
            Item key = SaveFile.Instance.Inventory.FirstOrDefault(item => item.GetType() == Type.GetType(chest.RequiredKeyName)); 
            chest.IsOpen = true;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Interact/Open Chest", 0.8f);
            if(chest.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite.name == "Treasure Chest (Locked)") {
                chest.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Environment/Treasure Chest (Open)", typeof(Sprite)) as Sprite;
            }
            chest.transform.Find("Interact Indicator").gameObject.SetActive(false);
            NotificationController.ShowNotificationWithGraphic(Label.Get(key.GetType() + "_Name") + ": " + Label.Get("OpenedChestUsedKeyNotification"), key.IconPath);
            if(interactable.AvailableOncePerCycle && !String.IsNullOrEmpty(interactable.InteractableId)) {
                SaveFile.Instance.AddFlag(Utils.GetFormattedFlag(interactable.InteractableId + "_Destroyed_[Cycle]"));
            }
            return true;
        }
        else if(!string.IsNullOrWhiteSpace(chest.RequiredKeyName)){
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Interact/Locked Chest", 0.8f);
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="LockedChestNeedKeyNotification"}); 
            return false;
        }
        else if(string.IsNullOrWhiteSpace(chest.RequiredKeyName)){
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Interact/Locked Chest", 0.8f);
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="LockedChestNotification"}); 
            return false;
        }
        return false;
    }
}
