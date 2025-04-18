using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Reckoning : Item
{
    public Greatsword_Reckoning(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(145, 145, 0.65f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttacksReducePercentageOfEnemyStaggerBar")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyAttackSpeed")};
    }
}
