using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_IronBlooded : Item
{
    public Boots_IronBlooded(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertXPercentOfStaggerDealtToYouIntoInjury")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health")};
    }
}
