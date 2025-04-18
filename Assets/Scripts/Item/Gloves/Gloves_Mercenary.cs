using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Mercenary : Item
{
    public Gloves_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Gloves; 
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertHealthToStaggerBar", 0.5f), new ItemEffect("ConvertHealthToStaggerBar", 0.5f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Tenacity")};
    }
}
