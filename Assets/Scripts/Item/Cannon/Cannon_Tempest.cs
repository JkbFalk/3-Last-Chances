using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_Tempest : Item
{
    public Cannon_Tempest(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealMoreDamageAndKnockbackToCloserEnemies")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RefundAmmoIfEnemyHitByBasicAttackWasClose")};
    }
}
