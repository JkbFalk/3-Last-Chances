using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Outfit_WeaponMaster : Item
{
    public Outfit_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BlademastersGarb")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttackDamage", 0.5f), new ItemEffect("CooldownReduction", 0.5f)};
    }
}
