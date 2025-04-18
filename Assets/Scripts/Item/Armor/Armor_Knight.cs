using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Armor_Knight : Item
{
    public Armor_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageReductionAfterBeingHit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}
