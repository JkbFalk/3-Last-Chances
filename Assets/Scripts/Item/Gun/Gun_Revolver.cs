using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Revolver : Item
{
    public Gun_Revolver(ItemGrade grade) : base(grade)
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
