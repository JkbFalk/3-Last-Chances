using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_ChargeLance : Item
{
    public Polearm_ChargeLance(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(115, 115, 0.85f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_Description(new(this)) {EffectTypeName="ChargeLanceAttack", DescriptionParameters=new() {"40", Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 3.125f)}}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.EnergyGain, new(this)) { PercentageAmount = 0.5f }};
    }
}
