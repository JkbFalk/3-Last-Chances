using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_WhiteWings : Item
{
    public TwinBlades_WhiteWings(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(110, 40, 1.2f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_RestoreHealthOnBasicAttack(new(this)) {HealthRestoreAmount = 0.5f} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType.Light, 1.875f, 0, new(this))};
    }
}

