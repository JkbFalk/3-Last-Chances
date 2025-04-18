using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_DragonsClaws : Item
{
    public Daggers_DragonsClaws(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(95, 80, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksPerformedWhileInStealthCountAsBackstab")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BackstabDamage")};
    }
}

