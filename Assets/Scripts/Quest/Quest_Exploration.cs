using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_Exploration : Quest {
    public Quest_Exploration() {
        UniqueQuest = false;
        Objectives = new() { new(this, 0)};
    }

    public override void AdditionalActionsOnQuestStart()
    {
        base.AdditionalActionsOnQuestStart();
        Objectives[0].ShowAsMissionObjective();
    }
}