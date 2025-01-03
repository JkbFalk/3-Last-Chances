using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_Harpoon : Item
{
    public Polearm_Harpoon(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(70, 130, 0.85f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_Description(new(this)) {EffectTypeName="HarpoonAttack", DescriptionParameters=new() {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 4.6875f)}}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Control, new(this)) { PercentageAmount = 0.5f}};
    }
}
