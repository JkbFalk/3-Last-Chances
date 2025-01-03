using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_WeaponMaster : Item
{
    public Armor_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_BlademastersGarb(new(this)) {DescriptionParameters=new List<String>{Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.75f).ToString(), Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.25f).ToString(), Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.75f).ToString(), Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.25f).ToString()}}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.EnergyGain, new(this)) { PercentageAmount = 0.25f }, new Effect_ChangeStat(Player.Instance.CooldownReduction, new(this)) { PercentageAmount = 0.25f } };
    }
}
