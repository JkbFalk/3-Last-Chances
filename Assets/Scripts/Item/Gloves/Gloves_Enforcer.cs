using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Enforcer : Item
{
    public Gloves_Enforcer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksGiveOnslaught")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorDuringBasicAttacks")};
    }
}