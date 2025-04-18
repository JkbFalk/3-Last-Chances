using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Artisan : Item
{
    public Gloves_Artisan(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Artisan;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("Gain2ExtraChargesOfHealthPotionAndIncreaseEffectivness")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ToolPower")};
    }
}
