using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_Jailer : Item
{
    public Boots_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RestoreStaggerFromDebuffs")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HealFromBeingDamagedByStackingEffects")};
    }
}
