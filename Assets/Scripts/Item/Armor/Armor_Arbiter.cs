using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Arbiter : Item
{
    public Armor_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealingDamageProlongsStaggered")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Stagger")};
    }
}
