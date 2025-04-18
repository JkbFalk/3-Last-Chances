using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Armor_Duelist : Item
{
    public Armor_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("SharpDamageReduction", 0.75f), new ItemEffect("SharpAmount", 0.25f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RestoreStaggerBarOnSuccesfulRiposteOrCounterWhileStaggered")};
    }
}
