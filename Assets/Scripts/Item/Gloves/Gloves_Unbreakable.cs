using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Unbreakable : Item
{
    public Gloves_Unbreakable(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Unbreakable;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("OnceEveryNSecondsWhenAboutToBeStaggeredRestoreStaggerBarInstead")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DamageReductionWhileBlocking")};
    }
}
