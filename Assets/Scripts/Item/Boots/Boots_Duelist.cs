using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_Duelist : Item
{
    public Boots_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RestoreHealthOnRiposteOrCounter")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorAfterRiposteOrCounter")};
    }
}
