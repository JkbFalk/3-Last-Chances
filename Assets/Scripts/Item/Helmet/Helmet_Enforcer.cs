using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Enforcer : Item
{
    public Helmet_Enforcer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksReduceCooldowns")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttackDamage")};
    }
}