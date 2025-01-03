using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Ignis1 : Mission
{
    public Mission_Ignis1() {
        Type = MissionType.MainQuest;
        NumberOfWeeksConsumed = 1;
        WeeksUntilExpiryMax = 9;
        FirstAppearsOnWeek = 2;
        Icon = "UI/Ignis";
        EnemyLevel = 5;
        MapMarker = "Town_IgnisManor";
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Cycle == 1;
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, "IgnisManor");
    }

    public static Dialogue LearnAboutVolcanoEruption() {
        return new Dialogue ("Mission_Ignis1", "LearnAboutVolcanoEruption", new List<DialogueLine>{
        new ("Ignis_LearnAboutBlainesDeath_0") {SpeakerName="Player", SpeakerPortrait="Player"},
        new ("Ignis_LearnAboutBlainesDeath_10") {SpeakerName="Laura", SpeakerPortrait="Laura", SpeakerIsMale=false},
        new ("Ignis_LearnAboutBlainesDeath_20") {SpeakerName="Player", SpeakerPortrait="Player"},
        new ("Ignis_LearnAboutBlainesDeath_30") {SpeakerName="Laura", SpeakerPortrait="Laura", SpeakerIsMale=false},
        new ("Ignis_LearnAboutBlainesDeath_40") {SpeakerName="Player", SpeakerPortrait="Player"}
    });}

    public static void Ignis_LearnAboutBlainesDeath_0() {
        SaveFile.Instance.AddFlag("Ignis_LearnedAboutVolcanoEruption");
    }

    public static void OnEnd_LearnAboutVolcanoEruption() {
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(80);
        GameController.Instance.GameplayMode = Constants.GameplayMode.InMenu;
        GameController.Instance.GameplayMode = Constants.GameplayMode.MissionSelect;
    }
}
