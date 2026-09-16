using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greatsword_Vengeance : Item
{
    public Greatsword_Vengeance(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(11.5f, 11.5f, 0.8f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ResistDeathAndGainDamageAgainstAttacker")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyDamage")};
    }
}
