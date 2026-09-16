using System.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Area_AnimaIsland
{
    public static void OnStart() {
        EventManager.UnitKnockedOut.AddListener(CheckDefeatedEnemy);
        EventManager.UnitWouldBeDefeated.AddListener(CheckDefeatedEnemy);
        EventManager.DestructibleDestroyed.AddListener(CheckDestructibleDestroyed);
        Area.Instance.transform.Find("Cycle" + SaveFile.Instance.Cycle).gameObject.SetActive(true);
        if(SaveFile.Instance.Cycle == 1) {
            Utils.GetUnit("Iris1").AddEffect(new Effect_CannotBeDefeated(false, new(Utils.GetUnit("Iris1"))) {
                IsRemovable = false
            });
            Area.ComponentInstance.AutoRestAfterCombat = true;
            StartAllDuels();
            EventManager.AbilityWasRipostedOrCountered.AddListener(CheckAbilityCountered);
            if(SaveFile.Instance.CurrentAreaLoadedFromSave == false) {
                UIManager.Instance.StartDialogue(IrisFirstEncounter());
                Utils.GetUnit("Iris1").PlayAnimation("IdleInCombat");
            }
            else if(SaveFile.Instance.GetQuest("Anima").CurrentObjective.Number != 0) {
                Utils.GetUnit("Iris1").GetComponent<FollowPlayer>().enabled = true;
                Utils.GetUnit("Unit_AnimaHound_1").gameObject.SetActive(false);
                Utils.GetUnit("Unit_AnimaHound_2").gameObject.SetActive(false);
                Utils.GetUnit("Unit_AnimaHound_3").gameObject.SetActive(false);
                Utils.GetUnit("Unit_AnimaHound_4").gameObject.SetActive(false);
            }
        }
        else if(SaveFile.Instance.Cycle == 2 && SaveFile.Instance.CurrentAreaLoadedFromSave == false) {
            UIManager.Instance.StartDialogue(InitialConfrontationCycle2());
            
        }
        else if(SaveFile.Instance.Cycle == 3 && SaveFile.Instance.CurrentAreaLoadedFromSave == false) {
        }
    }

    public static void StartAllDuels() {
        foreach(Unit u in Utils.GetAllUnits()) {
            if(u.Faction == Constants.Faction.DuelingEachOther) {
                u.AddEffect(new Effect_ChangeStat(u.Health, new(u)) {RegenerationPercentageAmount = 15, Id="DuelRegeneration"});
                u.CurrentTarget = u.GetClosestValidTarget();
            }
        }
    }

    public static void CheckDefeatedEnemy(DamageInstance damage) {
        if(SaveFile.Instance.Cycle == 1 && damage.TargetOfDamage.gameObject.name.Contains("Unit_AnimaHound") && SaveFile.Instance.HasFlag("AnimaIsland_FoughtMaginhart")) {
            List<Unit> units = Utils.GetSpecifiedUnits(new Func<Unit, bool>((unit) => (unit.gameObject.name.Contains("Unit_AnimaHound") && unit.InCombat && unit.KnockedOut == false)));
            if(units.Count == 0) {
                Utils.GetUnit("Iris1").SetToNeutralNPC();
                UIManager.Instance.ShowBlackScreen(1.5f);
                GameController.Instance.WaitAndRunMethod(1.5f, ConcludeFirstIrisFight);
            }
        }
        else if(SaveFile.Instance.Cycle == 1 && damage.TargetOfDamage.gameObject.name.Contains("Unit_Iris1") && SaveFile.Instance.HasFlag("AnimaIsland_FoughtIris")) {
            Utils.GetUnit("Iris1").SetToNeutralNPC();
            UIManager.Instance.ShowBlackScreen(1.5f);
            GameController.Instance.WaitAndRunMethod(1.5f, ConcludeFirstIrisFight);
        }
        else if(SaveFile.Instance.Cycle == 1 && (damage.TargetOfDamage.gameObject.name.Contains("AnimaBlademaster") || damage.TargetOfDamage.gameObject.name.Contains("AnimaSpearmaster") || damage.TargetOfDamage.gameObject.name.Contains("AnimaBowmaster"))) {
            UIManager.Instance.ShowBlackScreen(1.5f);
            GameController.Instance.WaitAndRunMethod(1.5f, StartIrisTalk, damage.TargetOfDamage.gameObject);
        }
        else if(SaveFile.Instance.Cycle == 1 && damage.TargetOfDamage.gameObject.name.Contains("WeaponPillagerBoss")) {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_WeaponPillagerAttack_10", SpeakerUnit=Utils.GetUnit("WeaponPillagerBoss")});
            SaveFile.Instance.GetQuest("Anima").AdvanceObjective(40);
            Utils.GetUnit("Maginhart2").transform.Find("Dialogue").gameObject.SetActive(false);
            Utils.GetUnit("Maginhart2").transform.Find("FinalDialogue").gameObject.SetActive(true);
        }
        else if(SaveFile.Instance.Cycle == 2 && damage.TargetOfDamage.gameObject.name.Contains("WeaponPillagerBossCycle2")) {
            NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle2Interactions_30", SpeakerUnit=Utils.GetUnit("WeaponPillagerBossCycle2")});
        }
    }

    public static void CheckAbilityCountered(Ability ability, bool was_countered) {
        if((SaveFile.Instance.GetQuest("Anima").CurrentObjective.Number == 0 || SaveFile.Instance.GetQuest("Anima").CurrentObjective.Number == 10) && SaveFile.Instance.AnimaAbilitiesCountered.Contains(ability.GetType()) == false) {
            SaveFile.Instance.AnimaAbilitiesCountered.Add(ability.GetType());
        }
        SaveFile.Instance.GetQuest("Anima").CurrentObjective.DescriptionParameters = new() {SaveFile.Instance.AnimaAbilitiesCountered.Count.ToString()};
        if(SaveFile.Instance.GetQuest("Anima").CurrentObjective.Number == 10 && SaveFile.Instance.AnimaAbilitiesCountered.Count >= 20) {
            Utils.GetUnit("WeaponPillager1").gameObject.SetActive(false);
            Area.Instance.transform.Find("Interactables/PillagerAggro").gameObject.SetActive(true);
            SaveFile.Instance.GetQuest("Anima").AdvanceObjective(20);
        }
    }

    public static int DestroyedTreesCounter;
    public static int DestroyedWeaponsCounter;
    public static void CheckDestructibleDestroyed(DestructibleEnvironment destructible) {
        if(destructible.gameObject.name.Contains("Tree")) {
            DestroyedTreesCounter++;
            if(DestroyedTreesCounter == 5) {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_30"});
            }
        }
        if(destructible.gameObject.name.Contains("LeftoverWeapon")) {
            DestroyedWeaponsCounter++;
            if(DestroyedWeaponsCounter == 15) {
                NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_40"});
            }
        }
    }

    public static void ConcludeFirstIrisFight() {
        UIManager.Instance.StartDialogue(AfterFirstFight());
    }

    public static void WeaponPillagerOpenShop(InteractableObject inter) {
        MenuManager.Instance.OpenShop(new() {
            new TwinBlades_Oath(Item.ItemGrade.Excellent),
            new Gun_Lament(Item.ItemGrade.Excellent),
            new Quest_UpgradeMaterials(Item.ItemGrade.Excellent) {BuyPrice = 15000},
            new Quest_UpgradeMaterials(Item.ItemGrade.Masterful) {BuyPrice = 50000}}, 
            "WeaponPillagerOpenShop");
    }

    public static void RegrowTree(DestructibleEnvironment destr) {
        GameObject newTree = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Environment/Tree " + (UnityEngine.Random.Range(0, 100) < 50 ? "3" : "4"))) as GameObject;
        newTree.transform.position = destr.transform.position;
        if(destr.GetComponent<ChangeTransformOverTime>() != null) {
            newTree.transform.position = new Vector2(destr.transform.position.x, destr.GetComponent<ChangeTransformOverTime>().EndY);
        }
        newTree.transform.localScale = Vector3.zero;
        newTree.GetComponent<DestructibleEnvironment>().enabled = false;
        GameController.Instance.WaitAndRunMethod(15, StartTreeRegrow, newTree);
    }

    public static void StartTreeRegrow(GameObject gameObject) {
        gameObject.GetComponent<DestructibleEnvironment>().enabled = true;
        ChangeTransformOverTime change = gameObject.AddComponent<ChangeTransformOverTime>();
        change.StartScale = 0;
        float size = UnityEngine.Random.Range(0.8f, 1.2f);
        change.EndScale = size;
        int time = UnityEngine.Random.Range(45, 120);
        change.ScaleTime = time;
        change.StartY = gameObject.transform.position.y + -2.0f * size;
        change.EndY = gameObject.transform.position.y;
        change.YTime = time;
    }

    public static Dialogue IrisFirstEncounter() {
        return new Dialogue ("Area_AnimaIsland", "IrisFirstEncounter", new List<DialogueLine>{
        new ("AnimaIsland_IrisFirstEncounter_0") {Animation="Appear", Speaker = "Player", WaitTimeBeforeAllowingToProceed = 2.5f},
        new ("AnimaIsland_IrisFirstEncounter_10") {Animation="Thinking", Speaker = "Maginhart1"},
        new ("AnimaIsland_IrisFirstEncounter_20") {Animation="ShoulderShrug", Speaker = "Player", WaitTimeBeforeAllowingToProceed = 3.5f},
        new ("AnimaIsland_IrisFirstEncounter_30") {Animation="Realization", Speaker = "Iris1", TurnSpeakersToFaceEachOther=false},
        new ("AnimaIsland_IrisFirstEncounter_40") {Animation="Surprised", Speaker = "Player", TurnSpeakersToFaceEachOther=false},
        new ("AnimaIsland_IrisFirstEncounter_50") {Animation="ThreatenWithLight", Speaker = "Iris1"},
        new ("AnimaIsland_IrisFirstEncounter_60") {Animation="Intrigued", Speaker = "Player", TurnSpeakersToFaceEachOther=false},
        new ("AnimaIsland_IrisFirstEncounter_70") {Animation="Stoic", Speaker = "Maginhart1"},
        new ("AnimaIsland_IrisFirstEncounter_80") {Animation="SternNo", Speaker = "Iris1"},
        new ("AnimaIsland_IrisFirstEncounter_90") {Animation="HandWave", Speaker = "Maginhart1"},
        new ("AnimaIsland_IrisFirstEncounter_100") {Animation="Disapprove", Speaker = "Iris1"},
        new ("AnimaIsland_IrisFirstEncounter_110") {Animation="Thinking", Speaker = "Player", TurnSpeakersToFaceEachOther=false},
        new ("AnimaIsland_IrisFirstEncounter_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("AnimaIsland_IrisFirstEncounterHelpIris_0"),
            new ("AnimaIsland_IrisFirstEncounterHelpMaginhart_0")
        }},
        new ("AnimaIsland_IrisFirstEncounterHelpIris_10") {Animation="FinallyDecided", Speaker = "Player"},
        new ("AnimaIsland_IrisFirstEncounterHelpIris_20") {Animation="ThreatenWithLight", Speaker = "Iris1"},
        new ("AnimaIsland_IrisFirstEncounterHelpIris_30") {Animation="Disapprove", Speaker = "Maginhart1", IdOfNextDialogueLine="END"},
        new ("AnimaIsland_IrisFirstEncounterHelpMaginhart_10") {Animation="ThreatenWithHeavy", Speaker = "Player"},
        new ("AnimaIsland_IrisFirstEncounterHelpMaginhart_20") {Animation="ThreatenWithLight", Speaker = "Iris1"},
        new ("AnimaIsland_IrisFirstEncounterHelpMaginhart_30") {Animation="Surprised", Speaker = "Player"},
        new ("AnimaIsland_IrisFirstEncounterHelpMaginhart_40") {Animation="HandWave", Speaker = "Maginhart1", IdOfNextDialogueLine="END"},
    }){PlayerStartingPosition = new Vector2(-85f, -5f), PlayerStartingFlipped = false, ReturnUnitsToOriginalPositions = false, AutoSaveOnDialogueEnd=false, DialogueSpeaker=Utils.GetUnit("Iris1")};}

    public static void OnStart_IrisFirstEncounter() {
        Utils.CreateVisualEffect(new(Player.Instance), "WaterSplash1", -85f, -6f);
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Footsteps/Footsteps_Water10", 0.8f);
    }

    public static void AnimaIsland_IrisFirstEncounter_0() {
        Area.Instance.transform.Find("Cycle1/IrisHunt").gameObject.SetActive(true);
        GameController.Instance.WaitAndRunMethod(1.5f, AnimaIsland_IrisFirstEncounter_0v2);
        Utils.GetUnit("Maginhart1").Actions.IsFlipped = true;
        Utils.GetUnit("Unit_AnimaHound_1").Actions.IsFlipped = true;
        Utils.GetUnit("Unit_AnimaHound_2").Actions.IsFlipped = true;
        Utils.GetUnit("Unit_AnimaHound_3").Actions.IsFlipped = true;
        Utils.GetUnit("Unit_AnimaHound_4").Actions.IsFlipped = true;
    }

    public static void AnimaIsland_IrisFirstEncounter_0v2() {
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Iris1").gameObject;
        Utils.GetUnit("Iris1").PlayAnimation("ThreatenWithLight");
    }

    public static void AnimaIsland_IrisFirstEncounter_20() {
        GameController.Instance.WaitAndRunMethod(1.0f, AnimaIsland_IrisFirstEncounter_20v2);
    }

    public static void AnimaIsland_IrisFirstEncounter_20v2() {
        GameController.Instance.WaitAndRunMethod(2.0f, AnimaIsland_IrisFirstEncounter_20v3);
        CameraController.Instance.CenteredOnObject = null;
        Player.Instance.CurrentTarget = Area.Instance.transform.Find("Cycle1/IrisHunt/FakeTarget").GetComponent<Unit>();
        Player.Instance.Actions.UseAbility(typeof(NPCAbility_ThunderStep), false, Area.Instance.transform.Find("Cycle1/IrisHunt/FakeTarget").GetComponent<Unit>());
    }

    public static void AnimaIsland_IrisFirstEncounter_20v3() {
        Player.Instance.PlayAnimation("ThreatenWithHeavy");
    }

    public static void OnEnd_AnimaIsland_IrisFirstEncounterHelpIris_30() {
        Quest_Anima quest = (Quest_Anima)SaveFile.Instance.GetQuest("Anima");
        quest.StartQuest(true);
        quest.GetObjective(0).UpdateStatusWithoutNotifying(QuestObjective.ObjectiveStatus.Current);
        quest.GetObjective(0).ShowAsMissionObjective();
        SaveFile.Instance.AddFlag("AnimaIsland_FoughtMaginhart");
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Area_AnimaIsland.PrepareDuelAgainstMaginhart"});
        PrepareDuelAgainstMaginhart();
    }
    
    public static void OnEnd_AnimaIsland_IrisFirstEncounterHelpMaginhart_40() {
        Quest_Anima quest = (Quest_Anima)SaveFile.Instance.GetQuest("Anima");
        quest.StartQuest(true);
        quest.GetObjective(0).UpdateStatusWithoutNotifying(QuestObjective.ObjectiveStatus.Current);
        quest.GetObjective(0).ShowAsMissionObjective();
        SaveFile.Instance.AddFlag("AnimaIsland_FoughtIris");
        GameController.Instance.SaveMidMissionInformation(new List<string> {"Area_AnimaIsland.PrepareDuelAgainstIris"});
        PrepareDuelAgainstIris();
    }

    public static List<Effect_ChangeStat> IrisBossBuffs = new();

    public static void PrepareDuelAgainstIris() {
        Unit Iris = Utils.GetUnit("Iris1");
        Player.Instance.Actions.IsFlipped = true;
        Utils.GetUnit("FakeTarget").gameObject.SetActive(false);
        Utils.GetUnit("Maginhart1").transform.position = new Vector2(-50f, -4.5f);
        Iris.AttackPlayer();
        Utils.GetUnit("Unit_AnimaHound_1").gameObject.SetActive(false);
        Utils.GetUnit("Unit_AnimaHound_2").gameObject.SetActive(false);
        Utils.GetUnit("Unit_AnimaHound_3").gameObject.SetActive(false);
        Utils.GetUnit("Unit_AnimaHound_4").gameObject.SetActive(false);
        IrisBossBuffs.Add(new Effect_ChangeStat(Iris.Health, new (Iris)) {PercentageAmount = 60});
        IrisBossBuffs.Add(new Effect_ChangeStat(Iris.StaggerBar, new (Iris)) {PercentageAmount = 40});
        IrisBossBuffs.Add(new Effect_ChangeStat(Iris.LightInjury, new (Iris)) {PercentageAmount = 50});
        IrisBossBuffs.Add(new Effect_ChangeStat(Iris.LightStagger, new (Iris)) {PercentageAmount = 50});
        IrisBossBuffs.Add(new Effect_ChangeStat(Iris.LightAttackSpeed, new (Iris)) {PercentageAmount = 20});
        foreach(Effect_ChangeStat eff in IrisBossBuffs) {
            Iris.AddEffect(eff);
        }
        Iris.Health.Current = Iris.Health.Maximum;
    }

    public static void PrepareDuelAgainstMaginhart() {
        Utils.GetUnit("FakeTarget").gameObject.SetActive(false);
        Utils.GetUnit("Maginhart1").transform.position = new Vector2(-50f, -4.5f);
        Utils.GetUnit("Iris1").CurrentTarget = Utils.GetUnit("Unit_AnimaHound_1");
        Utils.GetUnit("Unit_AnimaHound_1").AttackPlayer();
        Utils.GetUnit("Unit_AnimaHound_2").AttackPlayer();
        Utils.GetUnit("Unit_AnimaHound_3").AttackPlayer();
        Utils.GetUnit("Unit_AnimaHound_4").AttackPlayer();
        Utils.GetUnit("Unit_AnimaHound_1").CurrentTarget = Utils.GetUnit("Iris1");
        Utils.GetUnit("Unit_AnimaHound_2").CurrentTarget = Utils.GetUnit("Iris1");
    }

    public static Dialogue AfterFirstFight() {
        return new Dialogue ("Area_AnimaIsland", "AfterFirstFight", new List<DialogueLine>{
        new ("AnimaIsland_AfterFirstFight_0") {Animation="ShakeHead", Speaker = "Maginhart1"},
        new ("AnimaIsland_AfterFirstFight_10") {Animation="SternNo", Speaker = "Iris1"},
        new ("AnimaIsland_AfterFirstFight_20") {Animation="Stoic", Speaker = "Maginhart1"},
        new ("AnimaIsland_AfterFirstFight_30") {Animation="ThreatenWithLight", Speaker = "Iris1"},
        new ("AnimaIsland_AfterFirstFight_40") {Animation="Deflated", Speaker = "Maginhart1"},
        new ("AnimaIsland_AfterFirstFight_50") {Speaker = "Iris1", WaitTimeBeforeAllowingToProceed=1},
        new ("AnimaIsland_AfterFirstFight_60") {Animation="Accusing", Speaker = "Maginhart1"},
        new ("AnimaIsland_AfterFirstFight_70") {Animation="ThreatenWithLightNoLonger", Speaker = "Iris1"},
        new ("AnimaIsland_AfterFirstFight_80") {Animation="FinallyDecided", Speaker = "Maginhart1"},
        new ("AnimaIsland_AfterFirstFight_90") {Animation="Frustrated", Speaker = "Iris1"},
        new ("AnimaIsland_AfterFirstFight_100") {Animation="Flabbergasted", Speaker = "Player"},
        new ("AnimaIsland_AfterFirstFight_110") {Animation="HandwaveTheIssue", Speaker = "Iris1", WaitTimeBeforeAllowingToProceed=3.5f},
        new ("AnimaIsland_AfterFirstFight_120") {Speaker = "Player", TurnSpeakersToFaceEachOther = false},
        new ("AnimaIsland_AfterFirstFight_130") {Animation="Laugh", Speaker = "Iris1", TurnSpeakersToFaceEachOther = false},
        new ("AnimaIsland_AfterFirstFight_140") {Animation="HereWeGo", Speaker = "Player", TurnSpeakersToFaceEachOther = false},
        new ("AnimaIsland_AfterFirstFight_150") {Animation="PointDown", Speaker = "Iris1", TurnSpeakersToFaceEachOther = false},
        new ("AnimaIsland_AfterFirstFight_160") {Animation="Determined", Speaker = "Player", TurnSpeakersToFaceEachOther = false},
    }){PlayerStartingPosition = new Vector2(-60f, -7.5f), PlayerStartingFlipped = false, ReturnUnitsToOriginalPositions=false};}

    public static void OnStart_AfterFirstFight() {
        Area.Instance.transform.Find("Cycle1/WindBarrier").gameObject.SetActive(false);
        Utils.GetUnit("Maginhart1").transform.position = new Vector2(-56f, -4.5f);
        Utils.GetUnit("Iris1").transform.position = new Vector2(-64f, -5);
        Utils.GetUnit("Iris1").Actions.IsFlipped = false;
    }

    public static void AnimaIsland_AfterFirstFight_50() {
        GameController.Instance.WaitAndRunMethod(1.5f, AnimaIsland_AfterFirstFight_50v2);
    }

    public static void AnimaIsland_AfterFirstFight_50v2() {
        CameraController.Instance.CenteredOnObject = Player.Instance.gameObject;
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Dialogue/ForbiddenKnowledgeWarning", 0.9f);
        Player.Instance.PlayAnimation("Surprised");
    }

    public static void AnimaIsland_AfterFirstFight_110() {
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Maginhart1").gameObject;
        Utils.GetUnit("Maginhart1").Actions.MoveToPoint(-46f, -2f, true);
        GameController.Instance.WaitAndRunMethod(1, AnimaIsland_AfterFirstFight_110v2);
    }

    public static void AnimaIsland_AfterFirstFight_110v2() {
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Iris1").gameObject;
        Utils.GetUnit("Iris1").PlayAnimation("FollowAfterMe");
    }

    public static void AnimaIsland_AfterFirstFight_120() {
        Utils.GetUnit("Maginhart1").gameObject.SetActive(false);
        Utils.GetUnit("Iris1").Actions.MoveToPoint(-53f, -5.5f);
        Player.Instance.Actions.MoveToPoint(-54f, -6.5f);
    }

    public static void OnEnd_AfterFirstFight() {
        SaveFile.Instance.GetQuest("Anima").AdvanceObjective(10, new() {SaveFile.Instance.AnimaAbilitiesCountered.Count.ToString()});
        Utils.GetUnit("Iris1").GetComponent<FollowPlayer>().enabled = true;
        Area.Instance.transform.Find("Cycle1/Units").gameObject.SetActive(true);
        StartAllDuels();
        foreach(Effect_ChangeStat eff in IrisBossBuffs) {
            eff.EndThisEffect();
        }
    }

    public static Dialogue MaginhartInfo() {
        return new Dialogue ("Area_AnimaIsland", "MaginhartInfo", new List<DialogueLine>{
        new ("AnimaIsland_MaginhartInfo_0") {Animation="Stoic", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfo_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("AnimaIsland_MaginhartInfoEnergy_0"),
            new ("AnimaIsland_MaginhartInfoIris_0"),
            new ("AnimaIsland_MaginhartInfoFamily_0"),
            new ("AnimaIsland_MaginhartInfoPillager_0"),
            new ("AnimaIsland_MaginhartInfoLeave_0") {IdOfNextDialogueLine="END"},
        }},
        new ("AnimaIsland_MaginhartInfoEnergy_10") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoEnergy_20") {Animation="PointDown", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoEnergy_30") {Animation="Irritated", Speaker = "Player", IdOfNextDialogueLine="AnimaIsland_MaginhartInfo_Choices"},
        new ("AnimaIsland_MaginhartInfoIris_10") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoIris_20") {Animation="Stoic", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoIris_30") {Animation="Listening", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoIris_40") {Animation="Frustrated", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoIris_50") {Animation="Flabbergasted", Speaker = "Player", IdOfNextDialogueLine="AnimaIsland_MaginhartInfo_Choices"},
        new ("AnimaIsland_MaginhartInfoFamily_10") {Animation="TryingToRemember", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoFamily_20") {Animation="Thinking", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoFamily_30") {Animation="Surprised", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoFamily_40") {Animation="NotQuite", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoFamily_50") {Animation="SternNo", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoFamily_60") {Animation="Approving", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoFamily_70") {Animation="Surprised", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoFamily_80") {Animation="Stoic", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoFamily_90") {Animation="Sigh", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoFamily_100") {Animation="Stoic", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoFamily_110") {Animation="Irritated", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoFamily_120") {Animation="Stoic", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoFamily_130") {Animation="Frustrated", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoFamily_140") {Animation="NotQuite", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoFamily_150") {Animation="Sigh", Speaker = "Player", IdOfNextDialogueLine="AnimaIsland_MaginhartInfo_Choices"},
        new ("AnimaIsland_MaginhartInfoPillager_10") {Animation="LookingAround", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoPillager_20") {Animation="Stoic", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoPillager_30") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_MaginhartInfoPillager_40") {Animation="Stoic", Speaker = "Maginhart2"},
        new ("AnimaIsland_MaginhartInfoPillager_50") {Animation="Sigh", Speaker = "Player", IdOfNextDialogueLine="AnimaIsland_MaginhartInfo_Choices"}
    }){PlayerStartingPosition = new Vector2(-20f, 55f), PlayerStartingFlipped = false, SpeakerStartingFlipped=true, ReturnUnitsToOriginalPositions=false};}

    public static void OnStart_MaginhartInfo() {
        Utils.GetUnit("Iris1").transform.position = new Vector2(-22f, 53f);
        Utils.GetUnit("Iris1").Actions.IsFlipped = false;
    }

    public static void AnimaIsland_MaginhartInfoIris_50() {
        Player.Instance.PlayAnimation("Surprised");
    }

    public static Dialogue WeaponPillagerShop() {
        return new Dialogue ("Area_AnimaIsland", "WeaponPillagerShop", new List<DialogueLine>{
        new ("AnimaIsland_WeaponPillagerShop_0") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_WeaponPillagerShop_10") {Animation="ShoulderShrug", Speaker = "WeaponPillager1"},
        new ("AnimaIsland_WeaponPillagerShop_20") {Animation="YesMe", Speaker = "Player"},
        new ("AnimaIsland_WeaponPillagerShop_30") {Animation="Stoic", Speaker = "WeaponPillager1"},
        new ("AnimaIsland_WeaponPillagerShop_40") {Animation="YesYou", Speaker = "Player"},
        new ("AnimaIsland_WeaponPillagerShop_50") {Animation="RespectfulBow", Speaker = "WeaponPillager1"},
        new ("AnimaIsland_WeaponPillagerShop_60") {Animation="Impatient", Speaker = "Player"},
        new ("AnimaIsland_WeaponPillagerShop_70") {Animation="Laugh", Speaker = "WeaponPillager1"},
        new ("AnimaIsland_WeaponPillagerShop_80") {Animation="Surprised", Speaker = "Player"},
        new ("AnimaIsland_WeaponPillagerShop_90") {Animation="Mocking", Speaker = "WeaponPillager1"},
        new ("AnimaIsland_WeaponPillagerShop_100") {Animation="Sigh", Speaker = "Player"},
        new ("AnimaIsland_WeaponPillagerShop_110") {Animation="Thinking", Speaker = "WeaponPillager1"}
    }){PlayerStartingPosition = new Vector2(-16.5f, 33f), PlayerStartingFlipped = false};}

    public static void OnEnd_WeaponPillagerShop() {
        Utils.GetUnit("WeaponPillager1").transform.Find("Shop").gameObject.SetActive(true);
    }

    public static Dialogue BlademasterPre() {
        return new Dialogue ("Area_AnimaIsland", "BlademasterPre", new List<DialogueLine>{
        new ("AnimaIsland_BlademasterPre_0") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_BlademasterPre_10") {Animation="Approving", Speaker = "AnimaBlademaster"},
        new ("AnimaIsland_BlademasterPre_20") {Animation="YesYou", Speaker = "Player"},
        new ("AnimaIsland_BlademasterPre_30") {Animation="HandWave", Speaker = "AnimaBlademaster"},
        new ("AnimaIsland_BlademasterPre_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("AnimaIsland_BlademasterPreFight_0"),
            new ("AnimaIsland_BlademasterPreNotYet_0") {IdOfNextDialogueLine="END"}
        }},
        new ("AnimaIsland_BlademasterPreFight_10") {Animation="FinallyDecided", Speaker = "Player"},
        new ("AnimaIsland_BlademasterPreFight_20") {Animation="Approving", Speaker = "AnimaBlademaster", IdOfNextDialogueLine="END"},

    }){PlayerStartingPosition = new Vector2(-34f, 56f), PlayerStartingFlipped = true};}

    public static void OnEnd_AnimaIsland_BlademasterPreFight_20() {
        GameController.Instance.SaveMidMissionInformation(new() {"Area_AnimaIsland.StartBlademasterDuel"});
        StartBlademasterDuel();
    }
    
    public static void StartBlademasterDuel() {
        Utils.GetUnit("Iris1").GetComponent<FollowPlayer>().IsObservingFromAfar = true;
        Utils.GetUnit("AnimaBlademaster").AttackPlayer();
        Utils.GetUnit("AnimaBlademaster").transform.Find("Dialogue").gameObject.SetActive(false);
    }

    public static string MostRecentDuelistDefeated;

    public static void StartIrisTalk(GameObject defeated_enemy) {
        Utils.GetUnit("Iris1").GetComponent<FollowPlayer>().IsObservingFromAfar = false;
        MostRecentDuelistDefeated = defeated_enemy.name;
        UIManager.Instance.StartDialogue(SaveFile.Instance.IrisTalksCompleted == 0 ? IrisTalk1() : SaveFile.Instance.IrisTalksCompleted == 1 ? IrisTalk2() : IrisTalk3());
    }

    public static Dialogue SpearmasterPre() {
        return new Dialogue ("Area_AnimaIsland", "SpearmasterPre", new List<DialogueLine>{
        new ("AnimaIsland_SpearmasterPre_0") {Animation="Surprised", Speaker = "Player"},
        new ("AnimaIsland_SpearmasterPre_10") {Speaker = "AnimaSpearmaster"},
        new ("AnimaIsland_SpearmasterPre_20") {Animation="SternNo", Speaker = "Player"},
        new ("AnimaIsland_SpearmasterPre_30") {Speaker = "AnimaSpearmaster"},
        new ("AnimaIsland_SpearmasterPre_40") {Animation="Flabbergasted", Speaker = "Player"},
        new ("AnimaIsland_SpearmasterPre_50") {Speaker = "AnimaSpearmaster"},
        new ("AnimaIsland_SpearmasterPre_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("AnimaIsland_SpearmasterPreFight_0"),
            new ("AnimaIsland_SpearmasterPreNotYet_0") {IdOfNextDialogueLine="END"}
        }},
        new ("AnimaIsland_SpearmasterPreFight_10") {Animation="ThreatenWithHeavy", Speaker = "Player"},
        new ("AnimaIsland_SpearmasterPreFight_20") {Animation="PreparingForBattle", Speaker = "AnimaSpearmaster", IdOfNextDialogueLine="END"},
    }){PlayerStartingPosition = new Vector2(23.5f, 63.5f), PlayerStartingFlipped = false};}

    public static void OnStart_SpearmasterPre() {
        Utils.GetUnit("AnimaSpearmaster").PlayAnimation("ThreatenWithHeavy");
    }

    public static Dialogue BowmasterPre() {
        return new Dialogue ("Area_AnimaIsland", "BowmasterPre", new List<DialogueLine>{
        new ("AnimaIsland_BowmasterPre_0") {Animation="GracefulBow", Speaker = "AnimaBowmaster"},
        new ("AnimaIsland_BowmasterPre_10") {Animation="LookingAround", Speaker = "Player"},
        new ("AnimaIsland_BowmasterPre_20") {Animation="Stoic", Speaker = "AnimaBowmaster"},
        new ("AnimaIsland_BowmasterPre_30") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_BowmasterPre_40") {Animation="NotQuite", Speaker = "AnimaBowmaster"},
        new ("AnimaIsland_BowmasterPre_50") {Animation="Sigh", Speaker = "Player"},
        new ("AnimaIsland_BowmasterPre_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("AnimaIsland_BowmasterPreFight_0"),
            new ("AnimaIsland_BowmasterPreNotYet_0") {IdOfNextDialogueLine="END"}
        }},
        new ("AnimaIsland_BowmasterPreFight_10") {Animation="FinallyDecided", Speaker = "Player"},
        new ("AnimaIsland_BowmasterPreFight_20") {Animation="Approving", Speaker = "AnimaBowmaster", IdOfNextDialogueLine="END"},
    }){PlayerStartingPosition = new Vector2(-21f, -18.5f), PlayerStartingFlipped = true};}

    public static Dialogue IrisTalk1() {
        return new Dialogue ("Area_AnimaIsland", "IrisTalk1", new List<DialogueLine>{
        new ("AnimaIsland_IrisTalk1_0") {Animation="Approving", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk1_10") {Animation="Sigh", Speaker = "Player"},
        new ("AnimaIsland_IrisTalk1_20") {Animation="Intrigued", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk1_30") {Animation="HereWeGo", Speaker = "Player"},
        new ("AnimaIsland_IrisTalk1_40") {Animation="Bashful", Speaker = "Iris1"},
    }){PlayerStartingPosition = MostRecentDuelistDefeated.Contains("AnimaBlademaster") ? new Vector2(-35f, 45f) : MostRecentDuelistDefeated.Contains("AnimaSpearmaster") ? new Vector2(21f, 57f) : new Vector2(-4f, -18f), PlayerStartingFlipped = true, DialogueSpeaker=Utils.GetUnit("Iris1"), SpeakerStartingPosition = MostRecentDuelistDefeated.Contains("AnimaBlademaster") ? new Vector2(-37f, 45f) : MostRecentDuelistDefeated.Contains("AnimaSpearmaster") ? new Vector2(19f, 57f) : new Vector2(-6f, -18f), SpeakerStartingFlipped = false};}

    public static void AnimaIsland_IrisTalk1_0() {
        Player.Instance.PlayAnimation("Exhausted");
    }

    public static void AnimaIsland_IrisTalk1_50() {
        Utils.GetUnit("Iris1").PlayAnimation("TryToStop");
    }

    public static void OnEnd_IrisTalk1() {
        Utils.GetUnit("Iris1").GetComponent<FollowPlayer>().enabled = true;
        SaveFile.Instance.IrisTalksCompleted++;
        if(SaveFile.Instance.IrisTalksCompleted == 3) {
            SaveFile.Instance.AddPermanentPowerUp("AnimaIsland_3MajorDuels", 10);
            if(SaveFile.Instance.GetQuest("Anima").CurrentObjective.Number == 10) {
                Utils.GetUnit("WeaponPillager1").gameObject.SetActive(false);
                Area.Instance.transform.Find("Interactables/PillagerAggro").gameObject.SetActive(true);
                SaveFile.Instance.GetQuest("Anima").AdvanceObjective(20);
            }
        }

    }

    public static Dialogue IrisTalk2() {
        return new Dialogue ("Area_AnimaIsland", "IrisTalk2", new List<DialogueLine>{
        new ("AnimaIsland_IrisTalk2_0") {Animation="Sigh", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk2_10") {Animation="DontThinkSo", Speaker = "Player"},
        new ("AnimaIsland_IrisTalk2_20") {Animation="Deflated", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk2_30") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_IrisTalk2_40") {Animation="Sigh", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk2_50") {Animation="FollowAfterMe", Speaker = "Player"},
    }){PlayerStartingPosition = MostRecentDuelistDefeated.Contains("AnimaBlademaster") ? new Vector2(-35f, 45f) : MostRecentDuelistDefeated.Contains("AnimaSpearmaster") ? new Vector2(21f, 57f) : new Vector2(-4f, -18f), PlayerStartingFlipped = true, DialogueSpeaker=Utils.GetUnit("Iris1"), SpeakerStartingPosition = MostRecentDuelistDefeated.Contains("AnimaBlademaster") ? new Vector2(-39f, 45f) : MostRecentDuelistDefeated.Contains("AnimaSpearmaster") ? new Vector2(17f, 57f) : new Vector2(-8f, -18f), SpeakerStartingFlipped = true};}

    public static void OnEnd_IrisTalk2() {
        Utils.GetUnit("Iris1").GetComponent<FollowPlayer>().enabled = true;
        SaveFile.Instance.IrisTalksCompleted++;
        if(SaveFile.Instance.IrisTalksCompleted == 3) {
            SaveFile.Instance.AddPermanentPowerUp("AnimaIsland_3MajorDuels", 10);
        }
    }

    public static Dialogue IrisTalk3() {
        return new Dialogue ("Area_AnimaIsland", "IrisTalk3", new List<DialogueLine>{
        new ("AnimaIsland_IrisTalk3_0") {Animation="Intrigued", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk3_10") {Animation="Exhausted", Speaker = "Player"},
        new ("AnimaIsland_IrisTalk3_20") {Animation="ShoulderShrug", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk3_30") {Animation="YesYou", Speaker = "Player"},
        new ("AnimaIsland_IrisTalk3_40") {Animation="Irritated", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk3_50") {Animation="Thinking", Speaker = "Player"},
        new ("AnimaIsland_IrisTalk3_60") {Animation="Surprised", Speaker = "Iris1"},
        new ("AnimaIsland_IrisTalk3_70") {Animation="Frustrated", Speaker = "Player"}
    }){PlayerStartingPosition = MostRecentDuelistDefeated.Contains("AnimaBlademaster") ? new Vector2(-35f, 45f) : MostRecentDuelistDefeated.Contains("AnimaSpearmaster") ? new Vector2(21f, 57f) : new Vector2(-4f, -18f), PlayerStartingFlipped = true, DialogueSpeaker=Utils.GetUnit("Iris1"), SpeakerStartingPosition = MostRecentDuelistDefeated.Contains("AnimaBlademaster") ? new Vector2(-37f, 45f) : MostRecentDuelistDefeated.Contains("AnimaSpearmaster") ? new Vector2(19f, 57f) : new Vector2(-6f, -18f), SpeakerStartingFlipped = false};}

    public static void OnEnd_IrisTalk3() {
        Utils.GetUnit("Iris1").GetComponent<FollowPlayer>().enabled = true;
        SaveFile.Instance.IrisTalksCompleted++;
        if(SaveFile.Instance.IrisTalksCompleted == 3) {
            SaveFile.Instance.AddPermanentPowerUp("AnimaIsland_3MajorDuels", 10);
        }
    }

    public static Dialogue Cycle1Finale() {
        return new Dialogue ("Area_AnimaIsland", "Cycle1Finale", new List<DialogueLine>{
        new ("AnimaIsland_Cycle1Finale_0") {Animation="Approving", Speaker = "Maginhart2"},
        new ("AnimaIsland_Cycle1Finale_10") {Animation="LookingAround", Speaker = "Player"},
        new ("AnimaIsland_Cycle1Finale_20") {Animation="GracefulBow", Speaker = "Maginhart2"},
        new ("AnimaIsland_Cycle1Finale_30") {Animation="Sigh", Speaker = "Iris2"},
        new ("AnimaIsland_Cycle1Finale_40") {Animation="ShoulderShrug", Speaker = "Maginhart2"},
        new ("AnimaIsland_Cycle1Finale_50"){IdOfNextDialogueLine="END"},
    }){PlayerStartingPosition = new Vector2(-20f, 55f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("Maginhart2"), SpeakerStartingPosition = new Vector2(-17f, 55f), SpeakerStartingFlipped=true};}

    public static void OnStart_Cycle1Finale() {
        Utils.GetUnit("Iris1").transform.position = new Vector2(-22f, 53f);
        Utils.GetUnit("Iris1").Actions.IsFlipped = false;
    }

    public static void AnimaIsland_Cycle1Finale_40() {
        UIManager.Instance.ShowBlackScreen(5);
    }

    public static void OnEnd_Cycle1Finale() {
        SaveFile.Instance.CurrentMission.Finish(true);
    }

    public static void SpiesAggro() {
        Utils.GetUnit("SalutisAssassin").AttackPlayer();
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_20", SpeakerUnit=Utils.GetUnit("SalutisAssassin")});
    }

    public static void SpiesDefeated() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_25"});
    }

    public static void SmugglersAggro() {
        Utils.GetUnit("Adventurer").AttackPlayer();
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_10", SpeakerUnit=Utils.GetUnit("Adventurer")});
    }

    public static void SmugglersDefeated() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_11", SpeakerName="Smuggler", SpeakerPortrait="Adventurer"});
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_12"}, 10);
    }

    public static void PillagerAggro() {
        Utils.GetUnit("WeaponPillagerBoss").gameObject.SetActive(true);
        Utils.GetUnit("WeaponPillagerBoss").AttackPlayer();
        Utils.GetUnit("WeaponPillagerBoss").Actions.UseAbility(typeof(NPCAbility_JumpAndExplosionImpale));
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_WeaponPillagerAttack_0", SpeakerUnit=Utils.GetUnit("WeaponPillagerBoss")});
        SaveFile.Instance.GetQuest("Anima").AdvanceObjective(30);
    }

    public static void InterruptDuel1() {
        Utils.GetUnit("AnimaRookie1").AttackPlayer();
        Utils.GetUnit("AnimaRookie2").AttackPlayer();
        Utils.GetUnit("AnimaRookie1").UnitAI.MaxMoveRange = null;
        Utils.GetUnit("AnimaRookie2").UnitAI.MaxMoveRange = null;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_71", SpeakerUnit=Utils.GetUnit("AnimaRookie1")});
    }

    public static void InterruptDuel2() {
        Utils.GetUnit("AnimaRookie3").AttackPlayer();
        Utils.GetUnit("AnimaRookie4").AttackPlayer();
        Utils.GetUnit("AnimaRookie3").UnitAI.MaxMoveRange = null;
        Utils.GetUnit("AnimaRookie4").UnitAI.MaxMoveRange = null;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_72", SpeakerUnit=Utils.GetUnit("AnimaRookie3")});
    }

    public static void InterruptDuel3() {
        Utils.GetUnit("AnimaRookie5").AttackPlayer();
        Utils.GetUnit("AnimaRookie6").AttackPlayer();
        Utils.GetUnit("AnimaRookie5").UnitAI.MaxMoveRange = null;
        Utils.GetUnit("AnimaRookie6").UnitAI.MaxMoveRange = null;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_73", SpeakerUnit=Utils.GetUnit("AnimaRookie5")});
    }

    public static void InterruptDuel4() {
        Utils.GetUnit("AnimaRookie7").AttackPlayer();
        Utils.GetUnit("AnimaRookie8").AttackPlayer();
        Utils.GetUnit("AnimaRookie7").UnitAI.MaxMoveRange = null;
        Utils.GetUnit("AnimaRookie8").UnitAI.MaxMoveRange = null;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_74", SpeakerUnit=Utils.GetUnit("AnimaRookie7")});
    }

    public static void InterruptDuel5() {
        Utils.GetUnit("AnimaRookie9").AttackPlayer();
        Utils.GetUnit("AnimaRookie10").AttackPlayer();
        Utils.GetUnit("AnimaRookie9").UnitAI.MaxMoveRange = null;
        Utils.GetUnit("AnimaRookie10").UnitAI.MaxMoveRange = null;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_75", SpeakerUnit=Utils.GetUnit("AnimaRookie9")});
    }

    public static void InterruptDuel6() {
        Utils.GetUnit("AnimaGuardian1").AttackPlayer();
        Utils.GetUnit("AnimaGuardian2").AttackPlayer();
        Utils.GetUnit("AnimaGuardian1").UnitAI.MaxMoveRange = null;
        Utils.GetUnit("AnimaGuardian2").UnitAI.MaxMoveRange = null;
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_70", SpeakerUnit=Utils.GetUnit("AnimaGuardian1")});
    }

    public static void IrisExtraTalk1() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_80", SpeakerUnit=Utils.GetUnit("Iris1")});
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_81"}, 5);
    }

    public static void IrisExtraTalk2() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_90", SpeakerUnit=Utils.GetUnit("Iris1")});
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_91"}, 5);
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_92", SpeakerUnit=Utils.GetUnit("Iris1")}, 10);
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_93"}, 15);
    }

    public static void IrisExtraTalk3() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_100", SpeakerUnit=Utils.GetUnit("Iris1")});
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_101"}, 5);
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_102", SpeakerUnit=Utils.GetUnit("Iris1")}, 10);
    }

    public static void IrisExtraTalk4() {
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_110", SpeakerUnit=Utils.GetUnit("Iris1")});
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_111"}, 5);
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle1Interactions_112", SpeakerUnit=Utils.GetUnit("Iris1")}, 10);
    }








    // CYCLE 2

    public static Dialogue InitialConfrontationCycle2() {
        return new Dialogue ("Area_AnimaIsland", "InitialConfrontationCycle2", new List<DialogueLine>{
        new ("AnimaIsland_InitialConfrontationCycle2_0") {Animation="Appear", Speaker = "Player", WaitTimeBeforeAllowingToProceed = 1.5f},
        new ("AnimaIsland_InitialConfrontationCycle2_10") {Animation="YesYou", Speaker = "AnimaSuperGuardian1"},
        new ("AnimaIsland_InitialConfrontationCycle2_20") {Animation="Irritated", Speaker = "Player"},
        new ("AnimaIsland_InitialConfrontationCycle2_30") {Animation="GracefulBow", Speaker = "AnimaSuperGuardian1"},
        new ("AnimaIsland_InitialConfrontationCycle2_40") {Animation="FinallyDecided", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(-54f, -4f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("AnimaSuperGuardian1"), ReturnUnitsToOriginalPositions = false};}

    public static void AnimaIsland_InitialConfrontationCycle2_0() {
        GameController.Instance.WaitAndRunMethod(1, AnimaIsland_InitialConfrontationCycle2_0v2);
    }

    public static void AnimaIsland_InitialConfrontationCycle2_0v2() {
        Player.Instance.PlayAnimation("Surprised");
    }

    public static void BarrierGenerator(InteractableObject obj) {
        Area.Instance.transform.Find("Interactables/BarrierGenerator/Lootable Weapon_2").gameObject.SetActive(true);
        Area.Instance.transform.Find("Cycle2/WindBarrier_2").gameObject.SetActive(true);
    }

    public static void PillagerAggroCycle2() {
        Utils.GetUnit("WeaponPillagerBossCycle2").gameObject.SetActive(true);
        Utils.GetUnit("WeaponPillagerBossCycle2").AttackPlayer();
        Utils.GetUnit("WeaponPillagerBossCycle2").Actions.UseAbility(typeof(NPCAbility_JumpAndExplosionImpale));
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle2Interactions_20", SpeakerUnit=Utils.GetUnit("WeaponPillagerBossCycle2")});
    }

    public static void TryToBreakPast() {
        foreach(Unit u in Utils.GetSpecifiedUnits(unit => unit.name.Contains("Guardian") && unit.gameObject.activeInHierarchy)) {
            u.AttackPlayer();
        }
        Area.Instance.transform.Find("Cycle2/WindBarrier").gameObject.SetActive(false);
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle2Interactions_40", SpeakerUnit=Utils.GetUnit("Unit_AnimaSuperGuardian1")});
    }

    public static void CarveOpenAPath() {
        GameController.Instance.GetComponent<PlayerInput>().enabled = false;
        Player.Instance.Actions.IsFlipped = false;
        Player.Instance.PlayAnimation("SwordSweep", 0.05f, 0.33f);
        GameController.Instance.WaitAndRunMethod(0.65f, DestroyRocks);
    }

    public static void DestroyRocks() {
        foreach(Unit u in Utils.GetSpecifiedUnits(unit => unit.name.Contains("Guardian") && unit.gameObject.activeInHierarchy)) {
            u.Faction = Constants.Faction.Enemy;
            u.Actions.UseAbility(typeof(AI_RoamAround));
        }
        Area.Instance.transform.Find("Cycle2/WindBarrier").gameObject.SetActive(false);
        GameController.Instance.GetComponent<PlayerInput>().enabled = true;
        foreach(DestructibleEnvironment dest in Area.Instance.transform.Find("Interactables/Environment_DestructibleWall").GetComponentsInChildren<DestructibleEnvironment>(true)) {
            dest.enabled = true;
            dest.DestroyObject();
        }
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Hit/Rock_Destroy1", 1);
        MonoBehaviour.Destroy(Area.Instance.transform.Find("Interactables/Environment_DestructibleWall/Collider").gameObject);
        NotificationController.ShowCustomizedDialogueNotification(new () {Id ="AnimaIsland_Cycle2Interactions_40", SpeakerUnit=Utils.GetUnit("Unit_AnimaSuperGuardian1")});
    }

    public static void MemoryPieceInteract(InteractableObject obj) {
        if((obj.gameObject.name == "Memory Core2" && SaveFile.Instance.IrisMemoryPiecesFound < 1) || (obj.gameObject.name == "Memory Core3" && SaveFile.Instance.IrisMemoryPiecesFound < 2) || (obj.gameObject.name == "Memory Core4" && SaveFile.Instance.IrisMemoryPiecesFound < 3)) {
            NotificationController.ShowDialogueNotification("AnimaIsland_Cycle2Interactions_60");
        }
        else {
            obj.transform.parent.GetComponent<DestructibleEnvironment>().DestroyObject();
            SaveFile.Instance.IrisMemoryPiecesFound++;
            UIManager.Instance.StartDialogue(SaveFile.Instance.IrisMemoryPiecesFound == 1 ? IrisExplanation1() : SaveFile.Instance.IrisMemoryPiecesFound == 2 ? IrisExplanation2() : SaveFile.Instance.IrisMemoryPiecesFound == 3 ? IrisExplanation3() : SaveFile.Instance.IrisMemoryPiecesFound == 4 ? IrisExplanation4() : null);
        }
    }

    public static Dialogue IrisExplanation1() {
        return new Dialogue ("Area_AnimaIsland", "IrisExplanation1", new List<DialogueLine>{
        new ("AnimaIsland_IrisExplanation1_0") {Animation="PickUpItemFromTheGround", Speaker = "Player", WaitTimeBeforeAllowingToProceed=1.5f},
        new ("AnimaIsland_IrisExplanation1_10") {Animation="GracefulBow", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation1_20") {Animation="Intrigued", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation1_30") {Animation="Bashful", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation1_40") {Animation="ComposeOneself", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation1_50") {Animation="Thinking", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation1_60") {Animation="HereWeGo", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(-53.5f, -35f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static void AnimaIsland_IrisExplanation1_0() {
        GameController.Instance.WaitAndRunMethod(1f, TransportToMindspace);
    }

    public static void TransportToMindspace() {
        UIManager.Instance.ShowBlackScreen(0.1f);
        GameController.Instance.WaitAndRunMethod(0.5f, TransportToMindspacev2);
    }

    public static void TransportToMindspacev2() {
        Player.Instance.UnitAI.NavMeshAgent.enabled= false;
        Player.Instance.transform.position = new Vector2(196.5f, 200f);
        Player.Instance.UnitAI.NavMeshAgent.enabled= true;
        Player.Instance.PlayAnimation("Surprised");
        UIManager.Instance.HideBlackScreen(0.1f);
    }

    public static Dialogue IrisExplanation2() {
        return new Dialogue ("Area_AnimaIsland", "IrisExplanation2", new List<DialogueLine>{
        new ("AnimaIsland_IrisExplanation2_0") {Animation="HeavilyWounded", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation2_10") {Animation="Stoic", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation2_20") {Animation="TryToStop", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation2_30") {Animation="SternNo", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation2_40") {Animation="ComposeOneself", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation2_50") {Animation="Frustrated", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation2_60") {Animation="YesYou", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation2_70") {Animation="YesYou", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static void MoveToFloodedCaverns() {
        Utils.MoveIntoArea(false, "AnimaIsland_FloodedCaverns");
        Utils.ShouldStartFlipped = true;
    }

    public static Dialogue IrisExplanation3() {
        return new Dialogue ("Area_AnimaIsland_FloodedCaverns", "IrisExplanation3", new List<DialogueLine>{
        new ("AnimaIsland_IrisExplanation3_0") {Animation="HeavilyWounded", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation3_10") {Animation="Stoic", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation3_20") {Animation="TryToStop", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation3_30") {Animation="SternNo", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation3_40") {Animation="SmileThroughPain", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation3_50") {Animation="Frustrated", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation3_60") {Animation="YesYou", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation3_70") {Animation="YesYou", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation3_80") {Animation="YesYou", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static Dialogue IrisExplanation4() {
        return new Dialogue ("Area_AnimaIsland_FloodedCaverns", "IrisExplanation4", new List<DialogueLine>{
        new ("AnimaIsland_IrisExplanation4_0") {Animation="HeavilyWounded", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation4_10") {Animation="Stoic", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation4_20") {Animation="TryToStop", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation4_30") {Animation="SternNo", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation4_40") {Animation="SmileThroughPain", Speaker = "Player"},
        new ("AnimaIsland_IrisExplanation4_50") {Animation="Frustrated", Speaker = "InfoDumpIris"},
        new ("AnimaIsland_IrisExplanation4_60") {Animation="YesYou", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static Dialogue Cycle2Confrontation() {
        return new Dialogue ("Area_AnimaIsland_FloodedCaverns", "Cycle2Confrontation", new List<DialogueLine>{
        new ("AnimaIsland_Cycle2Confrontation_0") {Speaker = "Iris1", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle2Confrontation_10") {Animation="Stoic", Speaker = "Maginhart1", TurnSpeakersToFaceEachOther=false},
        new ("AnimaIsland_Cycle2Confrontation_20") {Animation="HeavilyWounded", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2Confrontation_30") {Animation="Explaining", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2Confrontation_40") {Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle2Confrontation_50") {Animation="Approving", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2Confrontation_60") {Animation="YesYou", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2Confrontation_70") {Animation="Sigh", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2Confrontation_80") {Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle2Confrontation_90") {Animation="Accusing", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2Confrontation_Choices") {Animation="Thinking", Speaker = "Player", Choices= new List<DialogueChoice> {
            new ("AnimaIsland_Cycle2ConfrontationIris_0"),
            new ("AnimaIsland_Cycle2ConfrontationMaginhart_0")
        }},
        new ("AnimaIsland_Cycle2ConfrontationIris_10") {Animation="FinallyDecided", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationIris_20") {Animation="Stoic", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2ConfrontationIris_30") {Animation="ThreatenWithHeavy", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationIris_40") {Animation="ThreatenWithLight", Speaker = "Maginhart1", IdOfNextDialogueLine="END"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhart_10") {Animation="FinallyDecided", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhart_20") {Animation="ComposeOneself", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhart_30") {Animation="ThreatenWithLight", Speaker = "Iris1", IdOfNextDialogueLine="END"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static void AnimaIsland_Cycle2Confrontation_0() {
        CameraController.Instance.CenteredOnObject = Utils.GetUnit("Iris1").gameObject;
    }

    public static Dialogue Cycle2ConfrontationIrisFinale() {
        return new Dialogue ("Area_AnimaIsland_FloodedCaverns", "Cycle2ConfrontationIrisFinale", new List<DialogueLine>{
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_0") {Animation="HeavilyWounded", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_10") {Animation="GracefulBow", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_20") {Animation="HandwaveTheIssue", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_30") {Animation="Irritated", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_40") {Animation="TryingToRemember", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_50") {Animation="YesMe", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_60") {Animation="FinallyDecided", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_70") {Animation="ShoulderShrug", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_80") {Animation="NotQuite", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_90") {Animation="HandwaveTheIssue", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationIrisFinale_100") {Speaker = "Iris1", ShowSpeakerBox=false},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static Dialogue Cycle2ConfrontationMaginhartFinale() {
        return new Dialogue ("Area_AnimaIsland_FloodedCaverns", "Cycle2ConfrontationMaginhartFinale", new List<DialogueLine>{
        new ("AnimaIsland_Cycle2ConfrontationMaginhartFinale_0") {Animation="HeavilyWounded", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhartFinale_10") {Animation="TendingToPlants", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhartFinale_20") {Animation="Frustrated", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhartFinale_30") {Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhartFinale_40") {Animation="SternNo", Speaker = "Player"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhartFinale_50") {Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle2ConfrontationMaginhartFinale_60") {Animation="YesYou", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    







    // CYCLE 3

    public static Dialogue Cycle3Intro() {
        return new Dialogue ("Area_AnimaIsland", "Cycle3Intro", new List<DialogueLine>{
        new ("AnimaIsland_Cycle3Intro_0") {Animation="Surprised", Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle3Intro_10") {Animation="PreparingForBattle", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false};}

    public static Dialogue SpireFloor1() {
        return new Dialogue ("Area_AnimaIsland_TheSpire", "SpireFloor1", new List<DialogueLine>{
        new ("AnimaIsland_SpireFloor1_0") {Animation="Headache", Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_SpireFloor1_10") {Animation="SmileThroughPain", Speaker = "Iris1"},
        new ("AnimaIsland_SpireFloor1_20") {Animation="SadChuckle", Speaker = "Iris1"},
        new ("AnimaIsland_SpireFloor1_30") {Animation="PreparingForBattle", Speaker = "Iris1"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("Iris1"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static Dialogue SpireFloor2() {
        return new Dialogue ("Area_AnimaIsland_TheSpire", "SpireFloor2", new List<DialogueLine>{
        new ("AnimaIsland_SpireFloor2_0") {Animation="Surprised", Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_SpireFloor2_10") {Animation="PreparingForBattle", Speaker = "Player"},
        new ("AnimaIsland_SpireFloor2_20") {Animation="PreparingForBattle", Speaker = "Player"},
        new ("AnimaIsland_SpireFloor2_30") {Animation="PreparingForBattle", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("Iris1"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static Dialogue SpireFloor3() {
        return new Dialogue ("Area_AnimaIsland_TheSpire", "SpireFloor3", new List<DialogueLine>{
        new ("AnimaIsland_SpireFloor3_0") {Animation="Surprised", Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_SpireFloor3_10") {Animation="PreparingForBattle", Speaker = "Player"},
        new ("AnimaIsland_SpireFloor3_20") {Animation="PreparingForBattle", Speaker = "Player"},
        new ("AnimaIsland_SpireFloor3_30") {Animation="PreparingForBattle", Speaker = "Player"},
        new ("AnimaIsland_SpireFloor3_40") {Animation="PreparingForBattle", Speaker = "Player"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static Dialogue Cycle3Duel() {
        return new Dialogue ("Area_AnimaIsland", "Cycle3Duel", new List<DialogueLine>{
        new ("AnimaIsland_Cycle3Duel_0") {Speaker = "Iris1", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle3Duel_10") {Animation="Stoic", Speaker = "Maginhart1", TurnSpeakersToFaceEachOther=false},
        new ("AnimaIsland_Cycle3Duel_20") {Animation="HeavilyWounded", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle3Duel_30") {Animation="Explaining", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle3Duel_40") {Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle3Duel_50") {Animation="Approving", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle3Duel_60") {Animation="YesYou", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle3Duel_70") {Animation="Sigh", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle3Duel_80") {Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle3Duel_90") {Animation="Accusing", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle3Duel_100") {Animation="Sigh", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle3Duel_110") {Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle3Duel_120") {Animation="Accusing", Speaker = "Iris1"},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}

    public static Dialogue Cycle3Finale() {
        return new Dialogue ("Area_AnimaIsland", "Cycle3Finale", new List<DialogueLine>{
        new ("AnimaIsland_Cycle3Finale_0") {Speaker = "Iris1", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle3Finale_10") {Animation="Stoic", Speaker = "Maginhart1", TurnSpeakersToFaceEachOther=false},
        new ("AnimaIsland_Cycle3Finale_20") {Animation="HeavilyWounded", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle3Finale_30") {Animation="Explaining", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle3Finale_40") {Speaker = "Player", ShowSpeakerBox=false},
        new ("AnimaIsland_Cycle3Finale_50") {Animation="Approving", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle3Finale_60") {Animation="YesYou", Speaker = "Maginhart1"},
        new ("AnimaIsland_Cycle3Finale_70") {Animation="Sigh", Speaker = "Iris1"},
        new ("AnimaIsland_Cycle3Finale_80") {Speaker = "Player", ShowSpeakerBox=false},
    }){PlayerStartingPosition = new Vector2(196.5f, 200f), PlayerStartingFlipped = false, DialogueSpeaker=Utils.GetUnit("InfoDumpIris"), SpeakerStartingPosition = new Vector2(200, 200f), SpeakerStartingFlipped=true};}
}
