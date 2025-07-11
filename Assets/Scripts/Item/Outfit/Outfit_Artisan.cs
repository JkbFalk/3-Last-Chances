using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Artisan : Item
{
    public Outfit_Artisan(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Artisan;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("PassivePowerUpPower", 0.5f), new ItemEffect("StancePower", 0.25f), new ItemEffect("ToolPower", 0.25f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ItemPower")};
    }
}
