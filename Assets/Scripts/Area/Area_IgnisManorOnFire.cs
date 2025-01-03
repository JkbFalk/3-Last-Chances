using System.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.U2D;
using TMPro;
using UnityEngine.UI;

public class Area_IgnisManorOnFire
{
    public static int TrainingCount = 0;
    public static List<Transform> PotentialExplosionPositions;

    public static void OnStart() {
        PotentialExplosionPositions = new();
        for(int i = 1; i < 225; i++) {
            PotentialExplosionPositions.Add(Area.Instance.transform.Find("PotentialExplosionPositions/PotentialExplosionPosition_" + i).transform);
        }
        CreateExplosion();

        EventManager.EnemyDefeated.AddListener(CheckDefeatedEnemy);
        EventManager.EnterCombat.AddListener(CheckEnteredCombat);
        if(SaveFile.Instance.CurrentAreaLoadedFromSave) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="IgnisManorOnFire_Interactions_90"});
            SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(210, new() {SaveFile.Instance.IgnisEnergy.ToString()});
            Area.Instance.transform.Find("Interactables/Interact On Enter").gameObject.SetActive(false);
            if(Utils.GetUnit("Hardworker").gameObject.activeSelf) {
                Utils.GetUnit("Hardworker").CurrentTarget = Utils.GetUnit("ArmorBoss");
                Utils.GetUnit("Hardworker").Health.Current = Utils.GetUnit("Hardworker").Health.Maximum * 0.8f;
                Utils.GetUnit("ArmorBoss").Health.Current = Utils.GetUnit("Hardworker").Health.Maximum * 0.9f;
                Utils.GetUnit("Hardworker").AddEffect(new Effect_Invincible(new(Utils.GetUnit("Hardworker"))));
                Utils.GetUnit("ArmorBoss").CurrentTarget = Utils.GetUnit("Hardworker");
                Utils.GetUnit("ArmorBoss").AddEffect(new Effect_Invincible(new(Utils.GetUnit("ArmorBoss"))));

                Utils.GetUnit("IgnisLancer").CurrentTarget = Utils.GetUnit("LivingFlame");
                Utils.GetUnit("IgnisLancer").Health.Current = Utils.GetUnit("IgnisLancer").Health.Maximum * 0.65f;
                Utils.GetUnit("LivingFlame").Health.Current = Utils.GetUnit("IgnisLancer").Health.Maximum * 0.8f;
                Utils.GetUnit("IgnisLancer").AddEffect(new Effect_Invincible(new(Utils.GetUnit("IgnisLancer"))));
                Utils.GetUnit("LivingFlame").CurrentTarget = Utils.GetUnit("IgnisLancer");
                Utils.GetUnit("LivingFlame").AddEffect(new Effect_Invincible(new(Utils.GetUnit("LivingFlame"))));
            }
        }
        else {
            GameController.Instance.MakeAutoSave();
        }
    }

    public static void CreateExplosion() {
        if(Area.Instance != null && Area.Instance.gameObject.name.Contains("IgnisManorOnFire") && Area.Instance.gameObject.IsDestroyed() == false) {
            List<Transform> validTransforms = PotentialExplosionPositions.Where(transform => Vector2.Distance(transform.position, Player.Instance.transform.position) < 20).ToList();
            if(validTransforms.Count > 0) {
                Transform pos = validTransforms[UnityEngine.Random.Range(0, validTransforms.Count)];
                Utils.CreateVisualEffect(new(Utils.GetUnit("Environment")), "IgnisManorOnFireRockEruption", pos.position.x, pos.position.y);
            }
            GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(0.05f, 0.2f), CreateExplosion);
        }
    }

    public static Dialogue Nathalie() {
        return new Dialogue ("Area_IgnisManorOnFire", "Nathalie", new List<DialogueLine>{
        new ("IgnisManorOnFire_Nathalie_0") {Animation="Surprised", Speaker = "Player", TurnSpeakersToFaceEachOther = false},
        new ("IgnisManorOnFire_Nathalie_10") {Animation="TendingToPlants", Speaker = "Nathalie", TurnSpeakersToFaceEachOther = false},
        new ("IgnisManorOnFire_Nathalie_20") {Animation="Doubtful", Speaker = "Player", TurnSpeakersToFaceEachOther = false},
        new ("IgnisManorOnFire_Nathalie_30") {Animation="TendingToPlants", Speaker = "Nathalie", TurnSpeakersToFaceEachOther = false},
        new ("IgnisManorOnFire_Nathalie_Choices") {Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManorOnFire_NathalieAskForAssistance_0"),
            new ("IgnisManorOnFire_NathalieLeave_0")
        }},
        new ("IgnisManorOnFire_NathalieAskForAssistance_10") {Animation="Explaining", Speaker = "Player"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_20") {Animation="ShoulderShrug", Speaker = "Nathalie"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_30") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_40") {Animation="Deflated", Speaker = "Player"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_50") {Animation="Thinking", Speaker = "Nathalie"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_60") {Animation="Realization", Speaker = "Player"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_70") {Animation="SternNo", Speaker = "Nathalie"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_80") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_90") {Animation="Sigh", Speaker = "Nathalie"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_100") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisManorOnFire_NathalieAskForAssistance_110") {Animation="Irritated", Speaker = "Nathalie", IdOfNextDialogueLine="END"},
        new ("IgnisManorOnFire_NathalieLeave_10") {Animation="WaveGoodbyeAloof", Speaker = "Player", TurnSpeakersToFaceEachOther = false},
        new ("IgnisManorOnFire_NathalieLeave_20") {Animation="TendingToPlants", Speaker = "Nathalie", TurnSpeakersToFaceEachOther = false},
    }){DialogueSpeaker=Utils.GetUnit("Nathalie"), PlayerStartingPosition = new Vector2(-53f, -26.5f), PlayerStartingFlipped = true, SpeakerStartingPosition = new Vector2(-55.5f, -26.5f), ReturnUnitsToOriginalPositions = true, SpeakerStartingFlipped = true};}

    public static bool CheckIfEnabled_IgnisManorOnFire_NathalieAskForAssistance_0() {
        return SaveFile.Instance.HasFlag("IgnisManorOnFire_LearnedAboutLibrary");
    }

    public static Dialogue MuseumArmour() {
        return new Dialogue ("Area_IgnisManorOnFire", "MuseumArmour", new List<DialogueLine>{
        new ("IgnisManorOnFire_MuseumArmor_0") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisManorOnFire_MuseumArmor_Choices") {Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManorOnFire_MuseumArmor_10") {IdOfNextDialogueLine="IgnisManorOnFire_MuseumArmor_20"},
            new ("Leave") {IdOfNextDialogueLine="END"}
        }},
        new ("IgnisManorOnFire_MuseumArmor_20") {Animation="HeavilyWounded", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(-4, -16.5f), PlayerStartingFlipped = true};}

    public static void IgnisManorOnFire_MuseumArmor_20() {
        NotificationController.ShowTextNotification("IgnisManorOnFire_MuseumArmor_30");
        Area.Instance.transform.Find("Environment/Museum Display/ArmorSet").gameObject.SetActive(false);
        Area.Instance.transform.Find("Environment/Museum Display/Dialogue").gameObject.SetActive(false);
        SaveFile.Instance.AddItem(typeof(Armor_Knight), Item.ItemGrade.Ultimate);
        SaveFile.Instance.AddItem(typeof(Helmet_Knight), Item.ItemGrade.Ultimate);
        SaveFile.Instance.AddItem(typeof(Gloves_Knight), Item.ItemGrade.Ultimate);
        SaveFile.Instance.AddItem(typeof(Boots_Knight), Item.ItemGrade.Ultimate);
        SaveFile.Instance.AddPermanentPowerUp("IgnisManorOnFire_MuseumArmour");
        SaveFile.Instance.ChangeIgnisEnergy(10);
    }

    public static void OnEnd_IgnisManorOnFire_NathalieAskForAssistance_110() {
        Utils.GetUnit("Unit_FemaleNPC1").gameObject.SetActive(false);
        Utils.GetUnit("Unit_FemaleNPC2").gameObject.SetActive(false);
        SaveFile.Instance.ChangeIgnisEnergy(20);
        SaveFile.Instance.AddFlag("IgnisManorOnFire_BurningLibraryRescued");
        SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter += 2;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id = (SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter == 6 && !SaveFile.Instance.HasFlag("IgnisManorOnFire_BurningLibraryRescued")) ? "IgnisManorOnFire_Interactions_87" : $"IgnisManorOnFire_Interactions_8{SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter}"});
        if(SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter == 8) {
            SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(220, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        }
    }

    public static void OnUpdate() {
        Utils.GetUnit("Unit_IgnisSwordmaster1").transform.position = new Vector2(-51.35f, 43.75f);
        Utils.GetUnit("Nathalie").gameObject.SetActive(!SaveFile.Instance.HasFlag("IgnisManorOnFire_BurningLibraryRescued"));
    }

    public static void HardworkerDuel() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManorOnFire_Interactions_40", SpeakerUnit=Utils.GetUnit("Hardworker")});
        Utils.GetUnit("Hardworker").EndEffect(typeof(Effect_Invincible));
        Utils.GetUnit("ArmorBoss").EndEffect(typeof(Effect_Invincible));
        Utils.GetUnit("ArmorBoss").AttackPlayer();
    }

    public static void LancerDuel() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManorOnFire_Interactions_60", SpeakerUnit=Utils.GetUnit("IgnisLancer")});
        Utils.GetUnit("IgnisLancer").EndEffect(typeof(Effect_Invincible));
        Utils.GetUnit("LivingFlame").EndEffect(typeof(Effect_Invincible));
        Utils.GetUnit("LivingFlame").AttackPlayer();
    }

    public static void CheckDefeatedEnemy(Damage damage) {
        if(damage.TargetOfDamage.gameObject.name.Contains("ArmorBoss")) {
            SaveFile.Instance.ChangeIgnisEnergy(20);
            if(Utils.GetUnit("IgnisLancer").KnockedOut == false) {
                Utils.GetUnit("Hardworker").InCombat = false;
                Utils.GetUnit("Hardworker").Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
                Utils.GetUnit("Hardworker").PlayAnimation("HeavilyWounded");
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManorOnFire_Interactions_41", SpeakerUnit=Utils.GetUnit("Hardworker")});
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManorOnFire_Interactions_42"}, 5);
            }
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("LivingFlame")) {
            SaveFile.Instance.ChangeIgnisEnergy(20);
            if(Utils.GetUnit("IgnisLancer").KnockedOut == false) {
                Utils.GetUnit("IgnisLancer").InCombat = false;
                Utils.GetUnit("IgnisLancer").Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
                Utils.GetUnit("IgnisLancer").PlayAnimation("StandingAround");
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManorOnFire_Interactions_61", SpeakerUnit=Utils.GetUnit("IgnisLancer")});
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManorOnFire_Interactions_62"}, 5);
            }
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("FireElemental")) {
            SaveFile.Instance.ChangeIgnisEnergy(3);
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("AnimatedArmor")) {
            SaveFile.Instance.ChangeIgnisEnergy(3);
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("IgnisSwordmaster")) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
            Area.Instance.transform.Find("Environment/Swordmaster Wall").GetComponent<DestructibleEnvironment>().DestroyObject();
            Area.Instance.transform.Find("Swordmaster Block").gameObject.SetActive(false);
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("IgnisAssassin1")) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("IgnisCaptain")) {
            SaveFile.Instance.ChangeIgnisEnergy(30);
            NotificationController.ShowCustomizedDialogueNotification(new() {Id = "IgnisManorOnFire_Interactions_140"});
            Area.Instance.transform.Find("Environment/Door/Closed").GetComponent<Door>().CanBeOpened = true;
            Area.Instance.transform.Find("Environment/Door_2/Closed").GetComponent<Door>().CanBeOpened = true;
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("IgnisAssassin2")) {
            SaveFile.Instance.ChangeIgnisEnergy(4);
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("IgnisKnight2")) {
            SaveFile.Instance.ChangeIgnisEnergy(4);
        }
        if(damage.TargetOfDamage.gameObject.name.Contains("IgnisPyromancer2")) {
            SaveFile.Instance.ChangeIgnisEnergy(4);
        }
    }

    public static void CheckEnteredCombat(Unit unit) {
        if(!SaveFile.Instance.HasFlag("IgnisManorOnFire_FlameCrazedComment") && unit.GetComponent<EnemyGroup>() != null && unit.GetComponent<EnemyGroup>().GroupName.Contains("FlameCrazed")) {
            SaveFile.Instance.AddFlag("IgnisManorOnFire_FlameCrazedComment");
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisManorOnFire_Interactions_70"});
        }
    }
    public static GameObject VictimToRescue;

    public static void RescueVictim(InteractableObject obj) {
        UIManager.Instance.ShowBlackScreen();
        VictimToRescue = obj.transform.parent.gameObject;
        GameController.Instance.WaitAndRunMethod(0.5f, FinishRescuingVictim);
    }

    public static void FinishRescuingVictim() {
        MonoBehaviour.Destroy(VictimToRescue);
        SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter++;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id = (SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter == 6 && !SaveFile.Instance.HasFlag("IgnisManorOnFire_BurningLibraryRescued")) ? "IgnisManorOnFire_Interactions_87" : $"IgnisManorOnFire_Interactions_8{SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter}"});
        UIManager.Instance.HideBlackScreen();
        SaveFile.Instance.ChangeIgnisEnergy(10);
        if(SaveFile.Instance.IgnisManorOnFire_VictimsSavedCounter == 8) {
            SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(220, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        }
    }

    public static void SwordMonumentsPowerUp(InteractableObject obj) {
        NotificationController.ShowCustomizedDialogueNotification(new() {Id ="IgnisManorOnFire_SwordMonuments_0"});
        NotificationController.ShowTextNotification("IgnisManorOnFire_SwordMonuments_10");
        SaveFile.Instance.AddPermanentPowerUp("IgnisManorOnFire_BurningSword");
        SaveFile.Instance.ChangeIgnisEnergy(15);
    }
    public static void MuseumSwordPowerUp(InteractableObject obj) {
        NotificationController.ShowCustomizedDialogueNotification(new() {Id ="IgnisManorOnFire_MuseumSword_0"});
        NotificationController.ShowTextNotification("IgnisManorOnFire_MuseumSword_10");
        SaveFile.Instance.AddPermanentPowerUp("IgnisManorOnFire_MuseumSword");
        SaveFile.Instance.ChangeIgnisEnergy(10);
    }

    public static void MeetAssassin(InteractableObject obj) {
        NotificationController.ShowCustomizedDialogueNotification(new() {Id ="IgnisManorOnFire_Interactions_110", SpeakerName="IgnisAssassin", SpeakerPortrait="IgnisAssassin"}, 5);
        NotificationController.ShowCustomizedDialogueNotification(new() {Id ="IgnisManorOnFire_Interactions_120"});
    }

    public static Dialogue IgnisCaptainIntroduction() {
        return new Dialogue ("Area_IgnisManorOnFire", "IgnisCaptainIntroduction", new List<DialogueLine>{
        new ("IgnisManorOnFire_IgnisCaptain_0") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisCaptain_10") {Animation="ShoulderShrug", Speaker = "IgnisCaptain"},
        new ("IgnisManorOnFire_IgnisCaptain_20") {Animation="SternNo", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisCaptain_30") {Animation="StandingGuard", Speaker = "IgnisCaptain"},
        new ("IgnisManorOnFire_IgnisCaptain_40") {Animation="Mocking", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisCaptain_50") {Animation="StandingGuard", Speaker = "IgnisCaptain"},
        new ("IgnisManorOnFire_IgnisCaptain_60") {Animation="GiveMeABreak", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisCaptain_70") {Animation="StandingGuard", Speaker = "IgnisCaptain"},
        new ("IgnisManorOnFire_IgnisCaptain_80") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisCaptain_90") {Animation="StandingGuard", Speaker = "IgnisCaptain"},
    }){DialogueSpeaker=Utils.GetUnit("IgnisCaptain"), PlayerStartingFlipped = false};}

    public static void CaptainAggro() {
        Utils.GetUnit("IgnisCaptain").AttackPlayer();
        NotificationController.ShowCustomizedDialogueNotification(new() {Id="IgnisManorOnFire_Interactions_150"});
    }

    public static void LibraryInfo() {
        NotificationController.ShowCustomizedDialogueNotification(new() {Id= SaveFile.Instance.HasFlag("IgnisManorOnFire_BurningLibraryRescued") ? "IgnisManorOnFire_Interactions_10" : "IgnisManorOnFire_Interactions_0"});
    }

    public static void AssassinTalk() {
        NotificationController.ShowCustomizedDialogueNotification(new() {Id="IgnisManorOnFire_Interactions_110", SpeakerName="IgnisAssassin", SpeakerPortrait="IgnisAssassin"});
        NotificationController.ShowCustomizedDialogueNotification(new() {Id="IgnisManorOnFire_Interactions_120"}, 5);
        Utils.GetUnit("IgnisAssassin1").AttackPlayer();
    }

    public static Dialogue Colten() {
        return new Dialogue ("Area_IgnisManorOnFire", "Colten", new List<DialogueLine>{
        new ("IgnisManorOnFire_ColtenConfirm_0") {Animation="Thinking", Speaker = "Player", TurnSpeakersToFaceEachOther=false},
        new ("IgnisManorOnFire_ColtenConfirm_Choices") {Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisManorOnFire_ColtenConfirm_10") {IdOfNextDialogueLine="IgnisManorOnFire_Colten_0"},
            new ("IgnisManorOnFire_ColtenConfirm_20") {IdOfNextDialogueLine="END"}
        }},
        new ("IgnisManorOnFire_Colten_0") {Animation="CallSomeoneOnPhone", Speaker = "Colten", TurnSpeakersToFaceEachOther=false},
        new ("IgnisManorOnFire_Colten_10") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisManorOnFire_Colten_20") {Animation="Sigh", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_30") {Animation="Accusing", Speaker = "Player"},
        new ("IgnisManorOnFire_Colten_40") {Animation="ShoulderShrug", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_50") {Animation="Doubtful", Speaker = "Player"},
        new ("IgnisManorOnFire_Colten_60") {Animation="HandWave", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_70") {Animation="ThreatenWithHeavy", Speaker = "Player"},
        new ("IgnisManorOnFire_Colten_80") {Animation="Surprised", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_90") {Animation="ShoulderShrug", Speaker = "Player", IdOfNextDialogueLine = SaveFile.Instance.IgnisEnergy < 500 ? "IgnisManorOnFire_Colten_100" : SaveFile.Instance.IgnisEnergy < 700 ? "IgnisManorOnFire_Colten_101" : "IgnisManorOnFire_Colten_102"},
        new ("IgnisManorOnFire_Colten_100") {Animation="ThreatenWithHeavy", Speaker = "Colten", IdOfNextDialogueLine="IgnisManorOnFire_Colten_110"},
        new ("IgnisManorOnFire_Colten_101") {Animation="ThreatenWithHeavy", Speaker = "Colten", IdOfNextDialogueLine="IgnisManorOnFire_Colten_110"},
        new ("IgnisManorOnFire_Colten_102") {Animation="Surprised", Speaker = "Colten", IdOfNextDialogueLine="IgnisManorOnFire_Colten_110"},
        new ("IgnisManorOnFire_Colten_110") {Speaker = "Player", WaitTimeBeforeAllowingToProceed=3},
        new ("IgnisManorOnFire_Colten_120") {Animation="Surprised", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_130") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisManorOnFire_Colten_140") {Animation="SternNo", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_150") {Animation="DestroySwordMonuments", Speaker = "Player", WaitTimeBeforeAllowingToProceed=3},
        new ("IgnisManorOnFire_Colten_160") {Animation="Surprised", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_170") {Animation="Deflated", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_180") {Animation="PreparingForBattle", Speaker = "Player"},
        new ("IgnisManorOnFire_Colten_190") {Animation="Sigh", Speaker = "Colten"},
        new ("IgnisManorOnFire_Colten_200") {Animation="FinallyDecided", Speaker = "Colten"},
    }){DialogueSpeaker=Utils.GetUnit("Colten"), PlayerStartingFlipped = false, PlayerStartingPosition= new Vector2(14, 0.5f), SpeakerStartingFlipped = false, SpeakerStartingPosition=new Vector2(18, 0.5f), ReturnUnitsToOriginalPositions=false, AutoSaveOnDialogueEnd=false};}

    public static void IgnisManorOnFire_Colten_0() {
        Utils.GetUnit("Colten").transform.Find("Dialogue").gameObject.SetActive(false);
        Utils.GetUnit("Colten").SpriteRenderers["Upper Body"].Bone.transform.Find("Consumable").gameObject.SetActive(false);
    }

    public static void IgnisManorOnFire_Colten_110() {
        Player.Instance.CanRun = false;
        Player.Instance.Actions.IsFlipped = true;
        Player.Instance.Actions.MoveToPoint(11, 0.5f);
        GameController.Instance.WaitAndRunMethod(1.5f, IgnisManorOnFire_Colten_110v2);
    }

    public static void IgnisManorOnFire_Colten_110v2() {
        Player.Instance.Actions.IsFlipped = false;
        Player.Instance.CanRun = true;
        Player.Instance.Actions.IsFlipped = false;
        Player.Instance.PlayAnimation("Accusing");
        Utils.GetUnit("Colten").PlayAnimation("Surprised");
        UIManager.Instance.ShowBlackScreen(1.5f);
    }

    public static void IgnisManorOnFire_Colten_120() {
        UIManager.Instance.HideBlackScreen();
        Player.Instance.UnitAI.NavMeshAgent.enabled = false;
        Player.Instance.transform.position = new Vector2(-50, 9);
        Utils.GetUnit("Colten").transform.position = new Vector2(-36, 9);
        Area.Instance.transform.Find("FireElementals").gameObject.SetActive(false);
    }

    public static void IgnisManorOnFire_Colten_150() {
        SaveFile.Instance.ChangeIgnisEnergy(100);
        for(int i = 0; i < 50; i++) {
            GameController.Instance.WaitAndRunMethodRealtime(2 + 0.03f * i, AdvanceFireWave);
        }
        GameController.Instance.WaitAndRunMethod(2f, CreateFireWave);
        GameController.Instance.WaitAndRunMethod(2.25f, DestroyMonuments);
        CameraController.Instance.CenteredOnObject = null;
        CameraController.Instance.ShakeScreen(2f, 0.02f, 1);
    }

    public static void CreateFireWave() {
        Area.Instance.transform.Find("Environment/VisualEffect_InfernalBlade").gameObject.SetActive(true);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Fire/Fire6");
    }

    public static void AdvanceFireWave() {
        ParticleSystem.ShapeModule shape = Area.Instance.transform.Find("Environment/VisualEffect_InfernalBlade").GetComponent<ParticleSystem>().shape;
        shape.radius += 0.15f;
    }

    public static void DestroyMonuments() {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Greatsword/Greatsword_HeavySwing3", 1);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Hit/Metal_Destroy1", 1);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Hit/Metal_Destroy2", 1);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Hit/Metal_Destroy3", 1);
        CameraController.Instance.CenteredOnObject = null;
        CameraController.Instance.ShakeScreen(0.5f, 0.5f, 2f);
        Area.Instance.transform.Find("Environment/Monument Scraps").gameObject.SetActive(true);
        foreach(Transform sword_monument in Area.Instance.transform.Find("Environment/Sword Tombs")) {
            sword_monument.GetComponent<DestructibleEnvironment>().DestroyObject();
        }
    }

    public static void OnEnd_IgnisManorOnFire_Colten_200() {
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(230, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Area_IgnisManorOnFire.StartColtenDuel"});
        StartColtenDuel();
    }

    public static Effect_PlundererAbilityAmplify PlayerBuffEffect;
    
    public static void StartColtenDuel() {
        Player.Instance.transform.position = new Vector2(-50, 9);
        Utils.GetUnit("Colten").transform.position = new Vector2(-36, 9);
        Utils.GetUnit("Colten").AttackPlayer();
        Utils.PlayIntermissionMusic("Ignis");
        Utils.GetUnit("Colten").AddEffect(new Effect_CannotBeDefeated(true, new(Utils.GetUnit("Colten"))));
        EventManager.UnitWouldBeDefeated.AddListener(CheckIfColtenDefeated);
        EventManager.HealthBarBroken.AddListener(CheckHealthBarBroken);
        EventManager.AbilityUsed.AddListener(CheckAbilityUsed);
        EventManager.EffectStarted.AddListener(CheckEffectStarted);
        if(Area.Instance.transform.Find("Interactables/Interact On Enter") != null) {
            Area.Instance.transform.Find("Interactables/Interact On Enter").gameObject.SetActive(false);
        }
        foreach(Unit u in Area.Instance.GetComponentsInChildren<Unit>()) {
            if(u is not Player && u.gameObject.name.Contains("Colten") == false) {
                u.gameObject.SetActive(false);
            }
        }
        GameController.Instance.InterruptMusicOnDeath = false;
        CanvasElements.UICanvasObject.transform.Find("Ignis Energy").gameObject.SetActive(true);
        CanvasElements.UICanvasObject.transform.Find("Ignis Energy/Amount").GetComponent<TextMeshProUGUI>().text = SaveFile.Instance.IgnisEnergy.ToString();
        CanvasElements.UICanvasObject.transform.Find("Ignis Energy").GetComponent<Slider>().value = SaveFile.Instance.IgnisEnergy / 2000;
        PlayerBuffEffect = new Effect_PlundererAbilityAmplify(SaveFile.Instance.IgnisEnergy / 10, new(Utils.GetUnit("Colten"))) {AmplifiedFamily = Ability.AbilityFamily.Ignis};
        Player.Instance.AddEffect(PlayerBuffEffect);
    }

    public static void CheckIfColtenDefeated(Damage dmg) {
        if(dmg.TargetOfDamage.gameObject.name.Contains("Colten")) {
            Utils.GetUnit("Colten").SetToNeutralNPC();
            GameController.Instance.InterruptMusicOnDeath = true;
            Utils.PlayIntermissionMusic("Sadness_40");
            UIManager.Instance.StartDialogue(FinalColten());
        } 
    }

    public static void CheckHealthBarBroken(Damage damage) {
        if(damage.TargetOfDamage.gameObject.name.Contains("Colten")) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="IgnisVolcano_Interactions_120", SpeakerUnit=Utils.GetUnit("Colten")});
        }
    }

    public static void CheckAbilityUsed(Ability ability) {
        if(ability.User is Player && ability.IsRiposte) {
            SaveFile.Instance.ChangeIgnisEnergy(50);
        }
        if(ability.User is Player && ability.IsCounter) {
            SaveFile.Instance.ChangeIgnisEnergy(100);
        }
    }

    public static void CheckEffectStarted(Effect effect) {
        if(effect.TargetOfEffect.gameObject.name.Contains("Colten") && effect is Effect_SoftStaggered) {
            SaveFile.Instance.ChangeIgnisEnergy(50);
        }
        else if(effect.TargetOfEffect.gameObject.name.Contains("Colten") && effect is Effect_HardStaggered) {
            SaveFile.Instance.ChangeIgnisEnergy(100);
        }
    }

    public static Dialogue FinalColten() {
        return new Dialogue ("Area_IgnisManorOnFire", "FinalColten", new List<DialogueLine>{
        new ("IgnisManorOnFire_IgnisConclusion_0") {Animation="HeavilyWounded", Speaker = "Colten"},
        new ("IgnisManorOnFire_IgnisConclusion_Choices") {Speaker = "Player", Animation="FinallyDecided", Choices= new List<DialogueChoice> {
            new ("IgnisManorOnFire_IgnisConclusionReap_0"),
            new ("IgnisManorOnFire_IgnisConclusionSpare_0")
        }},
        new ("IgnisManorOnFire_IgnisConclusionReap_10") {Animation="ShatterEnergyCore", Speaker = "Player", WaitTimeBeforeAllowingToProceed=2},
        new ("IgnisManorOnFire_IgnisConclusionReap_20") {Animation="SadChuckle", Speaker = "Colten"},
        new ("IgnisManorOnFire_IgnisConclusionReap_30") {Animation="NotQuite", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisConclusionReap_40") {Animation="Irritated", Speaker = "Colten"},
        new ("IgnisManorOnFire_IgnisConclusionReap_50") {Animation="ShoulderShrug", Speaker = "Player", IdOfNextDialogueLine="IgnisManorOnFire_IgnisConclusion_20"},
        new ("IgnisManorOnFire_IgnisConclusionSpare_10") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisConclusionSpare_20") {Animation="SternNo", Speaker = "Colten"},
        new ("IgnisManorOnFire_IgnisConclusionSpare_30") {Animation="ShoulderShrug", Speaker = "Player", WaitTimeBeforeAllowingToProceed=4},
        new ("IgnisManorOnFire_IgnisConclusionSpare_40") {Animation="WobblyGetUp", Speaker = "Colten"},
        new ("IgnisManorOnFire_IgnisConclusionSpare_50") {Animation="Thinking", Speaker = "Player"},
        new ("IgnisManorOnFire_IgnisConclusionSpare_60") {Animation="Deflated", Speaker = "Colten", IdOfNextDialogueLine="IgnisManorOnFire_IgnisConclusion_20"},
        new ("IgnisManorOnFire_IgnisConclusion_20") {Speaker = "Player", WaitTimeBeforeAllowingToProceed=2},
        new ("IgnisManorOnFire_IgnisConclusion_30") {Animation="RespectfulBow", Speaker = "IgnisCaptain"},
        new ("IgnisManorOnFire_IgnisConclusion_40") {Animation="Sigh", Speaker = "Player",},
        new ("IgnisManorOnFire_IgnisConclusion_50") {Animation="Frustrated", Speaker = "Player"},
    }){DialogueSpeaker=Utils.GetUnit("Colten"), PlayerStartingFlipped = false, PlayerStartingPosition=new Vector2(-44, 10), SpeakerStartingFlipped=true, SpeakerStartingPosition= new Vector2(-42, 10)};}

    public static void IgnisManorOnFire_IgnisConclusionReap_10() {
        GameController.Instance.WaitAndRunMethod(1.5f, IgnisManorOnFire_IgnisConclusionReap_10v2);
    }

    public static void IgnisManorOnFire_IgnisConclusionReap_10v2() {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Dialogue/ShatterEnergyCore", 0.9f);
        Utils.GetUnit("Colten").PlayAnimation("Flinching");
        Utils.CreateVisualEffect(new(Player.Instance), "ShatterEnergyCore", Player.Instance.SpriteRenderers["Right Hand"].Bone.position.x, Player.Instance.SpriteRenderers["Right Hand"].Bone.position.y);
    }

    public static void IgnisManorOnFire_IgnisConclusionSpare_30() {
        GameController.Instance.WaitAndRunMethod(1, IgnisManorOnFire_IgnisConclusionSpare_30v2);
    }

    public static void IgnisManorOnFire_IgnisConclusionSpare_30v2() {
        Player.Instance.CanRun = false;
        Player.Instance.Actions.MoveToPoint(-40, 8);
    }

    public static void IgnisManorOnFire_IgnisConclusion_20() {
        Utils.GetUnit("Colten").PlayAnimation("HeavilyWounded");
        Player.Instance.Actions.IsFlipped = false;
        Player.Instance.CanRun = false;
        Player.Instance.Actions.MoveToPoint(-19, 1);
        UIManager.Instance.ShowBlackScreen(1.5f);
        GameController.Instance.WaitAndRunMethod(2, IgnisManorOnFire_IgnisConclusion_20v2);
    }

    public static void IgnisManorOnFire_IgnisConclusion_20v2() {
        Player.Instance.transform.position = new Vector3(-19, 1);
        UIManager.Instance.HideBlackScreen(1.5f);
        Utils.GetUnit("IgnisCaptain").gameObject.SetActive(true);
        Utils.GetUnit("IgnisCaptain").transform.position = new Vector3(-16, 1);
        Utils.GetUnit("IgnisCaptain").PlayAnimation("StandingGuard");
    }

    public static void IgnisManorOnFire_IgnisConclusion_50() {
        UIManager.Instance.ShowBlackScreen(5);
    }

    public static void OnEnd_FinalColten() {
        Player.Instance.CanRun = true;
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(240);
        SaveFile.Instance.GetQuest("Ignis").CompleteQuest();
        SaveFile.Instance.IncreaseEnergyLevel(Ability.AbilityFamily.Ignis);
        SaveFile.Instance.CurrentMission.Finish(true);
    }
}
