using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;
[Serializable]
public class QuestObjective {
    public Type ParentQuest;
    public string JournalId;
    public int Number;
    [SerializeField]
    private List<string> _descriptionParameters = new();
    public List<string> DescriptionParameters {
        get => _descriptionParameters;
        set {
            _descriptionParameters = value;
            Utils.ShowMissionObjective(this);
        }
    }
    [Serializable]
    public enum ObjectiveStatus {Failed, Completed, Current, NotRevealed};
    
    public ObjectiveStatus _status = ObjectiveStatus.NotRevealed;
    public ObjectiveStatus Status {
        get => _status;
        set {
            ObjectiveStatus previousValue = _status;
            _status = value;
            if(previousValue != value) {
                Utils.ShowMissionObjective(this);
                EventManager.QuestObjectiveUpdated.Invoke(SaveFile.Instance.GetQuest(ParentQuest), this);
                Utils.CreateAuditLog("Quest objective (" + ParentQuest + " " + Number +") status updated: " + previousValue + " -> " + value);
                //NotificationController.ShowQuestObjectiveUpdateNotification(ParentQuest, this);
            }
        }
    }
    public QuestObjective(Quest parent_quest, int number) {
        ParentQuest = parent_quest.GetType();
        Number = number;
        JournalId = ParentQuest.GetType() + "_" + Number + "_Journal";
        _status = ObjectiveStatus.NotRevealed;
    }

    public void ShowAsMissionObjective(List<string> string_params = null) {
        Status = ObjectiveStatus.Current;
        if(string_params != null) {
            _descriptionParameters = string_params;
        }
        Utils.ShowMissionObjective(this);
    }

    public void UpdateStatusWithoutNotifying(ObjectiveStatus status) {
        _status = status;
    }
}