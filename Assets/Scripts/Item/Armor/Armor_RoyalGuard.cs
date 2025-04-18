using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_RoyalGuard : Item
{
    public Armor_RoyalGuard(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.RoyalGuard;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageReductionBasedOnAnalysisAmount")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("AnalysisAmount", 0.5f), new ItemEffect("AnalysisDecay", 0.5f)};
    }
}
