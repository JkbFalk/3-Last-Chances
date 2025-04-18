using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Reunion : Item
{
    public Gun_Reunion(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(60, 120, 1.2f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("StaggerEnemiesOnceEveryNSeconds")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedDamageToStaggered")};
    }
}
