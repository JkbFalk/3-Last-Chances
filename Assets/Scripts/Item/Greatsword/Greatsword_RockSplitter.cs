using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_RockSplitter : Item
{
    public Greatsword_RockSplitter(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(145, 145, 0.65f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_StrongBAsDealStaggerBasedOnEnemyStaggerBar(3.125f, new(this)) {Grade = GradeIndex}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_StrongBADealMoreDamage(1, new(this))};
    }
}
