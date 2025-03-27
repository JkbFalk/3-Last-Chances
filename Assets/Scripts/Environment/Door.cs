using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : InteractableObject
{
    public bool CanBeOpened = true;
    public string RequiredKeyName;
    public new void Start() {
        base.Start();
        gameObject.tag = "Interactable";
        ClassWithMethodOrDialogue = "Door";
        MethodName = "DoorSpecialInteraction";
        SpecialInteractionLabel = gameObject.name == "Open" ? "Interact_CloseDoor" : "Interact_OpenDoor";
    }

    public static bool DoorSpecialInteraction(InteractableObject interactable) {
        Door door = (Door)interactable;
        if(!string.IsNullOrWhiteSpace(door.RequiredKeyName) && SaveFile.Instance.HasKey(door.RequiredKeyName)) {
            Item key = SaveFile.Instance.Inventory.FirstOrDefault(item => item.GetType() == Type.GetType(door.RequiredKeyName)); 
            Utils.PlaySoundEffect(Player.Instance.AudioSource, door.transform.parent.Find("Open").gameObject.activeSelf ? "Interact/Close Door" : "Interact/Open Door", 0.8f);
            door.transform.parent.Find("Open").gameObject.SetActive(!door.transform.parent.Find("Open").gameObject.activeSelf);
            door.transform.parent.Find("Closed").gameObject.SetActive(!door.transform.parent.Find("Closed").gameObject.activeSelf);
            NotificationController.ShowNotificationWithGraphic(Label.Get(key.GetType() + "") + ": " + Label.Get("OpenedDoorUsedKeyNotification"), key.IconPath);
            return true;
        }
        else if(!string.IsNullOrWhiteSpace(door.RequiredKeyName)){
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Interact/Locked Door", 0.8f);
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="LockedDoorNeedKeyNotification"}); 
            return false;
        }
        else if(string.IsNullOrWhiteSpace(door.RequiredKeyName)){
            if(door.CanBeOpened) {
                Utils.PlaySoundEffect(Player.Instance.AudioSource, door.transform.parent.Find("Open").gameObject.activeSelf ? "Interact/Close Door" : "Interact/Open Door", 0.8f);
                door.transform.parent.Find("Open").gameObject.SetActive(!door.transform.parent.Find("Open").gameObject.activeSelf);
                door.transform.parent.Find("Closed").gameObject.SetActive(!door.transform.parent.Find("Closed").gameObject.activeSelf);
                return true;
            }
            else {
                NotificationController.ShowCustomizedDialogueNotification(new() {Id="LockedDoorNotification"}); 
                return false;
            }
        }
        return false;
    }
}
