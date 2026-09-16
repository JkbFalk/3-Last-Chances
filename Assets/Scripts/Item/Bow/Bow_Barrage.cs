using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Barrage : Item
{
    public Bow_Barrage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(12.5f, 3.5f, 1.2f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("CanBasicAttackNonStopAndDealMoreDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ArmorDuringBasicAttacks")};
    }
}
