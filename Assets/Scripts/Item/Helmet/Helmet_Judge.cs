using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Judge : Item
{
    public Helmet_Judge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("WhenEnemyExitsCrowdControlApplyExtraFrozen")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("GainEnergyWhenApplyingFrozen")};
    }
}
