using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Ebony : Item
{
    public Bow_Ebony(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        int[] baseStats = {70, 75, 85, 100, 125};
        SetBaseWeaponStats(baseStats[GradeIndex], baseStats[GradeIndex], 0.8f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("EbonySpecialScaling")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Armor")};
    }
}
