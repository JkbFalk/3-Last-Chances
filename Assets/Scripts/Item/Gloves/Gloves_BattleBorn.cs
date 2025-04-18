using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_BattleBorn : Item
{
    public Gloves_BattleBorn(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("LoseHealthWhileAboveHalfAndRegenWhileBelow")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Damage")};
    }
}
