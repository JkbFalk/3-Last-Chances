using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class Quest {

    [Serializable]
    public enum QuestStatus { NotStarted, InProgress, Completed };
    
    public QuestStatus Status = QuestStatus.NotStarted;
    [SerializeField]
    public QuestObjective CurrentObjective {
        get {
            return Objectives.FirstOrDefault(obj => obj.Status == QuestObjective.ObjectiveStatus.Current);
        }
    }

    public void UpdateStatus(QuestStatus new_status, bool show_notification = true) {
        QuestStatus previousValue = Status;
        Status = new_status;
        Utils.CreateAuditLog("Quest (" + GetType() + ") status updated: " + previousValue + " -> " + new_status);
        if(previousValue == QuestStatus.NotStarted && (new_status == QuestStatus.InProgress || new_status == QuestStatus.Completed)) {
            if(show_notification) {
                NotificationController.ShowQuestStatusUpdateNotification(this);
                Utils.PlaySoundEffect(null, "UI/QuestStarted", 0.8f);
            }
            StartedTime = DateTime.Now;
        }
        else if(previousValue == QuestStatus.Completed && new_status == QuestStatus.Completed && show_notification) {
            Utils.PlaySoundEffect(null, "UI/QuestCompleted", 0.8f);
        }
    }
    public DateTime StartedTime;
    public string Title { get; set; }
    public string Icon;
    private string _displayedTitle;
    public bool UniqueQuest = true;
    public bool IsMainQuest = true;

    public string Id;

    public Quest() {
        Id =  GetType() + "-" + Guid.NewGuid().ToString();
    }

    public string DisplayedTitle {
        get => _displayedTitle;
        set {
            if (_displayedTitle != value) {
                _displayedTitle = value;
                GameObject questDisplayItem = CanvasElements.UICanvas.ObjectivesDisplay.transform.Find(GetType().Name)?.gameObject;
                if (questDisplayItem != null) {
                    questDisplayItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _displayedTitle;
                }
            }
        }
    }
    public List<QuestObjective> Objectives;

    public virtual void CompleteQuest() {
        Status = QuestStatus.Completed;
        AdditionalActionsOnQuestComplete();
    }

    public virtual void AdditionalActionsOnQuestComplete() {
    }

    public virtual void StartQuest(bool show_notification = true) {
        UpdateStatus(QuestStatus.InProgress, show_notification);
        AdditionalActionsOnQuestStart();
    }

    public virtual void AdditionalActionsOnQuestStart() {
    }

    public void ChangeQuestTitleDisplay(string title_to_display) {
        GameObject questDisplayItem = CanvasElements.UICanvas.ObjectivesDisplay.transform.Find(GetType().Name)?.gameObject;
        if (questDisplayItem != null) {
            questDisplayItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = title_to_display;
        }
    }

    public void ChangeQuestDescriptionDisplay(string description_to_display) {
        GameObject questDisplayItem = CanvasElements.UICanvas.ObjectivesDisplay.transform.Find(GetType().Name)?.gameObject;
        if (questDisplayItem != null) {
            questDisplayItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = description_to_display;
        }
    }

    public static Quest GetQuest(Type type) {
        return SaveFile.Instance.GetQuest(type);
    }

    public QuestObjective GetObjective(int number) {
        QuestObjective objective = Objectives.FirstOrDefault(obj => obj.Number == number);
        if(objective == null) {
            Debug.LogError("Tried to get non-existent objective (number " + number + ") for quest: " + GetType());
        }
        return objective;
    }

    /// <summary>
    /// Set Current objective to Completed and new objective to Current
    /// </summary>
    /// <param name="number_of_new_objective">Number of objective to set to Current</param>
    /// <param name="notify">Should audibly notify about the change</param>
    public void AdvanceObjective(int number_of_new_objective, List<string> string_params = null) {
        QuestObjective newObjective = GetObjective(number_of_new_objective);
        QuestObjective oldObjective = Objectives.FirstOrDefault(obj => obj.Status == QuestObjective.ObjectiveStatus.Current);
        if(oldObjective != null) {
            oldObjective.Status = QuestObjective.ObjectiveStatus.Completed;
        }
        newObjective.Status = QuestObjective.ObjectiveStatus.Current;
        if(string_params != null) {
            newObjective.DescriptionParameters = string_params;
        }
    }
}