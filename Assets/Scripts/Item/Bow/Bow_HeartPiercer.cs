using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_HeartPiercer : Item
{
    public Bow_HeartPiercer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(8, 16, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("OnSwitchingToThisWeaponSpawnMarkerThatDealsInjuryOnHit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedInjury")};
    }
}
