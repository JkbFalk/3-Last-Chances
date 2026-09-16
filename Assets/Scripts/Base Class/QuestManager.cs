// FILE: Assets/Scripts/Base Class/QuestManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager
{
    private static QuestManager _instance;
    public static QuestManager Instance => _instance ??= new QuestManager();

    public List<Quest> Quests => SaveFile.Instance.Quests;

    public void InitializeQuestsForSave(SaveFile saveFile)
    {
        saveFile.Quests.Clear();
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(Quest)) && !type.IsAbstract))
        {
            saveFile.Quests.Add((Quest)Activator.CreateInstance(type));
        }
    }

    public bool CheckIfQuestIsInProgress(string questName)
    {
        return Quests.FirstOrDefault(quest => quest.Status == Quest.QuestStatus.InProgress &&
            (quest.GetType().Name == questName ||
             quest.GetType().Name.Replace("Quest_", "") == questName ||
             ("Quest_" + quest.GetType().Name) == questName)) != null;
    }

    public bool CheckIfQuestIsInProgress(Type questType)
    {
        return Quests.FirstOrDefault(quest => quest.Status == Quest.QuestStatus.InProgress && quest.GetType() == questType) != null;
    }

    public Quest GetQuest(string questName)
    {
        return Quests.FirstOrDefault(quest =>
            quest.GetType().Name == questName ||
            quest.GetType().Name.Replace("Quest_", "") == questName ||
            ("Quest_" + quest.GetType().Name) == questName);
    }

    public Quest GetQuest(Type questType)
    {
        return Quests.FirstOrDefault(quest => quest.GetType() == questType);
    }
}