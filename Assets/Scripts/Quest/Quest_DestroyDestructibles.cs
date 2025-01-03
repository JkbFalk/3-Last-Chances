using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_DestroyDestructibles : Quest {
    
    public Quest_DestroyDestructibles() {
        UniqueQuest = false;
        Objectives = new() { new(this, 0)};
    }

    public override void AdditionalActionsOnQuestStart()
    {
        base.AdditionalActionsOnQuestStart();
        EventManager.DestructibleDestroyed.AddListener(UpdateDestructibleCount);
        UpdateDestructibleCount(null);
        Utils.PlaySoundEffect(null, "UI/QuestStarted", 0.8f);
    }

    public override void AdditionalActionsOnQuestComplete()
    {
        base.AdditionalActionsOnQuestComplete();
        EventManager.DestructibleDestroyed.RemoveListener(UpdateDestructibleCount);
        Utils.PlaySoundEffect(null, "UI/QuestCompleted", 0.8f);
    }

    public void UpdateDestructibleCount(DestructibleEnvironment obj) {
        GameObject[] destructibles = GameObject.FindGameObjectsWithTag("Destructible");
        Objectives[0].ShowAsMissionObjective(new List<string> {destructibles.Length.ToString()});
        if(destructibles.Length == 0) {
            CompleteQuest();
            SaveFile.Instance.CurrentMission.Finish();
        }
    }
}