using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_ArcaneSword : Item
{
    public Longblade_ArcaneSword(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.RoyalGuard;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(135, 50, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksDealIncreasedDamageBasedOnMagicDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyAttackSpeed")};
    }
}
