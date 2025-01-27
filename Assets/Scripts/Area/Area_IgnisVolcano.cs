using System.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

public class Area_IgnisVolcano
{
    public static void OnStart() {
        for(int i = 1; i < (SaveFile.Instance.Cycle == 2 ? 242 : 126); i++) {
            PotentialExplosionPositions.Add(Area.Instance.transform.Find("PotentialExplosionPositions/PotentialExplosionPosition_" + i).transform);
        }
        CreateExplosion(); 
        if(SaveFile.Instance.Cycle == 2 && SaveFile.Instance.CurrentAreaLoadedFromSave == false) {
            UIManager.Instance.StartDialogue(FlameShadow1());
        }
        else if(SaveFile.Instance.Cycle == 3 && SaveFile.Instance.CurrentAreaLoadedFromSave == false) {
            GameController.Instance.WaitAndRunMethod(0.01f, PlayCycle3Intro);
        }
        if(SaveFile.Instance.Cycle == 3) {
            return;
        }
        Area.Instance.transform.Find("Environment/GiantArmourWall").gameObject.SetActive(SaveFile.Instance.DoesNotHaveFlag("IgnisVolcano_DefeatedGiantArmor"));
        EventManager.EnemyDefeated.AddListener(CheckDefeatedEnemy);
        _slainElementals = 0;
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum/Dialogue").gameObject.SetActive(!SaveFile.Instance.HasFlag("IgnisVolcano_SlewRetributionKnight"));
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_2/Dialogue").gameObject.SetActive(!SaveFile.Instance.HasFlag("IgnisVolcano_SlewHeavenlyHalberdKnight"));
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_3/Dialogue").gameObject.SetActive(!SaveFile.Instance.HasFlag("IgnisVolcano_SlewSeveranceKnight"));
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum/Loot").gameObject.SetActive(!SaveFile.Instance.HasFlag("IgnisVolcano_PickedUpRetribution") && SaveFile.Instance.HasFlag("IgnisVolcano_SlewRetributionKnight"));
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_2/Loot").gameObject.SetActive(!SaveFile.Instance.HasFlag("IgnisVolcano_PickedUpHeavenlyHalberd") && SaveFile.Instance.HasFlag("IgnisVolcano_SlewHeavenlyHalberdKnight"));
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_3/Loot").gameObject.SetActive(!SaveFile.Instance.HasFlag("IgnisVolcano_PickedUpSeverance") && SaveFile.Instance.HasFlag("IgnisVolcano_SlewSeveranceKnight"));
        if(SaveFile.Instance.HasFlag("IgnisVolcano_AwakenedRetributionKnight")) {
            RetributionAttack();
        }
        if(SaveFile.Instance.HasFlag("IgnisVolcano_AwakenedHeavenlyHalberdKnight")) {
            HeavenlyHalberdAttack();
        }
        if(SaveFile.Instance.HasFlag("IgnisVolcano_AwakenedSeveranceKnight")) {
            SeveranceAttack();
        }
    }

    public static void PlayCycle3Intro() {
        UIManager.Instance.StartDialogue(FinalVolcano());
    }

    private static int _slainElementals = 0;

