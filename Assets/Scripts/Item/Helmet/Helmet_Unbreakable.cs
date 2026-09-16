using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Unbreakable : Item
{
    public Helmet_Unbreakable(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Unbreakable;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ArmorWhileBlocking")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ProtectFromFlinchingOnce")};
    }
}
