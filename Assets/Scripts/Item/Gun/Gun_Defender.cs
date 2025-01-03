using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Defender : Item
{
    public Gun_Defender(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(60, 120, 1.2f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_AddKnockbackToBasicAttacks(new(this))};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType.Ranged, 1f, 2f, new(this)) };
    }
}
