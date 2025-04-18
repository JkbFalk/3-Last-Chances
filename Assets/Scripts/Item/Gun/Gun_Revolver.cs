using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Revolver : Item
{
    public Gun_Revolver(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(115, 115, 0.8f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealExtraDamageToUndamagedEnemies")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedTechniqueDamage")};
    }
}
