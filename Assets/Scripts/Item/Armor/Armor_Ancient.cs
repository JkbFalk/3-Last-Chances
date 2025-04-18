using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Ancient : Item
{
    public Armor_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AncientArmor")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DamageReduction")};
    }
}
