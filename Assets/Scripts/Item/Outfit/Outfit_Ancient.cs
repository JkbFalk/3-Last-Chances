using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Ancient : Item
{
    public Outfit_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AncientArmor", 0.2f), new ItemEffect("Armor", 0.8f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Stagger")};
    }
}
