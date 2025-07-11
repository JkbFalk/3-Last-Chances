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
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AncientArmor")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Armor")};
    }
}
