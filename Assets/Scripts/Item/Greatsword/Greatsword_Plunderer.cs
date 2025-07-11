using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greatsword_Plunderer : Item
{
    public Greatsword_Plunderer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(110, 130, 0.75f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("PlundererEmpower")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("PlundererArmor")};
    }
}
