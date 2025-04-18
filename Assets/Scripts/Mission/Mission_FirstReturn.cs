using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System;
using Unity.VisualScripting;

public class Mission_FirstReturn : Mission {

    public Mission_FirstReturn() {
        Type = MissionType.MainQuest;
        NumberOfWeeksConsumed = 0;
    }

    private int _counter = 0;

    public static Dialogue FirstDialogue() {
        return new Dialogue ("Mission_FirstReturn", "StartDialogue", new List<DialogueLine>{
            new DialogueLine("FirstReturn_FirstShadow_0") {AudioClip = "Dialogue/Water Bubbles 3"},
            new DialogueLine("FirstReturn_FirstShadow_10") {AudioClip = "Hit/Metal_Destroy3"},
            new DialogueLine("FirstReturn_FirstShadow_20") {Speaker = "Player", Animation = "WobblyGetUp"},
            new DialogueLine("FirstReturn_FirstShadow_30") {Speaker = "Shadow", AudioClip = "Criminal/Criminal_Daggers_MoveAbility"},
            new DialogueLine("FirstReturn_FirstShadow_40") {Animation = "Gun_StanceSwitch"},
            new DialogueLine("FirstReturn_FirstShadow_50") {Speaker = "Player", Animation = "ThreatenWithHeavy"},
            new DialogueLine("FirstReturn_FirstShadow_60") {Speaker = "Shadow",  Animation = "ShoulderShrug"},
            new DialogueLine("FirstReturn_FirstShadow_70") {Speaker = "Player", Animation = "Irritated"},
            new DialogueLine("FirstReturn_FirstShadow_80") {Speaker = "Shadow", Animation = "Mocking"},
            new DialogueLine("FirstReturn_FirstShadow_90") {Speaker = "Shadow" },
            new DialogueLine("FirstReturn_FirstShadow_100") {Speaker = "Player", Animation = "Thinking"},
            new DialogueLine("FirstReturn_FirstShadow_110") {Speaker = "Player", Animation = "Realization"},
            new DialogueLine("FirstReturn_FirstShadow_120") {Speaker = "Shadow", Animation = "IWouldHurryUpIfIWereYou"},
            new DialogueLine("FirstReturn_FirstShadow_130"),
            new DialogueLine("FirstReturn_FirstShadow_140") {Speaker = "Player"},      
            new DialogueLine("FirstReturn_FirstShadow_150") {Speaker = "Shadow", Animation = "WaveGoodbyeAloof"}                                                                                                 
        });
    }

    public override bool MissionShouldBeAvailable()
    {
        return false;
    }

    public static void FirstReturn_FirstShadow_0() {
        UIManager.Instance.ShowBlackScreen(0.01f);
        Utils.SetDefaultMusic("Mystery_60"); 
        Player.Instance.PlayAnimation("Stun"); 
    }

    public static void FirstReturn_FirstShadow_10() {
        UIManager.Instance.HideBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
        Area.Instance.transform.Find("MetalDestructibleLasting").gameObject.SetActive(true);
        GameController.Instance.WaitAndRunMethod(1, ((Mission_FirstReturn)SaveFile.Instance.CurrentMission).PauseMetalDestructible);
    }

    public static void FirstReturn_FirstShadow_30() {
        Player.Instance.PlayAnimation("Surprised"); 
        Utils.GetUnit("Shadow").gameObject.SetActive(true);
    }

