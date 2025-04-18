using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Gunslinger : Item
{
    public Helmet_Gunslinger(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealExtraDamageToEnemiesFarAway")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedDamage")};
    }
}
