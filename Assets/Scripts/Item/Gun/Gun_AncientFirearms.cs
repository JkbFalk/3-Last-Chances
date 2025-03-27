using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_AncientFirearms : Item
{
    public Gun_AncientFirearms(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(130, 100, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_AncientFirearmsDamage(new(this))};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AncientFirearmsDamageReduction(new(this))};
    }
}
