using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Oath : Item
{
    //40 overall at 0.5 attk, 30 at 0.75, 20 at 1, 15 at 1.5, 10 at 2
    public Greatsword_Oath(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(90, 150, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_RestoreHealthOnBreakingHealthBar(false, new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType.Heavy, 0, 3f, new(this))};
    }
}
