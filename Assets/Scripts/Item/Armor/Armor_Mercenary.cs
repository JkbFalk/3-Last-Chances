using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Mercenary : Item
{
    public Armor_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Armor; 
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageReductionBasedOnBarrierAmount")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BarrierAmount", 0.5f), new ItemEffect("BarrierDecay", 0.5f)};
    }
}
