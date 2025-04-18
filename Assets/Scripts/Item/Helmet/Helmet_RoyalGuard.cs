using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_RoyalGuard : Item
{
    public Helmet_RoyalGuard(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.RoyalGuard;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksGiveAnalysis")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("AttackSpeed")};
    }
}

