using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Artisan : Item
{
    public Boots_Artisan(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Artisan;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("Gain1ExtraUseOfToolsAndIncreaseToolPower")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("PassivePowerUpPower")};
    }
}
