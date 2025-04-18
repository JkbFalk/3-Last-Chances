using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Judge : Item
{
    public Gloves_Judge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DamagingAnEnemyFreezesThemInPlace")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Stagger")};
    }
}
