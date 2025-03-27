using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_JudicalCodex : Item
{
    public Gun_JudicalCodex(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(85, 155, 0.6f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}
