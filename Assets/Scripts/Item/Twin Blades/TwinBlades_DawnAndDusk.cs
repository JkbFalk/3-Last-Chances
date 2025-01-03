using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_DawnAndDusk : Item
{
    public TwinBlades_DawnAndDusk(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(95, 95, 1.1f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_DawnAndDusk(new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType.Light, 1, 1, new(this)) };
    }
}

