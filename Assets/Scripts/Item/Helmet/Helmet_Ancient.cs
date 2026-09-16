using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Ancient : Item
{
    public Helmet_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AncientHelmet", 0.2f), new ItemEffect("CooldownReduction", 0.8f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health")};
    }
}

