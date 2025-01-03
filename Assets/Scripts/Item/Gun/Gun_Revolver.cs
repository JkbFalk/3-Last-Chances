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
        return new List<Effect> { new Effect_ExtraDamageToUndamagedEnemies(5, 5, new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType.Ranged, 1, 1, new(this)) };
    }
}
