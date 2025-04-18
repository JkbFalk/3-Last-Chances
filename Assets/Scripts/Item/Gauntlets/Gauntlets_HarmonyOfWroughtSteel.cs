using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_HarmonyOfWroughtSteel : Item
{
    public Gauntlets_HarmonyOfWroughtSteel(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(80, 80, 1.3f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncisionRestoresHealthInsteadOfDealingInjury")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("IncisionDecay")};
    }
}
