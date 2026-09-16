using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Mercenary : Item
{
    public Helmet_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttacksGiveBarrier")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BarrierArmor")};
    }
}
