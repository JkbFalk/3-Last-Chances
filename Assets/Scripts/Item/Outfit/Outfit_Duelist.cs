using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Outfit_Duelist : Item
{
    public Outfit_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("SharpArmorWithoutLimit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RestoreStaggerBarOnSuccesfulRiposteOrCounterWhileStaggered")};
    }
}
