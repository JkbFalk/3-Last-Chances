using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Mercenary : Item
{
    public Boots_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Boots; 
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainPortionOfDamageTakenAsBarrier")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StaggerBarRegenWhileBarrier")};
    }
}
