using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_JudicalCodex : Item
{
    public Gun_JudicalCodex(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(8.5f, 15.5f, 0.6f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ApplyStunWithBasicAttacks")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DamageToStunned")};
    }
}
