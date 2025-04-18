using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Survivor : Item
{
    public Boots_Survivor(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainInvincibleUponFallingBelow25PHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health")};
    }
}
