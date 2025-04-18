using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_UpgradeMaterials : Item
{
    public Quest_UpgradeMaterials(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Quest;
        Icon = Utils.LoadSpriteFromMultiple("Quest Icons", grade + "Materials");
    }
}
