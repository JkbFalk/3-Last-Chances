using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gloves_Jailer : Item
{
    public Gloves_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DodgingAndCounteringAppliesChained")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ChainedDecay", 0.5f), new ItemEffect("Health", 0.5f)};
    }
}
