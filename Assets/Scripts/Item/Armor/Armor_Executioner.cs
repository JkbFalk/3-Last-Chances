using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Executioner : Item
{
    public Armor_Executioner(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("Every5thHitDealsMassivelyIncreasedDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Injury", 0.5f), new ItemEffect("TechniqueDamage", 0.5f)};
    }
}
