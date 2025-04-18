using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_Veilbreaker : Item
{
    public Cannon_Veilbreaker(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksPerformedWhileInStealthCountAsBackstab")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttackDamage", 0.5f), new ItemEffect("BackstabDamage", 0.5f)};
    }
}
