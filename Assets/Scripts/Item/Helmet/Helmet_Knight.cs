using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Helmet_Knight : Item
{
    public Helmet_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainTenacityAfterBeingHit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Armor")};
    }
}

