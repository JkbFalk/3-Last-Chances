using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
    private static NotificationController _instance = null;
    public static NotificationController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameController.Instance.GetComponent<NotificationController>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    public static List<InGameDialogue> DialogueQueue = new List<InGameDialogue>();
    public static void ShowTextNotification(string text, List<string> string_params = null)
    {
        if(string.IsNullOrEmpty(text) == false)
        {
            for(int i = 0; i < (GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene ? 2 : 1); i++) {
                GameObject notification = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_NotificationOnlyText")) as GameObject;
                if(string_params != null) {
                    notification.transform.Find("Background/Text").GetComponent<LabelInitializer>().string_params = string_params;
                }
                notification.transform.SetParent(i == 1 ? GameController.Objects.DialogueNotifications.transform : GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu ? MenuManager.Objects.Notifications.transform : UIManager.Objects.NotificationList.transform);
                notification.transform.Find("Background/Text").GetComponent<LabelInitializer>().SetLabel(Label.ContainsKey(text) ? ("{" + text + "}") : text);
                if(i == 0) {
                    MenuManager.Instance.AddHistoryEntry(notification.transform.Find("Background/Text").GetComponent<TextMeshProUGUI>().text);
                }
                notification.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                Instance.WaitAndRunMethodRealtime(0.01f, AdjustNotificationWidth, notification);
                notification.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public static void AdjustNotificationWidth(GameObject go) {
        if(go!= null && go.transform.Find("Background/Text").GetComponent<RectTransform>().sizeDelta.x > 1160) {
            go.transform.Find("Background/Text").GetComponent<ContentSizeFitter>().enabled = false;
            go.transform.Find("Background/Text").GetComponent<RectTransform>().sizeDelta = new Vector2(1160, 0);
            Instance.WaitAndRunMethodRealtime(0.01f, AdjustNotificationHeight, go);
        }
        else if(go!= null) {
            go.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = new Vector2(go.transform.Find("Background/Text").GetComponent<RectTransform>().sizeDelta.x + 40, 0);
            Instance.WaitAndRunMethodRealtime(0.01f, AdjustNotificationHeight, go);
        }
    }

    public static void AdjustNotificationHeight(GameObject go) {
        if(go== null) {
            return;
        }
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(go.transform.Find("Background/Text").GetComponent<RectTransform>().sizeDelta.x + 40, go.transform.Find("Background/Text").GetComponent<RectTransform>().sizeDelta.y);
        go.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = new Vector2(go.transform.Find("Background/Text").GetComponent<RectTransform>().sizeDelta.x + 40, go.transform.Find("Background/Text").GetComponent<RectTransform>().sizeDelta.y);
        Instance.WaitAndRunMethodRealtime(0.01f, RefreshNotificationsLayout, null);
    }

    public static void RefreshNotificationsLayout(GameObject fill) {
        LayoutRebuilder.ForceRebuildLayoutImmediate(MenuManager.Objects.Notifications.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(UIManager.Objects.NotificationList.GetComponent<RectTransform>());
        if(GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect && SceneManager.GetActiveScene().name == "MissionSelect") {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Utils.GetSceneRootObject("Mission Select").Find("Notifications/List").GetComponent<RectTransform>());
        }
    }

    public static void ShowDialogueNotification(string line_id, string speaker_name = "Player", string speaker_portrait = "Player") {
        ShowCustomizedDialogueNotification(new InGameDialogue() {Id = line_id, SpeakerName = speaker_name, SpeakerPortrait = speaker_portrait});
    }

    public static void ShowCustomizedDialogueNotification(InGameDialogue dialogue, float queue_time = 0) {
        if(DialogueQueue.FirstOrDefault(notif => notif.Id == dialogue.Id) != null) {
            return;
        }
        if(dialogue.SpeakerUnit == null && (String.IsNullOrWhiteSpace(dialogue.SpeakerName) || dialogue.SpeakerName == "Default" || dialogue.SpeakerName == "Player")) {
            dialogue.SpeakerUnit = Player.Instance;
            dialogue.SpeakerName = Utils.GetDisplayedNameForName(Player.Instance.DisplayedName);
            dialogue.SpeakerPortrait = Player.Instance.Portrait;
            DialogueQueue.Add(dialogue);
        }
        else {
            dialogue.SpeakerPortrait = String.IsNullOrWhiteSpace(dialogue.SpeakerPortrait) && dialogue.SpeakerUnit != null ? dialogue.SpeakerUnit.Portrait : dialogue.SpeakerPortrait;
            DialogueQueue.Add(dialogue);
        }
        UIManager.Objects.InGameDialogueHideOrShow.gameObject.SetActive(true);
        if(DialogueQueue.Count == 1) {
            DisplayDialogueNotification(DialogueQueue[0]);
        }
        else if(DialogueQueue[0].FramesRemaining > 11){
            DialogueQueue[0].FramesRemaining = (int)(queue_time * 50 * Settings.Instance.DialogueTextSpeed / 10) + 11;
        }
    }

    private static void RevealExtraNotificationCharacter(string dialogue_id) {
        if(DialogueQueue.Count == 0 || UIManager.Objects.InGameDialogueText.maxVisibleCharacters >= UIManager.Objects.InGameDialogueText.text.Length || dialogue_id != DialogueQueue[0].Id) {
            if(DialogueQueue.Count > 0 && dialogue_id == DialogueQueue[0].Id) {
                UIManager.Objects.InGameDialogueText.maxVisibleCharacters = UIManager.Objects.InGameDialogueText.text.Length;
            }
            return;
        }
        UIManager.Objects.InGameDialogueText.maxVisibleCharacters++;
        string currentChar = Utils.ConvertCharacterFromForeignLanguages(UIManager.Objects.InGameDialogueText.text[UIManager.Objects.InGameDialogueText.maxVisibleCharacters - 1].ToString().ToUpper());
        if(Constants.VOWELS.Contains(currentChar)) {
            GameController.Objects.AudioListener.pitch = (DialogueQueue[0].SpeakerUnit == null ? 1 : DialogueQueue[0].SpeakerUnit.VoicePitch) + UnityEngine.Random.Range(-0.1f, 0.1f);
            GameController.Objects.AudioListener.PlayOneShot(GameController.Instance.SpeechBeepClips[(DialogueQueue[0].SpeakerUnit == null ? "Player" : DialogueQueue[0].SpeakerUnit is Player ? "Player" : DialogueQueue[0].SpeakerUnit.IsMale ? "Male" : "Female") + currentChar]);
        }
        GameController.Instance.WaitAndRunMethodRealtime(0.03f / Settings.Instance.DialogueTextSpeed * 10, RevealExtraNotificationCharacter, dialogue_id);
    }

    public void FixedUpdate() {
        if(DialogueQueue.Count > 0) {
            DialogueQueue[0].FramesRemaining--;
            if(DialogueQueue[0].FramesRemaining == 10) {
                UIManager.Objects.InGameDialogueHideOrShow.HideOverTimeFromFull(0.15f);
            }
            else if(DialogueQueue[0].FramesRemaining <= 0) {
                DialogueQueue.RemoveAt(0);
                if(DialogueQueue.Count > 0) {
                    DisplayDialogueNotification(DialogueQueue[0]);
                }
                else {
                    UIManager.Objects.InGameDialogueHideOrShow.gameObject.SetActive(false);
                }
            }
        }
    }

    private static void DisplayDialogueNotification(InGameDialogue dialogue) {
        MenuManager.Instance.AddHistoryEntry(dialogue);
        UIManager.Objects.InGameDialogueText.text = Label.ContainsKey(dialogue.Id) ? Label.Get(dialogue.Id) : dialogue.Id;
        dialogue.FramesRemaining = (int)(100 + UIManager.Objects.InGameDialogueText.text.Length * 4f / (Settings.Instance.DialogueTextSpeed * 0.2f));
        UIManager.Objects.InGameDialoguePortraitImage.sprite = Resources.Load("Sprites/Face Portrait/" + (dialogue.SpeakerPortrait == "Default" || String.IsNullOrWhiteSpace(dialogue.SpeakerPortrait) ? "Default_Male" : dialogue.SpeakerPortrait), typeof(Sprite)) as Sprite;
        UIManager.Objects.InGameDialoguePortraitTitle.text = Utils.GetNameForUnit(dialogue.SpeakerUnit, dialogue.SpeakerName);
        UIManager.Objects.InGameDialogueHideOrShow.ShowOverTimeFromZero(0.4f);
        if(Settings.Instance.DialogueTextSpeed < Constants.MAX_DIALOGUE_SPEED) {
            UIManager.Objects.InGameDialogueText.maxVisibleCharacters = 0;
            RevealExtraNotificationCharacter(DialogueQueue[0].Id);
        }
    }

    
    public static void ShowNotificationWithGraphic(string label, string path_to_graphic, List<string> string_params = null) {
        Sprite graphic = Resources.Load("Sprites/" + path_to_graphic, typeof(Sprite)) as Sprite;
        ShowNotificationWithGraphic(label, graphic, string_params);
    }

    public static void ShowNotificationWithGraphic(string label, Sprite graphic, List<string> string_params = null) {
        for(int i = 0; i < (GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene ? 2 : 1); i++) {
            GameObject notification = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_NotificationWithGraphic")) as GameObject;
            notification.transform.SetParent(i == 1 ? GameController.Objects.DialogueNotifications.transform : GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu ? MenuManager.Objects.Notifications.transform : GameController.Instance.GameplayMode == Constants.GameplayMode.MissionSelect ? Utils.GetSceneRootObject("Mission Select").Find("Notifications/List").transform : UIManager.Objects.NotificationList.transform);
            if(string_params != null) {
                notification.transform.Find("Background/Text").GetComponent<LabelInitializer>().string_params = string_params;
            }
            notification.transform.Find("Background/Text").GetComponent<LabelInitializer>().SetLabel(Label.ContainsKey(label) ? ("{" + label + "}") : Utils.InsertLabelsIntoText(label));
            notification.transform.Find("Background/Text/Image").GetComponent<Image>().sprite = graphic;
            if(i == 0) {
                MenuManager.Instance.AddHistoryEntry(notification.transform.Find("Background/Text").GetComponent<TextMeshProUGUI>().text, notification.transform.Find("Background/Text/Image").GetComponent<Image>().sprite);
            }
            notification.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            Instance.WaitAndRunMethodRealtime(0.01f, AdjustNotificationWidth, notification);
            notification.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void WaitAndRunMethodRealtime(float seconds, Action<GameObject>  nameOfMethodToRun, GameObject game_object)
    {
        StartCoroutine(WaitAndRunMethodCoroutineRealtime(seconds, () => nameOfMethodToRun(game_object)));
    }

    private IEnumerator WaitAndRunMethodCoroutineRealtime(float seconds, Action nameOfMethodToRun)
    {
        yield return new WaitForSecondsRealtime(seconds);
        nameOfMethodToRun();
    }
 
    public static void ShowItemDropNotification(Item item, int sold_for = 0) {
        if(item != null)
        {
            for(int i = 0; i < (GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene ? 2 : 1); i++) {
                GameObject notification = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_NotificationWithGraphic")) as GameObject;
                notification.transform.SetParent(i == 1 ? GameController.Objects.DialogueNotifications.transform : GameController.Instance.GameplayMode == Constants.GameplayMode.InMenu ? MenuManager.Objects.Notifications.transform : UIManager.Objects.NotificationList.transform);
                string start_label = "{";
                
                if (item.Type == Constants.ItemType.Heavy || item.Type == Constants.ItemType.Light || item.Type == Constants.ItemType.Ranged)
                {
                    start_label += item.GetType().ToString() + "} ({ItemGrade_" + item.Grade.ToString() + "_Colored} {ItemClass_" + item.WeaponClass.ToString() + "}): ";
                }
                else
                {
                    start_label += item.GetType().ToString() + "} (" + (item.Grade == Item.ItemGrade.None ? "" : "{ItemGrade_" + item.Grade.ToString() + "_Colored} ") + ": ";
                }
                if(item.Type == Constants.ItemType.Tool && sold_for > 0) {
                    Item existing_item = SaveFile.Instance.Inventory.FirstOrDefault(i => i.GetType() == item.GetType() && i.Grade == item.Grade);
                    int amountAwayFromMax = existing_item.MaxAmount - existing_item.Amount;
                    start_label += string.Format(Label.Get("ItemToolAutoSellNotification"), new object[] {amountAwayFromMax, item.Amount - amountAwayFromMax, sold_for.ToString()});
                }
                else {
                    start_label += (sold_for <= 0 ? Label.Get("ItemDropNotification") :  string.Format(Label.Get( "ItemAutoSellNotification"), new object[] {sold_for.ToString()}));
                }
                notification.transform.Find("Background/Text").GetComponent<LabelInitializer>().SetLabel(start_label);
                MenuManager.Instance.SetRegularImage(notification.transform.Find("Background/Text/Image").gameObject, item);
                if(item.Type == Constants.ItemType.Tool || (item.Type == Constants.ItemType.Quest && item.Amount > 1)) {
                    notification.transform.Find("Background/Text/Image/Amount").GetComponent<TextMeshProUGUI>().text = "x" + item.Amount.ToString();
                }
                if(i == 0) {
                    MenuManager.Instance.AddHistoryEntry(notification.transform.Find("Background/Text").GetComponent<TextMeshProUGUI>().text, notification.transform.Find("Background/Text/Image").GetComponent<Image>().sprite);
                }
                notification.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                Instance.WaitAndRunMethodRealtime(0.01f, AdjustNotificationWidth, notification);
                notification.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public static void ShowQuestStatusUpdateNotification(Quest quest) {
        if(quest.Status == Quest.QuestStatus.NotStarted) {
            return;
        }
        ShowNotificationWithGraphic("<b>{" + quest.GetType() + "}</b> {QuestStatusUpdate_" + quest.Status.ToString() + "}", quest.Icon);
    }

    public static void ShowQuestObjectiveUpdateNotification(Quest quest, QuestObjective objective) {
        if(objective.Status == QuestObjective.ObjectiveStatus.NotRevealed) {
            return;
        }
        ShowNotificationWithGraphic("<b>{" + quest.GetType() + "}</b> {QuestObjectiveUpdate_" + objective.Status.ToString() + "}: " + Label.Get(quest.GetType() + "_" + objective.Number), quest.Icon);
    }

    public class InGameDialogue {
        public string Id;
        public string SpeakerName;
        public string SpeakerPortrait;
        public Unit SpeakerUnit;
        public int FramesRemaining = 0;
        public InGameDialogue() {}
    }
}
