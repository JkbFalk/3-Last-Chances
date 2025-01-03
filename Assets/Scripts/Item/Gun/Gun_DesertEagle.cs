using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_DesertEagle : Item
{
    public Gun_DesertEagle(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(85, 155, 0.6f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_BasicAttacksInflictStun(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Stun)}}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType.Ranged, 0, 3f, new(this)) };
    }
}
