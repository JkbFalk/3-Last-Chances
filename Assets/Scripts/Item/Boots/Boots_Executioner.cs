using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Executioner : Item
{
    public Boots_Executioner(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("TakedownsGrantEmpowered")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorAgainstNonBosses")};
    }
}
