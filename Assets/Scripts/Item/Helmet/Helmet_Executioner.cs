using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Executioner : Item
{
    public Helmet_Executioner(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertStaggerDealtIntoInjury")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("SalutisTechniqueDamage")};
    }
}
