using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_Convergence : Item
{
    public Cannon_Convergence(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksDealIncreasedDamageButAlsoInflictSelfStagger")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StaggerBar")};
    }
}
