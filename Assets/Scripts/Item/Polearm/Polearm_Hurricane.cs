using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_Hurricane : Item
{
    public Polearm_Hurricane(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(105, 105, 0.9f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_GainSpeedAndDamageDuringBACombo(new(this)) {SpeedBonusPerHit = GetFirstModifierEffectValue(false) * 0.0625f, DamageBonusPerHit = GetFirstModifierEffectValue() * 0.0625f, DescriptionParameters=new List<string>{Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.0625f), Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.0625f)}} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) {PercentageAmount=0.5f} };
    }
}
