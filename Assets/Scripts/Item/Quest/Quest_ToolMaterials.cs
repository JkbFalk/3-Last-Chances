using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_ToolMaterials : Item
{
    public Quest_ToolMaterials(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Quest;
        Icon = Utils.LoadSpriteFromMultiple("Quest Icons", "ToolMaterials");
    }
}
