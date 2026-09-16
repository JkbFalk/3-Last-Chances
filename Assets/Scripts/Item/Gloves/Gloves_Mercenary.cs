using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Mercenary : Item
{
    public Gloves_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Gloves; 
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainBarrierUponFallingBelow25PHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HealthRegenWhileBarrier")};
    }
}
