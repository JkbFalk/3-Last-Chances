using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Outfit_Knight : Item
{
    public Outfit_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainArmorAfterBeingHit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}
