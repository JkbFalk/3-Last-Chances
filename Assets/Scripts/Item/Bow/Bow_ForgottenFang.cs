using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_ForgottenFang : Item
{
    public Bow_ForgottenFang(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(8, 16, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("TripleArrowsShotWithoutSpendingExtraAmmo")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Damage")};
    }
}
