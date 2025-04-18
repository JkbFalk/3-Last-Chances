using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Ancient : Item
{
    public Gloves_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AncientGloves")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("EnergyGain")};
    }
}
