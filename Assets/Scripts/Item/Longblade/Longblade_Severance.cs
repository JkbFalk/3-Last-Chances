using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_Severance : Item
{
    public Longblade_Severance(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(135, 50, 1.1f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.HeavyInjury, new(this)) {PercentageAmount = 1.25f, RemainsActiveInOtherStances = true}};
    }
}
