using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Assassin : Item
{
    public Gloves_Assassin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DamageWhileStealthed")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StealthDuration")};
    }
}
