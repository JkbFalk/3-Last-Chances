using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_RoyalGuard : Item
{
    public Boots_RoyalGuard(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.RoyalGuard;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncreaseWeaponDamageByPortionOfMagicDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("WeaponDamage")};
    }
}
