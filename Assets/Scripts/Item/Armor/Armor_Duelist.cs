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
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("SharpDamageReductionWithoutLimit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RestoreStaggerBarOnSuccesfulRiposteOrCounterWhileStaggered")};
    }
}
