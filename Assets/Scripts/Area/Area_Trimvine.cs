using System.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Area_Trimvine
{

    public static void OnStart() {
        if(SaveFile.Instance.Week >= 40 && !SaveFile.Instance.Flags.Contains(Utils.GetFormattedFlag("Trimvine_TimelyHelpHelpedInTime_[Cycle]"))) {
            Area.Instance.transform.Find("Interactables/TimelyHelp_TooLate").gameObject.SetActive(true);
            Utils.GetUnit("TimelyHelpNPC").PlayAnimation("SittingInDespair");
            Area.Instance.transform.Find("Interactables/Treasure Tree")?.gameObject.SetActive(false);
        }
    }

    public static Dialogue LauraDialogue() {
        return new Dialogue ("Area_Trimvine", "LauraDialogue", new List<DialogueLine>{
        new ("LauraDialogue_Dialogue_0") {Speaker = "Laura"},
        new ("LauraDialogue_Dialogue_10") {Speaker = "Player",
        Choices= new List<DialogueChoice> {
            new ("LauraDialogue_LeoCounters_0"),
            new ("LauraDialogue_Study_0"),
            new ("LauraDialogue_Shop_0"),
            new ("End") {IdOfNextDialogueLine = "END"}
        }},
        new ("LauraDialogue_Shop_10") {Speaker = "Player" },
        new ("LauraDialogue_Shop_20") {Speaker = "Laura", IdOfNextDialogueLine="END"},
        new ("LauraDialogue_LeoCounters_10") {Speaker = "Laura", IdOfNextDialogueLine="LauraDialogue_LeoCounters_20"},
        new ("LauraDialogue_LeoCounters_20") {Speaker = "Player",
        Choices= new List<DialogueChoice> {
            new ("LauraDialogue_LeoIgnis_0"),
            new ("LauraDialogue_LeoAnima_0"),
            new ("LauraDialogue_LeoGlacies_0"),
            new ("LauraDialogue_LeoMolis_0"),
            new ("LauraDialogue_LeoSalutis_0"),
            new ("LauraDialogue_LeoTonitrui_0"),
            new ("LauraDialogue_LeoProprius_0"),
            new ("Back") {IdOfNextDialogueLine = "LauraDialogue_Dialogue_10"},
        }},
        new ("LauraDialogue_LeoIgnis_10") {Speaker = "Laura", Animation="Talking"},
        new ("LauraDialogue_LeoIgnis_20") {Speaker = "Player", Animation="Deflated"},
        new ("LauraDialogue_LeoIgnis_30") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_LeoIgnis_40") {Speaker = "Player"},
        new ("LauraDialogue_LeoIgnis_50") {Speaker = "Laura", Animation="Mocking"},
        new ("LauraDialogue_LeoIgnis_60") {Speaker = "Player", Animation="Realization"},
        new ("LauraDialogue_LeoIgnis_70") {Speaker = "Laura", Animation="HandWave"},
        new ("LauraDialogue_LeoIgnis_80") {Speaker = "Player", Animation="HereWeGo", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},

        new ("LauraDialogue_LeoAnima_10") {Speaker = "Laura", Animation="Explaining"},
        new ("LauraDialogue_LeoAnima_20") {Speaker = "Laura"},
        new ("LauraDialogue_LeoAnima_30") {Speaker = "Player", Animation="Doubtful"},
        new ("LauraDialogue_LeoAnima_40") {Speaker = "Laura", Animation="Thinking"},
        new ("LauraDialogue_LeoAnima_50") {Speaker = "Player", Animation="Surprised"},
        new ("LauraDialogue_LeoAnima_60") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_LeoAnima_70") {Speaker = "Player", Animation="Sigh", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},

        new ("LauraDialogue_LeoGlacies_10") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_LeoGlacies_20") {Speaker = "Player", Animation="Explaining"},
        new ("LauraDialogue_LeoGlacies_30") {Speaker = "Laura", Animation="SternNo"},
        new ("LauraDialogue_LeoGlacies_40") {Speaker = "Player", Animation="Sigh"},
        new ("LauraDialogue_LeoGlacies_50") {Speaker = "Laura", Animation="IWouldHurryUpIfIWereYou"},
        new ("LauraDialogue_LeoGlacies_60") {Speaker = "Player", Animation="Surprised"},
        new ("LauraDialogue_LeoGlacies_70") {Speaker = "Laura", Animation="Laugh"},
        new ("LauraDialogue_LeoGlacies_80") {Speaker = "Player", Animation="Doubtful"},
        new ("LauraDialogue_LeoGlacies_90") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_LeoGlacies_100") {Speaker = "Player", Animation="Frustrated"},
        new ("LauraDialogue_LeoGlacies_110") {Speaker = "Laura"},
        new ("LauraDialogue_LeoGlacies_120") {Speaker = "Player", Animation="Determined", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},

        new ("LauraDialogue_LeoMolis_10") {Speaker = "Laura", Animation="Explaining"},
        new ("LauraDialogue_LeoMolis_20") {Speaker = "Player", Animation="Thinking"},
        new ("LauraDialogue_LeoMolis_30") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_LeoMolis_40") {Speaker = "Player", Animation="Realization"},
        new ("LauraDialogue_LeoMolis_50") {Speaker = "Laura", Animation="YouFinallyGetIt"},
        new ("LauraDialogue_LeoMolis_60") {Speaker = "Player", Animation="Sigh"},
        new ("LauraDialogue_LeoMolis_70") {Speaker = "Laura", Animation="NotQuite"},
        new ("LauraDialogue_LeoMolis_80") {Speaker = "Player", Animation="Determined", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},

        new ("LauraDialogue_LeoSalutis_10") {Speaker = "Laura", Animation="Thinking"},
        new ("LauraDialogue_LeoSalutis_20") {Speaker = "Player", Animation="SternNo"},
        new ("LauraDialogue_LeoSalutis_30") {Speaker = "Laura", Animation="YesYou"},
        new ("LauraDialogue_LeoSalutis_40") {Speaker = "Player", Animation="Mocking"},
        new ("LauraDialogue_LeoSalutis_50") {Speaker = "Laura", Animation="Deflated"},
        new ("LauraDialogue_LeoSalutis_60") {Speaker = "Player", Animation="Realization"},
        new ("LauraDialogue_LeoSalutis_70") {Speaker = "Laura", Animation="Thinking"},
        new ("LauraDialogue_LeoSalutis_80") {Speaker = "Player"},
        new ("LauraDialogue_LeoSalutis_90") {Speaker = "Laura", Animation="Encourage"},
        new ("LauraDialogue_LeoSalutis_100") {Speaker = "Player", Animation="Thinking"},
        new ("LauraDialogue_LeoSalutis_110") {Speaker = "Laura", Animation="Explaining"},
        new ("LauraDialogue_LeoSalutis_120") {Speaker = "Player", Animation="ShoulderShrug", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},

        new ("LauraDialogue_LeoTonitrui_10") {Speaker = "Laura", Animation="Approving"},
        new ("LauraDialogue_LeoTonitrui_20") {Speaker = "Player"},
        new ("LauraDialogue_LeoTonitrui_30") {Speaker = "Laura", Animation="YesYou"},
        new ("LauraDialogue_LeoTonitrui_40") {Speaker = "Player", Animation="GiveMeABreak"},
        new ("LauraDialogue_LeoTonitrui_50") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_LeoTonitrui_60") {Speaker = "Player", Animation="Thinking"},
        new ("LauraDialogue_LeoTonitrui_70") {Speaker = "Laura", Animation="Talking"},
        new ("LauraDialogue_LeoTonitrui_80") {Speaker = "Player", Animation="Sigh", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},

        new ("LauraDialogue_LeoProprius_10") {Speaker = "Laura", Animation="Determined"},
        new ("LauraDialogue_LeoProprius_20") {Speaker = "Player", Animation="SternNo"},
        new ("LauraDialogue_LeoProprius_30") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_LeoProprius_40") {Speaker = "Laura"},
        new ("LauraDialogue_LeoProprius_50") {Speaker = "Player", Animation="Deflated"},
        new ("LauraDialogue_LeoProprius_60") {Speaker = "Laura", Animation="YesMe"},
        new ("LauraDialogue_LeoProprius_70") {Speaker = "Player", Animation="Approving", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},

        new ("LauraDialogue_Study_10") {Speaker = "Player", Animation="Doubtful"},
        new ("LauraDialogue_Study_20") {Speaker = "Laura", Animation="ShoulderShrug"},
        new ("LauraDialogue_Study_30") {Speaker = "Player"},
        new ("LauraDialogue_Study_40") {Speaker = "Laura", Animation="Explaining"},
        new ("LauraDialogue_Study_50") {Speaker = "Laura", Animation="Laugh"},
        new ("LauraDialogue_Study_60") {Speaker = "Player", Animation="Approving", IdOfNextDialogueLine="LauraDialogue_Dialogue_10"},
    }) {PlayerStartingPosition = new Vector2(13f, -0.5f), PlayerStartingFlipped = false, 
    DialogueSpeaker = Utils.GetUnit("Laura"), SpeakerStartingPosition = new Vector2(15.5f, -0.5f), SpeakerStartingFlipped = true};}

    public static void OnEnd_LauraDialogue_Shop_20() {
        List<Type> found_item_types = SaveFile.Instance.FoundItemTypes;
        List<Item> items = new();
        foreach(Type type in found_item_types) {
            items.Add((Item)Activator.CreateInstance(type, new object[] {Item.ItemGrade.Regular}));
        }
        MenuManager.Instance.OpenShop(items, "LaurasShop");
    }

    public static void LauraDialogue_Study_0() {
        SaveFile.Instance.AddFlag("Trimvine_TalkedAboutStudy");
    }

    public static bool CheckIfVisible_LauraDialogue_Study_0() {
        return !SaveFile.Instance.Flags.Contains("Trimvine_TalkedAboutStudy");
    }

    public static bool CheckIfEnabled_LauraDialogue_Study_0() {
        return SaveFile.Instance.Flags.Contains("Trimvine_CheckedTheDevice");
    }

    public static bool CheckIfVisible_LauraDialogue_Shop_0() {
        return SaveFile.Instance.Cycle != 1;
    }

    public static Dialogue StackedChest() {
        return new Dialogue ("Area_Trimvine", "StackedChest", new List<DialogueLine>{
        new ("Trimvine_StackingChest_0") {Speaker = "Player"},
        new ("Trimvine_StackingChest_10") {Speaker = "Player",
        Choices= new List<DialogueChoice> {
            new ("Trimvine_StackingChest1_0") {IdOfNextDialogueLine = "END", StringParams = new List<String>{(20000 + 2000 * SaveFile.Instance.Week).ToString()}}, 
            new ("Trimvine_StackingChest2_0") {IdOfNextDialogueLine = "END"}
        }},
    });}

    public static void Trimvine_StackingChest1_0() {
        SaveFile.Instance.AddFlag(Utils.GetFormattedFlag("Trimvine_TookStackingChest_[Cycle]"));
        SaveFile.Instance.Money += 20000 + 2000 * SaveFile.Instance.Week;
        Area.Instance.transform.Find("Interactables/Stacking Chest").gameObject.SetActive(false);
    }

    public static Dialogue TimelyHelp_Shadow() {
        return new Dialogue ("Area_Trimvine", "TimelyHelp_Shadow", new List<DialogueLine>{
        new ("TimelyHelp_Shadow_0") {Speaker = "Shadow", Animation="Mocking"},
        new ("TimelyHelp_Shadow_10") {Speaker = "Player", Animation="Sigh"},
        new ("TimelyHelp_Shadow_20") {Speaker = "Shadow", Animation="Explaining"},
        new ("TimelyHelp_Shadow_30") {Speaker = "Player", Animation="Doubtful"},
        new ("TimelyHelp_Shadow_40") {Speaker = "Shadow", Animation="SternNo"},
        new ("TimelyHelp_Shadow_50") {Speaker = "Player", Animation="Listening"},
        new ("TimelyHelp_Shadow_60") {Speaker = "Shadow"},
        new ("TimelyHelp_Shadow_70") {Speaker = "Player", Animation="Doubtful"},
        new ("TimelyHelp_Shadow_80") {Speaker = "Shadow", Animation="Frustrated"},
        new ("TimelyHelp_Shadow_90") {Speaker = "Player", Animation="ComeAtMe"},
        new ("TimelyHelp_Shadow_100") {Speaker = "Shadow", Animation="Sigh"},
        new ("TimelyHelp_Shadow_110") {Speaker = "Shadow", Animation="Determined", AudioClip="Dialogue/TrueSpeech"},
        new ("TimelyHelp_Shadow_120") {Speaker = "Shadow", Animation="Exhausted"},
        new ("TimelyHelp_Shadow_130") {
        Choices= new List<DialogueChoice> {
            new ("TimelyHelp_ShadowReject_0"),
            new ("TimelyHelp_ShadowAccept_0"),
            new ("TimelyHelp_ShadowInquire_0")
        }},
        new ("TimelyHelp_ShadowReject_10") {Speaker = "Player", Animation="SternNo"},
        new ("TimelyHelp_ShadowReject_20") {Speaker = "Shadow", Animation="Frustrated"},
        new ("TimelyHelp_ShadowReject_30") {Speaker = "Player", Animation="HaveItYourWayThen", IdOfNextDialogueLine="END"},
        new ("TimelyHelp_ShadowAccept_10") {Speaker = "Player", Animation="FinallyDecided"},
        new ("TimelyHelp_ShadowAccept_20") {Speaker = "Shadow", Animation="YouFinallyGetIt", IdOfNextDialogueLine="END"},
        new ("TimelyHelp_ShadowInquire_10") {Speaker = "Player", Animation="Doubtful"},
        new ("TimelyHelp_ShadowInquire_20") {Speaker = "Shadow", Animation="Irritated"},
        new ("TimelyHelp_ShadowInquire_30") {Speaker = "Player"},
        new ("TimelyHelp_ShadowInquire_40") {Speaker = "Shadow", Animation="ShoulderShrug"},
        new ("TimelyHelp_ShadowInquire_50") {Speaker = "Player", Animation="Doubtful"},
        new ("TimelyHelp_ShadowInquire_60") {Speaker = "Shadow", Animation="Frustrated"},
        new ("TimelyHelp_ShadowInquire_70") {Speaker = "Player", Animation="ShoulderShrug"},
        new ("TimelyHelp_ShadowInquire_80") {Speaker = "Shadow", Animation="HaveItYourWayThen", AudioClip="Dialogue/TrueSpeech"},
        new ("TimelyHelp_ShadowInquire_90") {
        Choices= new List<DialogueChoice> {
            new ("TimelyHelp_ShadowInquire1_0"),
            new ("TimelyHelp_ShadowInquire2_0")
        }},
        new ("TimelyHelp_ShadowInquire1_10") {Speaker = "Player", Animation="Approving"},
        new ("TimelyHelp_ShadowInquire1_20") {Speaker = "Shadow", Animation="YouFinallyGetIt", IdOfNextDialogueLine="END"},
        new ("TimelyHelp_ShadowInquire2_10") {Speaker = "Player", Animation="TryingToRemember"},
        new ("TimelyHelp_ShadowInquire2_20") {Speaker = "Shadow", Animation="ShoulderShrug"},
        new ("TimelyHelp_ShadowInquire2_30") {Speaker = "Player", Animation="Thinking"},
        new ("TimelyHelp_ShadowInquire2_40") {Speaker = "Player", Animation="Grateful"},
        new ("TimelyHelp_ShadowInquire2_50") {Speaker = "Shadow", Animation="SternNo", IdOfNextDialogueLine="END"}
    }){PlayerStartingPosition = new Vector2(3.5f, 7f), PlayerStartingFlipped = false};}

    public static bool CheckIfEnabled_TimelyHelp_ShadowInquire2_0() {
        return SaveFile.Instance.Flags.Contains("ReadTrueSpeechBook");
    }

    public static void OnEnd_TimelyHelp_ShadowInquire2_50() {
        SaveFile.Instance.AddFlag("ShadowTrueSpeechInfo");
        SaveFile.Instance.ExperiencePoints += 2500;
        Unit shadow = Utils.GetUnit("Shadow");
        Utils.CreateVisualEffect(new(Utils.GetUnit("Shadow")), "EntityFlamesBurst", shadow.transform.position.x, shadow.transform.position.y - 0.5f);
        shadow.gameObject.SetActive(false);
    }

    public static Dialogue TimelyHelp_Regular() {
        return new Dialogue ("Area_Trimvine", "TimelyHelp_Regular", new List<DialogueLine>{
        new ("TimelyHelp_Regular_0") {Animation="WaveGoodbyeAloof", Speaker = "Player"},
        new ("TimelyHelp_Regular_10") {Animation="Sigh",Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Regular_20") {Animation="Explaining",Speaker = "Player"},
        new ("TimelyHelp_Regular_30") {Animation="Deflated",Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Regular_40") {Speaker = "Player"},
        new ("TimelyHelp_Regular_50") {Animation="Frustrated",Speaker = "TimelyHelpNPC",
        Choices= new List<DialogueChoice> {
            new ("TimelyHelp_Regular1_0") {HideChoiceTextIfDisabled=false},
            new ("TimelyHelp_Regular2_0"),
            new ("TimelyHelp_Regular3_0")
        }},
        new ("TimelyHelp_Regular1_10") {Animation="GiveItem",Speaker = "Player"},
        new ("TimelyHelp_Regular1_20") {Animation="ReceiveItem",Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Regular1_30") {Animation="ShoulderShrug",Speaker = "Player"},
        new ("TimelyHelp_Regular1_40") {Animation="Determined",Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Regular1_50") {Animation="WaveGoodbyeAloof",Speaker = "Player", IdOfNextDialogueLine="END"},
        new ("TimelyHelp_Regular2_10") {Animation="Explaining",Speaker = "Player"},
        new ("TimelyHelp_Regular2_20") {Animation="Surprised",Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Regular2_30") {Animation="Approving",Speaker = "Player"},
        new ("TimelyHelp_Regular2_40") {Animation="Grateful",Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Regular2_50") {Animation="WaveGoodbyeAloof",Speaker = "Player", IdOfNextDialogueLine="END"},
        new ("TimelyHelp_Regular3_10") {Animation="Sigh",Speaker = "TimelyHelpNPC", IdOfNextDialogueLine="END"},
    }){PlayerStartingPosition = new Vector2(12.5f, 10.5f), PlayerStartingFlipped = false};}


    public static bool CheckIfEnabled_TimelyHelp_Regular1_0() {
        return SaveFile.Instance.Money >= 100000;
    }

    public static bool CheckIfVisible_TimelyHelp_Regular2_0() {
        return !SaveFile.Instance.HasFlag("Trimvine_TreasureDugUp_[Cycle]");
    }

    public static bool CheckIfEnabled_TimelyHelp_Regular2_0() {
        return SaveFile.Instance.HasFlag("Trimvine_TreasureUnderTree");
    }

    public static void TimelyHelp_Regular2_40() {
        UIManager.Instance.HideBlackScreen(0.25f);
        MonoBehaviour.Destroy(Area.Instance.transform.Find("Interactables/Treasure Tree/OnDestroy").gameObject);
        Area.Instance.transform.Find("Interactables/Treasure Tree").GetComponent<DestructibleEnvironment>().DestroyObject();
        SaveFile.Instance.AddFlag("Trimvine_TreasureDugUp_[Cycle]");
        SaveFile.Instance.AddFlag("Trimvine_GaveTreasure_[Cycle]");
        SaveFile.Instance.AddFlag("Trimvine_TimelyHelpHelpedInTime_[Cycle]");
        Utils.GetUnit("TimelyHelpNPC").transform.Find("Regular").gameObject.SetActive(false);
    }

    public static void TimelyHelp_Regular1_10() {
        SaveFile.Instance.Money -= 100000;
        SaveFile.Instance.AddFlag("Trimvine_TimelyHelpHelpedInTime_[Cycle]");
        Utils.GetUnit("TimelyHelpNPC").transform.Find("Regular").gameObject.SetActive(false);
    }

    public static Dialogue TimelyHelp_TreasureDugUp() {
        return new Dialogue ("Area_Trimvine", "TimelyHelp_TreasureDugUp", new List<DialogueLine>{
        new ("TimelyHelp_TreasureDugUp_0") {Animation="PickUpItemFromTheGround", Speaker = "Player"},
        new ("TimelyHelp_TreasureDugUp_10") {Animation="Thinking", Speaker = "Player", IdOfNextDialogueLine=SaveFile.Instance.Cycle == 2 ? "TimelyHelp_TreasureDugUp_20" : "TimelyHelp_TreasureDugUp_30"},
        new ("TimelyHelp_TreasureDugUp_20") {Animation="Thinking", Speaker = "Player", IdOfNextDialogueLine="END"},
        new ("TimelyHelp_TreasureDugUp_30") {Animation="Thinking", Speaker = "Player"},
    });}

    public static void TimelyHelp_TreasureDugUp_0() {
        SaveFile.Instance.Money += 153000;
        SaveFile.Instance.ExperiencePoints += 500;
    }

    public static Dialogue TimelyHelp_TooLate() {
        return new Dialogue ("Area_Trimvine", "TimelyHelp_TooLate", new List<DialogueLine>{
        new ("TimelyHelp_Despair_0") {Animation="Surprised", Speaker = "Player"},
        new ("TimelyHelp_Despair_10") {Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Despair_20") {Animation="SternNo", Speaker = "Player"},
        new ("TimelyHelp_Despair_30") {Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Despair_40") {Speaker = "Player"},
        new ("TimelyHelp_Despair_50") {Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Despair_60") {Animation="Determined", Speaker = "Player"}
    }){PlayerStartingPosition = new Vector2(12.5f, 10.5f), PlayerStartingFlipped = false};}

    public static void OnEnd_TimelyHelp_TooLate() {
        if(!SaveFile.Instance.HasFlag("Trimvine_TreasureUnderTree")) {
            SaveFile.Instance.ExperiencePoints += 1000;
        }
        SaveFile.Instance.AddFlag("Trimvine_TreasureUnderTree");
    }

    public static Dialogue TimelyHelp_InTime() {
        return new Dialogue ("Area_Trimvine", "TimelyHelp_InTime", new List<DialogueLine>{
        new ("TimelyHelp_Saved_0") {Animation="GiveItem",  Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Saved_10") {Animation="Bashful", Speaker = "Player"},
        new ("TimelyHelp_Saved_20") {Animation="Explaining",  Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Saved_30") {Animation="Surprised", Speaker = "Player"},
        new ("TimelyHelp_Saved_40") {Animation="HandWave",  Speaker = "TimelyHelpNPC"},
        new ("TimelyHelp_Saved_50") {Animation="ReceiveItem", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(12.5f, 10.5f), PlayerStartingFlipped = false};
    }

    public static void TimelyHelp_Saved_50() {
        if(SaveFile.Instance.HasFlag("Trimvine_GaveTreasure_[Cycle]")) {
            SaveFile.Instance.Money += 400000;
            SaveFile.Instance.ExperiencePoints += 5500;
        }
        else {
            SaveFile.Instance.Money += 200000;
            SaveFile.Instance.ExperiencePoints += 3500;
        }
    }

    public static Dialogue Relana_Dialogue() {
        return new Dialogue ("Area_Trimvine", "Relana_Dialogue", new List<DialogueLine>{
        new ("LaurasLab_Relana_0"),
        new ("LaurasLab_Relana_10") {Speaker = "Relana", Animation="WaveGoodbyeAloof"},
        new ("LaurasLab_Relana_20") {Speaker = "Player"},
        new ("LaurasLab_Relana_30") {Speaker = "Relana"},
        new ("LaurasLab_Relana_40") {Speaker = "Player", Animation="ShoulderShrug"},
        new ("LaurasLab_Relana_50") {Speaker = "Relana", Animation="Laugh"},
        new ("LaurasLab_Relana_60") {Speaker = "Player"},
        new ("LaurasLab_Relana_70") {Speaker = "Relana"},
        new ("LaurasLab_Relana_80") {Speaker = "Player", Animation="Sigh"},
        new ("LaurasLab_Relana_90") {Speaker = "Relana", Animation="YouFinallyGetIt"},
        new ("LaurasLab_Relana_100") {Speaker = "Player", Animation="Bashful"}
    }){PlayerStartingPosition = new Vector2(23f, -1f), PlayerStartingFlipped = false};
    }

    public static Dialogue TheDevice() {
        return new Dialogue ("Area_Trimvine", "TheDevice", new List<DialogueLine>{
        new ("LaurasLab_Device_0") {Speaker = "Player", Animation="TryingToRemember"},
        new ("LaurasLab_Device_10") {Speaker = "Player"},
        new ("LaurasLab_Device_20") {Speaker = "Player"}
    });}

    public static Dialogue Excalibur() {
        return new Dialogue ("Area_Trimvine", "Excalibur", new List<DialogueLine>{
        new ("Trimvine_Rockalibur_0") {Speaker = "Player", Animation="TryingToRemember"},
        new ("Trimvine_Rockalibur_10") {Speaker = "Player"},
        new ("Trimvine_Rockalibur_20") {Speaker = "Player"},
        new ("Trimvine_Rockalibur_30") {Speaker = "Player"},
        new ("Trimvine_Rockalibur_40") {Speaker = "Player",
        Choices= new List<DialogueChoice> {
            new ("Trimvine_Rockalibur1_0") {IdOfNextDialogueLine = "END"}, 
            new ("Trimvine_Rockalibur2_0") {IdOfNextDialogueLine = "END"}
        }},
    });}

    public static void Trimvine_Rockalibur1_0() {
        Area.Instance.transform.Find("Environment/Rock Pillar").GetComponent<DestructibleEnvironment>().DestroyObject();
        Area.Instance.transform.Find("Environment/Rock Pillar_2").GetComponent<DestructibleEnvironment>().DestroyObject();
        SaveFile.Instance.AddFlag("Trimvine_TookExcalibur_[Cycle]");
        SaveFile.Instance.AddItem(typeof(Greatsword_RockSplitter), Item.ItemGrade.Excellent);
        SaveFile.Instance.ExperiencePoints += 400;
        MonoBehaviour.Destroy(Area.Instance.transform.Find("Interactables/Excalibur").gameObject);
        Player.Instance.PlayAnimation("PickUpItemFromTheGround");
    }

    public static void TreeGuyDialogue(InteractableObject interactable) {
        if(!SaveFile.Instance.HasFlag("Trimvine_BladeTree_Destroyed_[Cycle]")) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Trimvine_Inter_50", SpeakerUnit=Utils.GetUnit("TreeGuy")});
        }
        else {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Trimvine_Inter_140", SpeakerUnit=Utils.GetUnit("TreeGuy")});
        }
    }

    public static void ShowStackingChest() {
        if(SaveFile.Instance.DoesNotHaveFlag("Trimvine_TookStackingChest_[Cycle]")) {
            Area.Instance.transform.Find("Interactables/Stacking Chest").gameObject.SetActive(true);
        }
    }

    public static bool CheckIfTreasureTreeIsDestructible() {
        return SaveFile.Instance.HasFlag("Trimvine_TreasureUnderTree");
    }
    
    public static void PickUpTreeBlade(InteractableObject interactable) {
        if(SaveFile.Instance.Inventory.FirstOrDefault(item => item.GetType() == typeof(Quest_HalfStiletto)) != null) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Trimvine_Inter_110"});
            SaveFile.Instance.RemoveItem(SaveFile.Instance.Inventory.FirstOrDefault(item => item.GetType() == typeof(Quest_HalfStiletto)));
            SaveFile.Instance.AddItem(typeof(Daggers_Stiletto), Item.ItemGrade.Excellent);
            SaveFile.Instance.ExperiencePoints += 1500;
        }
        else {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Trimvine_Inter_80"});
            SaveFile.Instance.AddItem(typeof(Quest_HalfStiletto));
        }
    }

    public static void PickUpGraveBlade(InteractableObject interactable) {
        if(SaveFile.Instance.Inventory.FirstOrDefault(item => item.GetType() == typeof(Quest_HalfStiletto)) != null) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Trimvine_Inter_110"});
            SaveFile.Instance.RemoveItem(SaveFile.Instance.Inventory.FirstOrDefault(item => item.GetType() == typeof(Quest_HalfStiletto)));
            SaveFile.Instance.AddItem(typeof(Daggers_Stiletto), Item.ItemGrade.Excellent);
            SaveFile.Instance.ExperiencePoints += 1500;
        }
        else {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Trimvine_Inter_100"});
            SaveFile.Instance.AddItem(typeof(Quest_HalfStiletto));
        }
    }

    public static void SecondTreeDestroyed() {
        if(SaveFile.Instance.HasFlag("Trimvine_BladeTree_Destroyed_[Cycle]")) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="Trimvine_Inter_90"});
        }
    }

    public static void UpgradeHeal(InteractableObject inter) {
        SaveFile.Instance.HealUpgrades++;
        SaveFile.Instance.AddFlag("Trimvine_HealResearch");
    }

    public static void FortificationPotion() {
        if(!SaveFile.Instance.HasFlag("Trimvine_FortificationPotion")) {
            SaveFile.Instance.AcquireItem("Tool_FortificationPotion_Unlock", 1, Item.ItemGrade.None);
            SaveFile.Instance.AddFlag("Trimvine_FortificationPotion");
        }
        else {
            NotificationController.ShowDialogueNotification("DuplicateToolFoundNotification");
        }
    }

    public static void TrimvineShop(InteractableObject inter) {
        MenuManager.Instance.OpenShop(new() {
        }, 
            "TrimvineShop");
    }
}
