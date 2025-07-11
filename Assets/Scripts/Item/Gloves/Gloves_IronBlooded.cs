using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_IronBlooded : Item
{
    public Gloves_IronBlooded(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RegeneratePortionOfInjuryTakenAsHealthOver30Seconds")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorIsPartiallyEffectiveWhileStaggered")};
    }
}
