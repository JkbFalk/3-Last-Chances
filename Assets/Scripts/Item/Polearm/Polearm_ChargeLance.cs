using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_ChargeLance : Item
{
    public Polearm_ChargeLance(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(12, 11, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReplaceAllBasicAttacksWithChargeAndImproveDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("EnergyGain")};
    }
}
