using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_RoyalGuard : Item
{
    public Outfit_RoyalGuard(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.RoyalGuard;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("MagicTechniquesEmpowerWeaponTechniquesAndViceVersa")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("TechniqueDamage")};
    }
}
