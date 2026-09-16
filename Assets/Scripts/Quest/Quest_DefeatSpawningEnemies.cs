using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_DefeatSpawningEnemies : Quest {

    public int DefeatedEnemyCount = - 1;
    public int SpawnEnemiesWhenBelow = 5;
    public int EnemyLevel = 1;
    public float Aggressiveness = 2;
    
    public Quest_DefeatSpawningEnemies() {
        UniqueQuest = false;
        Objectives = new() {new(this, 0)};
    }

    public override void AdditionalActionsOnQuestStart()
    {
        base.AdditionalActionsOnQuestStart();
        EventManager.UnitKnockedOut.AddListener(UpdateDefeatedEnemiesCount);
        UpdateDefeatedEnemiesCount(null);
        Utils.PlaySoundEffect(null, "UI/QuestStarted", 0.8f);
    }

    public override void AdditionalActionsOnQuestComplete()
    {
        base.AdditionalActionsOnQuestComplete();
        EventManager.UnitKnockedOut.RemoveListener(UpdateDefeatedEnemiesCount);
        Utils.PlaySoundEffect(null, "UI/QuestCompleted", 0.8f);
    }

    public void UpdateDefeatedEnemiesCount(DamageInstance damage) {
        DefeatedEnemyCount++;
        int enemy_count = Utils.GetAllUnits(true).Count;
        Objectives[0].ShowAsMissionObjective(new List<string> {DefeatedEnemyCount.ToString()});
        if(enemy_count < SpawnEnemiesWhenBelow) {
            Unit enemy = Utils.SpawnUnit(Constants.RegularEnemies[UnityEngine.Random.Range(0, Constants.RegularEnemies.Count)], GameObject.FindGameObjectWithTag("Area").transform.Find("Infinite Spawns"), EnemyLevel, Aggressiveness, true);
            enemy.AttackPlayer();
        }
    }
}