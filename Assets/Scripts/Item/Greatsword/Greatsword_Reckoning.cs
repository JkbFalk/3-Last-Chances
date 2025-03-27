using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Reckoning : Item
{
    public Greatsword_Reckoning(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(145, 145, 0.65f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}
