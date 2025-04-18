using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_WingsOfFreedom : Item
{
    public TwinBlades_WingsOfFreedom(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(110, 40, 1.2f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertChainedIntoHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ConvertedChainedGeneratesBarrier")};
    }
}

