using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_Oath : Item
{
    public TwinBlades_Oath(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(80, 80, 1.4f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("NonStrongBasicAttacksEmpowerYourNextStrongBasicAttack")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttackInjury")};
    }
}

