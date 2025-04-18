using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Survivor : Item
{
    public Armor_Survivor(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainHealthRegenerationThatIsDoubledWhenBelowHalfHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}
