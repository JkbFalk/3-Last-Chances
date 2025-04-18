using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Arbiter : Item
{
    public Helmet_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertAllInjuryToStaggerAgainstNonStaggered")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DamageToStaggered")};
    }
}

