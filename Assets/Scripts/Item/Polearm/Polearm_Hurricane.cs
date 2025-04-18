using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_Hurricane : Item
{
    public Polearm_Hurricane(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(105, 105, 0.9f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("NonStopBasicAttackingIncreasesDamageDealtAndSpeed")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ReduceDamageByAFlatAmountWhileBasicAttacking")};
    }
}
