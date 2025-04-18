using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Judge : Item
{
    public Boots_Judge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IgnorePortionOfEnemyTenacity")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Control")};
    }
}
