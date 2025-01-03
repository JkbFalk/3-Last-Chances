using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_ToolMaterials : Item
{
    public Quest_ToolMaterials(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Quest;
        Icon = Utils.LoadSpriteFromMultiple("Quest Icons", "ToolMaterials");
    }
}
