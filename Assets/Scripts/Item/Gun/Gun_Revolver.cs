using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Revolver : Item
{
    public Gun_Revolver(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(11.5f, 11.5f, 0.8f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealExtraDamageToUndamagedEnemies")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RefundAmmoAgainstUndamagedEnemies")};
    }
}
