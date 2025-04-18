using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gloves_WeaponMaster : Item
{
    public Gloves_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RestoreHealthOnWeaponDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("WeaponAttackSpeed")};
    }
}
