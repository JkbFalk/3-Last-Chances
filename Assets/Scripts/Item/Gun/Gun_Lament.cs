using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Lament : Item
{
    public Gun_Lament(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(135, 50, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ExecuteEnemiesBelowHealth(Constants.DamageType.Ranged, new(this)) {RemainsActiveInOtherStances=true} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_GainExtraAmmoOnFinishingOffEnemies(Constants.DamageType.Ranged, new(this)) {RemainsActiveInOtherStances=true}  };
    }
}
