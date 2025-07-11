using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Executioner : Item
{
    public Gloves_Executioner(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealIncreasedDamageToEnemiesBelow25PHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorAgainstBosses")};
    }
}
