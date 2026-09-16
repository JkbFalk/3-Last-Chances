using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Survivor : Item
{
    public Outfit_Survivor(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainHealthRegenerationThatIsTripledWhenBelowHalfHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("FlatHealth", 0.5f), new ItemEffect("Health", 0.5f)};
    }
}
