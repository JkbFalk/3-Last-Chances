using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Twilight : Item
{
    public Gun_Twilight(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(115, 115, 0.8f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}
