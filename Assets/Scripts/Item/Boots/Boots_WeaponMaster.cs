using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_WeaponMaster : Item
{
    public Boots_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.HeavyInjury, new(this)) {PercentageAmount = 0.625f, EffectTypeName="IncreasedWeaponDamageButDecreasedMagicDamage", DescriptionParameters=new List<string>{Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.625f)}}, 
        new Effect_ChangeStat(Player.Instance.HeavyStagger, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 0.625f}, 
        new Effect_ChangeStat(Player.Instance.LightInjury, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 0.625f}, 
        new Effect_ChangeStat(Player.Instance.LightStagger, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 0.625f}, 
        new Effect_ChangeStat(Player.Instance.RangedInjury, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 0.625f}, 
        new Effect_ChangeStat(Player.Instance.RangedStagger, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 0.625f}, 
        new Effect_ChangeStat(Player.Instance.MagicInjury, new(this)) {EffectTypeName="NoDescription", PercentageAmount = -0.625f}, 
        new Effect_ChangeStat(Player.Instance.MagicStagger, new(this)) {EffectTypeName="NoDescription", PercentageAmount = -0.625f}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 10 }};
    }
}
