using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Sage : Item
{
    public Armor_Sage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Sage;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableEffectOnEvent(new(this)) {EffectTypeName="MagicEnergyRefund", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.25f) + "%"}, ConditionCheckForAbilityEnergyConsumed = new Func<Ability, float, bool>((ability, cost) => 
                    (ability.User == Player.Instance && ability.ScalesWith == Constants.DamageType.Magic)), ActionOnAbilityEnergyConsumed = new Action<Ability, float, Effect_CustomizableEffectOnEvent> ((ability, cost, effect) =>  {
                    Player.Instance.Energy.Current += cost * (effect.LinearEffectValue * 0.25f) / 100;
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.EnergyGain, new(this)) { PercentageAmount = 0.5f } };
    }
}
