using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_EliminateAllEnemies : Quest {
   public Quest_EliminateAllEnemies() {
        UniqueQuest = false;
        Objectives = new() { new(this, 0)};
   }

    public override void AdditionalActionsOnQuestStart()
    {
        base.AdditionalActionsOnQuestStart();
        EventManager.EnemyDefeated.AddListener(UpdateDefeatedEnemiesCount);
        UpdateDefeatedEnemiesCount(null);
        Utils.PlaySoundEffect(null, "UI/QuestStarted", 0.8f);
    }

    public override void AdditionalActionsOnQuestComplete()
    {
        base.AdditionalActionsOnQuestComplete();
        EventManager.EnemyDefeated.RemoveListener(UpdateDefeatedEnemiesCount);
        SaveFile.Instance.CurrentMission.Finish();
        Utils.PlaySoundEffect(null, "UI/QuestCompleted", 0.8f);
    }

    public void UpdateDefeatedEnemiesCount(Damage damage) {
        int enemy_count = Utils.GetAllUnits(true).Count;
        Objectives[0].ShowAsMissionObjective(new List<string> {enemy_count.ToString()});
        if(enemy_count <= 0) {
            CompleteQuest();
        }
    }
}