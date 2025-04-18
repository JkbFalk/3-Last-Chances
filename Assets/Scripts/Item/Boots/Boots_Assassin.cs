using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Assassin : Item
{
    public Boots_Assassin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("StealthDuration")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("MovementSpeedDuringStealth")};
    }
}
