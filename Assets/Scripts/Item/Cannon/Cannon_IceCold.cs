using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_IceCold : Item
{
    public Cannon_IceCold(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("OnSwitchingToWeaponSpawnAMarkerThatDealsMAssiveStaggerAndStunOnHit")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Stagger", 0.5f), new ItemEffect("Control", 0.5f)};
    }
}