    public static List<Transform> PotentialExplosionPositions = new();
    public static void CreateExplosion() {
        if(Area.Instance != null && Area.Instance.gameObject.name.Contains("IgnisVolcano") && Area.Instance.gameObject.IsDestroyed() == false) {
            List<Transform> validTransforms = PotentialExplosionPositions.Where(transform => transform.IsDestroyed() == false && Vector2.Distance(transform.position, Player.Instance.transform.position) < 20).ToList();
            if(validTransforms.Count > 0) {
                Transform pos = validTransforms[UnityEngine.Random.Range(0, validTransforms.Count)];
                Utils.CreateVisualEffect(new("Environment"), "IgnisManorOnFireRockEruption", pos.position.x, pos.position.y);
                GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(0.5f, 1f), CreateExplosion);
            }
        }
    }

    public static Dialogue FlameShadow1() {
        return new Dialogue ("Area_IgnisVolcano", "FlameShadow1", new List<DialogueLine>{
        new ("IgnisVolcano_FlameShadow1_0") ,
        new ("IgnisVolcano_FlameShadow1_5") {Animation="Intrigued", Speaker = "Blaine1", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_FlameShadow1_10") {Animation="Exhausted", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow1_20") {Animation="ShoulderShrug", Speaker = "Blaine1"},
        new ("IgnisVolcano_FlameShadow1_30") {Animation="HereWeGo", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow1_40") {Animation="PointDown", Speaker = "Blaine1"},
        new ("IgnisVolcano_FlameShadow1_50") {WaitTimeBeforeAllowingToProceed=2.5f},
        new ("IgnisVolcano_FlameShadow1_60") {Animation="Surprised", Speaker = "Player", WaitTimeBeforeAllowingToProceed=4},
        new ("IgnisVolcano_FlameShadow1_70") {Animation="Surprised", Speaker = "Blaine1"},
        new ("IgnisVolcano_FlameShadow1_80") {Animation="Flabbergasted", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow1_90") {Animation="Doubtful", Speaker = "Blaine1"},
        new ("IgnisVolcano_FlameShadow1_100") {Animation="HandWave", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow1_110") {Speaker = "Blaine1"},
        new ("IgnisVolcano_FlameShadow1_120") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow1_130") {Animation="YesMe", Speaker = "FlameShadow1"},
        new ("IgnisVolcano_FlameShadow1_140") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow1_150") {Animation="FinallyDecided", Speaker = "FlameShadow1"},
        new ("IgnisVolcano_FlameShadow1_160") {Animation="TryToStop", Speaker = "Player", WaitTimeBeforeAllowingToProceed=0.55f}
    }){PlayerStartingPosition = new Vector2(-58.5f, -11.5f), PlayerStartingFlipped = false, SpeakerStartingFlipped=false, SpeakerStartingPosition=new(-54, -13.5f), DialogueSpeaker=Utils.GetUnit("Blaine1"), ReturnUnitsToOriginalPositions=false};}

    public static void IgnisVolcano_FlameShadow1_0() {
        GameController.Instance.WaitAndRunMethod(0.01f, IgnisVolcano_FlameShadow1_0v2);
    }
    public static void IgnisVolcano_FlameShadow1_0v2() {
        Player.Instance.PlayAnimation("StopRunning", 0, 0.25f);
    }
    public static void IgnisVolcano_FlameShadow1_50() {
        CameraController.Instance.CenteredOnObject = Player.Instance.gameObject;
        Player.Instance.Actions.MoveToPoint(-52.25f, -11.5f);
        GameController.Instance.WaitAndRunMethod(2.4f, EnableNextLine);
    }

    public static void EnableNextLine() {
        Player.Instance.PlayAnimation("IdleInCombat");
    }

    public static void IgnisVolcano_FlameShadow1_60() {
        Player.Instance.CanRun = true;
        Utils.GetUnit("FlameShadow1").gameObject.SetActive(true);
        Utils.GetUnit("FlameShadow1").PlayAnimation("SternNo");
        GameController.Instance.WaitAndRunMethod(1.5f, ClashWithFlameShadow);
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("FlameShadow1").gameObject;
    }

    public static void ClashWithFlameShadow() {
        Player.Instance.PlayAnimation("SwordSweep", 0.05f, 0.33f);
        Utils.GetUnit("FlameShadow1").PlayAnimation("PushedBack");
        GameController.Instance.WaitAndRunMethod(0.55f, PlayClashSound);
        GameController.Instance.WaitAndRunMethod(1.5f, FinishClashWithFlameShadow);
    }

    public static void PlayClashSound() {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Impact/Clash");
    }

    public static void FinishClashWithFlameShadow() {
        Player.Instance.PlayAnimation("IdleInCombat");
        Utils.GetUnit("FlameShadow1").PlayAnimation("IdleInCombat");
    }

    public static void IgnisVolcano_FlameShadow1_110() {
        Utils.GetUnit("Blaine1").Actions.MoveToPoint(-54, -9.5f);
        UIManager.Instance.ShowBlackScreen(1);
    }
    public static void IgnisVolcano_FlameShadow1_120() {
        Player.Instance.Actions.IsFlipped = false;
        Utils.GetUnit("Blaine1").gameObject.SetActive(false);
        UIManager.Instance.HideBlackScreen();
    }

    public static void IgnisVolcano_FlameShadow1_160() {
        Utils.CreateVisualEffect(new(Utils.GetUnit("FlameShadow1")), "FlameShadowDisappear", Utils.GetUnit("FlameShadow1").transform.position.x, Utils.GetUnit("FlameShadow1").transform.position.y);
        GameController.Instance.WaitAndRunMethod(0.5f, MakeFlameShadowDisappear);
    }

    public static void MakeFlameShadowDisappear() {
        Utils.GetUnit("FlameShadow1").gameObject.SetActive(false);
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(100, new() {SaveFile.Instance.IgnisEnergy.ToString()});
    }

    public static void CheckDefeatedEnemy(Damage damage) {
        if(damage.TargetOfDamage.gameObject.name.Contains("FireElemental")) {
            _slainElementals++;
            if(_slainElementals >= 8 && SaveFile.Instance.HasFlag("IgnisVolcano_TalkedFlameShadow3")) {
                ActivateFlameShadowBoss();
            }
            else if(_slainElementals >= 8 && !SaveFile.Instance.HasFlag("IgnisVolcano_OverchargedFlameShadow")) {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_Interactions_30"});
            }
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("FlameShadowBoss")) {
            Utils.GetUnit("FlameShadowBoss1").gameObject.SetActive(false);
            Utils.GetUnit("FlameShadow4").gameObject.SetActive(true);
            UIManager.Instance.StartDialogue(FlameShadow4());
            SaveFile.Instance.ChangeIgnisEnergy(24);
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("RetributionKnight1")) {
            SaveFile.Instance.AddFlag("IgnisVolcano_SlewRetributionKnight");
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("HeavenlyHalberdKnight1")) {
            SaveFile.Instance.AddFlag("IgnisVolcano_SlewHeavenlyHalberdKnight");
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("SeveranceKnight1")) {
            SaveFile.Instance.AddFlag("IgnisVolcano_SlewSeveranceKnight");
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("GiantArmour1")) {
            UIManager.Instance.ShowBlackScreen(2);
            SaveFile.Instance.AddFlag("IgnisVolcano_DefeatedGiantArmor");
            GameController.Instance.WaitAndRunMethod(2, StartFlameShadow2);
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("AnimatedArmor")) {
            SaveFile.Instance.ChangeIgnisEnergy(5);
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("ExplosivesExpert")) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        else if(damage.TargetOfDamage.gameObject.name.Contains("FireElemental")) {
            SaveFile.Instance.ChangeIgnisEnergy(2);
        }
    }

    public static void StartFlameShadow2() {
        Utils.GetUnit("FlameShadow2").gameObject.SetActive(true);
        Area.Instance.transform.Find("Environment/GiantArmourWall").gameObject.SetActive(false);
        UIManager.Instance.StartDialogue(FlameShadow2());
    }

    public static void MakeMountainPeakAccessible() {
        Area.Instance.transform.Find("Mountain Peak").GetComponent<SpriteRenderer>().color = Colors.GetColorFromCode("#8C6B6B");
    }

    public static Dialogue FlameShadow2() {
        return new Dialogue ("Area_IgnisVolcano", "FlameShadow2", new List<DialogueLine>{
        new ("IgnisVolcano_FlameShadow2_0") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2_10") {Animation="Thinking", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2_20") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2_30") {Animation="HereWeGo", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2_40") {Animation="PointDown", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2_50") {Animation="Listening", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2_60") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisVolcano_FlameShadow2Family_0"),
            new ("IgnisVolcano_FlameShadow2Remnant_0"),
            new ("IgnisVolcano_FlameShadow2Evolution_0")
        }},
        new ("IgnisVolcano_FlameShadow2Family_10") {Animation="Explaining", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2Family_20") {Animation="Surprised", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2Family_30") {Animation="Thinking", Speaker = "Player", IdOfNextDialogueLine="IgnisVolcano_FlameShadow2_Choices"},
        new ("IgnisVolcano_FlameShadow2Evolution_10") {Animation="Explaining", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2Evolution_20") {Animation="Doubtful", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2Evolution_30") {Animation="Thinking", Speaker = "Player", IdOfNextDialogueLine="IgnisVolcano_FlameShadow2_Choices"},
        new ("IgnisVolcano_FlameShadow2Remnant_10") {Animation="Explaining", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2Remnant_20") {Animation="Approving", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2Remnant_30") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2Remnant_40") {Animation="Impatient", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2Remnant_50") {Animation="Listening", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2Remnant_60") {Animation="Explaining", Speaker = "FlameShadow2"},
        new ("IgnisVolcano_FlameShadow2Remnant_70") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow2Remnant_80") {Speaker = "FlameShadow2"}
    }){PlayerStartingPosition = new Vector2(-43f, 21f), PlayerStartingFlipped = false, SpeakerStartingFlipped=true, SpeakerStartingPosition=new(-40, 21f), DialogueSpeaker=Utils.GetUnit("FlameShadow2"), ReturnUnitsToOriginalPositions=false};}

    public static bool CheckIfVisible_IgnisVolcano_FlameShadow2Family_0() {
        return !SaveFile.Instance.HasFlag("IgnisVolcano_GuessedFamily");
    }

    public static void IgnisVolcano_FlameShadow2Family_10() {
        SaveFile.Instance.AddFlag("IgnisVolcano_GuessedFamily");
    }

    public static bool CheckIfVisible_IgnisVolcano_FlameShadow2Evolution_0() {
        return !SaveFile.Instance.HasFlag("IgnisVolcano_GuessedEvolution");
    }
    public static void IgnisVolcano_FlameShadow2Evolution_10() {
        SaveFile.Instance.AddFlag("IgnisVolcano_GuessedEvolution");
    }

    public static void IgnisVolcano_FlameShadow2Remnant_10() {
        int amount = 50;
        if(SaveFile.Instance.HasFlag("IgnisVolcano_GuessedFamily")) {
            amount -= 10;
        }
        if(SaveFile.Instance.HasFlag("IgnisVolcano_GuessedEvolution")) {
            amount -= 10;
        }
        SaveFile.Instance.ChangeIgnisEnergy(amount);
    }

    public static void IgnisVolcano_FlameShadow2Remnant_80() {
        Utils.CreateVisualEffect(new(Utils.GetUnit("FlameShadow2")), "FlameShadowDisappear", Utils.GetUnit("FlameShadow2").transform.position.x, Utils.GetUnit("FlameShadow2").transform.position.y);
        Utils.GetUnit("FlameShadow2").gameObject.SetActive(false);
        SaveFile.Instance.AddFlag("IgnisVolcano_TalkedFlameShadow2");
    }

    public static Dialogue Blaine1() {
        return new Dialogue ("Area_IgnisVolcano", "Blaine1", new List<DialogueLine>{
        new ("IgnisVolcano_Blaine1_0") {Animation="Frustrated", Speaker = "Blaine2"},
        new ("IgnisVolcano_Blaine1_10") {Animation="HandWave", Speaker = "Player"},
        new ("IgnisVolcano_Blaine1_20") {Animation="YesYou", Speaker = "Blaine2"},
        new ("IgnisVolcano_Blaine1_30") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_Blaine1_40") {Animation="Realization", Speaker = "Blaine2"},
        new ("IgnisVolcano_Blaine1_50") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisVolcano_Blaine1_60") {Animation="HaveItYourWayThen", Speaker = "Blaine2"},
        new ("IgnisVolcano_Blaine1_70") {Animation="Impatient", Speaker = "Player"},
        new ("IgnisVolcano_Blaine1_80") {Animation="ShoulderShrug", Speaker = "Blaine2"},
    }){PlayerStartingPosition = new Vector2(-16.5f, 39), PlayerStartingFlipped = true, SpeakerStartingFlipped=false, SpeakerStartingPosition=new(-19, 39), DialogueSpeaker=Utils.GetUnit("Blaine2")};}

    public static void IgnisVolcano_Blaine1_10() {
        Area.Instance.transform.Find("Environment/River_2").gameObject.SetActive(false);
        Area.Instance.transform.Find("Environment/RiverGraphic_2").gameObject.SetActive(false);
        Area.Instance.transform.Find("NPCs/Unit_FlameShadow3").gameObject.SetActive(true);
    }

    public static void OnEnd_Blaine1() {
        Utils.GetUnit("Blaine2").gameObject.SetActive(false);
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(110, new() {SaveFile.Instance.IgnisEnergy.ToString()});
    }

    public static Dialogue FlameShadow3() {
        return new Dialogue ("Area_IgnisVolcano", "FlameShadow3", new List<DialogueLine>{
        new ("IgnisVolcano_FlameShadow3_0") {Animation="Sigh", Speaker = "FlameShadow3"},
        new ("IgnisVolcano_FlameShadow3_10") {Animation="Explaining", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow3_20") {Animation="Realization", Speaker = "FlameShadow3"},
        new ("IgnisVolcano_FlameShadow3_30") {Animation="GiveMeABreak", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow3_40") {Animation="FinallyDecided", Speaker = "FlameShadow3"},
        new ("IgnisVolcano_FlameShadow3_50") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow3_60") {Animation="ShoulderShrug", Speaker = "FlameShadow3"},
        new ("IgnisVolcano_FlameShadow3_70") {Animation="Intrigued", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(11f, 33.5f), PlayerStartingFlipped = false, SpeakerStartingFlipped=true, SpeakerStartingPosition=new(14, 33.5f), DialogueSpeaker=Utils.GetUnit("FlameShadow3")};}

    public static void OnEnd_FlameShadow3() {
        SaveFile.Instance.AddFlag("IgnisVolcano_TalkedFlameShadow3");
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(120, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        Utils.GetUnit("FlameShadow3").gameObject.SetActive(false);
        if(_slainElementals >= 6) {
            ActivateFlameShadowBoss();
        }
    }

    public static void ActivateFlameShadowBoss() {
        Utils.GetUnit("ExplosivesExpert1").InCombat = false;
        Utils.GetUnit("ExplosivesExpert1").gameObject.SetActive(false);
        Utils.GetUnit("FlameShadowBoss1").gameObject.SetActive(true);
        SaveFile.Instance.AddFlag("IgnisVolcano_OverchargedFlameShadow");
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_Interactions_40"});
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(130, new() {SaveFile.Instance.IgnisEnergy.ToString()});
    }

    public static void IgnisVolcanoShop(InteractableObject inter) {
        MenuManager.Instance.OpenShop(new() {}, 
            "IgnisVolcanoShop");
    }

    public static void AssassinsAggro() {
        if(SaveFile.Instance.HasFlag("IgnisVolcano_AmbushedByIgnis")) {
            return;
        }
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_Interactions_10"});
        SaveFile.Instance.AddFlag("IgnisVolcano_AmbushedByIgnis");
        Utils.GetUnit("Unit_IgnisAssassin1").gameObject.SetActive(true);
        Utils.GetUnit("Unit_IgnisAssassin2").gameObject.SetActive(true);
        GameController.Instance.WaitAndRunMethod(0.01f, Aggro2);
    }

    public static void Aggro2() {
        Utils.GetUnit("Unit_IgnisAssassin1").AttackPlayer();
        Utils.GetUnit("Unit_IgnisAssassin2").AttackPlayer();
        GameController.Instance.WaitAndRunMethod(0.05f, Aggro3);
    }

    public static void Aggro3() {
        new Damage(Utils.GetUnit("Unit_IgnisAssassin1"), new Ability_SourcelessDamage(Player.Instance), null) {Stagger = 5000}.CalculateDamage();
        Utils.GetUnit("Unit_IgnisAssassin1").Actions.PushUnitForward(-350);
        Utils.GetUnit("Unit_IgnisAssassin1").AddEffect(new Effect_Onslaught(100, new(Utils.GetUnit("Unit_IgnisAssassin2"))));
        new Damage(Utils.GetUnit("Unit_IgnisAssassin2"), new Ability_SourcelessDamage(Player.Instance), null) {Stagger = 5000}.CalculateDamage();
        Utils.GetUnit("Unit_IgnisAssassin2").Actions.PushUnitForward(-350);
        Utils.GetUnit("Unit_IgnisAssassin2").AddEffect(new Effect_Onslaught(100, new(Utils.GetUnit("Unit_IgnisAssassin2"))));
    }

    public static void AssassinsBackstab() {
        if(SaveFile.Instance.HasFlag("IgnisVolcano_AmbushedByIgnis")) {
            return;
        }
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_Interactions_10"});
        SaveFile.Instance.AddFlag("IgnisVolcano_AmbushedByIgnis");
        Utils.GetUnit("Unit_IgnisAssassin1").gameObject.SetActive(true);
        Utils.GetUnit("Unit_IgnisAssassin2").gameObject.SetActive(true);
        GameController.Instance.WaitAndRunMethod(0.01f, Backstab2);
    }

    public static void Backstab2() {
        Utils.GetUnit("Unit_IgnisAssassin1").AttackPlayer();
        Utils.GetUnit("Unit_IgnisAssassin1").Actions.UseAbility(typeof(NPCAbility_FlameBackstab));
        Utils.GetUnit("Unit_IgnisAssassin2").AttackPlayer();
        Utils.GetUnit("Unit_IgnisAssassin2").Actions.UseAbility(typeof(NPCAbility_FlameBackstab));
    }

    public static Dialogue FlameShadow4() {
        return new Dialogue ("Area_IgnisVolcano", "FlameShadow4", new List<DialogueLine>{
        new ("IgnisVolcano_FlameShadow4_0") {Animation="Exhausted", Speaker = "FlameShadow4"},
        new ("IgnisVolcano_FlameShadow4_10") {Animation="Grateful", Speaker = "Player"},
        new ("IgnisVolcano_FlameShadow4_20") {Animation="IWouldHurryUpIfIWereYou", Speaker = "FlameShadow4"},
        new ("IgnisVolcano_FlameShadow4_30") {Animation="Surprised", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(26.5f, 1.5f), PlayerStartingFlipped = false, SpeakerStartingFlipped=true, SpeakerStartingPosition=new(29, 1.5f), DialogueSpeaker=Utils.GetUnit("FlameShadow4"), ReturnUnitsToOriginalPositions=false};}

    public static void OnEnd_FlameShadow4() {
        Area.Instance.transform.Find("Environment/River_3").gameObject.SetActive(false);
        Area.Instance.transform.Find("Environment/RiverGraphic_3").gameObject.SetActive(false);
        Area.Instance.transform.Find("NPCs/Unit_FlameShadow4").gameObject.SetActive(false);
        Area.Instance.transform.Find("NPCs/Unit_Blaine3").gameObject.SetActive(true);
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(140, new() {SaveFile.Instance.IgnisEnergy.ToString()});
    }

    public static Dialogue Blaine2() {
        return new Dialogue ("Area_IgnisVolcano", "Blaine2", new List<DialogueLine>{
        new ("IgnisVolcano_Blaine2_0") {Animation="Exhausted", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_10") {Animation="Doubtful", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_20") {Animation="Realization", Speaker = "Blaine3", IdOfNextDialogueLine="IgnisVolcano_Blaine2_50"},
        new ("IgnisVolcano_Blaine2_50") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_60") {Animation="Accusing", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_70") {Animation="YesMe", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_80") {Animation="Thinking", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_90") {Animation="Listening", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_100") {Animation="Determined", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_110") {Animation="Irritated", Speaker = "Colten1"},
        new ("IgnisVolcano_Blaine2_120") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_130") {Animation="Grateful", Speaker = "Colten1"},
        new ("IgnisVolcano_Blaine2_140") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_150") {Animation="NotQuite", Speaker = "Colten2"},
        new ("IgnisVolcano_Blaine2_160") {Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_170") {Animation="SternNo", Speaker = "Colten2"},
        new ("IgnisVolcano_Blaine2_180") {Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_190") {Animation="PickUpItemFromEyeLevel", Speaker = "Colten2"},
        new ("IgnisVolcano_Blaine2_200") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_210") {Animation="KnockedOut", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_220") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_230") {Animation="FinallyDecided", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(30f, 43f), PlayerStartingFlipped = true, SpeakerStartingFlipped=false, SpeakerStartingPosition=new(27f, 43f), DialogueSpeaker=Utils.GetUnit("Blaine3")};}

    public static void IgnisVolcano_Blaine2_110() {
        Utils.GetUnit("Colten1").gameObject.SetActive(true);
    }

    public static void IgnisVolcano_Blaine2_130() {
        Utils.GetUnit("Colten2").gameObject.SetActive(true);
        GameController.Instance.WaitAndRunMethod(1f, BackstabBlaine);
    }

    public static void IgnisVolcano_Blaine2_140() {
        Player.Instance.Actions.IsFlipped = true;
    }

    public static void IgnisVolcano_Blaine2_190() {
        Utils.GetUnit("Blaine3").SpriteRenderers["Heavy"].SpriteRenderer.enabled = false;
    }

    public static void IgnisVolcano_Blaine2_200() {
        Utils.GetUnit("Colten2").PlayAnimation("Disappear");
    }

    public static void OnEnd_Blaine2() {
        SaveFile.Instance.IncreaseEnergyLevel(Ability.AbilityFamily.Ignis);
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(150);
        SaveFile.Instance.CurrentMission.Finish(true);
    }

    public static void BackstabBlaine() {
        Utils.GetUnit("Colten1").gameObject.SetActive(false);
        Utils.GetUnit("Colten2").PlayAnimation("BlackflameBackstab", 0, 0.33f);
        Utils.GetUnit("Colten2").transform.position = new Vector2(Utils.GetUnit("Blaine3").transform.position.x - 1, Utils.GetUnit("Blaine3").transform.position.y);
        Utils.CreateVisualEffect(new(Utils.GetUnit("Colten2")), "BlackflameBackstab", Utils.GetUnit("Colten2").transform.position.x, Utils.GetUnit("Colten2").transform.position.y - 0.5f);
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Colten2").gameObject;
        Utils.GetUnit("Blaine3").PlayAnimation("Backstabbed", 0.1f, 0.1f);
        GameController.Instance.WaitAndRunMethod(1.5f, BlaineWounded);
    }

    public static void BlaineWounded() {
        Utils.GetUnit("Blaine3").PlayAnimation("HeavilyWounded");
    }

    public static Dialogue FinalBlaine() {
        return new Dialogue ("Area_IgnisVolcano", "FinalBlaine", new List<DialogueLine>{
        new ("IgnisVolcano_Blaine2_0") {Animation="Exhausted", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_10") {Animation="Doubtful", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_20") {Animation="Realization", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_30") {Animation="Irritated", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_40") {Animation="FinallyDecided", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_50") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_60") {Animation="Accusing", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_70") {Animation="YesMe", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_80") {Animation="Thinking", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_90") {Animation="Listening", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_100") {Animation="Determined", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_110") {Animation="Irritated", Speaker = "Colten1"},
        new ("IgnisVolcano_Blaine2_120") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_130") {Animation="Grateful", Speaker = "Colten1"},
        new ("IgnisVolcano_Blaine2_140") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_150") {Animation="NotQuite", Speaker = "Colten2"},
        new ("IgnisVolcano_Blaine2_160") {Animation="WobblyGetUp", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_170") {Animation="SternNo", Speaker = "Colten2"},
        new ("IgnisVolcano_Blaine2_180") {Animation="Exhausted", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_190") {Animation="PickUpItemFromEyeLevel", Speaker = "Colten2"},
        new ("IgnisVolcano_Blaine2_200") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_210") {Animation="KnockedOut", Speaker = "Blaine3"},
        new ("IgnisVolcano_Blaine2_220") {Animation="ComposeOneself", Speaker = "Player"},
        new ("IgnisVolcano_Blaine2_230") {Animation="Disappear", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(27f, 43f), PlayerStartingFlipped = false, SpeakerStartingFlipped=true, SpeakerStartingPosition=new(30f, 43f), DialogueSpeaker=Utils.GetUnit("Blaine3")};}

    public static void IgnisVolcano_Blaine2_210() {
        Area.Instance.transform.Find("Environment/VisualEffect_RagingInferno").gameObject.SetActive(true);
    }

    public static Dialogue MausoleumRetribution() {
        return new Dialogue ("Area_IgnisVolcano", "MausoleumRetribution", new List<DialogueLine>{
        new ("IgnisVolcano_Mausoleum_0"),
        new ("IgnisVolcano_Mausoleum_Choices") {Choices= new List<DialogueChoice> {
            new ("IgnisVolcano_Mausoleum_10") {IdOfNextDialogueLine="IgnisVolcano_Mausoleum_40"},
            new ("Leave") {IdOfNextDialogueLine="END"},
        }},
        new ("IgnisVolcano_Mausoleum_40") {IdOfNextDialogueLine="END"},
    });}

    public static void OnEnd_IgnisVolcano_Mausoleum_40() {
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum/Dialogue").gameObject.SetActive(false);
        Utils.GetUnit("RetributionKnight1").gameObject.SetActive(true);
        Utils.GetUnit("RetributionKnight1").PlayAnimation("WobblyGetUp");
        GameController.Instance.WaitAndRunMethod(2, RetributionAttack);
        SaveFile.Instance.AddFlag("IgnisVolcano_AwakenedRetributionKnight");
    }

    public static void RetributionAttack() {
        Utils.GetUnit("RetributionKnight1").ChangeFaction(Constants.Faction.HostileToAll);
        Utils.GetUnit("RetributionKnight1").CurrentTarget = Utils.GetUnit("RetributionKnight1").GetClosestValidTarget();
    }

    public static Dialogue MausoleumHeavenlyHalberd() {
        return new Dialogue ("Area_IgnisVolcano", "MausoleumHeavenlyHalberd", new List<DialogueLine>{
        new ("IgnisVolcano_Mausoleum_0"),
            new ("IgnisVolcano_Mausoleum_Choices") {Choices= new List<DialogueChoice> {
            new ("IgnisVolcano_Mausoleum_20") {IdOfNextDialogueLine="IgnisVolcano_Mausoleum_50"},
            new ("Leave") {IdOfNextDialogueLine="END"},
        }},
        new ("IgnisVolcano_Mausoleum_50") {IdOfNextDialogueLine="END"},
    });}

    public static void OnEnd_IgnisVolcano_Mausoleum_50() {
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_2/Dialogue").gameObject.SetActive(false);
        Utils.GetUnit("HeavenlyHalberdKnight1").gameObject.SetActive(true);
        Utils.GetUnit("HeavenlyHalberdKnight1").PlayAnimation("WobblyGetUp");
        GameController.Instance.WaitAndRunMethod(2, HeavenlyHalberdAttack);
        SaveFile.Instance.AddFlag("IgnisVolcano_AwakenedHeavenlyHalberdKnight");
    }

    public static void HeavenlyHalberdAttack() {
        Utils.GetUnit("HeavenlyHalberdKnight1").ChangeFaction(Constants.Faction.HostileToAll);
        Utils.GetUnit("HeavenlyHalberdKnight1").CurrentTarget = Utils.GetUnit("HeavenlyHalberdKnight1").GetClosestValidTarget();
    }

    public static Dialogue MausoleumSeverance() {
        return new Dialogue ("Area_IgnisVolcano", "MausoleumSeverance", new List<DialogueLine>{
        new ("IgnisVolcano_Mausoleum_0"),
            new ("IgnisVolcano_Mausoleum_Choices") {Choices= new List<DialogueChoice> {
            new ("IgnisVolcano_Mausoleum_30") {IdOfNextDialogueLine="IgnisVolcano_Mausoleum_60"},
            new ("Leave") {IdOfNextDialogueLine="END"},
        }},
        new ("IgnisVolcano_Mausoleum_60") {IdOfNextDialogueLine="END"},
    });}

    public static void OnEnd_IgnisVolcano_Mausoleum_60() {
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_3/Dialogue").gameObject.SetActive(false);
        Utils.GetUnit("SeveranceKnight1").gameObject.SetActive(true);
        Utils.GetUnit("SeveranceKnight1").PlayAnimation("WobblyGetUp");
        GameController.Instance.WaitAndRunMethod(2, SeveranceAttack);
        SaveFile.Instance.AddFlag("IgnisVolcano_AwakenedSeveranceKnight");
    }

    public static void SeveranceAttack() {
        Utils.GetUnit("SeveranceKnight1").ChangeFaction(Constants.Faction.HostileToAll);
        Utils.GetUnit("SeveranceKnight1").CurrentTarget = Utils.GetUnit("SeveranceKnight1").GetClosestValidTarget();
    }

    public static void DestroyRetribution() {
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum/Item").gameObject.SetActive(false);
    }

    public static void DestroyHeavenlyHalberd() {
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_2/Item").gameObject.SetActive(false);
    }

    public static void DestroySeverance() {
        Area.Instance.transform.Find("Interactables/Weapon Mausoleum_3/Item").gameObject.SetActive(false);
    }

    public static Dialogue FirstIgnisLeader() {
        return new Dialogue ("Area_IgnisVolcano", "FirstIgnisLeader", new List<DialogueLine>{
        new ("IgnisVolcano_SentientArmor_0") {Animation="Intrigued", Speaker = "Player", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmor_10") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmor_20") {Animation="ShoulderShrug", Speaker = "Player", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmor_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("IgnisVolcano_SentientArmorWhyStare_0"),
            new ("IgnisVolcano_SentientArmorWhyExist_0"),
            new ("IgnisVolcano_SentientArmorIdentity_0") {HideChoiceTextIfDisabled=false},
            new ("Leave") {IdOfNextDialogueLine="END"},
        }},
        new ("IgnisVolcano_SentientArmorWhyStare_10") {Animation="Talking", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyStare_20") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyStare_30") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyStare_40") { Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyStare_50") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyStare_60") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyStare_70") {Animation="Realization", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyStare_80") {Animation="SittingAndPointingUp", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyStare_90") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", IdOfNextDialogueLine="IgnisVolcano_SentientArmor_Choices", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyExist_10") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyExist_20") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyExist_30") {Animation="Intrigued", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyExist_40") {Animation="SittingAndPointingUp", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyExist_50") {Animation="Listening", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyExist_60") {Animation="SittingAndPointingUp", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyExist_70") {Animation="Thinking", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyExist_80") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyExist_90") {Animation="ComposeOneself", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyExist_100") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyExist_110") {Animation="Realization", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorWhyExist_120") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorWhyExist_130") {Animation="Thinking", Speaker = "Player", IdOfNextDialogueLine="IgnisVolcano_SentientArmor_Choices"},
        new ("IgnisVolcano_SentientArmorIdentity_10") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_20") {Speaker = "SentientArmor", ShowSpeakerBox=false, TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_30") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_40") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_50") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_60") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_70") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_80") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_90") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_100") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_110") {Animation="Thinking", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_120") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_130") {Animation="Determined", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_140") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_150") {Animation="Approving", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_160") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_170") {Animation="Listening", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_180") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_190") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_200") {Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_210") {Animation="Realization", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_220") {Animation="SittingAndDisappointed", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_230") {Animation="HaveItYourWayThen", Speaker = "Player"},
        new ("IgnisVolcano_SentientArmorIdentity_240") {Animation="SittingAndPointingUp", Speaker = "SentientArmor", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_SentientArmorIdentity_250") {Speaker = "SentientArmor", IdOfNextDialogueLine="END", TurnSpeakersToFaceEachOther=false}
    }){PlayerStartingPosition = new Vector2(36f, 11f), PlayerStartingFlipped = false, SpeakerStartingFlipped=false, SpeakerStartingPosition=new(38, 12.5f), DialogueSpeaker=Utils.GetUnit("SentientArmor1")};}

    public static void IgnisVolcano_SentientArmorWhyStare_10() {
        SaveFile.Instance.AddFlag("IgnisVolcano_CanAskSentientArmorAboutMoon");
    }

    public static void IgnisVolcano_SentientArmorWhyExist_10() {
        SaveFile.Instance.AddFlag("IgnisVolcano_CanAskSentientArmorAboutIdentity");
    }

    public static bool CheckIfVisible_IgnisVolcano_SentientArmorWhyExist_0() {
        return SaveFile.Instance.HasFlag("IgnisVolcano_CanAskSentientArmorAboutMoon");
    }

    public static bool CheckIfVisible_IgnisVolcano_SentientArmorIdentity_0() {
        return SaveFile.Instance.HasFlag("IgnisVolcano_CanAskSentientArmorAboutIdentity");
    }

    public static bool CheckIfEnabled_IgnisVolcano_SentientArmorIdentity_0() {
        return SaveFile.Instance.IgnisEnergy >= 300 || (SaveFile.Instance.HasFlag("IgnisManor_MuseumWeapon") && SaveFile.Instance.HasFlag("IgnisManor_MuseumArmour"));
    }

    public static void IgnisVolcano_SentientArmorWhyStare_90() {
        if(SaveFile.Instance.HasFlag("IgnisVolcano_SentientArmorWhyStare") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        SaveFile.Instance.AddFlag("IgnisVolcano_SentientArmorWhyStare");
    }

    public static void IgnisVolcano_SentientArmorWhyExist_130() {
        if(SaveFile.Instance.HasFlag("IgnisVolcano_SentientArmorWhyExist") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        SaveFile.Instance.AddFlag("IgnisVolcano_SentientArmorWhyExist");
    }
    public static void IgnisVolcano_SentientArmorIdentity_250() {
        if(SaveFile.Instance.HasFlag("IgnisVolcano_SentientArmorIdentity") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        SaveFile.Instance.AddFlag("IgnisVolcano_SentientArmorIdentity");
    }

    public static Dialogue FinalVolcano() {
        return new Dialogue ("Area_IgnisVolcano", "FinalVolcano", new List<DialogueLine>{
        new ("IgnisVolcano_FinalVolcano_0") {Animation="StopRunning", Speaker = "Player"},
        new ("IgnisVolcano_FinalVolcano_10") {WaitTimeBeforeAllowingToProceed=2},
        new ("IgnisVolcano_FinalVolcano_20") {Animation="PickUpItemFromEyeLevel", Speaker = "Colten", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_FinalVolcano_30") {Animation="Accusing", Speaker = "Player"},
        new ("IgnisVolcano_FinalVolcano_40") {Animation="YesYou", Speaker = "Colten", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_FinalVolcano_50") {Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_60") {Animation="ShoulderShrug", Speaker = "Colten", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_FinalVolcano_70") {Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_80") {Animation="Mocking", Speaker = "Colten", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_FinalVolcano_90") {Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_100") {Animation="SternNo", Speaker = "Colten", TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_FinalVolcano_110") {Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_120") {Animation="Accusing", Speaker = "Colten", TurnSpeakersToFaceEachOther=false, WaitTimeBeforeAllowingToProceed=3},
        new ("IgnisVolcano_FinalVolcano_130") {Animation="PickUpItemFromTheGround", Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_140") {Animation="Surprised", Speaker = "Player"},
        new ("IgnisVolcano_FinalVolcano_150") {Animation="SwingingWeapon", Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_180") {Animation="StopRunning", Speaker = "Player", WaitTimeBeforeAllowingToProceed=1.5f},
        new ("IgnisVolcano_FinalVolcano_190") {Animation="Determined", Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_200") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisVolcano_FinalVolcano_210") {Animation="ThreatenWithHeavy", Speaker = "Blaine"},
        new ("IgnisVolcano_FinalVolcano_220") {Animation="FinallyDecided", Speaker = "Player"},
        new ("IgnisVolcano_FinalVolcano_230") {Animation="YouFinallyGetIt", Speaker = "Blaine"}
    }){PlayerStartingPosition = new Vector2(23f, 37f), PlayerStartingFlipped = false, SpeakerStartingFlipped=true, SpeakerStartingPosition=new(26f, 43f), DialogueSpeaker=Utils.GetUnit("Blaine"), SpeakerEndingPosition=new Vector2(31f, 43), PlayerEndingPosition=new Vector2(27.5f, 43), PlayerEndingFlipped = false, SpeakerEndingFlipped = true, AutoSaveOnDialogueEnd=false};}

    public static void IgnisVolcano_FinalVolcano_0() {
        CameraController.Instance.CenteredOnObject = Player.Instance.gameObject;
        Utils.SetDefaultMusic("IgnisDuel");
    }

    public static void IgnisVolcano_FinalVolcano_10() {
        Utils.GetUnit("Colten").gameObject.SetActive(true);
        Utils.GetUnit("Colten").PlayAnimation("BlackflameBackstab", 0, 0.33f);
        Utils.CreateVisualEffect(new(Utils.GetUnit("Colten")), "BlackflameBackstab", Utils.GetUnit("Colten").transform.position.x, Utils.GetUnit("Colten").transform.position.y - 0.5f);
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Colten").gameObject;
        Utils.GetUnit("Blaine").PlayAnimation("Backstabbed", 0.1f, 0.1f);
        Utils.GetUnit("SentientArmor").gameObject.SetActive(true);
        Utils.GetUnit("SentientArmor").PlayAnimation("AppearAndDefend", 0, 0);
        GameController.Instance.WaitAndRunMethod(1.5f, BlaineWounded2);
        GameController.Instance.WaitAndRunMethod(0.5f, MakeBackstabEffect);
    }

    public static void MakeBackstabEffect() {
        Utils.CreateVisualEffect(new(Utils.GetUnit("Colten")), "Counter", Utils.GetUnit("Colten").transform.position.x + 0.5f, Utils.GetUnit("Colten").transform.position.y);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Ability/Ability_CounterForce_Hit", 1);
    }

    public static void BlaineWounded2() {
        Utils.GetUnit("Blaine").PlayAnimation("HeavilyWounded");
        Utils.GetUnit("Blaine").DefaultAnimation = Utils.GetAnimationClip(Utils.GetUnit("Blaine").Animator, "HeavilyWounded");
        Utils.GetUnit("SentientArmor").Animator.enabled = false;
        foreach(ParticleSystem ps in Utils.GetUnit("SentientArmor").GetComponentsInChildren<ParticleSystem>()) {
            ps.Stop();
        }
        Utils.GetUnit("SentientArmor").Rigidbody2D.bodyType = RigidbodyType2D.Static;
        Damage.DeactivateUnit(Utils.GetUnit("SentientArmor"));
    }

    public static void IgnisVolcano_FinalVolcano_20() {
        Utils.CopyGameObjectAppearance(Utils.GetUnit("Colten").SpriteRenderers["Heavy"].SpriteRenderer.gameObject, Utils.GetUnit("Blaine").SpriteRenderers["Heavy"].SpriteRenderer.gameObject, false);
        Utils.GetUnit("Blaine").SpriteRenderers["Heavy"].SpriteRenderer.enabled = false;
        Area.Instance.transform.Find("FakePlunderer").gameObject.SetActive(true);
    }

    public static void IgnisVolcano_FinalVolcano_120() {
        Area.Instance.transform.Find("Environment/VisualEffect_RagingInferno").gameObject.SetActive(true);
        GameController.Instance.WaitAndRunMethod(3f, IgnisVolcano_FinalVolcano_120v2);
    }

    public static void IgnisVolcano_FinalVolcano_120v2() {
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Blaine").gameObject;
        Utils.GetUnit("Colten").PlayAnimation("Disappear");
    }

    public static void IgnisVolcano_FinalVolcano_130() {
        Utils.GetUnit("Blaine").SpriteRenderers["Heavy"].SpriteRenderer.enabled = true;
        Utils.CopyGameObjectAppearance(Utils.GetUnit("Blaine").SpriteRenderers["Heavy"].SpriteRenderer.gameObject, Area.Instance.transform.Find("FakePlunderer").gameObject);
    }

    public static void IgnisVolcano_FinalVolcano_150() {
        GameController.Instance.WaitAndRunMethod(3f, IgnisVolcano_FinalVolcano_150v2);
    }
    public static void IgnisVolcano_FinalVolcano_150v2() {
        Utils.GetUnit("Blaine").PlayAnimation("HeavilyWounded");
    }
    public static void IgnisVolcano_FinalVolcano_180() {
        GameController.Instance.WaitAndRunMethod(1.0f, IgnisVolcano_FinalVolcano_180v2);
    }

    public static void IgnisVolcano_FinalVolcano_180v2() {
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Blaine").gameObject;
        Utils.GetUnit("Blaine").PlayAnimation("TryToStop");
    }

    public static void OnEnd_FinalVolcano() {
        SaveFile.Instance.GetQuest("Ignis").AdvanceObjective(200, new() {SaveFile.Instance.IgnisEnergy.ToString()});
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_BlaineMidFightQuote_0", SpeakerUnit=Utils.GetUnit("Blaine")});
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Area_IgnisVolcano.PrepareDuel"});
        PrepareDuel();
        GameController.Instance.WaitAndRunMethod(2.0f, MakeBlaineAttackAnyway);
    }

    public static void MakeBlaineAttackAnyway() {
        Utils.GetUnit("Blaine").AttackPlayer();
        Utils.GetUnit("Blaine").Actions.UseAbility(typeof(NPCAbility_TripleHeavySlash));
    }

    public static void BuriedEnergy() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_Interactions_90"});
        NotificationController.ShowTextNotification("IgnisVolcano_Interactions_91");
        SaveFile.Instance.AddPermanentPowerUp("IgnisVolcano_BuriedEnergy");
        SaveFile.Instance.ChangeIgnisEnergy(10);
    }

    public static void FireRiver() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_Interactions_100"});
        NotificationController.ShowTextNotification("IgnisVolcano_Interactions_101");
        SaveFile.Instance.AddPermanentPowerUp("IgnisVolcano_FireRiver");
        SaveFile.Instance.ChangeIgnisEnergy(10);
    }

    public static void PrepareDuel() {
        Utils.SetDefaultMusic("IgnisDuel");
        Unit blaine = Utils.GetUnit("Blaine");
        blaine.DefaultAnimation = null;
        Player.Instance.transform.position = new Vector2(27.5f, 43);
        Player.Instance.Actions.IsFlipped = false;
        blaine.transform.position = new Vector2(31f, 43);
        blaine.Actions.IsFlipped = true;
        Area.Instance.transform.Find("Environment/VisualEffect_RagingInferno").gameObject.SetActive(true);
        Utils.GetUnit("FlameShadow").gameObject.SetActive(true);
        Effect_ChangeStat e1 = new Effect_ChangeStat(blaine.Health, new(blaine)) {RegenerationFlatAmount = -25, DisplayEffectIndicator = true, PathToEffectGraphic = "Effect/Bleed", IsRemovable = false};
        Effect_ChangeStat e2 = new Effect_ChangeStat(blaine.StaggerBar, new(blaine)) {RegenerationFlatAmount = -25, DisplayEffectIndicator = true, PathToEffectGraphic = "Effect/Burn", IsRemovable = false};
        blaine.AddEffect(e1);
        blaine.AddEffect(e2);
        blaine.AttackPlayer();
        blaine.Actions.UseAbility(typeof(NPCAbility_BlastDash));
        blaine.AddEffect(new Effect_CannotBeDefeated(true, new(blaine)) {IsRemovable=false});
        if(Area.Instance.transform.Find("FakePlunderer") != null) {
            Area.Instance.transform.Find("FakePlunderer").gameObject.SetActive(true);
            Utils.CopyGameObjectAppearance(blaine.SpriteRenderers["Heavy"].SpriteRenderer.gameObject, Area.Instance.transform.Find("FakePlunderer").gameObject);
        }
        EventManager.UnitWouldBeDefeated.AddListener(CheckIfBlaineDefeated);
        EventManager.UnitHealthChanged.AddListener(CheckIfBlaineQuote);
    }

    private static int _blaineQuoteCounter = 0;

    public static void CheckIfBlaineQuote(Unit unit) {
        if(unit.gameObject.name.Contains("Blaine") == false) {
            return;
        }
        if(_blaineQuoteCounter == 0 && unit.Health.Current < unit.Health.Maximum * 0.6f) {
            _blaineQuoteCounter++;
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_BlaineMidFightQuote_10", SpeakerUnit=Utils.GetUnit("Blaine")});
        }
        else if(_blaineQuoteCounter == 1 && unit.CurrentHealthBars == 1) {
            _blaineQuoteCounter++;
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_BlaineMidFightQuote_20", SpeakerUnit=Utils.GetUnit("Blaine")});
        }
        else if(_blaineQuoteCounter == 2 && unit.CurrentHealthBars == 1 && unit.Health.Current < unit.Health.Maximum * 0.6f) {
            _blaineQuoteCounter++;
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="IgnisVolcano_BlaineMidFightQuote_30", SpeakerUnit=Utils.GetUnit("Blaine")});
        }
    }

    public static void CheckIfBlaineDefeated(Damage damage) {
        if(damage.TargetOfDamage.gameObject.name.Contains("Blaine")) {
            Utils.SetDefaultMusic("Sadness_45");
            UIManager.Instance.StartDialogue(FarewellToBlaine());
        }
    }

    public static Dialogue FarewellToBlaine() {
        return new Dialogue ("Area_IgnisVolcano", "FarewellToBlaine", new List<DialogueLine>{
        new ("IgnisVolcano_FarewellToBlaine_0") {Animation="BroughtToKnees", Speaker = "Blaine"},
        new ("IgnisVolcano_FarewellToBlaine_10") {Animation="YesYou", Speaker = "Player"},
        new ("IgnisVolcano_FarewellToBlaine_20") {Animation="BroughtToKnees", Speaker = "Blaine"},
        new ("IgnisVolcano_FarewellToBlaine_30") {Animation="Frustrated", Speaker = "Player"},
        new ("IgnisVolcano_FarewellToBlaine_40") {Animation="BroughtToKnees", Speaker = "Blaine"},
        new ("IgnisVolcano_FarewellToBlaine_41") {Animation="Sigh", Speaker = "Player"},
        new ("IgnisVolcano_FarewellToBlaine_42") {Animation="BroughtToKnees", Speaker = "Blaine"},
        new ("IgnisVolcano_FarewellToBlaine_43") {Speaker = "Player"},
        new ("IgnisVolcano_FarewellToBlaine_44") {Animation="KnockedOut", Speaker = "Blaine", IdOfNextDialogueLine=SaveFile.Instance.HasFlag("IgnisVolcano_SentientArmorIdentity") ? "IgnisVolcano_FarewellToBlaine_50" : "IgnisVolcano_FarewellToBlaine_51", WaitTimeBeforeAllowingToProceed=2},
        new ("IgnisVolcano_FarewellToBlaine_50") {Animation="FinallyDecided", Speaker = "Player", IdOfNextDialogueLine="IgnisVolcano_FarewellToBlaine_60"},
        new ("IgnisVolcano_FarewellToBlaine_51") {Animation="FinallyDecided", Speaker = "Player", IdOfNextDialogueLine="IgnisVolcano_FarewellToBlaine_60"},
        new ("IgnisVolcano_FarewellToBlaine_60") {Speaker = "FlameShadow", WaitTimeBeforeAllowingToProceed=1, TurnSpeakersToFaceEachOther=false},
        new ("IgnisVolcano_FarewellToBlaine_70") {Animation="Disappear", Speaker = "Player", WaitTimeBeforeAllowingToProceed=2},
        new ("IgnisVolcano_FarewellToBlaine_80") {Animation="Appear", Speaker = "Player", WaitTimeBeforeAllowingToProceed=3},
        new ("IgnisVolcano_FarewellToBlaine_90") {Animation="Relief", Speaker = "FlameShadow"},
        new ("IgnisVolcano_FarewellToBlaine_100") {Animation="Stoic", Speaker = "Player"},
        new ("IgnisVolcano_FarewellToBlaine_110") {Animation="ShoulderShrug", Speaker = "FlameShadow", WaitTimeBeforeAllowingToProceed=2},
        new ("IgnisVolcano_FarewellToBlaine_120") {Animation="Determined", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(27.5f, 44f), PlayerStartingFlipped = false, SpeakerStartingFlipped=true, SpeakerStartingPosition=new(31f, 44f), DialogueSpeaker=Utils.GetUnit("Blaine")};}

    public static void IgnisVolcano_FarewellToBlaine_44() {
        Utils.GetUnit("Blaine").PlayAnimation("KnockedOut");
        GameController.Instance.WaitAndRunMethod(1.5f, IgnisVolcano_FarewellToBlaine_44v2);
    }

    public static void IgnisVolcano_FarewellToBlaine_44v2() {
        Utils.GetUnit("Blaine").Animator.enabled = false;
    }

    public static void IgnisVolcano_FarewellToBlaine_60() {
        GameController.Instance.WaitAndRunMethod(0.5f, IgnisVolcano_FarewellToBlaine_60v2);
        UIManager.Instance.ShowBlackScreen();
    }

    public static void IgnisVolcano_FarewellToBlaine_60v2() {
        UIManager.Instance.HideBlackScreen();
        Player.Instance.transform.position = new Vector2(15, 50);
        Player.Instance.Actions.IsFlipped = true;
        Player.Instance.PlayAnimation("HereWeGo");
    }

    public static void IgnisVolcano_FarewellToBlaine_70() {
        Utils.GetUnit("FlameShadow").DefaultAnimation = null;
        Utils.GetUnit("FlameShadow").PlayAnimation("Idle");
        UIManager.Instance.ShowBlackScreen(1f);
    }

    public static void IgnisVolcano_FarewellToBlaine_80() {
        UIManager.Instance.HideBlackScreen(2.5f);
        Player.Instance.Actions.IsFlipped = false;
        Player.Instance.SpriteRenderers["Heavy"].SpriteRenderer.enabled = false;
        Utils.GetUnit("FlameShadow").transform.position = new Vector2(4, 60f);
        Utils.GetUnit("FlameShadow").Actions.IsFlipped = true;
        Player.Instance.transform.position = new Vector2(-1, 58.5f);
        GameController.Instance.WaitAndRunMethod(1.5f, IgnisVolcano_FarewellToBlaine_80v2);
    }

    public static void IgnisVolcano_FarewellToBlaine_80v2() {
        Player.Instance.PlayAnimation("GraspItemAtEyeLevel");
        GameController.Instance.WaitAndRunMethod(1.5f, IgnisVolcano_FarewellToBlaine_80v3);
    }

    public static void IgnisVolcano_FarewellToBlaine_80v3() {
        Player.Instance.SpriteRenderers["Heavy"].SpriteRenderer.enabled = true;
        Utils.CopyGameObjectAppearance(Player.Instance.SpriteRenderers["Heavy"].SpriteRenderer.gameObject, Area.Instance.transform.Find("Environment/DragonsMaw").gameObject);
        Player.Instance.PlayAnimation("PullOutItemAtEyeLevel");
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Greatsword/Unsheathe");
        SaveFile.Instance.AddItem(new Greatsword_DragonsMaw(Item.ItemGrade.Flawless));
    }

    public static void IgnisVolcano_FarewellToBlaine_110() {
        GameController.Instance.WaitAndRunMethod(1.5f, IgnisVolcano_FarewellToBlaine_110v2);
    }

    public static void IgnisVolcano_FarewellToBlaine_110v2() {
        Utils.CreateVisualEffect(new(Utils.GetUnit("FlameShadow")), "FlameShadowDisappear", Utils.GetUnit("FlameShadow").transform.position.x, Utils.GetUnit("FlameShadow").transform.position.y);
        Utils.GetUnit("FlameShadow").gameObject.SetActive(false);
    }

    public static void OnEnd_FarewellToBlaine() {
        Utils.MoveIntoArea(true, "IgnisManorOnFire");
    }

    private static int _volcanoStashCounter = 0;
    public static void VolcanoStash() {
        _volcanoStashCounter++;
        if(_volcanoStashCounter >= 3) {
            Area.Instance.transform.Find("Interactables/VolcanoStash").gameObject.SetActive(true);
        }
    }
}
