using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_SanguineSpike : Item
{
    public Polearm_SanguineSpike(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(110, 80, 0.95f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_Description(new(this)) {EffectTypeName="JavelinAttack", DescriptionParameters=new() {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 3.125f)}}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.HeavyInjury, new(this)) { PercentageAmount = 1.25f, RemainsActiveInOtherStances = true }};
    }
}