    public static void OnEnd_StartDialogue() {
        SaveFile.Instance.UnlockAbility(typeof(Ability_WindRush));
        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 4; j++) {
                SaveFile.Instance.Stances[i].Abilities[j].Type = j == 0 ? typeof(Ability_WindRush) : null;
            }
        }
        Player.Instance.CurrentStance = Player.Instance.CurrentStance;
        NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstReturn_Inter_10"}); 
        Utils.CreateVisualEffect(new(Utils.GetUnit("Shadow")), "EntityFlamesBurst", -3.35f, -2.7f);
        Utils.GetUnit("Shadow").gameObject.SetActive(false);
        Area.Instance.transform.Find("DroppedItem/Interact Indicator").gameObject.SetActive(true);
    }

    public static Dialogue RykerFirstDialogue() {
        return new Dialogue ("Mission_FirstReturn", "RykerFirstDialogue", new List<DialogueLine>{
        new DialogueLine("FirstReturn_FirstRyker_0") {Speaker = "TutorialRyker", Animation = "Approving"},  
        new DialogueLine("FirstReturn_FirstRyker_10") {Speaker = "TutorialRyker", Animation = "Surprised", ShowSpeakerBox = false},
        new DialogueLine("FirstReturn_FirstRyker_20") {Speaker = "TutorialRyker"},
        new DialogueLine("FirstReturn_FirstRyker_30"), 
        new DialogueLine("FirstReturn_FirstRyker_40") {Speaker = "Player", Animation = "FinallyDecided", ShowSpeakerBox = false},
        new DialogueLine("FirstReturn_FirstRyker_50") {Speaker = "Player", 
        Choices = new List<DialogueChoice>{
            new DialogueChoice("FirstReturn_FirstRyker1_0"), 
            new DialogueChoice("FirstReturn_FirstRyker2_0")}}, 
        new DialogueLine("FirstReturn_FirstRyker1_10") {Speaker = "Player"},
        new DialogueLine("FirstReturn_FirstRyker1_20") {Speaker = "TutorialRyker", Animation = "Flabbergasted"},
        new DialogueLine("FirstReturn_FirstRyker1_30") {Speaker = "Player", Animation = "Determined"},
        new DialogueLine("FirstReturn_FirstRyker1_40") {Speaker = "TutorialRyker", Animation="Encourage", IdOfNextDialogueLine = "FirstReturn_FirstRyker_60"}, 
        new DialogueLine("FirstReturn_FirstRyker2_10") {Speaker = "Player"},
        new DialogueLine("FirstReturn_FirstRyker2_20") {Speaker = "TutorialRyker", Animation = "Doubtful"},
        new DialogueLine("FirstReturn_FirstRyker2_30") {Speaker = "TutorialRyker", IdOfNextDialogueLine = "FirstReturn_FirstRyker_60"}, 
        new DialogueLine("FirstReturn_FirstRyker_60") {Speaker = "Player", Animation = "GiveMeABreak"},
        new DialogueLine("FirstReturn_FirstRyker_70") {Speaker = "TutorialRyker", Animation = "ComeAtMe"},
        new DialogueLine("FirstReturn_FirstRyker_80") {Speaker = "Player",
        Choices= new List<DialogueChoice> {
            new DialogueChoice("FirstReturn_FirstRyker3_0") {IdOfNextDialogueLine="END"}, 
            new DialogueChoice("FirstReturn_FirstRyker4_0"), 
        }}}) {PlayerStartingFlipped = false, PlayerStartingPosition = new Vector2(-3, -1),
        DialogueSpeaker=Utils.GetUnit("TutorialRyker"), SpeakerStartingFlipped=true, SpeakerStartingPosition = new Vector2(0, -1), ReturnUnitsToOriginalPositions=false};
    }

    public static void FirstReturn_FirstRyker3_0() {
        Utils.SetDefaultMusic("Action_25");
        SaveFile.Instance.CurrentMission.MissionProgress += 10;
        EventManager.AbilityUsed.AddListener(((Mission_FirstReturn)SaveFile.Instance.CurrentMission).AbilityUsed);
        EventManager.PlayerTargetChanged.AddListener(((Mission_FirstReturn)SaveFile.Instance.CurrentMission).PlayerTargetChanged);
        Utils.GetUnit("TutorialRyker").AddEffect(new Effect_ChangeStat(Utils.GetUnit("TutorialRyker").LightStagger, new(Utils.GetUnit("TutorialRyker"))) {PercentageModifier = -50, IsRemovable = false});
        Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions(new() {"20,AI_Chase"});
        Utils.GetUnit("TutorialRyker").AttackPlayer();
    }

    public static void ContinueTutorial(InteractableObject inter) {
        ((Mission_FirstReturn)SaveFile.Instance.CurrentMission).MissionProgress += 5;
        Area.Instance.transform.Find("Environment/TutorialPrompt").gameObject.SetActive(false);
        Utils.GetUnit("TutorialRyker").AttackPlayer();
    }

    public static void FirstReturn_FirstRyker4_0() {
        GameController.Instance.WaitAndRunMethod(0.01f, ((Mission_FirstReturn)SaveFile.Instance.CurrentMission).SkipTutorial);
    }

    public void AbilityUsed(Ability ability) {
        if(MissionProgress == 30 && ability.Is(Ability.AbilityProperty.BasicAttack)) {
            _counter++;
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {_counter.ToString()});
            if(_counter >= 3) {
                MissionProgress += 10;
            }
        }
        else if(MissionProgress == 40 && ability.Is(Ability.AbilityProperty.BasicAttack)) {
            BasicAttack ba = (BasicAttack)ability;
            if(ba.IsNot(Ability.AbilityProperty.StrongBasicAttack)) {
                return;
            }
            _counter++;
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {_counter.ToString()});
            if(_counter >= 3) {
                MissionProgress += 5;
                Utils.GetUnit("TutorialRyker").SetToNeutralNPC();
                Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions(new() {{"20,UpperCut"},{"20,FastJabs"},{"20,AI_Chase"}});
                Player.Instance.InCombat = false;
                Area.Instance.transform.Find("Environment/TutorialPrompt").gameObject.SetActive(true);
                Player.Instance.SetInteractPromptToClosestInteractable();
            }
        }
        else if(MissionProgress == 50 && ability.GetType().IsSubclassOf(typeof(Ability_Dodge))) {
            _counter++;
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {_counter.ToString()});
            if(_counter >= 2) {
                MissionProgress += 5;
                Utils.GetUnit("TutorialRyker").SetToNeutralNPC();
                Player.Instance.InCombat = false;
                Area.Instance.transform.Find("Environment/TutorialPrompt").gameObject.SetActive(true);
                Player.Instance.SetInteractPromptToClosestInteractable();
            }
        }
        else if(MissionProgress == 60 && ability is Ability_Block) {
            _counter++;
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {_counter.ToString()});
            if(_counter >= 2) {
                MissionProgress += 5;
                Utils.GetUnit("TutorialRyker").SetToNeutralNPC();
                Player.Instance.InCombat = false;
                Area.Instance.transform.Find("Environment/TutorialPrompt").gameObject.SetActive(true);
                Player.Instance.SetInteractPromptToClosestInteractable();
            }
        }
        else if(MissionProgress == 70 && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))) {
            _counter++;
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {_counter.ToString()});
            if(_counter >= 2) {
                MissionProgress += 10;
                Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions(new() {{"20,AI_Chase"}});
            }
        }
        else if(MissionProgress == 80 && ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))) {
            _counter++;
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {_counter.ToString()});
            if(_counter >= 2) {
                MissionProgress += 10;
            }
        }
        else if(MissionProgress == 100 && ability is Ability_WindRush) {
            _counter++;
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {_counter.ToString()});
            if(_counter >= 2) {
                MissionProgress += 5;
                Utils.GetUnit("TutorialRyker").SetToNeutralNPC();
                Player.Instance.InCombat = false;
                Area.Instance.transform.Find("Environment/TutorialPrompt").gameObject.SetActive(true);
                Player.Instance.SetInteractPromptToClosestInteractable();
                Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions(new() {{"20,DelayedJumpSlam"},{"20,AI_Chase"}});
            }
        }
        else if(MissionProgress == 110 && ability.GetType().ToString().Contains("RiposteCounter")) {
            MissionProgress += 5;
            Utils.GetUnit("TutorialRyker").SetToNeutralNPC();
            Player.Instance.InCombat = false;
            Area.Instance.transform.Find("Environment/TutorialPrompt").gameObject.SetActive(true);
            Player.Instance.SetInteractPromptToClosestInteractable();
            Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions(new() {{"20,HeelCleaver"},{"20,AI_Chase"}});
            Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions();
        }
        else if(MissionProgress == 120 && ability.GetType().ToString().Contains("BackstepCounter")) {
            MissionProgress += 5;
            Utils.GetUnit("TutorialRyker").SetToNeutralNPC();
            Player.Instance.InCombat = false;
            Area.Instance.transform.Find("Environment/TutorialPrompt").gameObject.SetActive(true);
            Player.Instance.SetInteractPromptToClosestInteractable();
            Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions(new() {{"20,ChargedPunch"},{"20,AI_Chase"}});
            Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions();
        }
        else if(MissionProgress == 130 && ability.GetType().ToString().Contains("RollCounter")) {
            MissionProgress += 10;
            if(Player.Instance.Health.Current == Player.Instance.Health.Maximum) {
                Player.Instance.Health.Current -= 1;
            }
            Utils.GetUnit("TutorialRyker").SetToNeutralNPC();
            Utils.GetUnit("TutorialRyker").UnitAI.enabled = false;
            SaveFile.Instance.HealChargesRemaining = SaveFile.Instance.HealChargesRemaining == 0 ? 1 : SaveFile.Instance.HealChargesRemaining;
        }
        else if(MissionProgress == 140 && (ability is Ability_Heal || ability is Ability_ThrowItem || ability is Ability_ExpertBadge)) {
            Player.Instance.InCombat = false;
            MissionProgress += 10;
            Utils.GetUnit("TutorialRyker").transform.Find("2").gameObject.SetActive(true);
        }
    }

    public void PlayerTargetChanged() {
        if(MissionProgress == 90) {
            _counter++;
            if(_counter >= 2) {
                MissionProgress += 10;
            }
        }
    }

    public void SkipTutorial() {
        MissionProgress = 150;
        UIManager.Instance.StartDialogue(RykerDialogueBeforeBossFight());
    }

    public static Dialogue RykerDialogueBeforeBossFight() {
        return new Dialogue ("Mission_FirstReturn", "RykerDialogueBeforeBossFight", new List<DialogueLine>{
            new DialogueLine("FirstReturn_SecondRyker_0") {Speaker = "TutorialRyker", Animation = "NotBad"}, 
            new DialogueLine("FirstReturn_SecondRyker_10") {Speaker = "Player", Animation = "Frustrated"}, 
            new DialogueLine("FirstReturn_SecondRyker_20") {Speaker = "TutorialRyker", Animation = "ChargeRykerRockCage", WaitTimeBeforeAllowingToProceed=0.5f},
            new DialogueLine("FirstReturn_SecondRyker_30") {Speaker = "TutorialRyker", Animation = "RykerRockCage", WaitTimeBeforeAllowingToProceed=1}, 
            new DialogueLine("FirstReturn_SecondRyker_40") {Speaker = "Player", Animation = "PreparingForBattle"}, 
        }) {PlayerStartingFlipped = false, PlayerStartingPosition = new Vector2(-3, -1),
        DialogueSpeaker=Utils.GetUnit("TutorialRyker"), SpeakerStartingFlipped=true, SpeakerStartingPosition = new Vector2(0, -1), ReturnUnitsToOriginalPositions=false};
    }

    public static void FirstReturn_SecondRyker_30() {
        GameController.Instance.WaitAndRunMethod(0.25f, ((Mission_FirstReturn)SaveFile.Instance.CurrentMission).ChangeAreaLook);
    }

    public static void OnEnd_RykerDialogueBeforeBossFight() {
        Utils.GetUnit("TutorialRyker").EndEffect(typeof(Effect_ChangeStat));
        Utils.GetUnit("TutorialRyker").EndEffect(typeof(Effect_Unkillable));
        Utils.SetDefaultMusic("Action_40");
        Unit ryker = Utils.GetUnit("TutorialRyker");
        ryker.UnitAI.enabled = true;
        ryker.PlayAnimation("PreparingForBattle");
        ryker.Health.Maximum = 7500;
        ryker.Health.Current = ryker.Health.Maximum;
        ryker.CooldownReduction.Maximum = 1;
        ryker.CooldownReduction.Current = 1;
        ryker.Tenacity.Maximum = 1;
        Utils.GetUnit("TutorialRyker").UnitAI.InitializeAvailableActions(new() {{"20,UpperCut"},{"20,FastJabs"},{"20,AI_Chase"},{"20,HeelCleaver"},{"20,DelayedJumpSlam"},{"20,ChargedPunch"}});
        ryker.AttackPlayer();
        Utils.GetUnit("TutorialRyker").AddEffect(new Effect_CannotBeDefeated(false, new(Utils.GetUnit("TutorialRyker"))) {IsRemovable = false});
        EventManager.UnitWouldBeDefeated.AddListener(((Mission_FirstReturn)SaveFile.Instance.CurrentMission).RykerDefeated);
    }


    public void RykerDefeated(Damage damage) {
        Player.Instance.InCombat = false;
        Utils.SetDefaultMusic("Foreboding_35");
        Utils.GetUnit("TutorialRyker").transform.Find("3").gameObject.SetActive(true);
        Utils.GetUnit("TutorialRyker").ChangeFaction(Constants.Faction.Neutral);
        MissionProgress += 10;
        SaveFile.Instance.ExperiencePoints += 1000;
    }

    public void ChangeAreaLook() {
        GameObject.FindGameObjectWithTag("Area").transform.Find("Variant A").gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Area").transform.Find("Variant B").gameObject.SetActive(true);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Explosion/Explosion2", 1.2f);
        CameraController.Instance.CenteredOnObject = null;
        CameraController.Instance.ShakeScreen(2.5f, 0.4f, 5);
    }

    public static Dialogue RykerFinalDialogue() {
        return new Dialogue ("Mission_FirstReturn", "RykerFinalDialogue", new List<DialogueLine>{
            new DialogueLine("FirstReturn_FinalRyker_0") {Speaker = "TutorialRyker", Animation = "Exhausted"}, 
            new DialogueLine("FirstReturn_FinalRyker_10") {Speaker = "Player"}, 
            new DialogueLine("FirstReturn_FinalRyker_20") {Speaker = "TutorialRyker", Animation = "Approving"},   
            new DialogueLine("FirstReturn_FinalRyker_30") {Speaker = "Player", Animation="Determined"}, 
            new DialogueLine("FirstReturn_FinalRyker_40") {Speaker = "TutorialRyker", WaitTimeBeforeAllowingToProceed=3}, 
            new DialogueLine("FirstReturn_FinalRyker_50") {Speaker = "Player", Animation = "Bashful"}, 
        }) {PlayerStartingFlipped = false, PlayerStartingPosition = new Vector2(-3, -1),
        DialogueSpeaker=Utils.GetUnit("TutorialRyker"), SpeakerStartingFlipped=true, SpeakerStartingPosition = new Vector2(0, -1), ReturnUnitsToOriginalPositions=false};
    }

    public static void FirstReturn_FinalRyker_30() {
        Utils.GetUnit("TutorialRyker").PlayAnimation("Encourage");
    }

    public static void FirstReturn_FinalRyker_40() {
        Utils.GetUnit("TutorialRyker").gameObject.SetActive(false);
        Utils.GetUnit("TutorialRyker2").gameObject.SetActive(true);
        Unit ryker = Utils.GetUnit("TutorialRyker2");
        ryker.Awake();
        ryker.GetComponent<Actions>().Start();
        ryker.CanRun = false;
        ryker.Actions.MoveToPoint(11, 0);
        ryker.Actions.IsFlipped = false;
        CameraController.Instance.CenteredOnObject = ryker.gameObject;
        UIManager.Instance.ShowBlackScreen(2);
        GameController.Instance.WaitAndRunMethod(3, ((Mission_FirstReturn)SaveFile.Instance.CurrentMission).HideRyker);
    }

    public static void OnEnd_RykerFinalDialogue() {
        SaveFile.Instance.CurrentMission.MissionProgress += 10;
    }

    public void HideRyker() {
        UIManager.Instance.HideBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
        foreach(DestructibleEnvironment de in Area.Instance.transform.Find("Variant B/Blockade").GetComponentsInChildren<DestructibleEnvironment>()) {
            de.DestroyObject();
        }
        Utils.GetUnit("TutorialRyker2").gameObject.SetActive(false);
        Utils.GetUnit("Shadow").gameObject.SetActive(true);
    }

    public static Dialogue ShadowFinalDialogue() {
        return new Dialogue ("Mission_FirstReturn", "ShadowFinalDialogue", new List<DialogueLine>{
            new DialogueLine("FirstReturn_FinalShadow_0") {Speaker = "Shadow", Animation = "Mocking"}, 
            new DialogueLine("FirstReturn_FinalShadow_10") {Speaker = "Player", Animation = "SternNo"}, 
            new DialogueLine("FirstReturn_FinalShadow_20") {Speaker = "Shadow", Animation = "ShoulderShrug"},   
            new DialogueLine("FirstReturn_FinalShadow_30") {Speaker = "Player"},   
            new DialogueLine("FirstReturn_FinalShadow_40") {Speaker = "Shadow"},   
            new DialogueLine("FirstReturn_FinalShadow_50") {Speaker = "Player",  Animation = "Doubtful"},   
            new DialogueLine("FirstReturn_FinalShadow_60") {Speaker = "Shadow",  Animation = "YouFinallyGetIt"},   
            new DialogueLine("FirstReturn_FinalShadow_70") {Speaker = "Player",  Animation = "Doubtful"},   
            new DialogueLine("FirstReturn_FinalShadow_80") {Speaker = "Shadow",  Animation = "Irritated"},   
            new DialogueLine("FirstReturn_FinalShadow_90") {Speaker = "Shadow",  Animation = "FinallyDecided", AudioClip="Dialogue/TrueSpeech"},   
            new DialogueLine("FirstReturn_FinalShadow_100") {Speaker = "Player",  Animation = "Surprised"},   
            new DialogueLine("FirstReturn_FinalShadow_110") {Speaker = "Shadow", Animation="ShoulderShrug"},   
            new DialogueLine("FirstReturn_FinalShadow_120") {Speaker = "Shadow"},   
            new DialogueLine("FirstReturn_FinalShadow_130") {Speaker = "Player",  Animation = "Thinking"},   
            new DialogueLine("FirstReturn_FinalShadow_140") {Speaker = "Player",  Animation = "Thinking"},   
            new DialogueLine("FirstReturn_FinalShadow_150") {Speaker = "Player",  Animation = "Realization"},   
            new DialogueLine("FirstReturn_FinalShadow_160") {Speaker = "Shadow",  Animation = "NotBad"},   
            new DialogueLine("FirstReturn_FinalShadow_170") {Speaker = "Player",  Animation = "Deflated"},   
            new DialogueLine("FirstReturn_FinalShadow_180") {Speaker = "Shadow",  Animation = "WaveGoodbyeAloof"},   
            new DialogueLine("FirstReturn_FinalShadow_190") {Speaker = "Player",  Animation = "TakeOutPhone"},  
            new DialogueLine("FirstReturn_FinalShadow_200") {Speaker = "Player",  Animation = "CallSomeoneOnPhone"}
        }) {PlayerStartingFlipped = false, PlayerStartingPosition = new Vector2(10.5f, 1),
        DialogueSpeaker=Utils.GetUnit("Shadow"), SpeakerStartingFlipped=true, SpeakerStartingPosition = new Vector2(13, 1)};
    }

    public static void FirstReturn_FinalShadow_190() {
        GameController.Instance.WaitAndRunMethod(1, ((Mission_FirstReturn)SaveFile.Instance.CurrentMission).MakeShadowDisappear);
    }

    public static void FirstReturn_FinalShadow_200() {
        UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
    }

    public static void OnEnd_ShadowFinalDialogue() {
        SaveFile.Instance.FinishedPrologue = true;
        SaveFile.Instance.CurrentMission.Finish(true);
    }

    public void MakeShadowDisappear() {
        Utils.CreateVisualEffect(new(Utils.GetUnit("Shadow")), "EntityFlamesBurst", Utils.GetUnit("Shadow").transform.position.x, (Utils.GetUnit("Shadow").transform.position.y - 0.5f));
        Utils.GetUnit("Shadow").gameObject.SetActive(false);
    }

    public static void BadgePickedUp(InteractableObject obj) {
        SaveFile.Instance.AcquireItem("Permanent_ExpertBadge", 1, Item.ItemGrade.Regular);
        SaveFile.Instance.CurrentMission.MissionProgress += 10;
        EventManager.FinishedLoadingArea.AddListener(Mission_FirstReturn.MovedIntoArea);
        GameObject.FindGameObjectWithTag("Area").transform.Find("WarpZone").gameObject.SetActive(true);
        Item badge = SaveFile.Instance.Inventory.FirstOrDefault(item => item is Tool_ExpertBadge);
        MenuManager.Instance.Item1EquipmentSlot.EquipItem(badge, 1);
        MenuManager.Instance.UpdateEquippedUsableItems();
    }

    public static void MovedIntoArea() {
        SaveFile.Instance.CurrentMission.MissionProgress += 10;
        Utils.GetUnit("TutorialRyker").AddEffect(new Effect_Unkillable(new(Utils.GetUnit("TutorialRyker"))) {ShowsInUI = false, IsRemovable=false});
        Player.Instance.AddEffect(new Effect_Unkillable(new(Player.Instance)) {ShowsInUI = false, IsRemovable=false});
        Utils.GetUnit("TutorialRyker").CooldownReduction.Maximum = 9999;
        Utils.GetUnit("TutorialRyker").CooldownReduction.Current = 9999;
        NotificationController.ShowCustomizedDialogueNotification(new() {Id="FirstReturn_Inter_60"});
    }

    public void PauseMetalDestructible() {

        Area.Instance.transform.Find("MetalDestructibleLasting").GetComponent<ParticleSystem>().Pause();
        foreach(ParticleSystem ps in Area.Instance.transform.Find("MetalDestructibleLasting").GetComponentsInChildren<ParticleSystem>()) {
            ps.Pause();
        }
    }

    public static void OnEnterArea() {
        UIManager.Instance.ShowBlackScreen(0);
        GameController.Instance.transform.Find("Menu Canvas/Other Window/Window/Buttons/Escape Button").gameObject.SetActive(false);
        UIManager.Instance.StartDialogue(FirstDialogue());
        Utils.ShowMissionObjective("Mission_FirstReturn", "Mission_FirstReturn_Step_0", "UI/Cycle1");
    }

    public override void OnMissionProgressUpdated()
    {
        _counter = 0;
        Utils.ShowMissionObjective(GetType().ToString() , GetType() + "_Step_" + MissionProgress, "UI/Cycle1");
        if(MissionProgress == 30 || MissionProgress == 40 || MissionProgress == 50 || MissionProgress == 60 ||MissionProgress == 70 || MissionProgress == 80 ||MissionProgress == 90 ||MissionProgress == 100 ||MissionProgress == 110 ||MissionProgress == 120 || MissionProgress == 130 || MissionProgress == 140) {
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + MissionProgress, "UI/Cycle1", new List<string> {"0"});
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Mission_FirstReturn_Step_" + MissionProgress + "_Notification", SpeakerName="Ryker", SpeakerPortrait="Ryker"}); 
        }
        else if( MissionProgress == 45 || MissionProgress == 55 || MissionProgress == 65 || MissionProgress == 105 || MissionProgress == 115 || MissionProgress == 125) {
            Utils.ShowMissionObjective(GetType().ToString(), GetType() + "_Step_" + (MissionProgress + 5), "UI/Cycle1", new List<string> {"0"});
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Mission_FirstReturn_Step_" + (MissionProgress + 5) + "_Notification", SpeakerName="Ryker", SpeakerPortrait="Ryker"}); 
        }
    }

    public void TalkToRyker(InteractableObject interactable) {
        interactable.gameObject.SetActive(false);
        if(SaveFile.Instance.CurrentMission.MissionProgress == 20) {
            UIManager.Instance.StartDialogue(RykerFirstDialogue());
        }
        else if(SaveFile.Instance.CurrentMission.MissionProgress == 150) {
            UIManager.Instance.StartDialogue(RykerDialogueBeforeBossFight());
        }
        else if(SaveFile.Instance.CurrentMission.MissionProgress == 160) {
            UIManager.Instance.StartDialogue(RykerFinalDialogue());
        }
    }
}