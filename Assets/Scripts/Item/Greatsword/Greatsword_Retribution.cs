
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Retribution : Item
{
    public Greatsword_Retribution(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(150, 60, 0.85f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_DealExtraDamageAfterTakingDamage(1, 1, 3, new(this))};
    }

    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) {PercentageAmount=0.5f} };
    }
}
