using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_AdamantLotus : Item
{
    public Gauntlets_AdamantLotus(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(80, 80, 1.3f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttacksGrantBarrier")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttackStagger")};
    }
}
