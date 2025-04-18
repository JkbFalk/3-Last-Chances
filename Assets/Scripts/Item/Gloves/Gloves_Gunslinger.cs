using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Gunslinger : Item
{
    public Gloves_Gunslinger(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RestoreXAmmoEachTimeYouDealDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttackDamage")};
    }
}
