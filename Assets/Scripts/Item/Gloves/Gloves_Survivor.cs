using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Survivor : Item
{
    public Gloves_Survivor(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainInvincibleUponFallingBelow25PHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HealFromInjury")};
    }
}
