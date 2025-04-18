using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_WeaponMaster : Item
{
    public Boots_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncreasedWeaponDamageButDecreasedMagicDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DamageReductionDuringBasicAttacks")};
    }
}
