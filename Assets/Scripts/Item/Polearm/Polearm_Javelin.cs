using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_Javelin : Item
{
    public Polearm_Javelin(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(110, 80, 0.95f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReplaceAllBasicAttacksWithThrowAndIncreaseDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StrongBasicAttackDamage")};
    }
}
