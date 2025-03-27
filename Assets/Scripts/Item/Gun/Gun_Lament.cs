using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Lament : Item
{
    public Gun_Lament(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(135, 50, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}
