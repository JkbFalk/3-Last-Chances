using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Mercenary : Item
{
    public Outfit_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Outfit; 
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealMoreDamageBasedOnBarrier")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BarrierAmount", 0.5f), new ItemEffect("BarrierDecay", 0.5f)};
    }
}
