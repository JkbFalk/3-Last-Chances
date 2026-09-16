using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Unbreakable : Item
{
    public Outfit_Unbreakable(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Unbreakable;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReflectPortionOfBlockedDamageBackAtAttacker")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("EnergyGainFromBlocking")};
    }
}
