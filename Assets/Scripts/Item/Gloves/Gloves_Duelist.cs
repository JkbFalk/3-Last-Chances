using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Gloves_Duelist : Item
{
    public Gloves_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainSharpOnBasicAttacksAndOnslaughtOnRipostesAndCounters")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("SharpAmount")};
    }
}
