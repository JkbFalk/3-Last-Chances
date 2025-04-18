using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Sage : Item
{
    public Gloves_Sage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Sage;
        Type = Constants.ItemType.Gloves; 
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncreaseEnergyGainWhileEnergyIsBelowHalf")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("CooldownReduction")};
    }
}
