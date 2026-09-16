using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_Severance : Item
{
    public Longblade_Severance(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(13.5f, 5, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttacksApplyBleedBasedOnEnemyHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BleedAmount"), new ItemEffect("BleedDecay")};
    }
}
