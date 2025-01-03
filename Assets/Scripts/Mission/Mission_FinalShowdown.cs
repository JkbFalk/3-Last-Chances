using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_FinalShowdown : Mission
{
    public Mission_FinalShowdown() {
        Type = MissionType.MainQuest;
        EnemyLevel = 50;
        CanExpire = false;
        NumberOfWeeksConsumed = 0;
        MapMarker = "Penance";
        RemoveOtherMissions = true;
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Week == 51;
    }

    public override string GetDescriptionLabel() {
        return SaveFile.Instance.Cycle == 3 ? GetType() + "3_Description" : GetType() + "1and2_Description";
    }

    public override void OnStart() {
        base.OnStart();
        UIManager.Instance.StartDialogue(SaveFile.Instance.Cycle == 3 ? Cycle3() : Cycle1And2());
    }

    public static Dialogue Cycle1And2() {
        return new Dialogue ("Mission_FinalShowdown", "Cycle1And2", new List<DialogueLine>{
        new ("FinalShowdown_0_0")
    });}

    public static void OnEnd_Cycle1And2() {
        MenuManager.Instance.MoveToNextCycle();
        SaveFile.Instance.CurrentMission.Finish();
        SaveFile.Instance.CompletedStoryMissions.Remove(typeof(Mission_FinalShowdown));
        GameController.Instance.GameplayMode = Constants.GameplayMode.InMenu;
        GameController.Instance.GameplayMode = Constants.GameplayMode.MissionSelect;
    }

    public static Dialogue Cycle3() {
        return new Dialogue ("Mission_FinalShowdown", "Cycle3", new List<DialogueLine>{
        new ("FinalShowdown_0_1")
    });}

    public static void OnEnd_Cycle3() {
        GameController.Instance.GameplayMode = Constants.GameplayMode.OnStartScreen;
    }
}
