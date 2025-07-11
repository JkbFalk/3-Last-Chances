using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Alacrity : Item
{
    public Outfit_Alacrity(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertAttackSpeedToDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ConvertMovementSpeedToDamage")};
    }
}
