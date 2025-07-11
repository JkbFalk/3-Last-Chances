using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_SteelWarden : Item
{
    public Greatsword_SteelWarden(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(40, 160, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertArmorToHeavyStagger")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorDuringBasicAttacks")};
    }
}
