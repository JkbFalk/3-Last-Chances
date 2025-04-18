using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_JudicalCodex : Item
{
    public Gun_JudicalCodex(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(85, 155, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksStunEnemies")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedStagger")};
    }
}
