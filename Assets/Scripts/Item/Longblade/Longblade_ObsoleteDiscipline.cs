using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_ObsoleteDiscipline : Item
{
    public Longblade_ObsoleteDiscipline(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(100, 150, 0.7f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ApplySelfChainedToEnemies")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("GainChainedOnHeavyDamage")};
    }
}
