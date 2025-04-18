using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Artisan : Item
{
    public Helmet_Artisan(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Artisan;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("UsingToolsIncreasesToolPowerUntilEndOfCombat")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StancePower")};
    }
}
