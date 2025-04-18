using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_WeaponMaster : Item
{
    public Helmet_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("WeaponTechniquesEmpowerNextBasicAttackAndViceVersa")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("WeaponDamage")};
    }
}

