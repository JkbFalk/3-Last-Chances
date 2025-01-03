using System.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Area_IgnisManor
{
    public static int TrainingCount = 0;

    public static int AssassinTrapImpossible = 0;

    public static void OnStart() {
        if(SaveFile.Instance.Cycle == 2) {
            UIManager.Instance.StartDialogue(IgnisManor_Cycle2Introduction());
            CameraController.Instance.CenteredOnObject = Utils.GetUnit("Blaine_Cycle2").gameObject;
        }
        else if(SaveFile.Instance.Cycle == 3) {
            UIManager.Instance.StartDialogue(IgnisManor_Cycle3Introduction());
            CameraController.Instance.CenteredOnObject = Utils.GetUnit("Guard_Cycle3").gameObject;
        }
        else if(SaveFile.Instance.CurrentAreaLoadedFromSave == false) {
            Quest_Ignis quest = (Quest_Ignis)SaveFile.Instance.GetQuest("Ignis");
            quest.StartQuest(true);
            quest.GetObjective(0).UpdateStatusWithoutNotifying(QuestObjective.ObjectiveStatus.Current);
            quest.GetObjective(0).ShowAsMissionObjective();
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="IgnisManor_Interactables_0"});
        }
        EventManager.AbilityUsed.AddListener(CheckIfPerformingTraining);
    }

    public static void MakeAssassinTrapImpossible() {
        AssassinTrapImpossible++;
    }

    public static void OnStart_IgnisManor_Cycle2Introduction() {
        Utils.GetUnit("Blaine_Cycle2").gameObject.SetActive(true);
    }

    public static Dialogue IgnisManor_Cycle2Introduction() {
        return new Dialogue ("Area_IgnisManor", "IgnisManor_Cycle2Introduction", new List<DialogueLine>{
        new ("IgnisManor_Cycle2Introduction_0") {Animation="WaveGoodbyeAloof", Speaker = "Player"},
        new ("IgnisManor_Cycle2Introduction_10") {Animation="Doubtful", Speaker = "Blaine_Cycle2"},
        new ("IgnisManor_Cycle2Introduction_20") {Animation= "Sigh", Speaker = "Player"},
        new ("IgnisManor_Cycle2Introduction_30") {Animation="Approving", Speaker = "Blaine_Cycle2"},
        new ("IgnisManor_Cycle2Introduction_40") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisManor_Cycle2Introduction_50") {Animation="Thinking", Speaker = "Blaine_Cycle2"},
        new ("IgnisManor_Cycle2Introduction_60") {Animation="Determined", Speaker = "Player"},
        new ("IgnisManor_Cycle2Introduction_70") {Animation="FinallyDecided", Speaker = "Blaine_Cycle2"},
        new ("IgnisManor_Cycle2Introduction_80") {Animation="Talking", Speaker = "Player"},
        new ("IgnisManor_Cycle2Introduction_90") {Animation="Approving", Speaker = "Blaine_Cycle2"}
    }){DialogueSpeaker=Utils.GetUnit("Blaine_Cycle2"), PlayerStartingPosition = new Vector2(-45f, 1f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-42f, 1f), SpeakerStartingFlipped = true, ReturnUnitsToOriginalPositions = false};}

    public static void OnEnd_IgnisManor_Cycle2Introduction() {
        CanvasElements.SetActiveOnCanvasGroup(CanvasElements.UICanvasObject, false);
        Utils.MoveIntoArea(false, "IgnisVolcano");
    }

    public static void OnStart_IgnisManor_Cycle3Introduction() {
        Utils.GetUnit("Guard_Cycle3").gameObject.SetActive(true);
    }

    public static Dialogue IgnisManor_Cycle3Introduction() {
        return new Dialogue ("Area_IgnisManor", "IgnisManor_Cycle3Introduction", new List<DialogueLine>{
        new ("IgnisManor_Cycle3Intro_0") {Animation="StopRunning", Speaker = "Player"},
        new ("IgnisManor_Cycle3Intro_10") {Animation="Doubtful", Speaker = "Guard_Cycle3"},
        new ("IgnisManor_Cycle3Intro_20") {Animation= "Determined", Speaker = "Player"},
        new ("IgnisManor_Cycle3Intro_30") {Animation="Realization", Speaker = "Guard_Cycle3"}
    }){DialogueSpeaker=Utils.GetUnit("Guard_Cycle3"), PlayerStartingPosition = new Vector2(-45f, 1f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-42f, 1f), SpeakerStartingFlipped = true, ReturnUnitsToOriginalPositions = false};}

    public static void OnEnd_IgnisManor_Cycle3Introduction() {
        CanvasElements.SetActiveOnCanvasGroup(CanvasElements.UICanvasObject, false);
        Utils.MoveIntoArea(false, "IgnisVolcano_Summit");
    }

    public static Dialogue SabotageColten() {
        return new Dialogue ("Area_IgnisManor", "SabotageColten", new List<DialogueLine>{
        new (SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage4") ? "IgnisManor_Interactions_180" : SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage3") ? "IgnisManor_Interactions_170" : SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage2") ? "IgnisManor_Interactions_160" : "IgnisManor_Interactions_150") {Animation="TendingToPlants", Speaker = "Player", IdOfNextDialogueLine = "END"},
        });
    }

    public static void OnEnd_SabotageColten() {
        if(!SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage1")) {
            SaveFile.Instance.AddFlag("IgnisManor_ColtenSabotage1");
        }
        else if(!SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage2")){
            SaveFile.Instance.AddFlag("IgnisManor_ColtenSabotage2");
        }
        else if(!SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage3")){
            SaveFile.Instance.AddFlag("IgnisManor_ColtenSabotage3");
        }
        else if(!SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage4")){
            SaveFile.Instance.AddFlag("IgnisManor_ColtenSabotage4");
        }
    }

    public static void CheckIfPerformingTraining(Ability ability) {
        if(!SaveFile.Instance.HasFlag("IgnisManor_FinishedTraining") && ability.User == Player.Instance && Vector2.Distance(Player.Instance.transform.position, new Vector2(-27, 35.5f)) < 8 && (ability is BA_Greatsword_F || ability is BA_Polearm_F || ability is BA_Polearm_F)) {
            TrainingCount++;
            if(TrainingCount == 1) {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_Training_0"});
            }
            if(TrainingCount >= 5) {
                SaveFile.Instance.AddFlag("IgnisManor_FinishedTraining");
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_Training_10"});
                NotificationController.ShowTextNotification("IgnisManor_Training_20");
                SaveFile.Instance.ChangeIgnisEnergy(10);
                SaveFile.Instance.AddPermanentPowerUp("IgnisManor_WeaponTraining");
            }
        }
    }

    public static Dialogue Guard1() {
        return new Dialogue ("Area_IgnisManor", "Guard1", new List<DialogueLine>{
        new ("IgnisManor_Guard1_0") {Animation="HandWave", Speaker = "Player"},
        new ("IgnisManor_Guard1_10") {Animation="Doubtful", Speaker = "IgnisManorGuard"},
        new ("IgnisManor_Guard1_20") {Animation="YesMe", Speaker = "Player", IdOfNextDialogueLine = SaveFile.Instance.Level >= 30 ? "IgnisManor_Guard1_90" : "IgnisManor_Guard1_30"},
        new ("IgnisManor_Guard1_30") {Animation = "Mocking", Speaker = "IgnisManorGuard"},
        new ("IgnisManor_Guard1_40") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisManor_Guard1_50") {Animation="Talking", Speaker = "Player"},
        new ("IgnisManor_Guard1_60") {Animation="PreparingForBattle", Speaker = "IgnisManorGuard"},
        new ("IgnisManor_Guard1_70") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_Guard1_80") {Animation="Determined", Speaker = "IgnisManorGuard", IdOfNextDialogueLine="END"},
        new ("IgnisManor_Guard1_90") {Animation="Approving", Speaker = "IgnisManorGuard"},
    }){DialogueSpeaker=Utils.GetUnit("IgnisManorGuard"), PlayerStartingPosition = new Vector2(-17.5f, 0f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-15f, 0f), SpeakerStartingFlipped = true, ReturnUnitsToOriginalPositions = false};}

    public static void OnEnd_Guard1() {
        Utils.GetUnit("IgnisManorGuard").AttackPlayer();
        Utils.GetUnit("IgnisManorGuard").AddEffect(new Effect_CannotBeDefeated(true, new(Utils.GetUnit("IgnisManorGuard"))));
        EventManager.UnitWouldBeDefeated.AddListener(CheckIfGuardDefeated);
    }

    public static void IgnisManor_Guard1_90() {
        OpenFrontDoor();
        SaveFile.Instance.AddFlag("IgnisManor_GainedEntrance");
    }

    public static void CheckIfGuardDefeated(Damage damage) {
        if(damage.TargetOfDamage == Utils.GetUnit("IgnisManorGuard")) {
            Utils.GetUnit("IgnisManorGuard").SetToNeutralNPC();
            UIManager.Instance.ShowBlackScreen(1);
            GameController.Instance.WaitAndRunMethod(1, StartGuardDialogue);
        }
    }

    public static void StartGuardDialogue() {
        Utils.GetUnit("IgnisManorGuard").ChangeFaction(Constants.Faction.Neutral);
        UIManager.Instance.StartDialogue(Guard2());
    }

    public static Dialogue Guard2() {
        return new Dialogue ("Area_IgnisManor", "Guard2", new List<DialogueLine>{
        new ("IgnisManor_Guard2_0") {Animation="WobblyGetUp", Speaker = "IgnisManorGuard"},
        new ("IgnisManor_Guard2_10") {Animation="Determined", Speaker = "Player"},
    }){DialogueSpeaker=Utils.GetUnit("IgnisManorGuard"), PlayerStartingPosition = new Vector2(-16.5f, 0), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-14, 0), SpeakerStartingFlipped = true};}

    public static void OnEnd_Guard2() {
        OpenFrontDoor();
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(10);
        SaveFile.Instance.AddFlag("IgnisManor_GainedEntrance");
        Utils.GetUnit("IgnisManorGuard").PlayAnimation("StandingAround");
    }

    public static void OpenFrontDoor() {
        Area.Instance.transform.Find("Environment/FrontDoor1/Closed").GetComponent<Door>().CanBeOpened = true;
        Area.Instance.transform.Find("Environment/FrontDoor1/Closed").GetComponent<Door>().ActivateInteractable();
        Area.Instance.transform.Find("Environment/FrontDoor2/Closed").GetComponent<Door>().CanBeOpened = true;
        Area.Instance.transform.Find("Environment/FrontDoor2/Closed").GetComponent<Door>().ActivateInteractable();
        Area.Instance.transform.Find("Interactables/Entrance Block").gameObject.SetActive(false);
    }

    public static Dialogue Colten1() {
        return new Dialogue ("Area_IgnisManor", "Colten1", new List<DialogueLine>{
        new ("IgnisManor_Colten1_0") {Animation="SternNo", Speaker = "Colten"},
        new ("IgnisManor_Colten1_10") {Animation="Realization", Speaker = "Player"},
        new ("IgnisManor_Colten1_20") {Animation="Mocking", Speaker = "Player"},
        new ("IgnisManor_Colten1_30") {Animation="Irritated", Speaker = "Colten"},
        new ("IgnisManor_Colten1_40") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_Colten1_50") {Animation="Sigh", Speaker = "Colten"},
        new ("IgnisManor_Colten1_60") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisManor_Colten1_70") {Animation="GiveMeABreak", Speaker = "Colten"},
        new ("IgnisManor_Colten1_80") {Speaker = "Player"},
        new ("IgnisManor_Colten1_90") {Animation="ComeAtMe", Speaker = "Colten"},
        new ("IgnisManor_Colten1_100") {Animation="Thinking", Speaker = "Player"},
        new ("IgnisManor_Colten1_110") {Animation="Mocking", Speaker = "Colten"},
        new ("IgnisManor_Colten1_120") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisManor_Colten1_130") {Speaker = "Colten"}
    }){DialogueSpeaker=Utils.GetUnit("Colten"), PlayerStartingPosition = new Vector2(7f, 3f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(9, 3), SpeakerStartingFlipped = true};}

    public static void IgnisManor_Colten1_130() {
        Utils.GetUnit("Colten").CanRun = false;
        Utils.GetUnit("Colten").Actions.MoveToPoint(5, 5);
        UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }
    
    public static void OnEnd_Colten1() {
        Utils.GetUnit("Colten").CanRun = true;
        Utils.GetUnit("Colten").gameObject.SetActive(false);
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(20);
        Area.Instance.transform.Find("Interactables/InterruptMeeting").gameObject.SetActive(true);
    }

    public static void OnStart_Meeting() {
        Area.Instance.transform.Find("Environment/MeetingDoor1/Closed").GetComponent<Door>().CanBeOpened = true;
        Area.Instance.transform.Find("Environment/MeetingDoor2/Closed").GetComponent<Door>().CanBeOpened = true;
    }

    public static Dialogue Meeting() {
        return new Dialogue ("Area_IgnisManor", "Meeting", new List<DialogueLine>{
        new ("IgnisManor_Meeting_0") {Animation="LeaningOnTableAndDiscussing", Speaker = "Blaine"},
        new ("IgnisManor_Meeting_10") {Speaker = "Player", Animation="WaveGoodbyeAloof"},
        new ("IgnisManor_Meeting_20") {Animation="ShoulderShrug", Speaker = "Gerrald"},
        new ("IgnisManor_Meeting_30") {Animation="FinallyDecided", Speaker = "Maginhart"},
        new ("IgnisManor_Meeting_40") {Animation="Approving", Speaker = "Player"},
        new ("IgnisManor_Meeting_50") {Animation="Intrigued", Speaker = "Blaine"},
        new ("IgnisManor_Meeting_60") {Animation="Admiring", Speaker = "Maginhart"},
        new ("IgnisManor_Meeting_70") {Animation="Flabbergasted", Speaker = "Blaine"},
        new ("IgnisManor_Meeting_80") {Animation="HandWave", Speaker = "Gerrald"},
        new ("IgnisManor_Meeting_90") {Animation="FinallyDecided", Speaker = "Blaine"},
        new ("IgnisManor_Meeting_100") {Animation="Explaining", Speaker = "Alex"},
        new ("IgnisManor_Meeting_110") {Animation="Approving", Speaker = "Maginhart"},
        new ("IgnisManor_Meeting_120") {Animation="HereWeGo", Speaker = "Blaine"},
        new ("IgnisManor_Meeting_130") {Animation="Thinking", Speaker = "Alex"},
        new ("IgnisManor_Meeting_140") {Animation="NotQuite", Speaker = "Blaine"},
        new ("IgnisManor_Meeting_150") {Speaker = "Blaine"}
    }){PlayerStartingPosition = new Vector2(9f, 0), PlayerStartingFlipped = false};}

    public static void IgnisManor_Meeting_10() {
        Area.Instance.transform.Find("Environment/MeetingDoor1/Closed").GetComponent<Door>().ActivateInteractable();
        Area.Instance.transform.Find("Environment/MeetingDoor2/Closed").GetComponent<Door>().ActivateInteractable();
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Interact/Open Door", 0.8f);
    }

    public static void IgnisManor_Meeting_40() {
        UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }

    public static void IgnisManor_Meeting_50() {
        Player.Instance.transform.position = new Vector2(12, 0); 
        UIManager.Instance.HideBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }

    public static void IgnisManor_Meeting_150() {
        Utils.GetUnit("Blaine").Actions.MoveToPoint(19.5f, 8);
        UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }

    public static void OnEnd_Meeting() {
        SaveFile.Instance.AddFlag("IgnisManor_FinishedMeeting");
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(30);
    }

    public static void OnStart_Blaine1() {
        Utils.GetUnit("Maginhart").gameObject.SetActive(false);
        Utils.GetUnit("Gerrald").gameObject.SetActive(false);
    }

    public static Dialogue Blaine1() {
        return new Dialogue ("Area_IgnisManor", "Blaine1", new List<DialogueLine>{
        new ("IgnisManor_Blaine1_0") {Animation="Admiring", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_10") {Animation="HaveItYourWayThen", Speaker = "Player"},
        new ("IgnisManor_Blaine1_20") {Animation="ShoulderShrug", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_30") {Animation="Flabbergasted", Speaker = "Player"},
        new ("IgnisManor_Blaine1_40") {Animation="Explaining", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_50") {Animation="Doubtful", Speaker = "Player"},
        new ("IgnisManor_Blaine1_60") {Animation="Approving", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_70") {Animation="Accusing", Speaker = "Player"},
        new ("IgnisManor_Blaine1_80") {Animation="Sigh", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_BlaineWhoCaresAboutPublic_0"),
            new ("IgnisManor_BlaineCareAboutPublic_0")
        }},
        new ("IgnisManor_BlaineWhoCaresAboutPublic_10") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_BlaineWhoCaresAboutPublic_20") {Animation="Approving", Speaker = "Blaine2"},
        new ("IgnisManor_BlaineWhoCaresAboutPublic_30") {Animation="NotQuite", Speaker = "Player"},
        new ("IgnisManor_BlaineWhoCaresAboutPublic_40") {Animation="ShoulderShrug", Speaker = "Blaine2"},
        new ("IgnisManor_BlaineWhoCaresAboutPublic_50") {Animation="Accusing", Speaker = "Player"},
        new ("IgnisManor_BlaineWhoCaresAboutPublic_60") {Animation="YouFinallyGetIt", Speaker = "Blaine2", IdOfNextDialogueLine="IgnisManor_Blaine1_90"},
        new ("IgnisManor_BlaineCareAboutPublic_10") {Animation="Accusing", Speaker = "Player"},
        new ("IgnisManor_BlaineCareAboutPublic_20") {Animation="YesYou", Speaker = "Blaine2"},
        new ("IgnisManor_BlaineCareAboutPublic_30") {Animation="Realization", Speaker = "Player"},
        new ("IgnisManor_BlaineCareAboutPublic_40") {Animation="Laugh", Speaker = "Blaine2", IdOfNextDialogueLine="IgnisManor_Blaine1_90"},
        new ("IgnisManor_Blaine1_90") {Animation="FinallyDecided", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_100") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisManor_Blaine1_110") {Animation="TryingToRemember", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_120") {Animation="Explaining", Speaker = "Blaine2"},
        new ("IgnisManor_Blaine1_130") {Animation="HereWeGo", Speaker = "Player"},
        new ("IgnisManor_Blaine1_140") {Animation="Approving", Speaker = "Blaine2"}
    }){DialogueSpeaker=Utils.GetUnit("Blaine2"), PlayerStartingPosition = new Vector2(17.5f, 12), PlayerStartingFlipped = true, SpeakerStartingPosition = new Vector2(15, 12), SpeakerStartingFlipped = false};}

    public static void OnEnd_Blaine1() {
        SaveFile.Instance.AddFlag("IgnisManor_TalkedWithBlaine1");
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(40);
        SaveFile.Instance.GetQuest("Ignis").CurrentObjective.DescriptionParameters = new() {SaveFile.Instance.IgnisEnergy.ToString()};
    }

    public static Dialogue BlaineNotYet() {
        return new Dialogue ("Area_IgnisManor", "BlaineNotYet", new List<DialogueLine>{
        new ("IgnisManor_BlaineNotYet_0") {Animation="HandWave", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineNotYet_10") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_BlaineNotYet_20") {Animation="Approving", Speaker = "Blaine3"},
    }){DialogueSpeaker=Utils.GetUnit("Blaine3"), PlayerStartingPosition = new Vector2(-35, 10.5f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-32.5f, 10.5f), SpeakerStartingFlipped = true};}

    public static Dialogue BlaineWrapUp1() {
        return new Dialogue ("Area_IgnisManor", "BlaineWrapUp1", new List<DialogueLine>{
        new ("IgnisManor_BlaineWrapUp_0") {Animation="HandWave", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUp_10") {Animation="Sigh", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp_20") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUp_30") {Animation="StandingAround", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp_40") {Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUp_50") {Animation="ShoulderShrug", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp_60") {Animation="Flabbergasted", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUp_70") {Animation="Determined", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp_80") {Animation="HereWeGo", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUp_90") {Animation="FinallyDecided", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_BlaineWrapUpNoChoice_0"),
            new ("IgnisManor_BlaineWrapUpBetrayal_0")
        }},
        new ("IgnisManor_BlaineWrapUpNoChoice_10") {Animation="Approving", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpNoChoice_20") {Animation="Thinking", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUpNoChoice_30") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpNoChoice_40") {Animation="FinallyDecided", Speaker = "Blaine3", IdOfNextDialogueLine="IgnisManor_BlaineWrapUp_100"},
        new ("IgnisManor_BlaineWrapUpBetrayal_10") {Animation="Accusing", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpBetrayal_20") {Animation="Intrigued", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUpBetrayal_30") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpBetrayal_40") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpBetrayal_50") {Animation="Approving", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUpBetrayal_60") {Animation="SternNo", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpBetrayal_70") {Animation="ShoulderShrug", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUpBetrayal_80") {Animation="Realization", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpBetrayal_90") {Animation="Approving", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUpBetrayal_100") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpBetrayal_110") {Animation="YesYou", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUpBetrayal_120") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUpBetrayal_130") {Animation="StandingAround", Speaker = "Blaine3", IdOfNextDialogueLine="IgnisManor_BlaineWrapUp_100"},
        new ("IgnisManor_BlaineWrapUp_100") {Animation="Realization", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp_110") {Animation="GiveMeABreak", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUp_120") {Animation="ComeAtMe", Speaker = "Blaine3"}
    }){DialogueSpeaker=Utils.GetUnit("Blaine3"), PlayerStartingPosition = new Vector2(-35, 10.5f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-32.5f, 10.5f), SpeakerStartingFlipped = true, PlayerEndingPosition= new(-45, 7.5f), PlayerEndingFlipped=false, SpeakerEndingPosition=new(-41, 7.5f), SpeakerEndingFlipped=true};}

    public static void OnEnd_BlaineWrapUp1() {
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Area_IgnisManor.StartBlaineDuel"});
        StartBlaineDuel();
    }

    public static void StartBlaineDuel() {
        GameController.Instance.PlayBossMusicDuringNextCombat = true;
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(60, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        Utils.GetUnit("Blaine3").AttackPlayer();
        Utils.GetUnit("Blaine3").AddEffect(new Effect_CannotBeDefeated(true, new(Utils.GetUnit("Blaine3"))));
        EventManager.UnitWouldBeDefeated.AddListener(CheckIfBlaineDefeated);
    }

    public static void IgnisManor_BlaineWrapUpNoChoice_10() {
        SaveFile.Instance.ChangeIgnisEnergy(10);
        SaveFile.Instance.AddPermanentPowerUp("IgnisManor_BackstabPowerUp");
    }

    public static void IgnisManor_BlaineWrapUpBetrayal_10() {
        SaveFile.Instance.ChangeIgnisEnergy(20);
        SaveFile.Instance.AddPermanentPowerUp("IgnisManor_BurningPowerUp");
    }

    public static void CheckIfBlaineDefeated(Damage dmg) {
        if(dmg.TargetOfDamage.gameObject.name.Contains("Blaine3")) {
            SaveFile.Instance.ChangeIgnisEnergy(30);
            Utils.GetUnit("Blaine3").SetToNeutralNPC();
            UIManager.Instance.StartDialogue(BlaineWrapUp2());
        } 
    }

    public static Dialogue BlaineWrapUp2() {
        return new Dialogue ("Area_IgnisManor", "BlaineWrapUp2", new List<DialogueLine>{
        new ("IgnisManor_BlaineWrapUp2_0") {Animation="NotBad", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp2_10") {Animation="HereWeGo", Speaker = "Player"},
        new ("IgnisManor_BlaineWrapUp2_20") {Animation="NotQuite", Speaker = "Blaine3"},
        new ("IgnisManor_BlaineWrapUp2_30") {Animation="FinallyDecided", Speaker = "Player"},
    }){DialogueSpeaker=Utils.GetUnit("Blaine3"), PlayerStartingPosition = new Vector2(-45, 7.5f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-41f, 7.5f), SpeakerStartingFlipped = true};}

    public static void OnEnd_BlaineWrapUp2() {
        SaveFile.Instance.IncreaseEnergyLevel(Ability.AbilityFamily.Ignis);
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(70, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        SaveFile.Instance.GetQuest("Ignis").CurrentObjective.DescriptionParameters = new() {SaveFile.Instance.IgnisEnergy.ToString()};
        SaveFile.Instance.CurrentMission.MakeMissionFinishableUsingButton();
        GameController.Instance.MakeAutoSave();
    }

    public static List<GameObject> ExpiredIgnisOneTimeEnergyIncreases = new ();

    public static void SlightlyRaiseIgnisEnergy(InteractableObject obj) {
        if(ExpiredIgnisOneTimeEnergyIncreases.Contains(obj.gameObject) == false) {
            ExpiredIgnisOneTimeEnergyIncreases.Add(obj.gameObject);
            SaveFile.Instance.ChangeIgnisEnergy(2);
        }
    }

    public static void ModeratelyRaiseIgnisEnergy(InteractableObject obj) {
        if(ExpiredIgnisOneTimeEnergyIncreases.Contains(obj.gameObject) == false) {
            ExpiredIgnisOneTimeEnergyIncreases.Add(obj.gameObject);
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
    }

    public static Dialogue Nathalie() {
        return new Dialogue ("Area_IgnisManor", "Nathalie", new List<DialogueLine>{
        new ("IgnisManor_Nathalie_0") {Animation="Sigh", Speaker = "Nathalie"},
        new ("IgnisManor_Nathalie_Choices") {Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_NathalieIntroduction_0"),
            new ("IgnisManor_NathalieAskAboutPlants_0"),
            new ("IgnisManor_NathalieAskAboutFamily_0"),
            new ("IgnisManor_NathalieAskAboutIgnis_0"),
            new ("End") {IdOfNextDialogueLine="END"}
        }},
        new ("IgnisManor_NathalieIntroduction_10") {Animation="WaveGoodbyeAloof", Speaker = "Player"},
        new ("IgnisManor_NathalieIntroduction_20") {Animation="ShoulderShrug", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieIntroduction_30") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisManor_NathalieIntroduction_40") {Animation="Laugh", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieIntroduction_50") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisManor_NathalieIntroduction_60") {Animation="Laugh", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieIntroduction_70") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_NathalieIntroduction_80") {Animation="FinallyDecided", Speaker = "Nathalie", IdOfNextDialogueLine="IgnisManor_Nathalie_Choices"},
        new ("IgnisManor_NathalieAskAboutPlants_10") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutPlants_20") {Animation="Thinking", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutPlants_Choices") {Animation="FinallyDecided", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_NathalieAskAboutPlantsShort_0"),
            new ("IgnisManor_NathalieAskAboutPlantsLong_0")
        }},
        new ("IgnisManor_NathalieAskAboutPlantsShort_10") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutPlantsShort_20") {Animation="Explaining", Speaker = "Nathalie", IdOfNextDialogueLine="IgnisManor_Nathalie_Choices"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_10") {Animation="ComeAtMe", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_20") {Animation="Explaining", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_30") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_40") {Animation="PointDown", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_50") {Animation="Encourage", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_60") {Animation="Sigh", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_70") {Animation="Thinking", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutPlantsLong_80") {Animation="NotBad", Speaker = "Nathalie", IdOfNextDialogueLine="IgnisManor_Nathalie_Choices"},
        new ("IgnisManor_NathalieAskAboutFamily_10") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutFamily_20") {Animation="Sigh", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutFamily_30") {Animation="Talking", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutFamily_40") {Animation="Irritated", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutFamily_50") {Animation="Thinking", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutFamily_60") {Animation="Relief", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutFamily_70") {Animation="Irritated", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutFamily_80") {Animation="HandWave", Speaker = "Nathalie", IdOfNextDialogueLine="IgnisManor_Nathalie_Choices"},
        new ("IgnisManor_NathalieAskAboutIgnis_10") {Animation="Stoic", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutIgnis_20") {Animation="Thinking", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutIgnis_30") {Animation="Irritated", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutIgnis_40") {Animation="Intrigued", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutIgnis_50") {Animation="TryingToRemember", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutIgnis_60") {Animation="YouFinallyGetIt", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutIgnis_70") {Animation="HandWave", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutIgnis_80") {Animation="ComposeOneself", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutIgnis_90") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutIgnis_100") {Animation="Explaining", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutIgnis_110") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutIgnis_120") {Animation="ShoulderShrug", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutIgnis_130") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisManor_NathalieAskAboutIgnis_140") {Animation="TryingToRemember", Speaker = "Nathalie"},
        new ("IgnisManor_NathalieAskAboutIgnis_150") {Animation="FinallyDecided", Speaker = "Player", IdOfNextDialogueLine="IgnisManor_Nathalie_Choices"}
    }){DialogueSpeaker=Utils.GetUnit("Nathalie"), PlayerStartingPosition = new Vector2(-53f, -26.5f), PlayerStartingFlipped = true, SpeakerStartingPosition = new Vector2(-55.5f, -26.5f), SpeakerEndingFlipped = true, SpeakerStartingFlipped = false};}
 
    public static void OnEnd_Nathalie() {
        Utils.GetUnit("Nathalie").Actions.IsFlipped = true;
    }

    public static bool CheckIfVisible_IgnisManor_NathalieAskAboutFamily_0() {
        return SaveFile.Instance.Flags.Contains("IgnisManor_NathalieFinishedIntroduction");
    }

    public static void OnEnd_IgnisManor_NathalieIntroduction_80() {
        if(SaveFile.Instance.HasFlag("IgnisManor_NathalieIntroduction") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
        SaveFile.Instance.AddFlag("IgnisManor_NathalieIntroduction");
    }

    public static void OnEnd_IgnisManor_NathalieAskAboutPlantsShort_20() {
        if(SaveFile.Instance.HasFlag("IgnisManor_NathaliePlants}") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
        SaveFile.Instance.AddFlag("IgnisManor_NathaliePlants");
    }

    public static void OnEnd_IgnisManor_NathalieAskAboutPlantsLong_80() {
        if(SaveFile.Instance.HasFlag("IgnisManor_NathaliePlants") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
        SaveFile.Instance.AddFlag("IgnisManor_NathaliePlants");
    }

    public static void OnEnd_IgnisManor_NathalieAskAboutFamily_80() {
        if(SaveFile.Instance.HasFlag("IgnisManor_NathalieFamily") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
        SaveFile.Instance.AddFlag("IgnisManor_NathalieFamily");
    }

    public static void OnEnd_IgnisManor_NathalieAskAboutIgnis_150() {
        if(SaveFile.Instance.HasFlag("IgnisManor_NathalieIgnis") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        SaveFile.Instance.AddFlag("IgnisManor_NathalieIgnis");
    }

    public static bool CheckIfVisible_IgnisManor_NathalieAskAboutIgnis_0() {
        return SaveFile.Instance.Flags.Contains("IgnisManor_NathalieAskedAboutFamily");
    }

    public static void IgnisManor_NathalieIntroduction_10() {
        SaveFile.Instance.AddFlag("IgnisManor_NathalieFinishedIntroduction");
    }

    public static void IgnisManor_NathalieIntroduction_20() {
        Utils.GetUnit("Nathalie").DisplayedName = "Nathalie";
    }

    public static void IgnisManor_NathalieAskAboutFamily_10() {
        SaveFile.Instance.AddFlag("IgnisManor_NathalieAskedAboutFamily");
    }

    public static Dialogue SwordGraves() {
        return new Dialogue ("Area_IgnisManor", "SwordGraves", new List<DialogueLine>{
        new ("IgnisManor_SwordGraves_0") {},
        new ("IgnisManor_SwordGraves_10") {Animation="Intrigued", Speaker = "Player", ShowSpeakerBox=false},
        new ("IgnisManor_SwordGraves_20") {Animation="Admiring", Speaker = "Player", ShowSpeakerBox=false},
        new ("IgnisManor_SwordGraves_30") {Animation="HereWeGo", Speaker = "Player", ShowSpeakerBox=false}
    }){PlayerStartingPosition = new Vector2(-45f, 15f), PlayerStartingFlipped = false};}

    public static void OnEnd_SwordGraves() {
        if(SaveFile.Instance.HasFlag("IgnisManor_SwordMonuments") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
        SaveFile.Instance.AddFlag("IgnisManor_IgnisManor_SwordMonuments");
    }

    public static Dialogue AskAboutBook() {
        return new Dialogue ("Area_IgnisManor", "AskAboutBook", new List<DialogueLine>{
        new ("IgnisManor_AskAboutBook_0") {Animation="WaveGoodbyeAloof", Speaker = "Player"},
        new ("IgnisManor_AskAboutBook_10") {Animation="Laugh", Speaker = "IgnisElder"},
        new ("IgnisManor_AskAboutBook_20") {Animation="Grateful", Speaker = "Player"},
        new ("IgnisManor_AskAboutBook_30") {Animation="HandWave", Speaker = "IgnisElder", IdOfNextDialogueLine=SaveFile.Instance.HasFlag("IgnisManor_NathalieFinishedIntroduction") ? "IgnisManor_AskAboutBook_70" : "IgnisManor_AskAboutBook_40" },
        new ("IgnisManor_AskAboutBook_40") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisManor_AskAboutBook_50") {Animation="Deflated", Speaker = "IgnisElder"},
        new ("IgnisManor_AskAboutBook_60") {Animation="Grateful", Speaker = "Player", IdOfNextDialogueLine="END"},
        new ("IgnisManor_AskAboutBook_70") {Animation="Approving", Speaker = "Player"},
        new ("IgnisManor_AskAboutBook_80") {Animation="NotQuite", Speaker = "IgnisElder"},
        new ("IgnisManor_AskAboutBook_90") {Animation="Grateful", Speaker = "Player"}
    }){DialogueSpeaker=Utils.GetUnit("IgnisElder"), PlayerStartingPosition = new Vector2(3, -28.5f), PlayerStartingFlipped = false, PlayerEndingPosition=new Vector2(-4, -26.5f), PlayerEndingFlipped = true, SpeakerStartingPosition = new Vector2(5, -28.5f), SpeakerStartingFlipped = true, ReturnUnitsToOriginalPositions=false,};}

    public static void OnEnd_AskAboutBook() {
        SaveFile.Instance.AddFlag("IgnisManor_LearnedAboutIgnisImportantBook");
    }

    public static Dialogue Colten2() {
        return new Dialogue ("Area_IgnisManor", "Colten2", new List<DialogueLine>{
        new ("IgnisManor_Colten2_0") {Animation="HereWeGo", Speaker = "Colten2"},
        new ("IgnisManor_Colten2_10") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManor_Colten2_20") {Animation="Mocking", Speaker = "Colten2"},
        new ("IgnisManor_Colten2_30") {Animation="MockAndTakeOutHeavy", Speaker = "Player", IdOfNextDialogueLine = SaveFile.Instance.HasFlag("SabotagedColtenArena") ? "IgnisManor_Colten2_60" : "IgnisManor_Colten2_40"},
        new ("IgnisManor_Colten2_40") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisManor_Colten2_50") {Animation="ThreatenWithHeavy", Speaker = "Colten2", IdOfNextDialogueLine="END"},
        new ("IgnisManor_Colten2_60") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisManor_Colten2_70") {Animation="GiveMeABreak", Speaker = "Colten2"},
        new ("IgnisManor_Colten2_80") {Animation="HereWeGo", Speaker = "Player"}
    }){DialogueSpeaker=Utils.GetUnit("Colten2"), PlayerStartingPosition = new Vector2(-57f, 36f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-53.5f, 36), SpeakerStartingFlipped = true, ReturnUnitsToOriginalPositions=false};}

    public static void OnStart_Colten2() {
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Area_IgnisManor.NotifyAboutColtenIntentions"});
    }

    public static void IgnisManor_Colten2_40() {
        Area.Instance.transform.Find("Environment/VisualEffect_ColtenBarricade").gameObject.SetActive(true);
        CameraController.Instance.CenteredOnObject = Area.Instance.transform.Find("Environment/VisualEffect_ColtenBarricade").gameObject;
    }

    public static void IgnisManor_Colten2_60() {
        Area.Instance.transform.Find("Environment/VisualEffect_ColtenBarricade").gameObject.SetActive(true);
        CameraController.Instance.CenteredOnObject = Area.Instance.transform.Find("Environment/VisualEffect_ColtenBarricade").gameObject;
    }

    public static void NotifyAboutColtenIntentions() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_Colten2_90"});
    }

    public static Dialogue IgnisMausoleumRock() {
        return new Dialogue ("Area_IgnisManor", "IgnisMausoleumRock", new List<DialogueLine>{
        new ("IgnisManor_IgnisMausoleumRock_0") {},
        new ("IgnisManor_IgnisMausoleumRock_10") {Animation="Thinking", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(-7.5f, -13.5f), PlayerStartingFlipped = true};}

    public static void OnEnd_IgnisMausoleumRock() {
        if(SaveFile.Instance.HasFlag("IgnisManor_MuseumRock") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(2);
        }
        SaveFile.Instance.AddFlag("IgnisManor_MuseumRock");
    }

    public static Dialogue IgnisMausoleumPotion() {
        return new Dialogue ("Area_IgnisManor", "IgnisMausoleumPotion", new List<DialogueLine>{
        new ("IgnisManor_IgnisMausoleumPotion_0") {},
        new ("IgnisManor_IgnisMausoleumPotion_10") {Animation="Thinking", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(-4f, -13f), PlayerStartingFlipped = false};}

    public static void OnEnd_IgnisMausoleumPotion() {
        if(SaveFile.Instance.HasFlag("IgnisManor_MuseumPotion") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(2);
            SaveFile.Instance.HealUpgrades++;
        }
        SaveFile.Instance.AddFlag("IgnisManor_MuseumPotion");
    }

    public static Dialogue IgnisMausoleumFlower() {
        return new Dialogue ("Area_IgnisManor", "IgnisMausoleumFlower", new List<DialogueLine>{
        new ("IgnisManor_IgnisMausoleumFlower_0") {},
        new ("IgnisManor_IgnisMausoleumFlower_10") {Animation="ShoulderShrug", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(2, -12.5f), PlayerStartingFlipped = false};}

    public static void OnEnd_IgnisMausoleumFlower() {
        if(SaveFile.Instance.HasFlag("IgnisManor_MuseumFlower") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(2);
        }
        SaveFile.Instance.AddFlag("IgnisManor_MuseumFlower");
    }

    public static Dialogue IgnisMausoleumUltimateMaterial() {
        return new Dialogue ("Area_IgnisManor", "IgnisMausoleumUltimateMaterial", new List<DialogueLine>{
        new ("IgnisManor_IgnisMausoleumUltimateMaterial_0") {},
        new ("IgnisManor_IgnisMausoleumUltimateMaterial_10") {Animation="Sigh", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(7.5f, -13.5f), PlayerStartingFlipped = false};}

    public static void OnEnd_IgnisMausoleumUltimateMaterial() {
        if(SaveFile.Instance.HasFlag("IgnisManor_MuseumMaterials") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(2);
        }
        SaveFile.Instance.AddFlag("IgnisManor_MuseumMaterials");
    }

    public static Dialogue IgnisMausoleumArmour() {
        return new Dialogue ("Area_IgnisManor", "IgnisMausoleumArmour", new List<DialogueLine>{
        new ("IgnisManor_IgnisMausoleumArmour_0") {},
        new ("IgnisManor_IgnisMausoleumArmour_10") {Animation="Doubtful", Speaker = "Player"},
        new ("IgnisManor_IgnisMausoleumArmour_20") {Animation="Thinking", Speaker = "Player"},
        new ("IgnisManor_IgnisMausoleumArmour_30") {Animation="InvestigateClosely", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(-4.5f, -16.5f), PlayerStartingFlipped = true};}

    public static void OnEnd_IgnisMausoleumArmour() {
        if(SaveFile.Instance.HasFlag("IgnisManor_MuseumArmour") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        SaveFile.Instance.AddFlag("IgnisManor_MuseumArmour");
    }

    public static Dialogue IgnisMausoleumWeapon() {
        return new Dialogue ("Area_IgnisManor", "IgnisMausoleumWeapon", new List<DialogueLine>{
        new ("IgnisManor_IgnisMausoleumWeapon_0") {},
        new ("IgnisManor_IgnisMausoleumWeapon_10") {Animation="Doubtful", Speaker = "Player"},
        new ("IgnisManor_IgnisMausoleumWeapon_20") {Animation="InvestigateClosely", Speaker = "Player"},
        new ("IgnisManor_IgnisMausoleumWeapon_30") {Animation="ShoulderShrug", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(4.5f, -16.5f), PlayerStartingFlipped = false};}

    public static void OnEnd_IgnisMausoleumWeapon() {
        if(SaveFile.Instance.HasFlag("IgnisManor_MuseumWeapon") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        SaveFile.Instance.AddFlag("IgnisManor_MuseumWeapon");
    }

    public static void OnEnd_Colten2() {
        int sabotageLevel = SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage4") ? 4 : SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage3") ? 3 : SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage2") ? 2 : SaveFile.Instance.HasFlag("IgnisManor_ColtenSabotage1") ? 1 : 0;
        Player.Instance.AddEffect(new Effect_Poison(10f - 2f * sabotageLevel, new(Utils.GetUnit("Colten2"))));
        Player.Instance.AddEffect(new Effect_Feeble(50f - 10f * sabotageLevel, new(Utils.GetUnit("Colten2"))));
        Utils.GetUnit("Colten2").AttackPlayer();
        EventManager.EnemyDefeated.AddListener(CheckIfColten2Defeated);
    }

    public static void CheckIfColten2Defeated(Damage damage) {
        if(damage.TargetOfDamage.gameObject.name == "Unit_Colten2") {
            Area.Instance.transform.Find("Interactables/Colten Info").gameObject.SetActive(true);
            Area.Instance.transform.Find("Interactables/Colten Info").transform.position = damage.TargetOfDamage.transform.position;
            SaveFile.Instance.ChangeIgnisEnergy(30);
            Area.Instance.transform.Find("Environment/VisualEffect_ColtenBarricade").gameObject.SetActive(false);
            SaveFile.Instance.AddFlag("IgnisManor_DefeatedColten");
        }
    }

    public static Dialogue AskAboutIgnisBooks() {
        return new Dialogue ("Area_IgnisManor", "LearnedAboutIgnisImportantBook", new List<DialogueLine>{
        new ("IgnisManor_IgnisMausoleumWeapon_0") {},
        new ("IgnisManor_IgnisMausoleumWeapon_10") {Animation="Doubtful", Speaker = "Player"},
        new ("IgnisManor_IgnisMausoleumWeapon_20") {Animation="InvestigateClosely", Speaker = "Player"},
        new ("IgnisManor_IgnisMausoleumWeapon_30") {Animation="ShoulderShrug", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(4.5f, -16.5f), PlayerStartingFlipped = false};}

    public static void IgnisManorShop(InteractableObject inter) {
        MenuManager.Instance.OpenShop(new() {
        }, 
            "IgnisManorShop");
    }
                                                
    public static Dialogue InviteToDuel_Swordmaster() {
        return new Dialogue ("Area_IgnisManor", "InviteToDuel_Swordmaster", new List<DialogueLine>{
        new ("IgnisManor_Duel_0") {TurnSpeakersToFaceEachOther = false}, 
        new ("IgnisManor_Duel_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_Duel_1") {IdOfNextDialogueLine = "IgnisManor_Duel_30"},
            new ("Leave") {IdOfNextDialogueLine = "END"}
        }},
        new ("IgnisManor_Duel_30") {Animation="HereWeGo", Speaker = "IgnisSwordmaster", TurnSpeakersToFaceEachOther = false}
    }){DialogueSpeaker=Utils.GetUnit("IgnisSwordmaster"), PlayerStartingPosition = new Vector2(0f, 17f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(2f, 17), SpeakerStartingFlipped = false};}

    public static void OnEnd_IgnisManor_Duel_30() {
        StartDuel("IgnisSwordmaster");
    }

    public static Dialogue InviteToDuel_Lancer() {
        return new Dialogue ("Area_IgnisManor", "InviteToDuel_Swordmaster", new List<DialogueLine>{
        new ("IgnisManor_Duel_0"),
        new ("IgnisManor_Duel_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_Duel_1") {IdOfNextDialogueLine = "IgnisManor_Duel_10"},
            new ("Leave") {IdOfNextDialogueLine = "END"}
        }},
        new ("IgnisManor_Duel_10") {Animation="PreparingForBattle", Speaker = "IgnisLancer"}
    }){DialogueSpeaker=Utils.GetUnit("IgnisLancer"), PlayerStartingPosition = new Vector2(-4.0f, 5f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-2, 5), SpeakerStartingFlipped = true};}

    public static void OnEnd_IgnisManor_Duel_10() {
        StartDuel("IgnisLancer");
    }

    public static Dialogue InviteToDuel_Cannonier() {
        return new Dialogue ("Area_IgnisManor", "InviteToDuel_Cannonier", new List<DialogueLine>{
        new ("IgnisManor_Duel_0"),
        new ("IgnisManor_Duel_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_Duel_1") {IdOfNextDialogueLine = "IgnisManor_Duel_20"},
            new ("Leave") {IdOfNextDialogueLine = "END"}
        }},
        new ("IgnisManor_Duel_20") {Animation="ShoulderShrug", Speaker = "IgnisCannonier"}
    }){DialogueSpeaker=Utils.GetUnit("IgnisCannonier"), PlayerStartingPosition = new Vector2(-2f, -5f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(0, -5), SpeakerStartingFlipped = true};}

    public static void OnEnd_IgnisManor_Duel_20() {
        StartDuel("IgnisCannonier");
    }

    public static Dialogue InviteToDuel_Assassin() {
        return new Dialogue ("Area_IgnisManor", "InviteToDuel_Pyromancer", new List<DialogueLine>{
        new ("IgnisManor_InviteAssassinToDuel_0") {IdOfNextDialogueLine = (AssassinTrapImpossible >= 3) ? "IgnisManor_InviteAssassinToDuel_10" : SaveFile.Instance.IgnisEnergy >= 50 ? "IgnisManor_InviteAssassinToDuel_11" : "IgnisManor_InviteAssassinToDuel_Choices"},
        new ("IgnisManor_InviteAssassinToDuel_10") {IdOfNextDialogueLine = "IgnisManor_InviteAssassinToDuel_Choices"},
        new ("IgnisManor_InviteAssassinToDuel_11") {IdOfNextDialogueLine = "IgnisManor_InviteAssassinToDuel_Choices"},
        new ("IgnisManor_InviteAssassinToDuel_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_InviteAssassinToDuelBackstabbed_0"),
            new ("IgnisManor_InviteAssassinToDuelAvoided_0"),
            new ("IgnisManor_InviteAssassinToDuelWalkAway_0")
        }},
        new ("IgnisManor_InviteAssassinToDuelBackstabbed_10") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisManor_InviteAssassinToDuelBackstabbed_20") {Animation="FlameBackstab", Speaker = "IgnisAssassin", TurnSpeakersToFaceEachOther = false, WaitTimeBeforeAllowingToProceed=2},
        new ("IgnisManor_InviteAssassinToDuelBackstabbed_30") {Animation="Mocking", Speaker = "IgnisAssassin"},
        new ("IgnisManor_InviteAssassinToDuelBackstabbed_40") {Animation="Irritated", Speaker = "Player"},
        new ("IgnisManor_InviteAssassinToDuelBackstabbed_50") {Animation="ShoulderShrug", Speaker = "IgnisAssassin"},
        new ("IgnisManor_InviteAssassinToDuelBackstabbed_60") {Animation="ComeAtMe", Speaker = "IgnisAssassin", IdOfNextDialogueLine="END"},
        new ("IgnisManor_InviteAssassinToDuelAvoided_10") {Speaker = "Player"},
        new ("IgnisManor_InviteAssassinToDuelAvoided_20") {Animation="Irritated", Speaker = "IgnisAssassin"},
        new ("IgnisManor_InviteAssassinToDuelAvoided_30") {Animation="ComeAtMe", Speaker = "Player", IdOfNextDialogueLine="END"},
        new ("IgnisManor_InviteAssassinToDuelWalkAway_10"),
    }){PlayerStartingPosition = new Vector2(-28.5f, -15.5f), PlayerStartingFlipped = true};}

    public static bool CheckIfVisible_IgnisManor_InviteAssassinToDuelBackstabbed_0() {
        return SaveFile.Instance.IgnisEnergy < 50 && AssassinTrapImpossible < 3 && !SaveFile.Instance.HasFlag("DefeatedByIgnisAssassin");
    }

    public static bool CheckIfVisible_IgnisManor_InviteAssassinToDuelAvoided_0() {
        return SaveFile.Instance.IgnisEnergy >= 50 || AssassinTrapImpossible >= 3 || SaveFile.Instance.HasFlag("DefeatedByIgnisAssassin");
    }

    public static void IgnisManor_InviteAssassinToDuelBackstabbed_20() {
        Utils.GetUnit("IgnisAssassin").gameObject.SetActive(true);
        Utils.GetUnit("IgnisAssassin").Actions.IsFlipped = true;
        Utils.GetUnit("IgnisAssassin").PlayAnimation("FlameBackstab", 0, 0.33f);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Fire/FlameTeleport");
        Utils.CreateVisualEffect(new(Utils.GetUnit("IgnisAssassin")), "FlameBackstab", Utils.GetUnit("IgnisAssassin").transform.position.x, Utils.GetUnit("IgnisAssassin").transform.position.y - 0.5f);
        GameController.Instance.WaitAndRunMethod(1.8f, PlayPlayerWoundedAnimation);
        GameController.Instance.WaitAndRunMethod(1.0f, PlayPlayerAttackedAnimation);
    }

    public static void PlayPlayerAttackedAnimation() {
        Player.Instance.PlayAnimation("StealthAttacked", 0.1f, 0.1f);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Blade/Blade_BloodStab1");
    }

    public static void PlayPlayerWoundedAnimation() {
        Player.Instance.PlayAnimation("HeavilyWounded");
    }

    public static void OnEnd_IgnisManor_InviteAssassinToDuelBackstabbed_60() {
        Player.Instance.AddEffect(new Effect_Poison(2f, new(Utils.GetUnit("Unit_IgnisAssassin"))));
        Player.Instance.AddEffect(new Effect_Feeble(35, new(Utils.GetUnit("Unit_IgnisAssassin"))));
        StartDuel("IgnisAssassin");
    }

    public static void OnEnd_IgnisManor_InviteAssassinToDuelAvoided_10() {
        Player.Instance.Actions.IsFlipped = false;
        Utils.GetUnit("IgnisAssassin").gameObject.SetActive(true);
        Utils.GetUnit("IgnisAssassin").Actions.IsFlipped = true;
    }

    public static void OnEnd_IgnisManor_InviteAssassinToDuelAvoided_30() {
        StartDuel("IgnisAssassin");
    }

    public static Dialogue InviteToDuel_Pyromancer() {
        return new Dialogue ("Area_IgnisManor", "InviteToDuel_Pyromancer", new List<DialogueLine>{
        new ("IgnisManor_Duel_0"),
        new ("IgnisManor_Duel_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_Duel_1") {IdOfNextDialogueLine = "IgnisManor_Duel_40"},
            new ("Leave") {IdOfNextDialogueLine = "END"}
        }},
        new ("IgnisManor_Duel_40") {Animation="ShoulderShrug", Speaker = "IgnisPyromancer"}
    }){DialogueSpeaker=Utils.GetUnit("IgnisPyromancer"), PlayerStartingPosition = new Vector2(-15f, -15f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-13, -15), SpeakerStartingFlipped = true};}

    public static void OnEnd_IgnisManor_Duel_40() {
        StartDuel("IgnisPyromancer");
    }

    public static Dialogue InviteToDuel_Captain() {
        return new Dialogue ("Area_IgnisManor", "InviteToDuel_Captain", new List<DialogueLine>{
        new ("IgnisManor_Duel_0"),
        new ("IgnisManor_Duel_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManor_Duel_1") {IdOfNextDialogueLine = Area_IgnisManor.CheckIfWon3Duels() ? "IgnisManor_Duel_60" : "IgnisManor_Duel_50"},
            new ("Leave") {IdOfNextDialogueLine = "END"}
        }},
        new ("IgnisManor_Duel_50") {Animation="NotQuite", Speaker = "IgnisCaptain", IdOfNextDialogueLine="END"},
        new ("IgnisManor_Duel_60") {Animation="NotBad", Speaker = "IgnisCaptain"}
    }){DialogueSpeaker=Utils.GetUnit("IgnisCaptain"), PlayerStartingPosition = new Vector2(-22.5f, 36f), PlayerStartingFlipped = false, SpeakerStartingPosition = new Vector2(-20f, 36), SpeakerStartingFlipped = true};}

    public static void OnEnd_IgnisManor_Duel_60() {
        StartDuel("IgnisCaptain");
    }

    public static void StartDuel(string duelist_name) {
        GameController.Instance.WaitAndRunMethod(0.01f, SetPlayerStartDuelPosition);
        UIManager.Instance.HideBlackScreen(1);
        EventManager.UnitWouldBeDefeated.AddListener(FinishDuel);
        Player.Instance.AddEffect(new Effect_CannotBeDefeated(false, new(Player.Instance)));
        Area.Instance.transform.Find("NPCs").gameObject.SetActive(false);
        Area.Instance.transform.Find("Duel NPCs").gameObject.SetActive(true);
        Area.Instance.transform.Find("Duel NPCs/Unit_" + duelist_name).gameObject.SetActive(true);
        Area.Instance.transform.Find("Duel NPCs/Unit_" + duelist_name).transform.position = new Vector2(4, 0);
        Area.Instance.transform.Find("Duel NPCs/Unit_" + duelist_name).GetComponent<Unit>().AttackPlayer();
        Area.Instance.transform.Find("Duel NPCs/Unit_" + duelist_name).GetComponent<Unit>().AddEffect(new Effect_CannotBeDefeated(true, new(Area.Instance.transform.Find("Duel NPCs/Unit_" + duelist_name).GetComponent<Unit>())));
        foreach(Unit u in Area.Instance.transform.Find("Duel NPCs").GetComponentsInChildren<Unit>()) {
            u.PutAllWeaponsBehind();
            u.GetComponent<Animator>().speed = UnityEngine.Random.Range(0.7f, 1.3f);
        }
        if(duelist_name == "IgnisSwordmaster") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Swordmaster_0", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
        else if(duelist_name == "IgnisLancer") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Lancer_0", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Lancer_1", SpeakerUnit=Utils.GetUnit("IgnisLancer")}, 5);
        }
        else if(duelist_name == "IgnisCannonier") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Cannonier_0", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Cannonier_1", SpeakerUnit=Utils.GetUnit("IgnisCannonier")}, 5);
        }
        else if(duelist_name == "IgnisPyromancer") {
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").gameObject.SetActive(true);
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").transform.position = new Vector2(2, 0);
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").GetComponent<Unit>().AttackPlayer();
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").GetComponent<Unit>().AddEffect(new Effect_CannotBeDefeated(true, new(Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").GetComponent<Unit>())));
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").GetComponent<Unit>().EndEffect(typeof(Effect_CannotFight));
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisPyromancer").GetComponent<Unit>().EndEffect(typeof(Effect_CannotFight));
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Pyromancer_0", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
        else if(duelist_name == "IgnisAssassin") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Assassin_0", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Assassin_1", SpeakerUnit=Utils.GetUnit("IgnisAssassin")}, 5);
        }
        else if(duelist_name == "IgnisCaptain") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Captain_0", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
    }

    public static void SetPlayerStartDuelPosition() {
        Player.Instance.transform.position = new Vector2(-3f, 0);
        Player.Instance.Actions.IsFlipped = false;
    }

    public static bool CheckIfWon3Duels() {
        int counter = 0;
        if(SaveFile.Instance.HasFlag("IgnisManor_DefeatedIgnisSwordmaster")) {
            counter++;
        }
        if(SaveFile.Instance.HasFlag("IgnisManor_DefeatedIgnisLancer")) {
            counter++;
        }
        if(SaveFile.Instance.HasFlag("IgnisManor_DefeatedIgnisCannonier")) {
            counter++;
        }
        if(SaveFile.Instance.HasFlag("IgnisManor_DefeatedIgnisPyromancer")) {
            counter++;
        }
        if(SaveFile.Instance.HasFlag("IgnisManor_DefeatedIgnisAssassin")) {
            counter++;
        }
        return counter >= 3;
    }

    public static void FinishDuel(Damage damage) {
        EventManager.UnitWouldBeDefeated.RemoveListener(FinishDuel);
        if(damage.TargetOfDamage is Player) {
            PlayerLostDuel(damage);
            return;
        }
        if((damage.TargetOfDamage.gameObject.name == "Unit_IgnisPyromancer" && !Utils.GetUnit("IgnisBodyguard", true).CheckIfUnderEffect(typeof(Effect_CannotFight))) || (damage.TargetOfDamage.gameObject.name == "Unit_IgnisBodyguard" && !Utils.GetUnit("IgnisPyromancer", true).CheckIfUnderEffect(typeof(Effect_CannotFight)))) {
            damage.TargetOfDamage.SetToNeutralNPC();
            EventManager.UnitWouldBeDefeated.AddListener(FinishDuel);
            damage.TargetOfDamage.AddEffect(new Effect_CannotFight(new(damage.TargetOfDamage)));
        }
        else if(damage.TargetOfDamage.gameObject.name == "Unit_IgnisSwordmaster" || damage.TargetOfDamage.gameObject.name == "Unit_IgnisLancer" || damage.TargetOfDamage.gameObject.name == "Unit_IgnisCannonier" || damage.TargetOfDamage.gameObject.name == "Unit_IgnisAssassin" || damage.TargetOfDamage.gameObject.name == "Unit_IgnisCaptain" || (damage.TargetOfDamage.gameObject.name == "Unit_IgnisPyromancer" && Utils.GetUnit("IgnisBodyguard", true).CheckIfUnderEffect(typeof(Effect_CannotFight))) || (damage.TargetOfDamage.gameObject.name == "Unit_IgnisBodyguard" && Utils.GetUnit("Unit_IgnisPyromancer", true).CheckIfUnderEffect(typeof(Effect_CannotFight)))) {
            if(damage.TargetOfDamage.gameObject.name == "IgnisSwordmaster") {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Swordmaster_10", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            }
            else if(damage.TargetOfDamage.gameObject.name == "Unit_IgnisLancer") {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Lancer_10", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            }
            else if(damage.TargetOfDamage.gameObject.name == "Unit_IgnisCannonier") {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Cannonier_10", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            }
            else if(damage.TargetOfDamage.gameObject.name == "Unit_IgnisPyromancer" || damage.TargetOfDamage.gameObject.name == "Unit_IgnisBodyguard") {
                Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").GetComponent<Unit>().SetToNeutralNPC();
                Area.Instance.transform.Find("Duel NPCs/Unit_IgnisPyromancer").GetComponent<Unit>().SetToNeutralNPC();
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Pyromancer_10", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Pyromancer_11", SpeakerUnit=Utils.GetUnit("IgnisPyromancer")}, 5);
            }
            else if(damage.TargetOfDamage.gameObject.name == "Unit_IgnisAssassin") {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Assassin_10", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Assassin_11", SpeakerUnit=Utils.GetUnit("IgnisAssassin")}, 5);
            }
            else if(damage.TargetOfDamage.gameObject.name == "Unit_IgnisCaptain") {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Captain_10", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            }
            UIManager.Instance.ShowBlackScreen(1);
            GameController.Instance.WaitAndRunMethod(1, TransitionOutOfDuel, new string[] {damage.TargetOfDamage.gameObject.name, "W"});
            Player.Instance.InCombat = false;
        }
    }

    public static void PlayerLostDuel(Damage damage) {
        damage.SourceOfDamage.User.SetToNeutralNPC();
        if(damage.SourceOfDamage.User.gameObject.name == "IgnisSwordmaster") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Swordmaster_20", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
        else if(damage.SourceOfDamage.User.gameObject.name == "Unit_IgnisLancer") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Lancer_20", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
        else if(damage.SourceOfDamage.User.gameObject.name == "Unit_IgnisCannonier") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Cannonier_20", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
        else if(damage.SourceOfDamage.User.gameObject.name == "Unit_IgnisPyromancer" || damage.TargetOfDamage.gameObject.name == "Unit_IgnisBodyguard") {
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").GetComponent<Unit>().SetToNeutralNPC();
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisPyromancer").GetComponent<Unit>().SetToNeutralNPC();
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Pyromancer_20", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
        else if(damage.SourceOfDamage.User.gameObject.name == "Unit_IgnisAssassin") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Assassin_20", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Assassin_21", SpeakerUnit=Utils.GetUnit("IgnisAssassin")}, 5);
            SaveFile.Instance.AddFlag("DefeatedByIgnisAssassin");
        }
        else if(damage.SourceOfDamage.User.gameObject.name == "Unit_IgnisCaptain") {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManor_DuelQuips_Captain_20", SpeakerUnit=Utils.GetUnit("IgnisJudge")});
        }
        UIManager.Instance.ShowBlackScreen(1);
        damage.SourceOfDamage.User.ResetUnit();
        Player.Instance.ResetUnit();
        GameController.Instance.WaitAndRunMethod(1, TransitionOutOfDuel, new string[] {damage.SourceOfDamage.User.gameObject.name, "L"});
        Player.Instance.InCombat = false;
    }

    public static void TransitionOutOfDuel(string[] duel_info) {
        Player.Instance.transform.position = new Vector2(-3f, 0);
        UIManager.Instance.HideBlackScreen(1);
        Area.Instance.transform.Find("Duel NPCs/" + duel_info[0]).gameObject.SetActive(false);
        if(duel_info[0] == "Unit_IgnisPyromancer" || duel_info[0] == "Unit_IgnisBodyguard") {
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisBodyguard").gameObject.SetActive(false);
            Area.Instance.transform.Find("Duel NPCs/Unit_IgnisPyromancer").gameObject.SetActive(false);
        }
        Area.Instance.transform.Find("Duel NPCs").gameObject.SetActive(false);
        Area.Instance.transform.Find("NPCs").gameObject.SetActive(true);
        if(duel_info[1] == "W") {
            SaveFile.Instance.AddFlag("IgnisManor_Defeated" + duel_info[0].Replace("Unit_", "").Replace("IgnisBodyguard", "IgnisPyromancer"));
            SaveFile.Instance.ChangeIgnisEnergy(duel_info[0] == "Unit_IgnisCaptain" ? 20 : 10);
        }
        Utils.GetUnit("IgnisAssassin").gameObject.SetActive(false);
    }
}
