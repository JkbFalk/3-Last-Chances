using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Unbreakable : Item
{
    public Armor_Unbreakable(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Unbreakable;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReflectPortionOfBlockedDamageBackAtAttacker")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("FlatStaggerBar", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}
