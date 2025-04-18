using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Alacrity : Item
{
    public Armor_Alacrity(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertAttackSpeedToDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ConvertMovementSpeedToDamage")};
    }
}
