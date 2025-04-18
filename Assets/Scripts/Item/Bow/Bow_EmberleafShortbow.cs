using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_EmberleafShortbow : Item
{
    public Bow_EmberleafShortbow(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(80, 160, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("OnSwitchingToThisWeaponGainRangedAttackSpeedAndNextBasicAttackStunsEnemy")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Control")};
    }
}
