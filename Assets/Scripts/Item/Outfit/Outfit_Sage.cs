using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Sage : Item
{
    public Outfit_Sage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Sage;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncreaseMagicDamageButDecreaseWeaponDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("EnergyGain", 0.66f), new ItemEffect("CooldownReduction", 0.34f)};
    }
}
