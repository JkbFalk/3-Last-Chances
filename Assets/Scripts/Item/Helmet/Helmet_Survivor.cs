using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Survivor : Item
{
    public Helmet_Survivor(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("HealthRestorationPower")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ConvertHealthToInjury")};
    }
}
