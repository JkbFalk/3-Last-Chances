using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Arbiter : Item
{
    public Gloves_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("StaggeringAnEnemyHealsOnceEveryNSeconds")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Stagger")};
    }
}
