using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_IronBlooded : Item
{
    public Armor_IronBlooded(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReflectInjuryTakenBackAtAttacker")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("FlatHealth", 0.5f), new ItemEffect("Health", 0.5f)};
    }
}
