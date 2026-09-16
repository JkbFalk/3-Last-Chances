using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_IronBlooded : Item
{
    public Outfit_IronBlooded(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReflectInjuryTakenBackAtAttacker")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ConvertHealthToInjury")};
    }
}
