using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Ebony : Item
{
    public Bow_Ebony(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        int[] baseStats = {70, 75, 85, 100, 125};
        SetBaseWeaponStats(baseStats[GradeIndex], baseStats[GradeIndex], 0.8f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.RangedInjury, new(this)) { RemainsActiveInOtherStances = true, ScaleWithEffectValue = false, PercentageAmount = Grade == ItemGrade.Regular ? 10 : Grade == ItemGrade.Excellent ? 20 :Grade == ItemGrade.Masterful ? 30 :Grade == ItemGrade.Flawless ? 50 : 100 }, new Effect_ChangeStat(Player.Instance.RangedStagger, new(this)) { RemainsActiveInOtherStances = true, ScaleWithEffectValue = false, PercentageAmount = Grade == ItemGrade.Regular ? 10 : Grade == ItemGrade.Excellent ? 20 :Grade == ItemGrade.Masterful ? 30 :Grade == ItemGrade.Flawless ? 50 : 100 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) {PercentageAmount= Grade == ItemGrade.Masterful ? 0.4f :Grade == ItemGrade.Flawless ? 0.5f : Grade == ItemGrade.Ultimate ? 0.6f : 0} };
    }
}
