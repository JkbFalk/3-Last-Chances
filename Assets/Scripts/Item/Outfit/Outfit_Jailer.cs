using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Jailer : Item
{
    public Outfit_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageAndArmorForEachDebuff")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ChainedArmor")};
    }
}
