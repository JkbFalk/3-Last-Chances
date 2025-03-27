using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Reunion : Item
{
    public Gun_Reunion(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(60, 120, 1.2f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}
