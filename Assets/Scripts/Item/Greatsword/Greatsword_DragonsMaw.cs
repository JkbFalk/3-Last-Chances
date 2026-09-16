using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greatsword_DragonsMaw : Item
{
    public Greatsword_DragonsMaw(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(10, 16, 0.7f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("HeavyDamageAppliesBurn", 0.8f), new ItemEffect("BlazingShadowWatchesYourBack", 0.2f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BurnAmountWhileBelowNHealth")};
    }
}
