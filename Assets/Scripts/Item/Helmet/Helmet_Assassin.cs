using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Assassin : Item
{
    public Helmet_Assassin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Helmet; 
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReducedBackstabCooldown")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BackstabDamage")};
    }
}
