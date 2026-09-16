using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Assassin : Item
{
    public Outfit_Assassin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncreasedBackstabDamageWhileStealthed")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("GainStealthUponFallingBelow50PHealth")};
    }
}
