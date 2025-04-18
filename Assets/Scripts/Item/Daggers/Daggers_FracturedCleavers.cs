using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_FracturedCleavers : Item
{
    public Daggers_FracturedCleavers(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(95, 80, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksPushBackAndDecreaseEnemyTenacity")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightAttackSpeed")};
    }
}

