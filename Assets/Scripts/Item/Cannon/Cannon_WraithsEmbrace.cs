using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_WraithsEmbrace : Item
{
    public Cannon_WraithsEmbrace(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("FinalAmmoDealsMassivelyIncreasedDamageEveryNSeconds")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedTechniqueDamage")};
    }
}
