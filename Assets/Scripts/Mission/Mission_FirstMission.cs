using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mission_FirstMission : Mission
{
    public Mission_FirstMission() {
        Type = MissionType.MainQuest;
        WeeksUntilExpiryMax = 1;
        Icon = "UI/Chest01";
        MapMarker = "Warehouse_FirstMission";
        ExperienceReward = 3000;
        EnemyLevel = 1;
        RemoveOtherMissions = !Settings.Instance.SkipPrologue;
    }

    public override bool MissionShouldBeAvailable() {
        return SaveFile.Instance.Week == 1 && SaveFile.Instance.Cycle == 1;
    }

    public override void OnStart()
    {
        base.OnStart();
        EventManager.FinishedLoadingArea.AddListener(OnFinishedLoadingArea);
        Utils.MoveIntoArea(true, "FirstMission");
    }

    public override void OnFinishedLoadingArea() {
        NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstMission_Inter_0"});
        Player.Instance.PlayAnimation("HereWeGo");
        SaveFile.Instance.GetQuest("3LastChances").StartQuest(true);
        SaveFile.Instance.GetQuest("3LastChances").AdvanceObjective(10);
        //EscapedWithMap();
    }

    public static void SecretPassageEnter(InteractableObject obj) {
        Player.Instance.InCombat = false;
        UIManager.Instance.ShowBlackScreen(0.25f);
        GameController.Instance.WaitAndRunMethodRealtime(0.5f, ContinueSecretPassageEnter);
    }

    private static void ContinueSecretPassageEnter() {
        UIManager.Instance.HideBlackScreen(0.25f);
        Area.Instance.transform.Find("Secret Passage").gameObject.SetActive(false);
        Player.Instance.transform.position = new Vector2(-22, -13.5f);
    }

    public static void SecretPassageExit(InteractableObject obj) {
        Player.Instance.InCombat = false;
        UIManager.Instance.ShowBlackScreen(0.25f);
        GameController.Instance.WaitAndRunMethodRealtime(0.5f, ContinueSecretPassageExit);
    }

    public static void SecretPassageLeave(InteractableObject obj) {
        if(SaveFile.Instance.HasFlag("FirstMission_MetLeo")) {
            ((Mission_FirstMission)SaveFile.Instance.CurrentMission).EscapedWithMap();
        }
        else {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstMission_Inter_90"});
        }
    }

    public static void CollapsedTunnelInteract(InteractableObject obj) {
        Mission_FirstMission mission = (Mission_FirstMission)SaveFile.Instance.CurrentMission;
        if(SaveFile.Instance.HasFlag("FirstMission_FixedCollapsedTunnel") && SaveFile.Instance.HasFlag("FirstMission_MetLeo")) {
            ((Mission_FirstMission)SaveFile.Instance.CurrentMission).EscapedWithMap();
        }
        else if(SaveFile.Instance.HasFlag("FirstMission_FixedCollapsedTunnel") && SaveFile.Instance.HasFlag("FirstMission_FoundMap") == false && SaveFile.Instance.HasFlag("FirstMission_MetLeo") == false){
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstMission_Inter_90"});
        }
        else if(SaveFile.Instance.HasFlag("FirstMission_FixedCollapsedTunnel") && SaveFile.Instance.HasFlag("FirstMission_FoundMap")){
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstMission_Inter_160"});
        }
        else if(SaveFile.Instance.HasFlag("FirstMission_FixedCollapsedTunnel") == false && SaveFile.Instance.HasFlag("FirstMission_FoundMap")){
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstMission_Inter_161"});
        }
        else if(SaveFile.Instance.HasFlag("FirstMission_FixedCollapsedTunnel") == false && SaveFile.Instance.HasFlag("FirstMission_MetLeo")){
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstMission_Inter_140"});
        }
        else {
            UIManager.Instance.StartDialogue(CollapsedTunnelDialogue());
        }
    }

    private static void ContinueSecretPassageExit() {
        UIManager.Instance.HideBlackScreen(0.25f);
        Player.Instance.transform.position = new Vector2(-29.5f, -9.5f);
    }

    public void EscapedWithMap() {
        UIManager.Instance.ShowBlackScreen(0.5f);
        GameController.Instance.WaitAndRunMethod(0.5f, ContinueEscapedWithMap);
    }

    public void ContinueEscapedWithMap() {
        EventManager.FinishedLoadingArea.AddListener(StartClaireDialogue);
        Utils.MoveIntoArea(true, "FirstMission_Forest");
    }

    public void StartClaireDialogue() {
        UIManager.Instance.StartDialogue(ClarisePreBattle());
    }

    public static bool AllGuardsKnockedOut = false;

    public static Dialogue CollapsedTunnelDialogue() {
        return new Dialogue ("Mission_FirstMission", "CollapsedTunnelDialogue", new List<DialogueLine>{
        new ("FirstMission_Tunnel_0") {Speaker = "Player"},
        new ("FirstMission_Tunnel_10") {Speaker = "Player",
        Choices= new List<DialogueChoice> {
            new ("FirstMission_Tunnel1_0") {IdOfNextDialogueLine = "FirstMission_Tunnel1_10"}, 
            new ("FirstMission_Tunnel2_0") {IdOfNextDialogueLine = "FirstMission_Tunnel2_10"}, 
            new ("Leave") {IdOfNextDialogueLine = "END"}
        }},
        new ("FirstMission_Tunnel1_10") {Speaker = "Player", IdOfNextDialogueLine = "END" },
        new ("FirstMission_Tunnel2_10") {Speaker = "Player", IdOfNextDialogueLine = "END" }
    }) {AutoSaveOnDialogueEnd=false};}

    public static bool CheckIfVisible_FirstMission_Tunnel1_0() {
        int enemyCount = 0;
        Transform area = Area.Instance.transform;
        for(int i = 1; i <= 8; i++) {
            if(area.Find("NPCs/Guard " + i + " (Sleep)").GetComponent<Unit>().KnockedOut == false) {
                enemyCount++;
            }
        }
        return enemyCount != 0;
    }

    public static bool CheckIfEnabled_FirstMission_Tunnel2_0() {
        int enemyCount = 0;
        Transform area = Area.Instance.transform;
        for(int i = 1; i <= 8; i++) {
            if(area.Find("NPCs/Guard " + i + " (Sleep)").GetComponent<Unit>().KnockedOut == false) {
                enemyCount++;
            }
        }
        return enemyCount == 0;
    }

    public static void FirstMission_Tunnel1_0() {
        UIManager.Instance.HideBlackScreen(1);
        int enemyCount = 0;
        Transform area = Area.Instance.transform;
        for(int i = 1; i <= 8; i++) {
            if(area.Find("NPCs/Guard " + i + " (Sleep)").GetComponent<Unit>().KnockedOut == false) {
                area.Find("NPCs/Guard " + i + " (Sleep)").transform.position = area.Find("Guard Positions/Guard " + i).position;
                area.Find("NPCs/Guard " + i + " (Sleep)").GetComponent<Actions>().IsFlipped = false;
                area.Find("NPCs/Guard " + i + " (Sleep)").GetComponent<Unit>().GetEffect(typeof(Effect_Sleep))?.EndThisEffect();
                area.Find("NPCs/Guard " + i + " (Sleep)").GetComponent<Unit>().PlayAnimation("IdleInCombat", 0);
                enemyCount++;
            }
        }
        SaveFile.Instance.ExperiencePoints += 500;
        SaveFile.Instance.AddFlag("FirstMission_FixedCollapsedTunnel");
        Area.Instance.transform.Find("Environment/Rock Pillar").gameObject.SetActive(false);
    }

    public static void FirstMission_Tunnel2_0() {
        UIManager.Instance.HideBlackScreen(1);
        SaveFile.Instance.ExperiencePoints += 500;
        SaveFile.Instance.AddFlag("FirstMission_FixedCollapsedTunnel");
        Area.Instance.transform.Find("Environment/Rock Pillar").gameObject.SetActive(false);
    }

    public static Dialogue EncounterLeo() {
        return new Dialogue("Mission_FirstMission", "EncounterLeo", new List<DialogueLine>{
        new ("FirstMission_Leo_0") {Speaker = "Player", Animation = "Surprised"},
        new ("FirstMission_Leo_10") {Speaker = "Leo", ShowSpeakerBox = false},
        new ("FirstMission_Leo_20") {Speaker = "Leo", ShowSpeakerBox = false},
        new ("FirstMission_Leo_30") {Speaker = "Leo", Animation = "ShoulderShrug"},
        new ("FirstMission_Leo_40") {Speaker = "Leo", Animation = "YesYou"},
        new ("FirstMission_Leo_50") {Speaker = "Player", Animation = "SternNo"},
        new ("FirstMission_Leo_60") {Speaker = "Leo", Animation = "YesMe"},
        new ("FirstMission_Leo_70") {Speaker = "Player", Animation = "Mocking"},
        new ("FirstMission_Leo_80") {Speaker = "Leo", Animation = "SternNo"},
        new ("FirstMission_Leo_90") {Speaker = "Player", Animation = "ShoulderShrug"},
        new ("FirstMission_Leo_100") {Speaker = "Leo", Animation = "Sigh"},
        new ("FirstMission_Leo_110") {Speaker = "Player", Animation = "NotQuite"},
        new ("FirstMission_Leo_120") {Speaker = "Leo", Animation = "SternNo"},
        new ("FirstMission_Leo_130") {Speaker = "Leo", Animation = "ShoulderShrug",
            Choices= new List<DialogueChoice> {
            new DialogueChoice("FirstMission_Leo1_0") {}, 
            new DialogueChoice("FirstMission_Leo2_0") {Disabled=true, HideChoiceTextIfDisabled=false}, 
        }},
        new ("FirstMission_Leo1_10") {Speaker = "Leo", Animation="Disappear"},
        new ("FirstMission_Leo1_20") {Speaker = "Player", ShowSpeakerBox = false, Animation = "PreparingForBattle", IdOfNextDialogueLine="END"},
    }) {PlayerStartingFlipped = true, PlayerStartingPosition = new Vector2(-10f, 6.5f), AutoSaveOnDialogueEnd=false};
    }

    public static void OnStart_EncounterLeo() {
        Utils.SetDefaultMusic("Foreboding_67");
    }

    public static void OnEnd_EncounterLeo() {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Enemy")) {
            if(go.GetComponent<Unit>().CheckIfUnderEffect(typeof(Effect_Sleep))) {
                go.GetComponent<Unit>().EndEffect(typeof(Effect_Sleep));
            }
        }
        SaveFile.Instance.AddFlag("FirstMission_MetLeo");
        SaveFile.Instance.CurrentMission.AutoSaveAfterCombat = false;
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Mission_FirstMission.OnEnd_EncounterLeo2"});
        OnEnd_EncounterLeo2();
    }

    public static void OnEnd_EncounterLeo2() {
        Area.Instance.transform.Find("Leo Group").gameObject.SetActive(true);
        if(Area.Instance.transform.Find("Leo Group/Unit_Leo") != null) {
            Area.Instance.transform.Find("Leo Group/Unit_Leo").gameObject.SetActive(false);
        }
        Area.Instance.transform.Find("Leo Group/Wall 0").gameObject.SetActive(true);
        Area.Instance.transform.Find("Interactables/Dialogue (3)/Interact Indicator").gameObject.SetActive(true);
        Area.Instance.transform.Find("Interactables/Dialogue (5)").GetComponent<InteractableObject>().CannotInteractWithDuringCombat = false;
        Utils.GetUnit("Wall 1").PlayAnimation("ShieldGuard");
        Utils.GetUnit("Wall 1").AddEffect(new Effect_Invincible(new(Utils.GetUnit("Wall 1"))) {
            ShowsInUI = false
        });
        Utils.GetUnit("Wall 2").PlayAnimation("ShieldGuard");
        Utils.GetUnit("Wall 2").AddEffect(new Effect_Invincible(new (Utils.GetUnit("Wall 2"))) {
            ShowsInUI = false
        });
        Utils.GetUnit("ShieldGiant").AttackPlayer();
        Utils.GetUnit("ShieldGiant").Actions.UseAbility(typeof(NPCAbility_ShieldCharge));
        Utils.GetUnit("Criminal_Daggers").AttackPlayer();
        Utils.GetUnit("Criminal_Daggers2").AttackPlayer();
        Utils.GetUnit("Criminal_Grenadier1").AttackPlayer();
        Utils.GetUnit("Criminal_Grenadier2").AttackPlayer();
        UIManager.Instance.HideBlackScreen();
        GameController.Instance.WaitAndRunMethod(0.01f, PlayActionMusic);
        foreach(Unit u in Utils.GetAllUnits()) {
            if(u.CheckIfUnderEffect(typeof(Effect_Sleep))) {
                u.GetEffect(typeof(Effect_Sleep)).EndThisEffect();
            }
        }
    }

    public static void PlayActionMusic() {
        Utils.SetDefaultMusic("Action_57");
    }


    public static Dialogue ClarisePreBattle() {
        return new Dialogue("Mission_FirstMission", "ClarisePreBattle", new List<DialogueLine>{
            new ("FirstMission_Forest_0") {Speaker = "Player", Animation="StopRunning", ShowSpeakerBox = false},
            new ("FirstMission_Forest_10") {Speaker = "Clarise1", WaitTimeBeforeAllowingToProceed=5},
            new ("FirstMission_Forest_20") {Speaker = "Player", ShowSpeakerBox = false},
            new ("FirstMission_Forest_30") {Speaker = "Player", ShowSpeakerBox = false},
            new ("FirstMission_Forest_40") {Speaker = "Player", ShowSpeakerBox = false, WaitTimeBeforeAllowingToProceed=5},
            new ("FirstMission_Forest_50") {Speaker = "Clarise1", Animation = "Intrigued"},
            new ("FirstMission_Forest_60") {Speaker = "Player", Animation = "ThreatenWithLight"},
            new ("FirstMission_Forest_70") {Speaker = "Clarise1", Animation = "Laugh"},
            new ("FirstMission_Forest_80") {Speaker = "Player", Animation = "IdleInCombat"}
        }) {PlayerStartingPosition = new Vector2(-10f, 0.6f), SpeakerStartingPosition = new Vector2(8.75f, 6.2f), PlayerEndingFlipped = false, PlayerEndingPosition = new Vector2(-0.5f, 0.5f), DialogueSpeaker=Utils.GetUnit("Clarise1"), SpeakerEndingFlipped=true, SpeakerEndingPosition=new Vector2(3, 0.5f), AutoSaveOnDialogueEnd=false};
    }

    public static void FirstMission_Forest_0() {
        CameraController.Instance.transform.position = new Vector3(0, 0, -100);
        Player.Instance.UnitAI.NavMeshAgent.enabled = false;
        Player.Instance.Actions.PushUnitForwardSpecifiedMeters(3);
        Utils.SetDefaultMusic("Action_72");
    }

    public static void FirstMission_Forest_10() {
        Utils.GetUnit("Clarise1").Actions.IsFlipped = true;
        Player.Instance.PlayAnimation("Assassinated");
        Utils.GetUnit("Clarise1").CurrentTarget = Player.Instance;
        Utils.GetUnit("Clarise1").Actions.UseAbility(typeof(NPCAbility_ClariseAssassination));
        GameController.Instance.WaitAndRunMethod(3.05f, FirstMission_Forest_10_2);
    }

    public static void FirstMission_Forest_10_2() {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Hit/Blade_Hit7");
    }

    public static void FirstMission_Forest_20() {
        UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }

    public static void FirstMission_Forest_30() {
        Player.Instance.PlayAnimation("IdleInCombat", 0);
        Utils.GetUnit("Clarise1").Actions.EndCurrentAbility();
        Utils.GetUnit("Clarise1").Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
        Utils.GetUnit("Clarise1").transform.position = new Vector2(8.75f, 6.2f);
        UIManager.Instance.HideBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }

    public static void FirstMission_Forest_40() {
        GameController.Instance.WaitAndRunMethod(3, FirstMission_Forest_40_2);
        Utils.GetUnit("Clarise1").RemoveAllCooldowns();
        Utils.GetUnit("Clarise1").CurrentTarget = Player.Instance;
        Utils.GetUnit("Clarise1").Actions.UseAbility(typeof(NPCAbility_ClariseAssassination));
    }

    public static void FirstMission_Forest_40_2() {
        Player.Instance.Actions.UseAbility(typeof(Ability_BackStep));
    }

    public static void OnEnd_ClarisePreBattle() {
        GameController.Instance.StopAllCoroutines();
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Mission_FirstMission.OnClariseFightStart", "Mission_FirstMission.LostToClarise"});
        OnClariseFightStart();
    }

    public static void OnClariseFightStart() {
        Utils.SetDefaultMusic("Action_72");
        Utils.GetUnit("Clarise1").AttackPlayer();
        Utils.GetUnit("Clarise1").AddEffect(new Effect_CannotBeDefeated(true, new(Utils.GetUnit("Clarise1"))));
        Utils.GetUnit("Clarise1").UnitAI.NavMeshAgent.enabled = true;
        EventManager.UnitWouldBeDefeated.AddListener(((Mission_FirstMission)SaveFile.Instance.CurrentMission).OnClariseDefeated);
    }

    public static void LostToClarise() {
        SaveFile.Instance.AddFlag("FirstMission_LostToClariseAtLeastOnce");
    }

    public void OnClariseDefeated(Damage damage) {
        if(damage.TargetOfDamage.gameObject.name.Contains("Clarise1")) {
            UIManager.Instance.ShowBlackScreen(1);
            GameController.Instance.WaitAndRunMethod(1, StartClariseDialogue);
        }
    }

    public void StartClariseDialogue() {
        UIManager.Instance.StartDialogue(ClarisePostBattle());
    }

    public static Dialogue ClarisePostBattle() {
        return new Dialogue("Mission_FirstMission", "ClarisePostBattle", new List<DialogueLine>{
            new (SaveFile.Instance.HasFlag("FirstMission_LostToClariseAtLeastOnce") ? "FirstMission_Forest2_0" : "FirstMission_Forest2_1") {Speaker = "Clarise1", Animation="NotQuite"},
            new ("FirstMission_Forest2_10") {Speaker = "Player", Animation="HeavilyWounded"},
            new ("FirstMission_Forest2_20") {Speaker = "Clarise1", Animation="ShoulderShrug"},
            new ("FirstMission_Forest2_30") {Speaker = "Player", Animation="Accusing"},
            new ("FirstMission_Forest2_40") {Speaker = "Clarise1", Animation="NotQuite"},
            new ("FirstMission_Forest2_50") {Speaker = "Player", Animation = "Intrigued"},
            new ("FirstMission_Forest2_60") {Speaker = "Clarise1", Animation = "Explaining"},
            new ("FirstMission_Forest2_70") {Speaker = "Player"},
            new ("FirstMission_Forest2_80") {Speaker = "Clarise1", Animation = "ShoulderShrug"},
            new ("FirstMission_Forest2_90") {Speaker = "Player", Animation = "Irritated"},
            new ("FirstMission_Forest2_100") {Speaker = "Clarise1", Animation = "NotQuite"},
            new ("FirstMission_Forest2_110") {Speaker = "Player", Animation = "Thinking"},
            new ("FirstMission_Forest2_120") {Speaker = "Clarise1", Animation = "Thinking"},
            new ("FirstMission_Forest2_130") {Speaker = "Player", Animation = "Sigh"},
            new ("FirstMission_Forest2_135") {Speaker = "Player", Animation = "Thinking"},
            new ("FirstMission_Forest2_140") {Animation = "HereWeGo", Speaker = "Player"}
        }) {PlayerStartingFlipped = false, PlayerStartingPosition = new Vector2(-0.5f, 0.5f), DialogueSpeaker=Utils.GetUnit("Clarise1"), SpeakerStartingFlipped=true, SpeakerStartingPosition=new Vector2(3, 0.5f)};
    }

    public static void FirstMission_Forest2_0() {
        UIManager.Instance.HideBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
        Player.Instance.transform.position = new Vector2(-6, 0.6f);
        Player.Instance.Actions.IsFlipped = false;
        Utils.GetUnit("Clarise1").UnitAI.enabled = false;
        Utils.GetUnit("Clarise1").PlayAnimation("Exhausted", 0);
        Utils.GetUnit("Clarise1").transform.position = new Vector2(-3f, 0.6f);
        Utils.GetUnit("Clarise1").Actions.IsFlipped = true;
    }

    public static void FirstMission_Forest2_110() {
        Utils.GetUnit("Clarise1").DisplayedName = "Clarise";
    }

    public static void FirstMission_Forest2_130() {
        Utils.GetUnit("Clarise1").PlayAnimation("Disappear", 0);
    }

    public static void FirstMission_Forest2_140() {
        UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }

    public static void OnEnd_ClarisePostBattle() {
        SaveFile.Instance.GetQuest("3LastChances").GetObjective(20).Status = QuestObjective.ObjectiveStatus.NotRevealed;
        SaveFile.Instance.GetQuest("3LastChances").GetObjective(30).Status = QuestObjective.ObjectiveStatus.Current;
        SaveFile.Instance.CurrentMission.Finish(true);
    }

    public static void PickUpMap(InteractableObject obj) {
        Area.Instance.transform.Find("Leo Group").gameObject.SetActive(true);
        Area.Instance.transform.Find("Interactables/Leo Dialogue Trigger").gameObject.SetActive(true);
        SaveFile.Instance.AddItem(new Polearm_Shattershield(Item.ItemGrade.Regular));
        Utils.GetUnit("Leader").SpriteRenderers["Heavy"].SpriteRenderer.gameObject.SetActive(false);
        SaveFile.Instance.AddFlag("FirstMission_FoundMap");
        SaveFile.Instance.GetQuest("3LastChances").GetObjective(10).Status = QuestObjective.ObjectiveStatus.NotRevealed;
        SaveFile.Instance.GetQuest("3LastChances").GetObjective(20).ShowAsMissionObjective();
    }
}
