using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Arbiter : Item
{
    public Outfit_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealingDamageProlongsStaggered")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DamageToStaggered")};
    }
}
