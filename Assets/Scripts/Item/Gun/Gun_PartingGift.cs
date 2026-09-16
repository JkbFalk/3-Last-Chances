using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_PartingGift : Item
{
    public Gun_PartingGift(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(6, 12, 1.2f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealExtraRangedStaggerWithCooldown")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedTechniqueDamage")};
    }
}
