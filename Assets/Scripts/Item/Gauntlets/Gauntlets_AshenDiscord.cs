using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlets_AshenDiscord : Item
{
    public Gauntlets_AshenDiscord(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Gauntlets;
        SetBaseWeaponStats(8, 8, 1.3f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("LightDamageConvertsAllFreezeIntoBurnOrBurnIntoFreeze")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightStagger")};
    }
}
