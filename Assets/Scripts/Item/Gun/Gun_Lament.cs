using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Lament : Item
{
    public Gun_Lament(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(13.5f, 5, 0.75f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("FinishOffLowHealthEnemies")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("TakedownsRestoreAmmo")};
    }
}
