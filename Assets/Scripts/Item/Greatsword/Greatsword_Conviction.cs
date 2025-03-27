using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Conviction : Item
{
    //40 overall at 0.5 attk, 30 at 0.75, 20 at 1, 15 at 1.5, 10 at 2
    public Greatsword_Conviction(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(90, 150, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}
