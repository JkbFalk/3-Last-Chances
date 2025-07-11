using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_MeetingWithLaura : Mission
{
    public Mission_MeetingWithLaura() {
        Type = MissionType.MainQuest;
        NumberOfWeeksConsumed = 0;
        WeeksUntilExpiryMax = 1;
        Icon = "UI/HouseYellow";
        MapMarker = "Town_Trimvine";
        CanAbandonMission = false;
        RemoveOtherMissions = SaveFile.Instance.Cycle == 1 ? !Settings.Instance.SkipPrologue : false;
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Week == 2 && SaveFile.Instance.Cycle == 1 && !SaveFile.Instance.HasFlag("Trimvine_MeetingWithLaura_[Cycle]");
    }

    public override void OnStart()
    {
        base.OnStart();
        Utils.MoveIntoArea(true, "Trimvine_LaurasLab");
        EventManager.FinishedLoadingArea.AddListener(OnFinishLoading);
    }

    public void OnFinishLoading() {
        Area.Instance.transform.Find("Interactables/Back Exit (Warp)").gameObject.SetActive(false);
        Area.Instance.transform.Find("Interactables/Front Exit (Warp)").gameObject.SetActive(false);
        if(SaveFile.Instance.Cycle == 1) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="LaurasLab_Inter_170"});
        }
        SaveFile.Instance.GetQuest("3LastChances").GetObjective(30).ShowAsMissionObjective();
    }

    public override void OnEnd()
    {
        RemoveOtherMissions = false;
    }

    public static Dialogue MeetingWithLaura_1() {
        return new Dialogue("Mission_MeetingWithLaura", "MeetingWithLaura_1", new List<DialogueLine>{
            new ("MeetingWithLaura_Dialogue1_0") {Speaker = "Laura", Animation="Frustrated"},
            new ("MeetingWithLaura_Dialogue1_10") {Speaker = "Player", Animation="ShoulderShrug"},
            new ("MeetingWithLaura_Dialogue1_20") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_30") {Speaker = "Player", ShowSpeakerBox=false},
            new ("MeetingWithLaura_Dialogue1_40") {Speaker = "Player", Animation="IWouldHurryUpIfIWereYou"},
            new ("MeetingWithLaura_Dialogue1_50") {Speaker = "Laura", Animation="SternNo"},
            new ("MeetingWithLaura_Dialogue1_60") {Speaker = "Laura", Animation="ShoulderShrug"},
            new ("MeetingWithLaura_Dialogue1_70") {Speaker = "Player", Animation="Frustrated"},
            new ("MeetingWithLaura_Dialogue1_80") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_90") {Speaker = "Player"},
            new ("MeetingWithLaura_Dialogue1_100") {Speaker = "Laura", Animation="Determined"},
            new ("MeetingWithLaura_Dialogue1_110") {Speaker = "Player"},
            new ("MeetingWithLaura_Dialogue1_120") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_130") {Speaker = "Player", Animation="Doubtful"},
            new ("MeetingWithLaura_Dialogue1_140") {Speaker = "Laura", Animation="PassItem"},
            new ("MeetingWithLaura_Dialogue1_150") {Speaker = "Player", Animation="Irritated"},
            new ("MeetingWithLaura_Dialogue1_160") {Speaker = "Laura", Animation="GiveMeABreak"},
            new ("MeetingWithLaura_Dialogue1_170") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_180") {Speaker = "Laura", Animation="HereWeGo"},
            new ("MeetingWithLaura_Dialogue1_190") {Speaker = "Player", Animation="Explaining"},
            new ("MeetingWithLaura_Dialogue1_200") {Speaker = "Player", ShowSpeakerBox=false},
            new ("MeetingWithLaura_Dialogue1_210") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_220") {Speaker = "Player", Animation="ShoulderShrug"},
            new ("MeetingWithLaura_Dialogue1_230") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_240") {Speaker = "Player"},
            new ("MeetingWithLaura_Dialogue1_250") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_260") {Speaker = "Player", Animation="TryingToRemember"},
            new ("MeetingWithLaura_Dialogue1_270") {Speaker = "Laura", Animation="Deflated"},
            new ("MeetingWithLaura_Dialogue1_280") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_290") {Speaker = "Player", Animation="Doubtful"},
            new ("MeetingWithLaura_Dialogue1_300") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_310") {Speaker = "Player"},
            new ("MeetingWithLaura_Dialogue1_320") {Speaker = "Laura", Animation="ShoulderShrug"},
            new ("MeetingWithLaura_Dialogue1_330") {Speaker = "Player", Animation="GiveMeABreak"},
            new ("MeetingWithLaura_Dialogue1_340") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_350") {Speaker = "Player"},
            new ("MeetingWithLaura_Dialogue1_360") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_370") {Speaker = "Laura", Animation="Explaining"},
            new ("MeetingWithLaura_Dialogue1_380") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_390") {Speaker = "Player", WaitTimeBeforeAllowingToProceed=1.5f},
            new ("MeetingWithLaura_Dialogue1_400") {Speaker = "Laura", Animation="PassItem"},
            new ("MeetingWithLaura_Dialogue1_410") {Speaker = "Laura"},
            new ("MeetingWithLaura_Dialogue1_420") {Speaker = "Player", Animation="WaveGoodbyeAloof"}
        }) {PlayerStartingPosition = new Vector2(13f, -0.5f), PlayerStartingFlipped = false, 
    DialogueSpeaker = Utils.GetUnit("Laura"), SpeakerStartingPosition = new Vector2(15.5f, -0.5f), SpeakerStartingFlipped = true, SpeakerEndingPosition=new Vector2(15.5f, -0.5f), SpeakerEndingFlipped=false, ReturnUnitsToOriginalPositions=false};
    }

    public static void OnStart_MeetingWithLaura_1() {
        Utils.GetUnit("Laura").transform.Find("Dialogue").gameObject.SetActive(false);
    }

    public static void MeetingWithLaura_Dialogue1_10() {
        Utils.GetUnit("Laura").PlayAnimation("Idle");
    }

    public static void MeetingWithLaura_Dialogue1_140() {
        Player.Instance.PlayAnimation("CatchItem");
        SaveFile.Instance.Money += 93000;
    }

    public static void MeetingWithLaura_Dialogue1_200() {
        UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);

    }

    public static void MeetingWithLaura_Dialogue1_210() {
        UIManager.Instance.HideBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
        Player.Instance.PlayAnimation("SittingAndRelaxing");
        Player.Instance.transform.position = new Vector2(11.5f, -4f);
        Utils.GetUnit("Laura").PlayAnimation("SittingAndContemplating");
        Utils.GetUnit("Laura").transform.position = new Vector2(15.5f, -3.7f);
        Area.Instance.transform.Find("Interactables/Laura's Desk/True Speech").gameObject.SetActive(true);
    }

    public static void MeetingWithLaura_Dialogue1_390() {
        Player.Instance.Actions.MoveToPoint(12.5f, -1.5f);
    }

    public static void MeetingWithLaura_Dialogue1_400() {
        Player.Instance.PlayAnimation("CatchItem");
        SaveFile.Instance.AddItem(typeof(Quest_TrimvineKeyToLaurasLab));
    }

    public static void OnEnd_MeetingWithLaura_1() {
        SaveFile.Instance.ExperiencePoints += 2000;
        Utils.GetUnit("Laura").transform.Find("Dialogue").gameObject.SetActive(true);
        Utils.GetUnit("Laura").PlayAnimation("ReadingBook");
        Area.Instance.transform.Find("Interactables/Back Exit (Warp)").gameObject.SetActive(true);
        Area.Instance.transform.Find("Interactables/Front Exit (Warp)").gameObject.SetActive(true);
        SaveFile.Instance.GetQuest("3LastChances").AdvanceObjective(100);
        SaveFile.Instance.CurrentMission = new Mission_TrimvineExploration();
        Quest_Exploration exploration = new Quest_Exploration() {Icon = "UI/HouseYellow"};
        Utils.ShowMissionObjective(exploration.Objectives[0]);
        UIManager.Objects.EscapeMissionButton.gameObject.SetActive(true);
        UIManager.Objects.EscapeMissionButtonLabel.SetLabel("{FinishMission}");
    }
}
