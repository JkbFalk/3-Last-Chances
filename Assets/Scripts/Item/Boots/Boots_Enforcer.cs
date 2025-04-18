using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Enforcer : Item
{
    public Boots_Enforcer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("SharpAndAnalysisAlsoIncreaseBasicAttackDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("AttackSpeed")};
    }
}