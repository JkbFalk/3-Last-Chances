using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Duelist : Item
{
    public Helmet_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainSharpOnRiposteCounterOrDodge")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("SharpDecay")};
    }
}

