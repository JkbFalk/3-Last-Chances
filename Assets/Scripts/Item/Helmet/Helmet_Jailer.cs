using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Helmet_Jailer : Item
{
    public Helmet_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealingOrTakingDamageAppliesChainedToYou")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ChainedAmount", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}

