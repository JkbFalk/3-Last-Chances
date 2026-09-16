
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Retribution : Item
{
    public Greatsword_Retribution(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(15, 6, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AfterGettingHitIncreaseDamageOfNextAttack")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttackDamage")};
    }
}
