using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_RestoredVow : Item
{
    public TwinBlades_RestoredVow(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(80, 80, 1.4f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("NonStrongBasicAttacksEmpowerYourStrongBasicAttacks")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttackInjury")};
    }
}

