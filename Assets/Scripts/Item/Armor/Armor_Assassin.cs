using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Assassin : Item
{
    public Armor_Assassin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncreasedBackstabDamageWhileStealthed")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Injury")};
    }
}
